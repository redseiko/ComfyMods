namespace Configula;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<int> WindowWidth { get; private set; }
  public static ConfigEntry<int> WindowHeightOffset { get; private set; }

  public static ConfigEntry<bool> UseAlternateDrawTooltip { get; private set; }
  public static ConfigEntry<int> AlternateTooltipWidth { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod (restart required).");

    WindowWidth =
        config.BindInOrder(
            "Window",
            "windowWidth",
            650,
            "Width of the ConfigurationManager window.",
            new AcceptableValueRange<int>(650, 1920));

    WindowHeightOffset =
        config.BindInOrder(
            "Window",
            "windowHeightOffset",
            100,
            "Height (offset) of the ConfigurationManager window.",
            new AcceptableValueRange<int>(0, 540));

    UseAlternateDrawTooltip =
        config.BindInOrder(
            "Tooltip",
            "useAlternateDrawTooltip",
            true,
            "If set, will use alternative DrawTooltip() for customization and Unity-v6 compatibility.");

    AlternateTooltipWidth =
        config.BindInOrder(
            "Tooltip",
            "alternateTooltipWidth",
            400,
            "Width of the alternate tooltip (clamped to window width).",
            new AcceptableValueRange<int>(100, 1000));
  }
}
