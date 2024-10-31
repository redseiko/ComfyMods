namespace ComfySigns;

using System.Reflection;

using BepInEx;

using ComfyLib;

using HarmonyLib;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class ComfySigns : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.comfysigns";
  public const string PluginName = "ComfySigns";
  public const string PluginVersion = "1.9.0";

  void Awake() {
    ComfyConfigUtils.BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }
}
