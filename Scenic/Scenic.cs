namespace Scenic;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Scenic : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.scenic";
  public const string PluginName = "Scenic";
  public const string PluginVersion = "1.5.0";

  void Awake() {
    BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }
}
