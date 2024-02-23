namespace Configula;

using System.Collections.Generic;

using ConfigurationManager;

using UnityEngine;

public sealed class ColorSettingField {
  public readonly ColorFloatInputField RedInput = new("R");
  public readonly ColorFloatInputField GreenInput = new("G");
  public readonly ColorFloatInputField BlueInput = new("B");
  public readonly ColorFloatInputField AlphaInput = new("A");
  public readonly HexColorInputField HexInput = new();

  Color _value;
  bool _showSliders = false;

  public void SetValue(Color value) {
    _value = value;

    RedInput.SetValue(value.r);
    GreenInput.SetValue(value.g);
    BlueInput.SetValue(value.b);
    AlphaInput.SetValue(value.a);
    HexInput.SetValue(value);
  }

  public Color GetValue() {
    return new(RedInput.CurrentValue, GreenInput.CurrentValue, BlueInput.CurrentValue, AlphaInput.CurrentValue);
  }

  public void DrawField() {
    GUILayout.BeginVertical();
    GUILayout.BeginHorizontal();

    HexInput.DrawField();

    GUILayout.Space(3f);
    GUIHelper.BeginColor(_value);
    GUILayout.Label(string.Empty, GUILayout.ExpandWidth(true));

    if (Event.current.type == EventType.Repaint) {
      GUI.DrawTexture(GUILayoutUtility.GetLastRect(), _colorTexture);
    }

    GUIHelper.EndColor();
    GUILayout.Space(3f);

    if (GUILayout.Button(_showSliders ? "\u2207" : "\u2261", GUILayout.MinWidth(45f), GUILayout.ExpandWidth(false))) {
      _showSliders = !_showSliders;
    }

    GUILayout.EndHorizontal();

    if (_showSliders) {
      GUILayout.Space(4f);
      GUILayout.BeginHorizontal(GUI.skin.box);

      RedInput.DrawField();
      GUILayout.Space(3f);
      GreenInput.DrawField();
      GUILayout.Space(3f);
      BlueInput.DrawField();
      GUILayout.Space(3f);
      AlphaInput.DrawField();

      GUILayout.EndHorizontal();
    }

    GUILayout.EndVertical();
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

    colorField.DrawField();
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
