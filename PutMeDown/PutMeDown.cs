using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PutMeDown.PluginConfig;

namespace PutMeDown {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class PutMeDown : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.putmedown";
    public const string PluginName = "PutMeDown";
    public const string PluginVersion = "1.0.0";

    Harmony _harmony;

    void Awake() {
      BindConfig(Config);

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }

    void OnDestroy() {
      _harmony?.UnpatchSelf();
    }
  }
}
