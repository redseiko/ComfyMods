using System.Collections.Generic;

namespace EnRoute {
  public static class RouteNearbyManager {
    public static readonly HashSet<long> NearbyUserIds = new();

    public static void RefreshNearbyPlayers() {
      NearbyUserIds.Clear();

      ZoneSystem zoneSystem = ZoneSystem.m_instance;
      ZDOID playerCharacterId = ZNet.m_instance.m_characterID;
      Vector2i playerZone = zoneSystem.GetZone(ZNet.m_instance.m_referencePosition);

      foreach (ZNet.PlayerInfo playerInfo in ZNet.m_instance.m_players) {
        ZDOID characterId = playerInfo.m_characterID;

        if (characterId.IsNone() || characterId == playerCharacterId) {
          continue;
        }

        if (!playerInfo.m_publicPosition
            || Vector2i.Distance(playerZone, zoneSystem.GetZone(playerInfo.m_position)) <= 2) {
          NearbyUserIds.Add(characterId.UserID);
        }
      }
    }

    static readonly ZPackage _package = new();

    public static void RouteRPCToNearbyPeers(ZRoutedRpc routedRpc, ZRoutedRpc.RoutedRPCData rpcData) {
      foreach (ZNetPeer netPeer in routedRpc.m_peers) {
        if (netPeer.IsReady()) {
          RouteRPCToPeer(netPeer, NearbyUserIds, rpcData);
        }
      }
    }

    public static void RouteRPCToPeer(ZNetPeer netPeer, HashSet<long> targetPeerIds, ZRoutedRpc.RoutedRPCData rpcData) {
      if (netPeer.m_server) {
        rpcData.m_targetPeerID = netPeer.m_uid;
        RouteRPCToPeer(netPeer, rpcData);
        RouteToStats.LogRouteToServer(rpcData.m_methodHash);
      }

      if (targetPeerIds.Count > 0) {
        foreach (long targetPeerId in targetPeerIds) {
          rpcData.m_targetPeerID = targetPeerId;
          RouteRPCToPeer(netPeer, rpcData);
        }

        RouteToStats.LogRouteToNearby(rpcData.m_methodHash, targetPeerIds.Count);
      }
    }

    static void RouteRPCToPeer(ZNetPeer netPeer, ZRoutedRpc.RoutedRPCData rpcData) {
      _package.Clear();
      rpcData.Serialize(_package);
      netPeer.m_rpc.Invoke("RoutedRPC", _package);
    }
  }
}
