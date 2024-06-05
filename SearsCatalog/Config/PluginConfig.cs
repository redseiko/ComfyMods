namespace SearsCatalog;

using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<int> BuildHudPanelRows { get; private set; }
  public static ConfigEntry<int> BuildHudPanelColumns { get; private set; }
  public static ConfigEntry<Vector2> BuildHudPanelPosition { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

    BuildHudPanelRows =
        config.BindInOrder(
            "BuildHud.Panel",
            "buildHudPanelRows",
            defaultValue: 6,
            "BuildHud.Panel visible rows (vanilla: 6).",
            new AcceptableValueRange<int>(1, 14));

    BuildHudPanelRows.OnSettingChanged(BuildHudController.SetupBuildHudPanel);

    BuildHudPanelColumns =
        config.BindInOrder(
            "BuildHud.Panel",
            "buildHudPanelColumns",
            defaultValue: 15,
            "BuildHud.Panel visible columns (vanilla: 15).",
            new AcceptableValueRange<int>(1, 26));

    BuildHudPanelColumns.OnSettingChanged(BuildHudController.SetupBuildHudPanel);

    BuildHudPanelPosition =
        config.BindInOrder(
            "BuildHud.Panel",
            "buildHudPanelPosition",
            Vector2.zero,
            "BuildHud.Panel position relative to center of the screen.");

    BuildHudPanelPosition.OnSettingChanged(BuildHudController.SetBuildPanelPosition);
  }
}
