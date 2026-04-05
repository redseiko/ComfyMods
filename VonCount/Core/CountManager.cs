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

  static readonly ConcurrentDictionary<int, int> _routedRPCCounts = new();

  public static void CountRoutedRPC(ZRoutedRpc.RoutedRPCData routedRPCData) {
    int methodHash = routedRPCData.m_methodHash;

    if (!_routedRPCCounts.TryGetValue(methodHash, out int count)) {
      count = 0;
    }

    _routedRPCCounts[methodHash] = count + 1;
  }

  public static void LogRPCCounts() {
    StringBuilder message = new();

    foreach (KeyValuePair<int, int> pair in _routedRPCCounts.OrderByDescending(pair => pair.Value)) {
      if (!_stableHashCodeCache.TryGetValue(pair.Key, out string routedRPCName)) {
        routedRPCName = pair.Key.ToString("D");
      }

      message.AppendLine($"{routedRPCName}, {pair.Value:D}");
    }

    VonCount.LogInfo(message.ToString());
  }
}
