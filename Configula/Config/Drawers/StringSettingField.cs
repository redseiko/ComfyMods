namespace Configula;

using ConfigurationManager;

using UnityEngine;

public sealed class StringSettingField {
  public static void DrawString(SettingEntryBase configEntry) {
    string configValue = (string) configEntry.Get();

    string textValue =
        GUILayout.TextField(
            configValue, GUIResources.WordWrapTextField.Value, GUILayout.MinWidth(75f), GUILayout.ExpandWidth(true));

    if (textValue != configValue) {
      configEntry.Set(textValue);
    }
  }
}
