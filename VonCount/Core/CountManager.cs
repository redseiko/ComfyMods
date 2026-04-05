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

  public sealed class MethodCount {
    public readonly ConcurrentDictionary<int, int> MethodHashCounts = [];

    public void IncrementCount(int methodHash, int value = 1) {
      if (!MethodHashCounts.TryGetValue(methodHash, out int methodHashCount)) {
        methodHashCount = 0;
      }

      MethodHashCounts[methodHash] = methodHashCount + value;
    }
  }

  public sealed class SenderRPCCount {
    public int TotalCount { get; private set; } = 0;
    public readonly ConcurrentDictionary<int, int> MethodHashCounts = [];

    public void IncrementCount(int methodHash, int value = 1) {
      TotalCount += value;

      if (!MethodHashCounts.TryGetValue(methodHash, out int methodHashCount)) {
        methodHashCount = 0;
      }

      MethodHashCounts[methodHash] = methodHashCount + value;
    }
  }

  static readonly ConcurrentDictionary<int, int> _routedRPCByMethodHash = new();
  static readonly ConcurrentDictionary<long, SenderRPCCount> _routedRPCBySenderId = new();

  public static void CountRoutedRPC(ZRoutedRpc.RoutedRPCData routedRPCData) {
    int methodHash = routedRPCData.m_methodHash;

    if (!_routedRPCByMethodHash.TryGetValue(methodHash, out int methodCount)) {
      methodCount = 0;
    }

    _routedRPCByMethodHash[methodHash] = methodCount + 1;

    long senderId = routedRPCData.m_senderPeerID;

    if (!_routedRPCBySenderId.TryGetValue(senderId, out SenderRPCCount senderRPCCount)) {
      senderRPCCount = new();
      _routedRPCBySenderId[senderId] = senderRPCCount;
    }

    senderRPCCount.IncrementCount(methodHash, 1);
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

    foreach (
        KeyValuePair<long, SenderRPCCount> pair
            in _routedRPCBySenderId.OrderByDescending(pair => pair.Value.TotalCount)) {
      if (!_playerInfoCache.TryGetValue(pair.Key, out string steamId)) {
        steamId = pair.Key.ToString("D");
      }

      message.AppendLine($"{pair.Key:D}, {steamId}, {pair.Value.TotalCount:D}");
    }

    VonCount.LogInfo(message.ToString());
  }

  public static void LogRPCCountsForSenderId(long senderId) {
    if (!_routedRPCBySenderId.TryGetValue(senderId, out SenderRPCCount senderRPCCount)) {
      VonCount.LogInfo($"No entry found for SenderId {senderId}.");
      return;
    }

    if (!_playerInfoCache.TryGetValue(senderId, out string steamId)) {
      steamId = senderId.ToString("D");
    }

    StringBuilder message = new($"RoutedRPCs for SenderId {senderId} ({steamId})\n");

    message.AppendLine($"TotalCount: {senderRPCCount.TotalCount}\n");

    foreach (KeyValuePair<int, int> pair in senderRPCCount.MethodHashCounts.OrderByDescending(pair => pair.Value)) {
      if (!_stableHashCodeCache.TryGetValue(pair.Key, out string routedRPCName)) {
        routedRPCName = pair.Key.ToString("D");
      }

      message.AppendLine($"{pair.Key:D}, {routedRPCName}, {pair.Value:D}");
    }

    VonCount.LogInfo(message.ToString());
  }
}
