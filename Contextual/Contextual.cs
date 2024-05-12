namespace Contextual;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Contextual : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.contextual";
  public const string PluginName = "Contextual";
  public const string PluginVersion = "1.0.0";

  void Awake() {
    BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }
}
