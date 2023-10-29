using System.Collections.Generic;

using ConfigurationManager;

using UnityEngine;

namespace Configula {
  public class ColorSettingField {
    public readonly ColorFloatInputField RedInput = new("R");
    public readonly ColorFloatInputField GreenInput = new("G");
    public readonly ColorFloatInputField BlueInput = new("B");
    public readonly ColorFloatInputField AlphaInput = new("A");
    public readonly HexColorInputField HexInput = new();
    public bool ShowSliders = false;

    public void SetValue(Color value) {
      RedInput.SetValue(value.r);
      GreenInput.SetValue(value.g);
      BlueInput.SetValue(value.b);
      AlphaInput.SetValue(value.a);
      HexInput.SetValue(value);
    }

    public Color GetValue() {
      return new(RedInput.CurrentValue, GreenInput.CurrentValue, BlueInput.CurrentValue, AlphaInput.CurrentValue);
    }

    static readonly Dictionary<SettingEntryBase, ColorSettingField> _colorSettingFieldCache = new();
    static readonly Texture2D _colorTexture = GUIHelper.CreateColorTexture(10, 10, Color.white);

    public static void DrawColor(SettingEntryBase configEntry) {
      Color configValue = (Color) configEntry.Get();

      if (!_colorSettingFieldCache.TryGetValue(configEntry, out ColorSettingField colorField)) {
        colorField = new();
        colorField.SetValue(configValue);

        _colorSettingFieldCache[configEntry] = colorField;
      } else if (GUIFocus.HasChanged() || GUIHelper.IsEnterPressed() || colorField.GetValue() != configValue) {
        colorField.SetValue(configValue);
      }

      GUILayout.BeginVertical(GUI.skin.box);
      GUILayout.BeginHorizontal();

      colorField.HexInput.DrawField();

      GUILayout.Space(3f);
      GUIHelper.BeginColor(configValue);
      GUILayout.Label(string.Empty, GUILayout.ExpandWidth(true));

      if (Event.current.type == EventType.Repaint) {
        GUI.DrawTexture(GUILayoutUtility.GetLastRect(), _colorTexture);
      }

      GUIHelper.EndColor();
      GUILayout.Space(3f);

      if (GUILayout.Button(
              colorField.ShowSliders ? "\u2228" : "\u2261",
              GUILayout.MinWidth(40f),
              GUILayout.ExpandWidth(false))) {
        colorField.ShowSliders = !colorField.ShowSliders;
      }

      GUILayout.EndHorizontal();

      if (colorField.ShowSliders) {
        GUILayout.Space(4f);
        GUILayout.BeginHorizontal();

        colorField.RedInput.DrawField();
        GUILayout.Space(3f);
        colorField.GreenInput.DrawField();
        GUILayout.Space(3f);
        colorField.BlueInput.DrawField();
        GUILayout.Space(3f);
        colorField.AlphaInput.DrawField();

        GUILayout.EndHorizontal();
      }

      GUILayout.EndVertical();

      Color value = colorField.GetValue();

      if (value == configValue) {
        value = colorField.HexInput.CurrentValue;
      }

      if (value != configValue) {
        configEntry.Set(value);
        colorField.SetValue(value);
      }
    }
  }
}
