namespace EnRoute;

using System.Collections.Generic;

public static class RouteManager {
  public static readonly int RoutedRPCHashCode = "RoutedRPC".GetStableHashCode();

  public static readonly Dictionary<long, ZNetPeer> NetPeers = [];
  public static readonly Dictionary<long, RouteRecord> NetPeerRouting = [];

  public static void OnAddPeer(ZNetPeer netPeer) {
    NetPeers[netPeer.m_uid] = netPeer;
    NetPeerRouting[netPeer.m_uid] = new(netPeer);
  }

  public static void OnRemovePeer(ZNetPeer netPeer) {
    NetPeers.Remove(netPeer.m_uid);
    NetPeerRouting.Remove(netPeer.m_uid);
  }

  public static void RefreshRouteRecords() {
    foreach (RouteRecord record in NetPeerRouting.Values) {
      record.Sector = ZoneSystem.GetZone(record.NetPeer.m_refPos);
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
      SerializeRoutedRPCInvoke(rpcData, RoutedRPCHashCode, _package);
      SendPackage(netPeer.m_rpc, _package);
    }
  }

  static void RouteRPCToEverybody(ZRoutedRpc routedRpc, ZRoutedRpc.RoutedRPCData rpcData) {
    if (EnRouteManager.NearbyRPCMethodHashCodes.Contains(rpcData.m_methodHash)
        && NetPeerRouting.TryGetValue(rpcData.m_senderPeerID, out RouteRecord record)) {
      RouteToNearby(routedRpc, rpcData, record);
    } else {
      RouteToPeers(routedRpc, rpcData, rpcData.m_senderPeerID);
    }
  }

  static void RouteToNearby(ZRoutedRpc routedRpc, ZRoutedRpc.RoutedRPCData rpcData, RouteRecord record) {
    if (record.NearbyUserIds.Count <= 0) {
      return;
    }

    SerializeRoutedRPCInvoke(rpcData, RoutedRPCHashCode, _package);

    foreach (long peerId in record.NearbyUserIds) {
      if (TryGetPeer(peerId, out ZNetPeer netPeer)) {
        SendPackage(netPeer.m_rpc, _package);
      }
    }
  }

  static void RouteToPeers(ZRoutedRpc routedRpc, ZRoutedRpc.RoutedRPCData rpcData, long senderPeerId) {
    SerializeRoutedRPCInvoke(rpcData, RoutedRPCHashCode, _package);

    foreach (ZNetPeer netPeer in routedRpc.m_peers) {
      if (netPeer.m_uid != senderPeerId && netPeer.IsReady()) {
        SendPackage(netPeer.m_rpc, _package);
      }
    }
  }

  static void SerializeRoutedRPCInvoke(ZRoutedRpc.RoutedRPCData rpcData, int methodhash, ZPackage package) {
    package.Clear();

    package.Write(0);
    package.Write(0);

    int size = package.Size();
    rpcData.WriteToPackage(package);

    long position = package.m_stream.Position;
    package.m_stream.Position = 0;

    package.Write(methodhash);
    package.Write(package.Size() - size);

    package.m_stream.Position = position;
  }

  static void SendPackage(ZRpc rpc, ZPackage package) {
    if (rpc.m_socket.IsConnected()) {
      rpc.m_sentPackages++;
      rpc.m_sentData += package.Size();
      rpc.m_socket.Send(package);
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
