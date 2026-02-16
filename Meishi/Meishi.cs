namespace Meishi;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Meishi : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.meishi";
  public const string PluginName = "Meishi";
  public const string PluginVersion = "1.0.0";

  void Awake() {
    BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }
}
