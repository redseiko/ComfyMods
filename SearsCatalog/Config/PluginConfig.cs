namespace SearsCatalog;

using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<int> BuildHudPanelRows { get; private set; }
  public static ConfigEntry<int> BuildHudPanelColumns { get; private set; }
  public static ConfigEntry<Vector2> BuildHudPanelPosition { get; private set; }

  public static ConfigEntry<float> BuildHudPanelScrollSensitivity { get; private set; }

  public static ConfigEntry<Vector2> CategoriesRectPosition { get; private set; }
  public static ConfigEntry<Vector2> CategoriesRectSizeDelta { get; private set; }

  public static ConfigEntry<bool> TabBorderIsEnabled { get; private set; }
  public static ConfigEntry<Vector2> TabBorderRectPosition { get; private set; }
  public static ConfigEntry<Vector2> TabBorderRectSizeDelta { get; private set; }

  public static ConfigEntry<bool> InputHelpIsEnabled { get; private set; }
  public static ConfigEntry<Vector2> InputHelpRectPosition { get; private set; }
  public static ConfigEntry<Vector2> InputHelpRectSizeDelta { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");

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

    BuildHudPanelScrollSensitivity =
        config.BindInOrder(
            "BuildHud.Panel",
            "buildHudPanelScrollSensitivity",
            1400f,
            "BuildHud.Panel scroll-sensitivity when using the mouse-wheel.",
            new AcceptableValueRange<float>(0f, 3000f));

    BuildHudPanelScrollSensitivity.OnSettingChanged(BuildHudController.SetBuildPanelScrollSensitivity);

    CategoriesRectPosition =
        config.BindInOrder(
            "BuildHud.Panel.Categories",
            "categoriesRectPosition",
            new Vector2(0f, -39f),
            "Hud.m_pieceCategoryRoot<RectTransform>.anchoredPosition value (vanilla is X: 0, Y: -39).");

    CategoriesRectPosition.OnSettingChanged(BuildHudController.HandleCategoriesConfigChange);

    CategoriesRectSizeDelta =
        config.BindInOrder(
            "BuildHud.Panel.Categories",
            "categoriesRectSizeDelta",
            new Vector2(-20f, 32.8127f),
            "Hud.m_pieceCategoryRoot<RectTransform>.sizeDelta value (vanilla is X: -180, Y: 32.8127).");

    CategoriesRectSizeDelta.OnSettingChanged(BuildHudController.HandleCategoriesConfigChange);

    TabBorderIsEnabled =
        config.BindInOrder(
            "BuildHud.Panel.TabBorder",
            "tabBorderIsEnabled",
            true,
            "Hud.m_pieceCategoryRoot.TabBorder<GameObject>.activeSelf value (vanilla is: true).");

    TabBorderIsEnabled.OnSettingChanged(BuildHudController.HandleCategoriesConfigChange);

    TabBorderRectPosition =
        config.BindInOrder(
            "BuildHud.Panel.TabBorder",
            "tabBorderRectPosition",
            new Vector2(0f, -31.056f),
            "Hud.m_pieceCategoryRoot.TabBorder<RectTransform>.anchoredPosition value (vanilla is X: 0, Y: -31.056).");

    TabBorderRectPosition.OnSettingChanged(BuildHudController.HandleCategoriesConfigChange);

    TabBorderRectSizeDelta =
        config.BindInOrder(
            "BuildHud.Panel.TabBorder",
            "tabBorderSizeDelta",
            new Vector2(-30f, 4f),
            "Hud.m_pieceCategoryRoot.TabBorder<RectTransform>.sizeDelta value (vanilla is X: 130, Y: 4).");

    TabBorderRectSizeDelta.OnSettingChanged(BuildHudController.HandleCategoriesConfigChange);

    InputHelpIsEnabled =
        config.BindInOrder(
            "BuildHud.Panel.InputHelp",
            "inputHelpIsEnabled",
            false,
            "Hud.m_pieceSelectionWindow.InputHelp<GameObject>.activeSelf value (vanilla is: true).");

    InputHelpIsEnabled.OnSettingChanged(BuildHudController.HandleCategoriesConfigChange);

    InputHelpRectPosition =
        config.BindInOrder(
            "BuildHud.Panel.InputHelp",
            "inputHelpRectPosition",
            new Vector2(-89.84f, -278.5f),
            "Hud.m_pieceSelectionWindow.InputHelp<RectTransform>.anchoredPosition value "
                + "(vanilla is X: -89.84, Y: -278.5).");

    InputHelpRectPosition.OnSettingChanged(BuildHudController.HandleCategoriesConfigChange);

    InputHelpRectSizeDelta =
        config.BindInOrder(
            "BuildHud.Panel.InputHelp",
            "inputHelpRectSizeDelta",
            new Vector2(-984f, 100f),
            "Hud.m_pieceSelectionWindow.InputHelp<RectTransform>.sizeDelta value (vanilla is X: -984, Y: 100).");

    InputHelpRectSizeDelta.OnSettingChanged(BuildHudController.HandleCategoriesConfigChange);
  }
}
