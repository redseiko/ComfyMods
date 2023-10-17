using System.Collections.Generic;

namespace EnRoute {
  public static class RouteManager {
    public static readonly Dictionary<long, ZNetPeer> NetPeers = new();
    public static readonly Dictionary<long, RouteRecord> NetPeerRouting = new();

    public static void OnAddPeer(ZNetPeer netPeer) {
      NetPeers[netPeer.m_uid] = netPeer;
      NetPeerRouting[netPeer.m_uid] = new(netPeer);
    }

    public static void OnRemovePeer(ZNetPeer netPeer) {
      NetPeers.Remove(netPeer.m_uid);
      NetPeerRouting.Remove(netPeer.m_uid);
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

        if (record.Sector.IsSectorInRange(otherRecord.Sector, 2)) {
          record.NearbyUserIds.Add(otherRecord.UserId);
        }
      }
    }

    static readonly ZPackage _package = new();

    public static void RouteRPC(ZRoutedRpc routedRpc, ZRoutedRpc.RoutedRPCData rpcData) {
      if (rpcData.m_targetPeerID == ZRoutedRpc.Everybody) {
        RouteRPCToEverybody(routedRpc, rpcData);
      } else if (TryGetPeer(rpcData.m_targetPeerID, out ZNetPeer netPeer)) {
        netPeer.m_rpc.Invoke("RoutedRPC", rpcData.SerializeTo(_package));
      }
    }

    static void RouteRPCToEverybody(ZRoutedRpc routedRpc, ZRoutedRpc.RoutedRPCData rpcData) {
      if (EnRoute.NearbyMethodHashCodes.Contains(rpcData.m_methodHash)
          && NetPeerRouting.TryGetValue(rpcData.m_senderPeerID, out RouteRecord record)) {
        if (record.NearbyUserIds.Count <= 0) {
          return;
        }

        rpcData.SerializeTo(_package);

        foreach (long peerId in record.NearbyUserIds) {
          if (TryGetPeer(peerId, out ZNetPeer netPeer)) {
            netPeer.m_rpc.Invoke("RoutedRPC", _package);
          }
        }
      } else {
        rpcData.SerializeTo(_package);

        foreach (ZNetPeer netPeer in routedRpc.GetNonSenderReadyPeers(rpcData.m_senderPeerID)) {
          netPeer.m_rpc.Invoke("RoutedRPC", _package);
        }
      }
    }

    static bool TryGetPeer(long targetPeerId, out ZNetPeer netPeer) {
      if (NetPeers.TryGetValue(targetPeerId, out netPeer) && netPeer != null && netPeer.IsReady()) {
        return true;
      }

      netPeer = default;
      return false;
    }
  }
}
