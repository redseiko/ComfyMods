namespace VonCount;

using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;

public static class CountManager {  
  static readonly ConcurrentDictionary<int, string> _stableHashCodeCache = new();

  public static void CacheStableHashCode(int hash, string str) {
    _stableHashCodeCache.TryAdd(hash, str);
  }

  static readonly ConcurrentDictionary<long, string> _playerInfoCache = new();

  public static void ProcessPlayerListUpdate(List<ZNet.PlayerInfo> playerInfos) {
    foreach (ZNet.PlayerInfo playerInfo in playerInfos) {
      _playerInfoCache[playerInfo.m_characterID.UserID] = playerInfo.m_userInfo.m_id.m_userID;
    }
  }

  static readonly ConcurrentDictionary<int, int> _routedRPCByMethodHash = new();
  static readonly ConcurrentDictionary<long, int> _routedRPCBySenderId = new();

  public static void CountRoutedRPC(ZRoutedRpc.RoutedRPCData routedRPCData) {
    int methodHash = routedRPCData.m_methodHash;

    if (!_routedRPCByMethodHash.TryGetValue(methodHash, out int methodCount)) {
      methodCount = 0;
    }

    _routedRPCByMethodHash[methodHash] = methodCount + 1;

    long senderId = routedRPCData.m_senderPeerID;

    if (!_routedRPCBySenderId.TryGetValue(senderId, out int senderCount)) {
      senderCount = 0;
    }

    _routedRPCBySenderId[senderId] = senderCount + 1;
  }

  public static void LogRPCCountsByMethodHash() {
    StringBuilder message = new($"RoutedRPCs by MethodHash\n");

    foreach (KeyValuePair<int, int> pair in _routedRPCByMethodHash.OrderByDescending(pair => pair.Value)) {
      if (!_stableHashCodeCache.TryGetValue(pair.Key, out string routedRPCName)) {
        routedRPCName = pair.Key.ToString("D");
      }

      message.AppendLine($"{pair.Key:D}, {routedRPCName}, {pair.Value:D}");
    }

    VonCount.LogInfo(message.ToString());
  }

  public static void LogRPCCountsBySenderId() {
    StringBuilder message = new($"RoutedRPCs by SenderId\n");

    foreach (KeyValuePair<long, int> pair in _routedRPCBySenderId.OrderByDescending(pair => pair.Value)) {
      if (!_playerInfoCache.TryGetValue(pair.Key, out string steamId)) {
        steamId = pair.Key.ToString("D");
      }

      message.AppendLine($"{pair.Key:D}, {steamId}, {pair.Value:D}");
    }

    VonCount.LogInfo(message.ToString());
  }
}
