namespace EnRoute;

using System.Collections.Generic;
using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class EnRoute : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.enroute";
  public const string PluginName = "EnRoute";
  public const string PluginVersion = "1.4.0";

  Harmony _harmony;

  void Awake() {
    BindConfig(Config);
    SetupNearbyRPCMethods();

    _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }

  void OnDestroy() {
    _harmony?.UnpatchSelf();
  }

  public static void SetupNearbyRPCMethods() {
    NearbyRPCMethodByHashCode.Clear();
    string[] names = NearbyRPCMethodNames.Value.Split(new char[] { ',' }, System.StringSplitOptions.RemoveEmptyEntries);

    foreach (string name in names) {
      NearbyRPCMethodByHashCode[name.GetStableHashCode()] = name;
    }

    NearbyRPCMethodHashCodes.Clear();
    NearbyRPCMethodHashCodes.UnionWith(NearbyRPCMethodByHashCode.Keys);
  }

  public static readonly Dictionary<int, string> NearbyRPCMethodByHashCode = new();
  public static readonly HashSet<int> NearbyRPCMethodHashCodes = new();

  public static long NetTimeTicks = 0L;
}
