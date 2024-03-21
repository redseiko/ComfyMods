namespace Chatter;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Chatter : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.chatter";
  public const string PluginName = "Chatter";
  public const string PluginVersion = "2.6.0";

  Harmony _harmony;

  void Awake() {
    BindConfig(Config);

    _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }

  void OnDestroy() {
    _harmony?.UnpatchSelf();
  }
}
