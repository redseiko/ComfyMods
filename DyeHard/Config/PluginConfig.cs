namespace DyeHard;

using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

public static class PluginConfig {
  public static ConfigFile CurrentConfig { get; private set; }

  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<bool> OverridePlayerHairColor { get; private set; }
  public static ConfigEntry<Color> PlayerHairColor { get; private set; }
  public static ConfigEntry<string> PlayerHairColorHex { get; private set; }
  public static ConfigEntry<float> PlayerHairGlow { get; private set; }

  public static ConfigEntry<bool> OverridePlayerHairItem { get; private set; }
  public static ConfigEntry<string> PlayerHairItem { get; private set; }

  public static ConfigEntry<bool> OverridePlayerBeardItem { get; private set; }
  public static ConfigEntry<string> PlayerBeardItem { get; private set; }

  public static ConfigEntry<Vector3> OffsetCharacterPreviewPosition { get; private set; }

  public static void BindConfig(ConfigFile config) {
    CurrentConfig = config;

    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");

    OverridePlayerHairColor =
        config.BindInOrder(
            "HairColor",
            "overridePlayerHairColor",
            false,
            "Enable/disable overriding your player's hair color.");

    PlayerHairColor =
        config.BindInOrder(
            "HairColor",
            "playerHairColor",
            Color.white,
            "Sets the color for your player's hair.");

    PlayerHairColorHex =
        config.BindInOrder(
            "HairColor",
            "playerHairColorHex",
            "#FFFFFF",
            "Sets the color of player hair, in HTML hex form (alpha unsupported).");

    PlayerHairGlow =
        config.BindInOrder(
            "HairColor",
            "playerHairGlow",
            1f,
            "Hair glow multiplier for the hair color. Zero removes all color.",
            new AcceptableValueRange<float>(0f, 3f));

    OffsetCharacterPreviewPosition =
        config.BindInOrder(
            "Preview",
            "offsetCharacterPreviewPosition",
            new Vector3(1f, 0f, 1f),
            "Offsets the position of the character preview.");

    IsModEnabled.OnSettingChanged(DyeManager.SetPlayerZDOHairColor);

    OverridePlayerHairColor.OnSettingChanged(DyeManager.SetPlayerZDOHairColor);
    PlayerHairColor.OnSettingChanged(UpdatePlayerHairColorHexValue);
    PlayerHairColorHex.OnSettingChanged(UpdatePlayerHairColorValue);
    PlayerHairGlow.OnSettingChanged(DyeManager.SetPlayerZDOHairColor);

    IsModEnabled.OnSettingChanged(DyeManager.SetCharacterPreviewPosition);
    OffsetCharacterPreviewPosition.OnSettingChanged(DyeManager.SetCharacterPreviewPosition);
  }

  static void UpdatePlayerHairColorHexValue() {
    Color color = PlayerHairColor.Value;
    color.a = 1f; // Alpha transparency is unsupported.

    PlayerHairColorHex.Value = $"#{ColorUtility.ToHtmlStringRGB(color)}";
    PlayerHairColor.Value = color;
    DyeManager.SetPlayerZDOHairColor();
  }

  static void UpdatePlayerHairColorValue() {
    if (ColorUtility.TryParseHtmlString(PlayerHairColorHex.Value, out Color color)) {
      color.a = 1f; // Alpha transparency is unsupported.
      PlayerHairColor.Value = color;
      DyeManager.SetPlayerZDOHairColor();
    }
  }

  public static void BindCustomizationConfig(ObjectDB objectDb, PlayerCustomizaton customization) {
    if (OverridePlayerHairItem != null || OverridePlayerBeardItem != null) {
      return;
    }

    string[] hairItems =
        objectDb.GetAllItems(ItemDrop.ItemData.ItemType.Customization, "Hair")
            .Select(item => item.name)
            .AlphanumericSort()
            .ToArray();

    OverridePlayerHairItem =
        CurrentConfig.BindInOrder(
            "Hair",
            "overridePlayerHairItem",
            false,
            "Enable/disable overriding your player's hair.");

    PlayerHairItem =
        CurrentConfig.BindInOrder(
            "Hair",
            "playerHairItem",
            customization.m_noHair.name,
            "If non-empty, sets/overrides the player's hair (if any).",
            new AcceptableValueList<string>(hairItems));

    IsModEnabled.OnSettingChanged(DyeManager.SetPlayerHairItem);
    OverridePlayerHairItem.OnSettingChanged(DyeManager.SetPlayerHairItem);
    PlayerHairItem.OnSettingChanged(DyeManager.SetPlayerHairItem);

    string[] beardItems =
        objectDb.GetAllItems(ItemDrop.ItemData.ItemType.Customization, "Beard")
            .Select(item => item.name)
            .AlphanumericSort()
            .ToArray();

    OverridePlayerBeardItem =
        CurrentConfig.BindInOrder(
            "Beard",
            "overridePlayerBeardItem",
            false,
            "Enable/disable overriding your player's beard.");

    PlayerBeardItem =
        CurrentConfig.BindInOrder(
            "Beard",
            "playerBeardItem",
            customization.m_noBeard.name,
            "If non-empty, sets/overrides the player's beard (if any).",
            new AcceptableValueList<string>(beardItems));

    IsModEnabled.OnSettingChanged(DyeManager.SetPlayerBeardItem);
    OverridePlayerBeardItem.OnSettingChanged(DyeManager.SetPlayerBeardItem);
    PlayerBeardItem.OnSettingChanged(DyeManager.SetPlayerBeardItem);
  }

  public static IEnumerable<string> AlphanumericSort(this IEnumerable<string> text) {
    return text.OrderBy(x => Regex.Replace(x, @"\d+", m => m.Value.PadLeft(50, '0')));
  }
}
