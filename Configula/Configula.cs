namespace Configula;

using System;
using System.Collections.Generic;
using System.Reflection;

using BepInEx;

using ConfigurationManager;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[BepInPlugin(PluginGuid, PluginName, PluginVersion)]
[BepInDependency(ConfigurationManager.GUID, BepInDependency.DependencyFlags.HardDependency)]
public sealed class Configula : BaseUnityPlugin {
  public const string PluginGuid = "redseiko.valheim.configula";
  public const string PluginName = "Configula";
  public const string PluginVersion = "1.2.0";

  void Awake() {
    BindConfig(Config);
    BindSettingDrawHandlers();

    Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
  }

  static void BindSettingDrawHandlers() {
    Dictionary<Type, Action<SettingEntryBase>> settingDrawHandlers = SettingFieldDrawer.SettingDrawHandlers;

    settingDrawHandlers[typeof(string)] = StringSettingField.DrawString;
    settingDrawHandlers[typeof(float)] = FloatSettingField.DrawFloat;
    settingDrawHandlers[typeof(Vector2)] = Vector2SettingField.DrawVector2;
    settingDrawHandlers[typeof(Vector3)] = Vector3SettingField.DrawVector3;
    settingDrawHandlers[typeof(Vector4)] = Vector4SettingField.DrawVector4;
    settingDrawHandlers[typeof(Quaternion)] = QuaternionSettingField.DrawQuaternion;
    settingDrawHandlers[typeof(Color)] = ColorSettingField.DrawColor;
  }
}
