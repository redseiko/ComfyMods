namespace LicensePlate;

using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

    BindShipNameConfig(config);
    BindCartNameConfig(config);
  }

  public static ConfigEntry<bool> ShowShipNames { get; private set; }
  public static ConfigEntry<float> ShipNameMinimumDistance { get; private set; }
  public static ConfigEntry<float> ShipNameCutoffDistance { get; private set; }
  public static ConfigEntry<Vector3> ShipNameDisplayOffset { get; private set; }
  public static ConfigEntry<float> ShipNameTimeToLive { get; private set; }
  public static ConfigEntry<int> ShipNameFontSize { get; private set; }
  public static ConfigEntry<bool> ShipNameStripHtmlTags { get; private set; }

  static void BindShipNameConfig(ConfigFile config) {
    ShowShipNames =
        config.BindInOrder(
            "ShipName",
            "showShipNames",
            true,
            "Show custom names over ships.");

    ShipNameMinimumDistance =
        config.BindInOrder(
            "ShipName",
            "shipNameMinimumDistance",
            0f,
            "Minimum distance for custom ship names to appear/disappear. Must be less than cutoff distance.",
            new AcceptableValueRange<float>(0f, 20f));

    static void ValidateShipNameMinimumDistance() {
      ShipNameMinimumDistance.Value = Mathf.Min(ShipNameMinimumDistance.Value, ShipNameCutoffDistance.Value);
    }

    ShipNameMinimumDistance.OnSettingChanged(ValidateShipNameMinimumDistance);

    ShipNameCutoffDistance =
        config.BindInOrder(
            "ShipName",
            "shipNameCutoffDistance",
            25f,
            "Cutoff distance for custom ship names to appear/disappear.",
            new AcceptableValueRange<float>(1f, 40f));

    ShipNameCutoffDistance.OnSettingChanged(ValidateShipNameMinimumDistance);

    ShipNameDisplayOffset =
        config.BindInOrder(
            "ShipName",
            "shipNameDisplayOffset",
            Vector3.up,
            "Display offset for custom ship names.");

    ShipNameTimeToLive =
        config.BindInOrder(
            "ShipName",
            "shipNameTimeToLive",
            60f,
            "Time (in seconds) for a custom ship name to be displayed.",
            new AcceptableValueRange<float>(0f, 300f));

    ShipNameFontSize =
        config.BindInOrder(
            "ShipName",
            "shipNameFontSize",
            20,
            "Font size for custom ship names.",
            new AcceptableValueRange<int>(2, 64));

    ShipNameStripHtmlTags =
        config.BindInOrder(
            "ShipName",
            "shipNameStripHtmlTags",
            false,
            "If true, html tags will be stripped from custom ship names when they are displayed.");
  }

  public static ConfigEntry<bool> ShowCartNames { get; private set; }
  public static ConfigEntry<float> CartNameMinimumDistance { get; private set; }
  public static ConfigEntry<float> CartNameCutoffDistance { get; private set; }
  public static ConfigEntry<Vector3> CartNameDisplayOffset { get; private set; }
  public static ConfigEntry<float> CartNameTimeToLive { get; private set; }
  public static ConfigEntry<int> CartNameFontSize { get; private set; }
  public static ConfigEntry<bool> CartNameStripHtmlTags { get; private set; }

  private static void BindCartNameConfig(ConfigFile config) {
    ShowCartNames = config.BindInOrder("CartName", "showCartNames", true, "Show custom names over carts.");

    CartNameMinimumDistance =
        config.BindInOrder(
            "CartName",
            "cartNameMinimumDistance",
            0f,
            "Minimum distance for custom cart names to appear/disappear. Must be less than cutoff distance.",
            new AcceptableValueRange<float>(0f, 10f));

    static void ValidateCartNameMinimumDistance() {
      CartNameMinimumDistance.Value = Mathf.Min(CartNameMinimumDistance.Value, CartNameCutoffDistance.Value);
    }

    CartNameMinimumDistance.OnSettingChanged(ValidateCartNameMinimumDistance);

    CartNameCutoffDistance =
        config.BindInOrder(
            "CartName",
            "cartNameCutoffDistance",
            10f,
            "Cutoff distance for custom cart names to appear/disappear.",
            new AcceptableValueRange<float>(1f, 25f));

    CartNameCutoffDistance.OnSettingChanged(ValidateCartNameMinimumDistance);

    CartNameDisplayOffset =
        config.BindInOrder("CartName", "cartNameDisplayOffset", Vector3.up, "Display offset for custom cart names.");

    CartNameTimeToLive =
        config.BindInOrder(
            "CartName",
            "cartNameTimeToLive",
            60f,
            "Time (in seconds) for a custom cart name to be displayed.",
            new AcceptableValueRange<float>(0f, 300f));

    CartNameFontSize =
        config.BindInOrder(
            "CartName",
            "cartNameFontSize",
            20,
            "Font size for custom cart names.",
            new AcceptableValueRange<int>(2, 64));

    CartNameStripHtmlTags =
        config.BindInOrder(
            "CartName",
            "cartNameStripHtmlTags",
            false,
            "If true, html tags will be stripped from custom cart names when they are displayed.");
  }
}
