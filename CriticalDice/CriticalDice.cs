namespace CriticalDice;

using System.Reflection;

using BepInEx;

using BetterZeeRouter;

using HarmonyLib;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
[BepInDependency(BetterZeeRouter.PluginGuid, BepInDependency.DependencyFlags.HardDependency)]
public sealed class CriticalDice : BaseUnityPlugin {
  public const string PluginGUID = "redseiko.valheim.criticaldice";
  public const string PluginName = "CriticalDice";
  public const string PluginVersion = "1.7.0";

  void Awake() {
    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);

    SayHandler.Register(); 
  }
}
