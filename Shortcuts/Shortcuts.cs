namespace Shortcuts;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public sealed class Shortcuts : BaseUnityPlugin {
  public const string PluginGUID = "redseiko.valheim.shortcuts";
  public const string PluginName = "Shortcuts";
  public const string PluginVersion = "1.7.0";

  void Awake() {
    BindConfig(Config);

    if (IsModEnabled.Value) {
      Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
    }
  }
}
