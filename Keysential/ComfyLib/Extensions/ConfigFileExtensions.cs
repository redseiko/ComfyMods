namespace ComfyLib;

using System;
using System.Collections.Generic;

using BepInEx.Configuration;

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
              Browsable = browsable,
              CustomDrawer = customDrawer,
              HideDefaultButton = hideDefaultButton,
              Order = GetSettingOrder(section)
            }));
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

  public static readonly char[] CommaSeparator = new char[] { ',' };

  public static List<string> GetStringList(this ConfigEntry<string> configEntry) {
    string[] entries = configEntry.Value.Split(CommaSeparator, StringSplitOptions.RemoveEmptyEntries);
    List<string> stringList = new(capacity: entries.Length);

    foreach (string entry in entries) {
      stringList.Add(entry.Trim());
    }

    return stringList;
  }

  static readonly Dictionary<ConfigEntry<string>, List<string>> _cachedStringLists = new();

  public static List<string> GetCachedStringList(this ConfigEntry<string> configEntry) {
    if (!_cachedStringLists.TryGetValue(configEntry, out List<string> stringList)) {
      stringList = configEntry.GetStringList();
      _cachedStringLists[configEntry] = stringList;

      configEntry.ConfigFile.ConfigReloaded += (_, _) => RefreshCachedStringList(configEntry);
      configEntry.SettingChanged += (_, _) => RefreshCachedStringList(configEntry);
    }

    return stringList;
  }

  static void RefreshCachedStringList(ConfigEntry<string> configEntry) {
    if (_cachedStringLists.TryGetValue(configEntry, out List<string> stringList)) {
      stringList.Clear();

      string[] entries = configEntry.Value.Split(CommaSeparator, StringSplitOptions.RemoveEmptyEntries);
      stringList.Capacity = entries.Length;

      foreach (string entry in entries) {
        stringList.Add(entry.Trim());
      }
    }
  }

  public static bool IsEmptyOrContains(this List<string> stringList, string entry) {
    return stringList.Count <= 0 || stringList.Contains(entry);
  }

  internal sealed class ConfigurationManagerAttributes {
    public Action<ConfigEntryBase> CustomDrawer;
    public bool? Browsable;
    public bool? HideDefaultButton;
    public int? Order;
  }
}
