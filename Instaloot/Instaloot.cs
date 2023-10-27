using System.Reflection;

using BepInEx;

using HarmonyLib;

using static Instaloot.PluginConfig;

namespace Instaloot {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class Instaloot : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.instaloot";
    public const string PluginName = "Instaloot";
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
