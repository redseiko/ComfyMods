using System.Reflection;

using BepInEx;

using HarmonyLib;

using UnityEngine;

using static BetterZeeLog.PluginConfig;

namespace BetterZeeLog {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class BetterZeeLog : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.betterzeelog";
    public const string PluginName = "BetterZeeLog";
    public const string PluginVersion = "1.6.0";

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
}
