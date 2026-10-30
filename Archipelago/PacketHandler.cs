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

    public static void CheckLocations(params long[] locations)
    {
        var packet = new LocationChecksPacket()
        {
            Locations = locations
        };
        SendPacket(packet);
    }
    
    private static void SendPacket(ArchipelagoPacketBase packet)
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
        Plugin.LOGGER.LogInfo($"Received items packet, containing:");
        foreach (var item in packet.Items)
        {
            string itemName = _SESSION.Items.GetItemName(item.Item);
            Plugin.LOGGER.LogInfo($"  {itemName}");
        }
    }
}