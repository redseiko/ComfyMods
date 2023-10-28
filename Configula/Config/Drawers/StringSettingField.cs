using System;

using ConfigurationManager;

using UnityEngine;

namespace Configula {
  public class StringSettingField {
    public static readonly Lazy<GUIStyle> StringTextFieldStyle =
        new(() => new(GUI.skin.textField) { wordWrap = true });

    public static void DrawString(SettingEntryBase configEntry) {
      string configValue = (string) configEntry.Get();

      string textValue =
          GUILayout.TextField(
              configValue, StringTextFieldStyle.Value, GUILayout.MinWidth(75f), GUILayout.ExpandWidth(true));

      if (textValue != configValue) {
        configEntry.Set(textValue);
      }
    }
  }
}
