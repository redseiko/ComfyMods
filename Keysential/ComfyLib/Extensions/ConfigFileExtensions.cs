namespace ComfyLib;

using System;
using System.Collections.Generic;

using BepInEx.Configuration;

public static class ConfigFileExtensions {
  static readonly Dictionary<string, int> _sectionToSettingOrder = [];

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
      AcceptableValueBase acceptableValues,
      bool browsable = true,
      bool hideDefaultButton = false,
      bool hideSettingName = false,
      bool isAdvanced = false,
      bool readOnly = false) {
    return config.Bind(
        section,
        key,
        defaultValue,
        new ConfigDescription(
            description,
            acceptableValues,
            new ConfigurationManagerAttributes {
              Browsable = browsable,
              CustomDrawer = default,
              HideDefaultButton = hideDefaultButton,
              HideSettingName = hideSettingName,
              IsAdvanced = isAdvanced,
              Order = GetSettingOrder(section),
              ReadOnly = readOnly,
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
      bool hideSettingName = false,
      bool isAdvanced = false,
      bool readOnly = false) {
    return config.Bind(
        section,
        key,
        defaultValue,
        new ConfigDescription(
            description,
            acceptableValues: default,
            new ConfigurationManagerAttributes {
              Browsable = browsable,
              CustomDrawer = customDrawer,
              HideDefaultButton = hideDefaultButton,
              HideSettingName = hideSettingName,
              IsAdvanced = isAdvanced,
              Order = GetSettingOrder(section),
              ReadOnly = readOnly,
            }));
  }

  public static void OnSettingChanged<T>(this ConfigEntry<T> configEntry, Action settingChangedHandler) {
    configEntry.SettingChanged += (_, _) => settingChangedHandler();
  }

  public static void OnSettingChanged<T>(this ConfigEntry<T> configEntry, Action<T> settingChangedHandler) {
    configEntry.SettingChanged +=
        (_, eventArgs) => settingChangedHandler((T) ((SettingChangedEventArgs) eventArgs).ChangedSetting.BoxedValue);
  }

  public static void OnSettingChanged<T>(
      this ConfigEntry<T> configEntry, Action<ConfigEntry<T>> settingChangedHandler) {
    configEntry.SettingChanged +=
        (_, eventArgs) => settingChangedHandler((ConfigEntry<T>) ((SettingChangedEventArgs) eventArgs).ChangedSetting);
  }

  public static readonly char[] CommaSeparator = [','];

  public static List<string> GetStringList(this ConfigEntry<string> configEntry) {
    string[] entries = configEntry.Value.Split(CommaSeparator, StringSplitOptions.RemoveEmptyEntries);
    List<string> stringList = new(capacity: entries.Length);

    foreach (string entry in entries) {
      stringList.Add(entry.Trim());
    }

    return stringList;
  }

  static readonly Dictionary<ConfigEntry<string>, List<string>> _cachedStringLists = [];

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
    public bool? HideSettingName;
    public bool? IsAdvanced;
    public int? Order;
    public bool? ReadOnly;
  }
}
