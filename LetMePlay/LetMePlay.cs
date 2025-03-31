namespace LetMePlay;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public sealed class LetMePlay : BaseUnityPlugin {
  public const string PluginGUID = "redseiko.valheim.letmeplay";
  public const string PluginName = "LetMePlay";
  public const string PluginVersion = "1.6.1";

  public void Awake() {
    BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
  }
}
