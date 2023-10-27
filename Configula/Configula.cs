using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BepInEx;

using ConfigurationManager;

using HarmonyLib;

using UnityEngine;

namespace Configula {

  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  [BepInDependency(ConfigurationManager.ConfigurationManager.GUID, BepInDependency.DependencyFlags.HardDependency)]
  public class Configula : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.configula";
    public const string PluginName = "Configula";
    public const string PluginVersion = "1.0.0";

    Harmony _harmony;

    void Awake() {
      PatchConfigManager();
      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }

    void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    static void PatchConfigManager() {
      Assembly assembly = Assembly.GetAssembly(typeof(ConfigurationManager.ConfigurationManager));
      ZLog.Log($"Assembly: {assembly.FullName}");

      Type settingFieldDrawerType = assembly.GetType("ConfigurationManager.SettingFieldDrawer");
      ZLog.Log($"Type: {settingFieldDrawerType.FullName}");

      Dictionary<Type, Action<SettingEntryBase>> settingDrawHandlers =
          (Dictionary<Type, Action<SettingEntryBase>>)
              AccessTools.Property(settingFieldDrawerType, "SettingDrawHandlers").GetValue(null);
      settingDrawHandlers[typeof(float)] = FloatConfigEntry.DrawFloat;
      settingDrawHandlers[typeof(Vector2)] = VectorConfigEntry.DrawVector2;

      //ConfigurationManager.ConfigurationManager.RegisterCustomSettingDrawer(typeof(float), FloatConfigEntry.DrawFloat);

      ZLog.Log($"Handlers: {string.Join("\n", settingDrawHandlers.Select(p => $"{p.Key.FullName} - {p.Value.Method.Name}"))}");
    }
  }
}
