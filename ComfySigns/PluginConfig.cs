using System;
using System.Collections.Generic;
using System.Linq;

using BepInEx.Configuration;

using ComfyLib;

using HarmonyLib;

using TMPro;

using UnityEngine;

namespace ComfySigns {
  public static class PluginConfig {
    public static ConfigEntry<bool> IsModEnabled { get; private set; }
    public static ConfigEntry<bool> UseFallbackFonts { get; private set; }

    public static void BindConfig(ConfigFile config) {
      IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");
      UseFallbackFonts = config.BindInOrder("Fonts", "useFallbackFonts", true, "Use fallback fonts to support additional characters.");

      _fejdStartupBindConfigQueue.Enqueue(() => BindLoggingConfig(config));
      _fejdStartupBindConfigQueue.Enqueue(() => BindSignConfig(config));
      _fejdStartupBindConfigQueue.Enqueue(() => BindSignEffectConfig(config));
    }

    public static ConfigEntry<bool> SuppressUnicodeNotFoundWarning { get; private set; }

    public static void BindLoggingConfig(ConfigFile config) {
      SuppressUnicodeNotFoundWarning =
          config.BindInOrder(
              "Logging",
              "suppressUnicodeNotFoundWarning",
              true,
              "Hide 'The character with Unicode value ... was not found...' log warnings.");

      SuppressUnicodeNotFoundWarning.SettingChanged +=
          (_, _) => TMP_Settings.instance.m_warningsDisabled = SuppressUnicodeNotFoundWarning.Value;

      TMP_Settings.instance.m_warningsDisabled = SuppressUnicodeNotFoundWarning.Value;
    }

    public static ConfigEntry<string> SignDefaultTextFont { get; private set; }
    public static ExtendedColorConfigEntry SignDefaultTextColor { get; private set; }

    public static ConfigEntry<bool> SignTextIgnoreSizeTags { get; private set; }

    public static void BindSignConfig(ConfigFile config) {
      string[] fontNames =
          Resources.FindObjectsOfTypeAll<Font>()
              .Select(f => f.name)
              .Concat(Resources.FindObjectsOfTypeAll<TMP_FontAsset>().Select(f => f.name))
              .OrderBy(f => f)
              .ToArray();

      SignDefaultTextFont =
          config.BindInOrder(
              "Sign.Text",
              "defaultTextFont",
              "Valheim-Norse",
              "Sign.m_textWidget.font default value.",
              new AcceptableValueList<string>(fontNames));

      SignDefaultTextFont.SettingChanged += ComfySigns.OnSignConfigChanged;

      SignDefaultTextColor =
          new(config,
              "Sign.Text",
              "defaultTextColor",
              Color.white,
              "Sign.m_textWidget.color default value.");

      SignDefaultTextColor.ConfigEntry.SettingChanged += ComfySigns.OnSignConfigChanged;

      SignTextIgnoreSizeTags =
          config.Bind(
              "Sign.Text.Tags",
              "ignoreSizeTags",
              false,
              "if set, ignore any and all <size> tags in sign text when rendered locally.");

      SignTextIgnoreSizeTags.SettingChanged += ComfySigns.OnSignTextTagsConfigChanged;
    }

    public static ConfigEntry<float> SignEffectMaximumRenderDistance { get; private set; }
    public static ConfigEntry<bool> SignEffectEnablePartyEffect{ get; private set; }

    public static void BindSignEffectConfig(ConfigFile config) {
      SignEffectMaximumRenderDistance =
          config.BindInOrder(
              "SignEffect",
              "maximumRenderDistance",
              64f,
              "Maximum distance that signs can be from player to render sign effects.",
              new AcceptableValueRange<float>(0f, 128f));

      SignEffectMaximumRenderDistance.SettingChanged += ComfySigns.OnSignEffectConfigChanged;

      SignEffectEnablePartyEffect =
          config.BindInOrder(
              "SignEffect.Party",
              "enablePartyEffect",
              false,
              "Enables the 'Party' Sign effect for signs using the party tag.");

      SignEffectEnablePartyEffect.SettingChanged += ComfySigns.OnSignEffectConfigChanged;
    }

    static readonly Queue<Action> _fejdStartupBindConfigQueue = new();

    [HarmonyPatch(typeof(FejdStartup))]
    static class FejdStartupPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(FejdStartup.Awake))]
      static void AwakePostfix() {
        while (_fejdStartupBindConfigQueue.Count > 0) {
          _fejdStartupBindConfigQueue.Dequeue()?.Invoke();
        }
      }
    }
  }
}
