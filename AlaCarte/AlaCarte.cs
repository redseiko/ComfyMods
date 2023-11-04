using System.Reflection;

using BepInEx;

using HarmonyLib;

using static AlaCarte.PluginConfig;

namespace AlaCarte {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class AlaCarte : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.alacarte";
    public const string PluginName = "AlaCarte";
    public const string PluginVersion = "1.2.0";

    Harmony _harmony;

    void Awake() {
      BindConfig(Config);

      if (IsModEnabled.Value) {
        _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
      }
    }

    void OnDestroy() {
      _harmony?.UnpatchSelf();
    }
  }
}
