namespace StatusQuo;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInIncompatibility("randyknapp.mods.minimalstatuseffects")]
public sealed class StatusQuo : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.statusquo";
  public const string PluginName = "StatusQuo";
  public const string PluginVersion = "1.0.0";

  void Awake() {
    BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }
}
