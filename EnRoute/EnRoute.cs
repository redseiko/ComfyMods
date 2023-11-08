using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BepInEx;

using HarmonyLib;

namespace EnRoute {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class EnRoute : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.enroute";
    public const string PluginName = "EnRoute";
    public const string PluginVersion = "1.3.0";

    Harmony _harmony;

    void Awake() {
      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }

    void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    public static readonly string[] NearbyRPCMethods = {
      "Step",
      // "DestroyZDO"
    };

    public static readonly Dictionary<int, string> NearbyRPCMethodByHashCode =
        NearbyRPCMethods.ToDictionary(method => method.GetStableHashCode());

    public static readonly HashSet<int> NearbyMethodHashCodes = NearbyRPCMethodByHashCode.Keys.ToHashSet();

    public static long NetTimeTicks = 0L;
  }
}
