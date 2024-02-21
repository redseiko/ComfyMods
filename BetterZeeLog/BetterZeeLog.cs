namespace BetterZeeLog;

using System.Reflection;

using BepInEx;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[BepInPlugin(PluginGUID, PluginName, PluginVersion)]
public sealed class BetterZeeLog : BaseUnityPlugin {
  public const string PluginGUID = "redseiko.valheim.betterzeelog";
  public const string PluginName = "BetterZeeLog";
  public const string PluginVersion = "1.8.0";

  Harmony _harmony;
    
  void Awake() {
    BindConfig(Config);

    if (IsModEnabled.Value) {
      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);

      if (RemoveStackTraceForNonErrorLogType.Value) {
        Application.SetStackTraceLogType(LogType.Log, StackTraceLogType.None);
        Application.SetStackTraceLogType(LogType.Warning, StackTraceLogType.None);
      }
    }
  }

  void OnDestroy() {
    _harmony?.UnpatchSelf();
  }
}
