namespace Instaloot;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Instaloot : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.instaloot";
  public const string PluginName = "Instaloot";
  public const string PluginVersion = "1.1.0";

  void Awake() {
    BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }
}
