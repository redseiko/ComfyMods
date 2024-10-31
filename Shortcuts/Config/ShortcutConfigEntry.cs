﻿namespace Shortcuts;

using System;
using System.Linq;

using BepInEx.Configuration;

using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.Controls;

public sealed class ShortcutConfigEntry {
  public readonly ConfigEntry<KeyboardShortcut> BaseConfigEntry;

  KeyboardShortcut _shortcut = KeyboardShortcut.Empty;
  KeyCode _mainKey = KeyCode.None;
  KeyCode[] _modifierKeys = [];
  int _modifierKeyCount = 0;

  ButtonControl _mainKeyControl = default;
  ButtonControl[] _modifierKeyControls = [];

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

    _mainKeyControl = GetButtonControl(_mainKey);
    _modifierKeyControls = _modifierKeyCount > 0 ? new ButtonControl[_modifierKeyCount] : [];

    for (int i = 0; i < _modifierKeyCount; i++) {
      _modifierKeyControls[i] = GetButtonControl(_modifierKeys[i]);
    }
  }

  public static ButtonControl GetButtonControl(KeyCode keyCode) {
    return (ButtonControl) InputSystem.FindControl(ZInput.KeyCodeToPath(keyCode));
  }

  public bool IsKeyDown() {
    if (_mainKeyControl == null || !_mainKeyControl.wasPressedThisFrame) {
      return false;
    }

    if (_modifierKeyCount > 0) {
      for (int i = 0; i < _modifierKeyCount; i++) {
        if (!_modifierKeyControls[i].isPressed) {
          return false;
        }
      }
    }

    return true;
  }
}
