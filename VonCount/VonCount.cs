namespace VonCount;

using System.Reflection;

using BepInEx;

using HarmonyLib;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
public sealed class VonCount : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.voncount";
  public const string PluginName = "VonCount";
  public const string PluginVersion = "1.0.0";

  void Awake() {
    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }
}
