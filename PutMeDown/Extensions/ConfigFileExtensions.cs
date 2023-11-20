using System;
using System.Collections.Generic;

using BepInEx.Configuration;

using HarmonyLib;

namespace ComfyLib {
  public static class ConfigFileExtensions {
    static readonly Dictionary<string, int> _sectionToSettingOrder = new();

    static int GetSettingOrder(string section) {
      if (!_sectionToSettingOrder.TryGetValue(section, out int order)) {
        order = 0;
      }

      _sectionToSettingOrder[section] = order - 1;
      return order;
    }

    public static ConfigEntry<T> BindInOrder<T>(
        this ConfigFile config,
        string section,
        string key,
        T defaultValue,
        string description,
        AcceptableValueBase acceptableValues) {
      return config.Bind(
          section,
          key,
          defaultValue,
          new ConfigDescription(
              description,
              acceptableValues,
              new ConfigurationManagerAttributes {
                Order = GetSettingOrder(section)
              }));
    }

    public static ConfigEntry<T> BindInOrder<T>(
        this ConfigFile config,
        string section,
        string key,
        T defaultValue,
        string description,
        Action<ConfigEntryBase> customDrawer = null,
        bool browsable = true,
        bool hideDefaultButton = false,
        bool hideSettingName = false) {
      return config.Bind(
          section,
          key,
          defaultValue,
          new ConfigDescription(
              description,
              null,
              new ConfigurationManagerAttributes {
                Browsable = true,
                CustomDrawer = customDrawer,
                HideDefaultButton = hideDefaultButton,
                Order = GetSettingOrder(section)
              }));
    }

    public static void LateBindConfig(this ConfigFile config, Action<ConfigFile> lateBindConfigAction) {
      _lateBindConfigQueue.Enqueue(() => lateBindConfigAction.Invoke(config));
    }

    static readonly Queue<Action> _lateBindConfigQueue = new();

    [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.Start))]
    static class LateBindConfigPatch {
      static void Postfix() {
        while (_lateBindConfigQueue.Count > 0) {
          _lateBindConfigQueue.Dequeue()?.Invoke();
        }
      }
    }

    public static void OnSettingChanged<T>(this ConfigEntry<T> configEntry, Action settingChangedHandler) {
      configEntry.SettingChanged += (_, _) => settingChangedHandler();
    }

    public static void OnSettingChanged<T>(this ConfigEntry<T> configEntry, Action<T> settingChangedHandler) {
      configEntry.SettingChanged +=
          (_, eventArgs) =>
              settingChangedHandler.Invoke((T) ((SettingChangedEventArgs) eventArgs).ChangedSetting.BoxedValue);
    }

    public static void OnSettingChanged<T>(
        this ConfigEntry<T> configEntry, Action<ConfigEntry<T>> settingChangedHandler) {
      configEntry.SettingChanged +=
          (_, eventArgs) =>
              settingChangedHandler.Invoke(
                  (ConfigEntry<T>) ((SettingChangedEventArgs) eventArgs).ChangedSetting.BoxedValue);
    }

    internal sealed class ConfigurationManagerAttributes {
      public Action<ConfigEntryBase> CustomDrawer;
      public bool? Browsable;
      public bool? HideDefaultButton;
      public int? Order;
    }
  }
}
