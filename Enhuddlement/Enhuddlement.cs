using System.Reflection;

using BepInEx;

using HarmonyLib;

using static Enhuddlement.PluginConfig;

namespace Enhuddlement {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class Enhuddlement : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.enhuddlement";
    public const string PluginName = "Enhuddlement";
    public const string PluginVersion = "1.3.2";

    public static Harmony HarmonyInstance { get; private set; }

    void Awake() {
      BindConfig(Config);

      HarmonyInstance = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }

    void OnDestroy() {
      HarmonyInstance?.UnpatchSelf();
    }
  }
}
