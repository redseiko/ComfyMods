namespace ComfyLib;

using System;
using System.Collections.Generic;

using BepInEx.Configuration;

public static class CachedStringSetExtension {
  public static readonly char[] CommaSeparator = [','];

  public static HashSet<string> GetStringSet(this ConfigEntry<string> configEntry) {
    string[] entries = configEntry.Value.Split(CommaSeparator, StringSplitOptions.RemoveEmptyEntries);
    HashSet<string> stringSet = new(capacity: entries.Length);

    foreach (string entry in entries) {
      stringSet.Add(entry.Trim());
    }

    return stringSet;
  }

  static readonly Dictionary<ConfigEntry<string>, HashSet<string>> _cachedStringSets = [];

  public static HashSet<string> GetCachedStringSet(this ConfigEntry<string> configEntry) {
    if (!_cachedStringSets.TryGetValue(configEntry, out HashSet<string> stringSet)) {
      stringSet = configEntry.GetStringSet();
      _cachedStringSets[configEntry] = stringSet;

      configEntry.ConfigFile.ConfigReloaded += (_, _) => RefreshCachedStringSet(configEntry);
      configEntry.SettingChanged += (_, _) => RefreshCachedStringSet(configEntry);
    }

    return stringSet;
  }

  static void RefreshCachedStringSet(ConfigEntry<string> configEntry) {
    if (_cachedStringSets.TryGetValue(configEntry, out HashSet<string> stringSet)) {
      stringSet.Clear();

      string[] entries = configEntry.Value.Split(CommaSeparator, StringSplitOptions.RemoveEmptyEntries);

      foreach (string entry in entries) {
        stringSet.Add(entry.Trim());
      }
    }
  }

  public static bool IsEmptyOrContains(this HashSet<string> stringSet, string entry) {
    return stringSet.Count <= 0 || stringSet.Contains(entry);
  }
}
