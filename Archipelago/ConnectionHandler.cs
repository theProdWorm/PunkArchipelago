using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;

namespace PunkArchipelago.Archipelago;

public static class ConnectionHandler
{
    public static string HOST = "archipelago.gg";
    public static string PORT = "";
    public static string SLOT = "";
    public static string PASS = "";
    public static bool DEATH_LINK;
    
    private static ArchipelagoSession _SESSION;

    private static string _SEED;

    public static bool IS_CONNECTED => _SESSION != null && _SESSION.Socket.Connected;
    
    public static async Task TryConnect()
    {
        await Disconnect();
        
        Plugin.LOGGER.LogInfo("Trying to connect to archipelago server..");
        
        _SESSION = ArchipelagoSessionFactory.CreateSession($"{HOST}:{PORT}");

        RoomInfoPacket roomInfo = await _SESSION.ConnectAsync();
        if (roomInfo == null)
        {
            Plugin.LOGGER.LogError("Failed to connect to archipelago server. Host and/or port is invalid.");
            return;
        }
        
        Plugin.LOGGER.LogInfo($"Connected to {HOST}:{PORT}");
        Plugin.LOGGER.LogInfo($"DeathLink: {DEATH_LINK}");
        
        LoginResult result = await _SESSION.LoginAsync("PUNK", SLOT, ItemsHandlingFlags.AllItems, 
            tags: DEATH_LINK ? ["DeathLink"] : null,
            password: roomInfo.Password ? PASS : null,
            version: new Version(0, 6, 8));

        if (result is LoginSuccessful success)
        {
            Plugin.LOGGER.LogInfo($"Successfully logged in as {SLOT}");
            
            if (_SESSION.RoomState.Seed.Equals(_SEED))
            {
                ItemRegister.Update(_SESSION.Items.AllItemsReceived);
            }
            else
            {
                ItemRegister.Initialize(_SESSION.Items.AllItemsReceived);
                _SEED = _SESSION.RoomState.Seed;
            }
            
            Options.SetFrom(success.SlotData);
            // PacketHandler.Initialize(_SESSION);

            _SESSION.Socket.SocketClosed += OnDisconnected;
            PacketHandler.Initialize(_SESSION);
            return;
        }
        
        LoginFailure failure = (LoginFailure) result;
        string errorMessage = $"Failed to login to {HOST}:{PORT} as {SLOT}" + (roomInfo.Password ? $" with password {PASS}" : "");
        foreach (var error in failure.Errors)
            errorMessage += $"\n    {error}";
        foreach (var error in failure.ErrorCodes)
            errorMessage += $"\n    {error}";
        
        Plugin.LOGGER.LogError(errorMessage);
    }

    private static async Task Disconnect()
    {
        if (_SESSION is null)
            return;
        
        _SESSION.Socket.SocketClosed -= OnDisconnected;
        await _SESSION.Socket.DisconnectAsync();
    }

    private static async void OnDisconnected(string reason)
    {
        Plugin.LOGGER.LogError($"Socket connection closed. Reason: {reason}");
        
        await TryConnect();
    }
}