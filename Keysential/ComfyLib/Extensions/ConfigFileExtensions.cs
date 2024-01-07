using System;
using System.Collections.Generic;

using BepInEx.Configuration;

namespace ComfyLib {
  public static class ConfigFileExtensions {
    static readonly char[] _commaSeparator = new char[] { ',' };

    public static List<string> GetStringList(this ConfigEntry<string> configEntry) {
      string[] entries = configEntry.Value.Split(_commaSeparator, StringSplitOptions.RemoveEmptyEntries);
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

        string[] entries = configEntry.Value.Split(_commaSeparator, StringSplitOptions.RemoveEmptyEntries);
        stringList.Capacity = entries.Length;

        foreach (string entry in entries) {
          stringList.Add(entry.Trim());
        }
      }
    }

    public static bool IsEmptyOrContains(this List<string> stringList, string entry) {
      return stringList.Count <= 0 || stringList.Contains(entry);
    }
  }
}
