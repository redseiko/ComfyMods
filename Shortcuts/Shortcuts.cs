using System;
using System.Reflection;
using System.Reflection.Emit;

using BepInEx;

using HarmonyLib;

using UnityEngine;

using static Shortcuts.PluginConfig;

namespace Shortcuts {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class Shortcuts : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.shortcuts";
    public const string PluginName = "Shortcuts";
    public const string PluginVersion = "1.4.0";

    Harmony _harmony;

    public void Awake() {
      BindConfig(Config);

      if (IsModEnabled.Value) {
        _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
      }
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    public static readonly CodeMatch InputGetKeyDownMatch =
        new(OpCodes.Call, AccessTools.Method(typeof(ZInput), nameof(ZInput.GetKeyDown), new Type[] { typeof(KeyCode) }));

    public static readonly CodeMatch InputGetKeyMatch =
        new(OpCodes.Call, AccessTools.Method(typeof(ZInput), nameof(ZInput.GetKey), new Type[] { typeof(KeyCode) }));
  }
}