﻿namespace Shortcuts;

using System;
using System.Linq;

using BepInEx.Configuration;

using UnityEngine;

public sealed class ShortcutConfigEntry {
  public readonly ConfigEntry<KeyboardShortcut> BaseConfigEntry;

  KeyboardShortcut _shortcut = KeyboardShortcut.Empty;
  KeyCode _mainKey = KeyCode.None;
  KeyCode[] _modifierKeys = new KeyCode[0];
  int _modifierKeyCount = 0;

  public ShortcutConfigEntry(ConfigEntry<KeyboardShortcut> configEntry) {
    BaseConfigEntry = configEntry;
    BaseConfigEntry.SettingChanged += OnSettingChanged;

    SetShortcut(BaseConfigEntry.Value);
  }

  void OnSettingChanged(object sender, EventArgs eventArgs) {
    SetShortcut(((ConfigEntry<KeyboardShortcut>) ((SettingChangedEventArgs) eventArgs).ChangedSetting).Value);
  }

  public void SetShortcut(KeyboardShortcut shortcut) {
    _shortcut = shortcut;
    _mainKey = _shortcut.MainKey;
    _modifierKeys = _shortcut.Modifiers.ToArray();
    _modifierKeyCount = _modifierKeys.Length;
  }

  public bool IsKeyDown() {
    if (!ZInput.m_instance.Input_GetKeyDown(_mainKey, false)) {
      return false;
    }

    if (_modifierKeyCount > 0) {
      for (int i = 0; i < _modifierKeyCount; i++) {
        if (!ZInput.m_instance.Input_GetKey(_modifierKeys[i])) {
          return false;
        }
      }
    }

    return true;
  }
}
