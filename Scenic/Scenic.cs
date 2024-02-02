namespace Scenic;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Scenic : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.scenic";
  public const string PluginName = "Scenic";
  public const string PluginVersion = "1.4.0";

  Harmony _harmony;

  void Awake() {
    BindConfig(Config);

    _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }

  void OnDestroy() {
    _harmony?.UnpatchSelf();
  }
}
