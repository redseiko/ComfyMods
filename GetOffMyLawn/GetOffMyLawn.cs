using BepInEx;
using BepInEx.Logging;

using HarmonyLib;

using System.Reflection;

using static GetOffMyLawn.PluginConfig;

namespace GetOffMyLawn {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class GetOffMyLawn : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.getoffmylawn";
    public const string PluginName = "GetOffMyLawn";
    public const string PluginVersion = "1.4.1";

    public static ManualLogSource PluginLogger { get; private set; }
    Harmony _harmony;

    public void Awake() {
      PluginLogger = Logger;
      BindConfig(Config);

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    public static readonly int HealthHashCode = "health".GetStableHashCode();

    public static long ApplyDamageCount { get; set; } = 0L;
    public static long ApplyDamageCountLastMin { get; set; } = 0L;
  }
}
