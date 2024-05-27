namespace Volumetry;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Volumetry : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.volumetry";
  public const string PluginName = "Volumetry";
  public const string PluginVersion = "0.2.0";

  void Awake() {
    BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }
}
