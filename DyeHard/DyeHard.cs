namespace DyeHard;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public sealed class DyeHard : BaseUnityPlugin {
  public const string PluginGUID = "redseiko.valheim.dyehard";
  public const string PluginName = "DyeHard";
  public const string PluginVersion = "1.6.0";

  void Awake() {
    BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
  }
}
