namespace EulersRuler;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public sealed class EulersRuler : BaseUnityPlugin {
  public const string PluginGUID = "redseiko.valheim.eulersruler";
  public const string PluginName = "EulersRuler";
  public const string PluginVersion = "1.8.0";

  void Awake() {
    BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
  }
}
