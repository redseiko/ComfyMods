using System.Collections.Generic;

namespace EnRoute {
  public static class RouteManager {
    public const int SectorRange = 2;

    public static readonly Dictionary<long, ZNetPeer> NetPeers = new();
    public static readonly Dictionary<ZNetPeer, RouteRecord> NetPeerRouting = new();

    public static void OnAddPeer(ZNetPeer netPeer) {
      NetPeers[netPeer.m_uid] = netPeer;
      NetPeerRouting[netPeer] = new(netPeer);
    }

    public static void OnRemovePeer(ZNetPeer netPeer) {
      NetPeers.Remove(netPeer.m_uid);
      NetPeerRouting.Remove(netPeer);
    }

    public static void RefreshRouteRecords() {
      ZoneSystem zoneSystem = ZoneSystem.instance;

      foreach (RouteRecord record in NetPeerRouting.Values) {
        record.Sector = zoneSystem.GetZone(record.NetPeer.m_refPos);
      }

      foreach (RouteRecord record in NetPeerRouting.Values) {
        RefreshRouteRecord(record);
      }
    }

    public static void RefreshRouteRecord(RouteRecord record) {
      record.NearbyUserIds.Clear();

      foreach (RouteRecord otherRecord in NetPeerRouting.Values) {
        if (otherRecord.UserId == record.UserId) {
          continue;
        }

        if (Vector2i.Distance(otherRecord.Sector, record.Sector) <= SectorRange) {
          record.NearbyUserIds.Add(otherRecord.UserId);
        }
      }
    }
  }
}
