namespace BetterServerPortals;

using System;
using System.Collections.Generic;
using System.IO;

using UnityEngine;

public sealed class SyncedAuthList {
  readonly string _filename;
  readonly HashSet<string> _entries;

  DateTime _lastLoadTime;
  float _nextCheckLoadTime;

  public SyncedAuthList(string filename, string comment) {
    _filename = filename;
    _entries = [];

    _lastLoadTime = DateTime.MinValue;
    _nextCheckLoadTime = default;

    Initialize(comment);
  }

  void Initialize(string comment) {
    if (!File.Exists(_filename)) {
      using (StreamWriter writer = File.AppendText(_filename)) {
        writer.WriteLine($"// {comment}");
      }
    }

    Reload();
  }

  public int Count() {
    CheckLoad();
    return _entries.Count;
  }

  public bool Contains(string entry) {
    CheckLoad();
    return _entries.Contains(entry);
  }

  public HashSet<string> GetEntries() {
    CheckLoad();
    return _entries;
  }

  public void Reload() {
    try {
      DateTime lastWriteTime = File.GetLastWriteTimeUtc(_filename);

      if (lastWriteTime <= _lastLoadTime) {
        return;
      }

      _lastLoadTime = lastWriteTime;
      _entries.Clear();

      Load();
    } catch (Exception exception) {
      ZLog.LogError(exception);
    }
  }

  void CheckLoad() {
    float realtimeSinceStartup = Time.realtimeSinceStartup;

    if (realtimeSinceStartup > _nextCheckLoadTime) {
      Reload();
      _nextCheckLoadTime = realtimeSinceStartup + 10f;
    }
  }

  void Load() {
    using (StreamReader reader = new(_filename)) {
      string line;

      while ((line = reader.ReadLine()) != null) {
        if (TryParseLine(line, out string entry)) {
          _entries.Add(entry);
        }
      }
    }
  }

  bool TryParseLine(string line, out string entry) {
    entry = string.Empty;

    if (line.Length <= 0) {
      return false;
    }

    if (line.StartsWith("//", StringComparison.Ordinal)) {
      return false;
    }

    entry = line.Trim();

    if (entry.Length <= 0) {
      return false;
    }

    return true;
  }
}
