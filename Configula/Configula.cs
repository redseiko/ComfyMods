using System;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;

using ConfigurationManager;

using HarmonyLib;

using UnityEngine;

using static Configula.PluginConfig;

namespace Configula {

  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  [BepInDependency(ConfigurationManager.ConfigurationManager.GUID, BepInDependency.DependencyFlags.HardDependency)]
  public class Configula : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.configula";
    public const string PluginName = "Configula";
    public const string PluginVersion = "1.0.0";

    Harmony _harmony;

    void Awake() {
      BindConfig(Config);
      BindSettingDrawHandlers();

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }

    void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    static void BindSettingDrawHandlers() {
      Dictionary<Type, Action<SettingEntryBase>> settingDrawHandlers = SettingFieldDrawer.SettingDrawHandlers;

      settingDrawHandlers[typeof(string)] = StringSettingField.DrawString;
      settingDrawHandlers[typeof(float)] = FloatSettingField.DrawFloat;
      settingDrawHandlers[typeof(Vector2)] = Vector2SettingField.DrawVector2;
      settingDrawHandlers[typeof(Vector3)] = Vector3SettingField.DrawVector3;
      settingDrawHandlers[typeof(Color)] = ColorSettingField.DrawColor;
    }
  }
}
