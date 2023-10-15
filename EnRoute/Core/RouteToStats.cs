using System;
using System.Collections.Generic;
using System.Linq;

namespace EnRoute {
  public static class RouteToStats {
    public static long RouteToNearbyCount = 0L;
    public static long RouteToServerCount = 0L;

    public static readonly Dictionary<int, long> RouteToServerMap = new();

    public static void LogRouteToServer(int rpcMethodHash) {
      if (!RouteToServerMap.TryGetValue(rpcMethodHash, out long count)) {
        count = 0L;
      }

      RouteToServerMap[rpcMethodHash] = count + 1L;
      RouteToServerCount++;
    }

    public static readonly Dictionary<int, long> RouteToNearbyMap = new();

    public static void LogRouteToNearby(int rpcMethodHash, int offset) {
      if (!RouteToNearbyMap.TryGetValue(rpcMethodHash, out long count)) {
        count = 0L;
      }

      RouteToNearbyMap[rpcMethodHash] = count + offset;
      RouteToNearbyCount += offset;
    }

    public static string LogRouteStats(Dictionary<int, long> routeToMap) {
      return string.Join(
          ", ", routeToMap.Select(pair => $"{EnRoute.NearbyRPCMethodByHashCode[pair.Key]}: {pair.Value}"));
    }

    public static void LogStats(TimeSpan timeElapsed) {
      ZLog.Log($"RouteToServer: {LogRouteStats(RouteToServerMap)}, Elapsed: {timeElapsed:hh\\:mm\\:ss}");
      ZLog.Log($"RouteToNearby: {LogRouteStats(RouteToNearbyMap)}, Elapsed: {timeElapsed:hh\\:mm\\:ss}");
    }
  }
}
