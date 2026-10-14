using System;
using System.Threading.Tasks;
using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Packets;

namespace PunkArchipelago;

public static class ArchipelagoConnection
{
    public static string HOST = "archipelago.gg";
    public static string PORT = "";
    public static string SLOT = "";
    public static string PASS = "";
    public static bool DEATH_LINK;
    
    private static ArchipelagoSession SESSION;
    
    public static async Task TryConnect()
    {
        ArchipelagoPlugin.Logger.LogInfo("Trying to connect to archipelago server..");
        
        SESSION = ArchipelagoSessionFactory.CreateSession($"{HOST}:{PORT}");
        
        RoomInfoPacket roomInfo = await SESSION.ConnectAsync();
        if (roomInfo == null)
        {
            ArchipelagoPlugin.Logger.LogError("Failed to connect to archipelago server. Host and/or port is invalid.");
            return;
        }
        
        LoginResult result = await SESSION.LoginAsync("Punk", SLOT, ItemsHandlingFlags.AllItems, 
            tags: DEATH_LINK ? ["DeathLink"] : null,
            password: roomInfo.Password ? PASS : null,
            version: new Version(0, 6, 8));

        if (result.Successful)
            return;
        
        LoginFailure failure = (LoginFailure) result;
        string errorMessage = $"Failed to login to {HOST}:{PORT} as {SLOT}" + (roomInfo.Password ? $" with password {PASS}" : "");
        foreach (var error in failure.Errors)
            errorMessage += $"\n    {error}";
        foreach (var error in failure.ErrorCodes)
            errorMessage += $"\n    {error}";
        
        // SESSION.ConnectionInfo.
        
        ArchipelagoPlugin.Logger.LogError(errorMessage);

    }
}