using System.Reflection;

using BepInEx;

using HarmonyLib;

using static ReturnToSender.PluginConfig;

namespace ReturnToSender {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class ReturnToSender : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.returntosender";
    public const string PluginName = "ReturnToSender";
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
