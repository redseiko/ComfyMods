namespace ColorfulDamage;

using System.Reflection;

using BepInEx;

using HarmonyLib;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class ColorfulDamage : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.colorfuldamage";
  public const string PluginName = "ColorfulDamage";
  public const string PluginVersion = "1.2.0";

  void Awake() {
    PluginConfig.BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }
}
