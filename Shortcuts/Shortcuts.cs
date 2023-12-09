using System.Reflection;

using BepInEx;

using HarmonyLib;

using static Shortcuts.PluginConfig;

namespace Shortcuts {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public sealed class Shortcuts : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.shortcuts";
    public const string PluginName = "Shortcuts";
    public const string PluginVersion = "1.5.0";

    Harmony _harmony;

    void Awake() {
      BindConfig(Config);

      if (IsModEnabled.Value) {
        _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
      }
    }

    void OnDestroy() {
      _harmony?.UnpatchSelf();
    }
  }
}
