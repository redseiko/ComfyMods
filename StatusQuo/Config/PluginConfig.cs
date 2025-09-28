namespace StatusQuo;

using BepInEx.Configuration;

using ComfyLib;

using TMPro;

using UnityEngine;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<Vector2> StatusEffectListRectPosition { get; private set; }
  public static ConfigEntry<int> StatusEffectMaxRows { get; private set; }
  public static ConfigEntry<Vector2> StatusEffectRectSizeDelta { get; private set; }
  public static ConfigEntry<Color> StatusEffectBackgroundColor { get; private set; }

  public static ConfigEntry<Color> StatusEffectNameFontColor { get; private set; }
  public static ConfigEntry<float> StatusEffectNameFontSize { get; private set; }
  public static ConfigEntry<bool> StatusEffectNameFontAutoSizing { get; private set; }
  public static ConfigEntry<TextOverflowModes> StatusEffectNameTextOverflowMode { get; set; }
  public static ConfigEntry<TextWrappingModes> StatusEffectNameTextWrappingMode { get; set; }

  public static ConfigEntry<Color> StatusEffectTimeFontColor { get; private set; }
  public static ConfigEntry<float> StatusEffectTimeFontSize { get; private set; }
  public static ConfigEntry<bool> StatusEffectTimeFontAutoSizing { get; private set; }
  public static ConfigEntry<TextOverflowModes> StatusEffectTimeTextOverflowMode { get; private set; }
  public static ConfigEntry<TextWrappingModes> StatusEffectTimeTextWrappingMode { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");

    IsModEnabled.OnSettingChanged(HudUtils.ToggleHudSetup);

    // StatusEffectList

    StatusEffectListRectPosition =
        config.BindInOrder(
            "StatusEffectList",
            "rectPosition",
            new Vector2(-250f, -40f),
            "StatusEffectListRoot<RectTransform>.anchoredPosition value.");

    StatusEffectListRectPosition.OnSettingChanged(HandleStatusEffectListConfigChange);

    // StatusEffect

    StatusEffectMaxRows =
        config.BindInOrder(
            "StatusEffect",
            "maxRows",
            8,
            "StatusEffect max-rows per column; if 0, then max-rows is unlimited.",
            new AcceptableValueRange<int>(0, 20));

    StatusEffectMaxRows.OnSettingChanged(HandleStatusEffectConfigChange);

    StatusEffectRectSizeDelta =
        config.BindInOrder(
            "StatusEffect",
            "rectSizeDelta",
            new Vector2(225f, 30f),
            "StatusEffect<RectTransform>.sizeDelta value.");

    StatusEffectRectSizeDelta.OnSettingChanged(HandleStatusEffectConfigChange);

    StatusEffectBackgroundColor =
        config.BindInOrder(
            "StatusEffect",
            "backgroundColor",
            new Color(0f, 0f, 0f, 0.3f),
            "StatusEffect<Image>.color value.");

    StatusEffectBackgroundColor.OnSettingChanged(HandleStatusEffectConfigChange);

    // StatusEffect.Name

    StatusEffectNameFontColor =
        config.BindInOrder(
            "StatusEffect.Name",
            "fontColor",
            Color.white,
            "StatusEffect.Name<TextMeshProUGUI>.color value.");

    StatusEffectNameFontColor.OnSettingChanged(HandleStatusEffectConfigChange);

    StatusEffectNameFontSize =
        config.BindInOrder(
            "StatusEffect.Name",
            "fontSize",
            16f,
            "StatusEffect.Name<TextMeshProUGUI>.fontSize value.",
            new AcceptableValueRange<float>(0f, 48f));

    StatusEffectNameFontSize.OnSettingChanged(HandleStatusEffectConfigChange);

    StatusEffectNameFontAutoSizing =
        config.BindInOrder(
            "StatusEffect.Name",
            "fontAutoSizing",
            false,
            "StatusEffect.Name<TextMeshProUGUI>.enableAutoSizing value.");

    StatusEffectNameFontAutoSizing.OnSettingChanged(HandleStatusEffectConfigChange);

    StatusEffectNameTextOverflowMode =
        config.BindInOrder(
            "StatusEffect.Name",
            "textOverflowMode",
            TextOverflowModes.Ellipsis,
            "StatusEffect.Name<TextMeshProUGUI>.overflowMode value.",
            new AcceptableValueEnumList<TextOverflowModes>([
              TextOverflowModes.Ellipsis,
              TextOverflowModes.Overflow,
              TextOverflowModes.Truncate,
            ]));

    StatusEffectNameTextOverflowMode.OnSettingChanged(HandleStatusEffectConfigChange);

    StatusEffectNameTextWrappingMode =
        config.BindInOrder(
            "StatusEffect.Name",
            "textWrappingMode",
            TextWrappingModes.Normal,
            "StatusEffect.Name<TextMeshProUGUI>.textWrappingMode value.",
            new AcceptableValueEnumList<TextWrappingModes>([
              TextWrappingModes.Normal,
              TextWrappingModes.NoWrap,
            ]));

    StatusEffectNameTextWrappingMode.OnSettingChanged(HandleStatusEffectConfigChange);

    // StatusEffect.Time

    StatusEffectTimeFontColor =
        config.BindInOrder(
            "StatusEffect.Time",
            "fontColor",
            new Color(1f, 0.717f, 0.36f, 1f),
            "StatusEffect.Time<TextMeshProUGUI>.color value.");

    StatusEffectTimeFontColor.OnSettingChanged(HandleStatusEffectConfigChange);

    StatusEffectTimeFontSize =
        config.BindInOrder(
            "StatusEffect.Time",
            "fontSize",
            18f,
            "StatusEffect.Time<TextMeshProUGUI>.fontSize value.",
            new AcceptableValueRange<float>(0f, 48f));

    StatusEffectTimeFontSize.OnSettingChanged(HandleStatusEffectConfigChange);

    StatusEffectTimeFontAutoSizing =
        config.BindInOrder(
            "StatusEffect.Time",
            "fontAutoSizing",
            false,
            "StatusEffect.Time<TextMeshProUGUI>.enableAutoSizing value.");

    StatusEffectTimeFontAutoSizing.OnSettingChanged(HandleStatusEffectConfigChange);

    StatusEffectTimeTextOverflowMode =
        config.BindInOrder(
            "StatusEffect.Time",
            "textOverflowMode",
            TextOverflowModes.Overflow,
            "StatusEffect.Time<TextMeshProUGUI>.overflowMode value.",
            new AcceptableValueEnumList<TextOverflowModes>([
              TextOverflowModes.Ellipsis,
              TextOverflowModes.Overflow,
              TextOverflowModes.Truncate,
            ]));

    StatusEffectTimeTextOverflowMode.OnSettingChanged(HandleStatusEffectConfigChange);

    StatusEffectTimeTextWrappingMode =
        config.BindInOrder(
            "StatusEffect.Time",
            "textWrappingMode",
            TextWrappingModes.NoWrap,
            "StatusEffect.Time<TextMeshProUGUI>.textWrappingMode value.",
            new AcceptableValueEnumList<TextWrappingModes>([
              TextWrappingModes.Normal,
              TextWrappingModes.NoWrap,
            ]));

    StatusEffectTimeTextWrappingMode.OnSettingChanged(HandleStatusEffectConfigChange);
  }

  static void HandleStatusEffectListConfigChange() {
    if (IsModEnabled.Value && Hud.m_instance) {
      HudUtils.SetupStatusEffectListRoot(Hud.m_instance, toggleOn: true);
    }
  }

  static void HandleStatusEffectConfigChange() {
    if (IsModEnabled.Value && Hud.m_instance) {
      HudUtils.SetupStatusEffects(Hud.m_instance);
    }
  }
}
