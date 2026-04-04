namespace VonCount;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class VonCount : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.voncount";
  public const string PluginName = "VonCount";
  public const string PluginVersion = "1.0.0";

  void Awake() {
    BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }
}
