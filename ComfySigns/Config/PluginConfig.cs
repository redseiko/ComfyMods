namespace ComfySigns;

using System.Linq;

using BepInEx.Configuration;

using ComfyLib;

using TMPro;

using UnityEngine;

public static class PluginConfig {
  public static ConfigFile CurrentConfig { get; private set; }

  public static ConfigEntry<bool> IsModEnabled { get; private set; }
  public static ConfigEntry<bool> UseFallbackFonts { get; private set; }

  [ComfyConfig]
  public static void BindConfig(ConfigFile config) {
    CurrentConfig = config;

    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");

    UseFallbackFonts =
        config.BindInOrder(
            "Fonts",
            "useFallbackFonts",
            true,
            "Use fallback fonts to support additional characters.");
  }

  public static ConfigEntry<bool> SuppressUnicodeNotFoundWarning { get; private set; }

  [ComfyConfig(typeof(FejdStartup), nameof(FejdStartup.Awake))]
  public static void BindLoggingConfig(ConfigFile config) {
    SuppressUnicodeNotFoundWarning =
        config.BindInOrder(
            "Logging",
            "suppressUnicodeNotFoundWarning",
            true,
            "Hide 'The character with Unicode value ... was not found...' log warnings.");

    SuppressUnicodeNotFoundWarning.OnSettingChanged(SetWarningsDisabled);
    SetWarningsDisabled(SuppressUnicodeNotFoundWarning.Value);
  }

  static void SetWarningsDisabled(bool warningsDiabled) {
    TMP_Settings.instance.m_warningsDisabled = warningsDiabled;
  }

  public static ConfigEntry<string> SignDefaultTextFontAsset { get; private set; }
  public static ConfigEntry<Color> SignDefaultTextFontColor { get; private set; }

  public static ConfigEntry<float> SignTextMaximumRenderDistance { get; private set; }
  public static ConfigEntry<bool> SignTextIgnoreSizeTags { get; private set; }

  [ComfyConfig(typeof(FejdStartup), nameof(FejdStartup.Awake))]
  public static void BindSignConfig(ConfigFile config) {
    string[] fontNames =
        Resources.FindObjectsOfTypeAll<TMP_FontAsset>()
            .Select(f => f.name)
            .Concat(Resources.FindObjectsOfTypeAll<Font>().Select(f => f.name))
            .ToArray();

    SignDefaultTextFontAsset =
        config.BindInOrder(
            "Sign.Text",
            "defaultTextFontAsset",
            "Valheim-Norse",
            "Sign.m_textWidget.fontAsset (TMP) default value.",
            new AcceptableValueList<string>(fontNames));

    SignDefaultTextFontAsset.OnSettingChanged(SignUtils.OnSignConfigChanged);

    SignDefaultTextFontColor =
        config.BindInOrder(
            "Sign.Text",
            "defaultTextFontColor",
            Color.white,
            "Sign.m_textWidget.color default value.");

    SignDefaultTextFontColor.OnSettingChanged(SignUtils.OnSignConfigChanged);

    SignTextMaximumRenderDistance =
        config.BindInOrder(
            "Sign.Text.Render",
            "maximumRenderDistance",
            192f,
            "Maximum distance that signs can be from player to render sign text.",
            new AcceptableValueRange<float>(0f, 192f));

    SignTextIgnoreSizeTags =
        config.Bind(
            "Sign.Text.Tags",
            "ignoreSizeTags",
            false,
            "if set, ignore any and all <size> tags in sign text when rendered locally.");

    SignTextIgnoreSizeTags.OnSettingChanged(SignUtils.OnSignTextTagsConfigChanged);
  }

  public static ConfigEntry<float> SignEffectMaximumRenderDistance { get; private set; }
  public static ConfigEntry<bool> SignEffectEnablePartyEffect { get; private set; }

  [ComfyConfig(typeof(FejdStartup), nameof(FejdStartup.Awake))]
  public static void BindSignEffectConfig(ConfigFile config) {
    SignEffectMaximumRenderDistance =
        config.BindInOrder(
            "SignEffect",
            "maximumRenderDistance",
            64f,
            "Maximum distance that signs can be from player to render sign effects.",
            new AcceptableValueRange<float>(0f, 128f));

    SignEffectMaximumRenderDistance.OnSettingChanged(SignUtils.OnSignEffectConfigChanged);

    SignEffectEnablePartyEffect =
        config.BindInOrder(
            "SignEffect.Party",
            "enablePartyEffect",
            false,
            "Enables the 'Party' Sign effect for signs using the party tag.");

    SignEffectEnablePartyEffect.OnSettingChanged(SignUtils.OnSignEffectConfigChanged);
  }
}
