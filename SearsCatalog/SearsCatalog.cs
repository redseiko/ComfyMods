namespace SearsCatalog;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class SearsCatalog : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.searscatalog";
  public const string PluginName = "SearsCatalog";
  public const string PluginVersion = "1.6.0";

  void Awake() {
    BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }
}
