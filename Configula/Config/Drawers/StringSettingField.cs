using ConfigurationManager;

using UnityEngine;

namespace Configula {
  public class StringSettingField {
    public static void DrawString(SettingEntryBase configEntry) {
      string configValue = (string) configEntry.Get();
      string textValue = GUILayout.TextArea(configValue, GUILayout.MinWidth(75f), GUILayout.ExpandWidth(true));

      if (textValue != configValue) {
        configEntry.Set(textValue);
      }
    }
  }
}
