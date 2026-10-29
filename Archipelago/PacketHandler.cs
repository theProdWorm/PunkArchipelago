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

    private static void SendLootCachePacket()
    {
        
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
        
    }
}