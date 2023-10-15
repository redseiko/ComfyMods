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

    public static void RouteRPC(ZRoutedRpc routedRpc, ZRoutedRpc.RoutedRPCData rpcData) {
      if (rpcData.m_targetPeerID == ZRoutedRpc.Everybody
          && EnRoute.NearbyMethodHashCodes.Contains(rpcData.m_methodHash)) {
        foreach (ZNetPeer netPeer in routedRpc.GetReadyPeers()) {
          RouteRPCToPeer(netPeer, NearbyUserIds, rpcData);
        }
      } else {
        rpcData.SerializeTo(_package);

        foreach (ZNetPeer netPeer in routedRpc.GetReadyPeers()) {
          netPeer.m_rpc.Invoke("RoutedRPC", _package);
        }
      }
    }

    public static void RouteRPCToPeer(ZNetPeer netPeer, HashSet<long> targetPeerIds, ZRoutedRpc.RoutedRPCData rpcData) {
      if (netPeer.m_server) {
        rpcData.m_targetPeerID = netPeer.m_uid;
        netPeer.m_rpc.Invoke("RoutedRPC", rpcData.SerializeTo(_package));

        RouteToStats.LogRouteToServer(rpcData.m_methodHash);
      }

      if (targetPeerIds.Count > 0) {
        foreach (long targetPeerId in targetPeerIds) {
          rpcData.m_targetPeerID = targetPeerId;
          netPeer.m_rpc.Invoke("RoutedRPC", rpcData.SerializeTo(_package));
        }

        RouteToStats.LogRouteToNearby(rpcData.m_methodHash, targetPeerIds.Count);
      }
    }
  }
}
