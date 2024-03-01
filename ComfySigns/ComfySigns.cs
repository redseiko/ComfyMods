namespace ComfySigns;

using System.Reflection;

using BepInEx;

using ComfyLib;

using HarmonyLib;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class ComfySigns : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.comfysigns";
  public const string PluginName = "ComfySigns";
  public const string PluginVersion = "1.7.0";

  Harmony _harmony;

  void Awake() {
    ComfyConfigUtils.BindConfig(Config);

    _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }

  void OnDestroy() {
    _harmony?.UnpatchSelf();
  }
}
