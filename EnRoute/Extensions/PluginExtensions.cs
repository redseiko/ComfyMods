using System.Collections.Generic;

namespace EnRoute {
  public static class PluginExtensions {
    public static IEnumerable<ZNetPeer> GetReadyPeers(this ZRoutedRpc routedRpc) {
      foreach (ZNetPeer netPeer in routedRpc.m_peers) {
        if (netPeer.IsReady()) {
          yield return netPeer;
        }
      }
    }

    public static IEnumerable<ZNetPeer> GetNonSenderReadyPeers(this ZRoutedRpc routedRpc, long senderId) {
      foreach (ZNetPeer netPeer in routedRpc.m_peers) {
        if (netPeer.m_uid != senderId && netPeer.IsReady()) {
          yield return netPeer;
        }
      }
    }

    public static ZPackage SerializeTo(this ZRoutedRpc.RoutedRPCData rpcData, ZPackage package) {
      package.Clear();
      rpcData.Serialize(package);
      return package;
    }

    public static bool IsSectorInRange(this Vector2i source, Vector2i target, int range) {
      return target.x + range >= source.x - range
          && target.x - range <= source.x + range
          && target.y + range >= source.y - range
          && target.y - range <= source.y + range;
    }
  }
}
