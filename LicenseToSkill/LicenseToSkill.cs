namespace LicenseToSkill;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public sealed class LicenseToSkill : BaseUnityPlugin {
  public const string PluginGUID = "redseiko.valheim.comfytools.licensetoskill";
  public const string PluginName = "LicenseToSkill";
  public const string PluginVersion = "1.4.0";

  void Awake() {
    BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
  }
}
