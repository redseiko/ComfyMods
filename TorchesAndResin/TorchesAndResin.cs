namespace TorchesAndResin;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class TorchesAndResin : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.torchesandresin";
  public const string PluginName = "TorchesAndResin";
  public const string PluginVersion = "1.8.0";

  void Awake() {
    BindConfig(Config);

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }
}
