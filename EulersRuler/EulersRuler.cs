namespace EulersRuler;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public sealed class EulersRuler : BaseUnityPlugin {
  public const string PluginGUID = "redseiko.valheim.eulersruler";
  public const string PluginName = "EulersRuler";
  public const string PluginVersion = "1.7.0";

  Harmony _harmony;

  void Awake() {
    BindConfig(Config);

    _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
  }

  void OnDestroy() {
    _harmony?.UnpatchSelf();
  }
}
