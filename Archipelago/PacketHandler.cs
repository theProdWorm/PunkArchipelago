using Archipelago.MultiClient.Net;
using Archipelago.MultiClient.Net.Enums;
using Archipelago.MultiClient.Net.Helpers;
using Archipelago.MultiClient.Net.Packets;

namespace PunkArchipelago.Archipelago;

public static class PacketHandler
{
    private static ArchipelagoSession _SESSION;

    internal static void Initialize(ArchipelagoSession session)
    {
        _SESSION = session;
    
        _SESSION.Socket.PacketReceived += PacketReceived;
    }

    public static void SendPacket(ArchipelagoPacketBase packet)
    {
        _SESSION.Socket.SendPacket(packet);
    }

    private static void PacketReceived(ArchipelagoPacketBase packet)
    {
        switch (packet.PacketType)
        {
            case ArchipelagoPacketType.ReceivedItems:
                OnReceivedItems((ReceivedItemsPacket) packet);
                break;
        }
    }

    private static void OnReceivedItems(ReceivedItemsPacket packet)
    {
        Plugin.LOGGER.LogInfo($"Received items packet");
    }
}