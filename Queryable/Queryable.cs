namespace Queryable;

using System.Reflection;

using BepInEx;

using ComfyLib;

using HarmonyLib;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Queryable : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.queryable";
  public const string PluginName = "Queryable";
  public const string PluginVersion = "1.0.0";

  void Awake() {
    ComfyLogger.BindLogger(Logger);
    PluginConfig.BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }
}
