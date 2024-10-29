namespace EnRoute;

using System.Collections.Generic;

using static PluginConfig;

public static class EnRouteManager {
  public static readonly Dictionary<int, string> NearbyRPCMethodByHashCode = [];
  public static readonly HashSet<int> NearbyRPCMethodHashCodes = [];

  public static long NetTimeTicks = 0L;

  public static void SetupNearbyRPCMethods() {
    NearbyRPCMethodByHashCode.Clear();
    string[] names = NearbyRPCMethodNames.Value.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

    foreach (string name in names) {
      NearbyRPCMethodByHashCode[name.GetStableHashCode()] = name;
    }

    NearbyRPCMethodHashCodes.Clear();
    NearbyRPCMethodHashCodes.UnionWith(NearbyRPCMethodByHashCode.Keys);

    EnRoute.LogInfo($"NearbyRPCMethods set to: {string.Join(", ", NearbyRPCMethodByHashCode.Values)}");
  }
}
