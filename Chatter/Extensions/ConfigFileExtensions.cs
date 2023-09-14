using System;
using System.Collections.Generic;

using BepInEx.Configuration;

using Chatter;

using TMPro;

namespace ComfyLib {
  public static class ConfigFileExtensions {
    static readonly Dictionary<string, int> _sectionToOrderCache = new();

    static int GetSettingOrder(string section) {
      if (!_sectionToOrderCache.TryGetValue(section, out int order)) {
        order = 0;
      }

      _sectionToOrderCache[section] = order - 1;
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
                HideSettingName = hideSettingName,
                Order = GetSettingOrder(section)
              }));
    }

    public static StringListConfigEntry BindInOrder(
        this ConfigFile config, string section, string key, string description, string valuesSeparator) {
      return new StringListConfigEntry(config, section, key, description, valuesSeparator);
    }

    public static void OnSettingChanged<T>(
        this ConfigEntry<T> configEntry, Action settingChangedHandler) {
      configEntry.SettingChanged += (_, _) => settingChangedHandler();
    }

    public static void OnSettingChanged<T>(
        this ConfigEntry<T> configEntry, Action<T> settingChangedHandler) {
      configEntry.SettingChanged += (_, _) => settingChangedHandler(configEntry.Value);
    }

    internal sealed class ConfigurationManagerAttributes {
      public Action<ConfigEntryBase> CustomDrawer;
      public bool? Browsable;
      public bool? HideDefaultButton;
      public bool? HideSettingName;
      public int? Order;
    }
  }
}
