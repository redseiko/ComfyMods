namespace LicenseToSkill;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public sealed class LicenseToSkill : BaseUnityPlugin {
  public const string PluginGUID = "redseiko.valheim.comfytools.licensetoskill";
  public const string PluginName = "LicenseToSkill";
  public const string PluginVersion = "1.3.0";

  Harmony _harmony;

  void Awake() {
    BindConfig(Config);

    _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
  }

  void OnDestroy() {
    _harmony?.UnpatchSelf();
  }
}
