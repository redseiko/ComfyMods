using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

namespace SearsCatalog {
  public static class PluginConfig {
    public static ConfigEntry<bool> IsModEnabled { get; private set; }

    public static ConfigEntry<int> BuildHudPanelRows { get; private set; }
    public static ConfigEntry<int> BuildHudPanelColumns { get; private set; }

    public static ConfigEntry<Vector2> BuildHudPanelPosition { get; private set; }

    public static ConfigEntry<float> CategoryRootSizeWidthOffset { get; private set; }
    public static ConfigEntry<float> TabBorderSizeWidthOffset { get; private set; }
    public static ConfigEntry<Vector2> InputHelpSizeDeltaOffset { get; private set; }

    public static void BindConfig(ConfigFile config) {
      IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      BuildHudPanelRows =
          config.BindInOrder(
              "BuildHud.Panel",
              "buildHudPanelRows",
              defaultValue: 6,
              "BuildHud.Panel visible rows (vanilla: 6).",
              new AcceptableValueRange<int>(1, 14));

      BuildHudPanelRows.OnSettingChanged(SearsCatalog.SetupBuildHudPanel);

      BuildHudPanelColumns =
          config.BindInOrder(
              "BuildHud.Panel",
              "buildHudPanelColumns",
              defaultValue: 15,
              "BuildHud.Panel visible columns (vanilla: 15).",
              new AcceptableValueRange<int>(1, 26));

      BuildHudPanelColumns.OnSettingChanged(SearsCatalog.SetupBuildHudPanel);

      BuildHudPanelPosition =
          config.BindInOrder(
              "BuildHud.Panel",
              "buildHudPanelPosition",
              Vector2.zero,
              "BuildHud.Panel position relative to center of the screen.");

      CategoryRootSizeWidthOffset =
          config.BindInOrder(
              "BuildHud.Panel.PieceSelection",
              "categoryRootSizeWidthOffset",
              defaultValue: -155f,
              "BuildHud.Panel.CategoryRoot.sizeDelta width offset relative to panel width.");

      CategoryRootSizeWidthOffset.OnSettingChanged(SearsCatalog.SetupPieceSelectionWindow);

      TabBorderSizeWidthOffset =
          config.BindInOrder(
              "BuildHud.Panel.PieceSelection",
              "tabBorderSizeWidthOffset",
              defaultValue: -45f,
              "BuildHud.Panel.PieceSelection.TabBorder.sizeDelta width offset relative to panel width.");

      TabBorderSizeWidthOffset.OnSettingChanged(SearsCatalog.SetupPieceSelectionWindow);

      InputHelpSizeDeltaOffset =
          config.BindInOrder(
              "BuildHud.Panel.PieceSelection",
              "inputHelpSizeDeltaOffset",
              defaultValue: new Vector2(-85f, 0f),
              "BuildHud.Panel.PieceSelection.InputHelp.sizeDelta offset relative to panel size.");

      InputHelpSizeDeltaOffset.OnSettingChanged(SearsCatalog.SetupPieceSelectionWindow);
    }
  }
}
