namespace ReturnToSender;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class ReturnToSender : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.returntosender";
  public const string PluginName = "ReturnToSender";
  public const string PluginVersion = "1.3.0";

  void Awake() {
    BindConfig(Config);

    if (IsModEnabled.Value) {
      Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }
  }
}
