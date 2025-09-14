namespace Chatter;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class Chatter : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.chatter";
  public const string PluginName = "Chatter";
  public const string PluginVersion = "2.12.0";

  public static Harmony HarmonyInstance { get; private set; }

  void Awake() {
    BindConfig(Config);

    HarmonyInstance = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }
}
