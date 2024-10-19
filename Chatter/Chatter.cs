namespace Chatter;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Chatter : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.chatter";
  public const string PluginName = "Chatter";
  public const string PluginVersion = "2.9.0";

  void Awake() {
    BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }
}
