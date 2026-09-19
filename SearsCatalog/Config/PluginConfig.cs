namespace SearsCatalog;

using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<bool> BuildUiCanMovePanel { get; private set; }
  public static ConfigEntry<Vector2> BuildUiPanelPosition { get; private set; }

  public static ConfigEntry<bool> BuildUiCanResizePanel { get; private set; }
  public static ConfigEntry<Vector2> BuildUiPanelSizeDelta { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");

    BuildUiCanMovePanel =
        config.BindInOrder(
            "BuildUi.Panel",
            "canMovePanel",
            defaultValue: true,
            "Allow moving the BuildUi panel.");

    BuildUiPanelPosition =
        config.BindInOrder(
            "BuildUi.Panel",
            "panelPosition",
            defaultValue: Vector2.zero,
            "BuildUi panel RectTransform.anchoredPosition value.");

    BuildUiCanResizePanel =
        config.BindInOrder(
            "BuildUi.Panel",
            "canResizePanel",
            defaultValue: true,
            "Allow resizing the BuildUi panel.");

    BuildUiPanelSizeDelta =
        config.BindInOrder(
            "BuildUi.Panel",
            "panelSizeDelta",
            defaultValue: Vector2.zero,
            "BuildUi panel RectTransform.sizeDelta value.");
  }
}
