﻿using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

namespace ZoneScouter {
  public class PluginConfig {
    public static ConfigEntry<bool> IsModEnabled { get; private set; }

    public static ConfigEntry<bool> ShowSectorInfoPanel { get; private set; }
    public static ConfigEntry<Vector2> SectorInfoPanelPosition { get; private set; }
    public static ConfigEntry<Color> SectorInfoPanelBackgroundColor { get; private set; }

    public static ConfigEntry<int> SectorInfoPanelFontSize { get; private set; }

    public static ConfigEntry<Color> PositionValueXTextColor { get; private set; }
    public static ConfigEntry<Color> PositionValueYTextColor { get; private set; }
    public static ConfigEntry<Color> PositionValueZTextColor { get; private set; }

    public static ConfigEntry<bool> ShowZDOManagerContent { get; private set; }

    public static ConfigEntry<bool> ShowSectorZdoCountGrid { get; private set; }
    public static ConfigEntry<GridSize> SectorZdoCountGridSize { get; private set; }

    public static ConfigEntry<Color> CellZdoCountBackgroundImageColor { get; private set; }
    public static ConfigEntry<int> CellZdoCountTextFontSize { get; private set; }
    public static ConfigEntry<Color> CellZdoCountTextColor { get; private set; }

    public static ConfigEntry<Color> CellSectorBackgroundImageColor { get; private set; }
    public static ConfigEntry<int> CellSectorTextFontSize { get; private set; }
    public static ConfigEntry<Color> CellSectorTextColor { get; private set; }

    public static ConfigEntry<bool> ShowSectorBoundaries { get; private set; }
    public static ConfigEntry<Color> SectorBoundaryColor { get; private set; }

    public enum GridSize {
      ThreeByThree = 3,
      FiveByFive = 5
    }

    public static void BindConfig(ConfigFile config) {
      IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      IsModEnabled.OnSettingChanged(() => ZoneScouter.ToggleSectorInfoPanel());
      IsModEnabled.OnSettingChanged(() => SectorBoundaries.ToggleSectorBoundaries());

      ShowSectorInfoPanel =
          config.BindInOrder(
              "SectorInfoPanel",
              "showSectorInfoPanel",
              true,
              "Show the SectorInfoPanel on the Hud.");

      ShowSectorInfoPanel.OnSettingChanged(() => ZoneScouter.ToggleSectorInfoPanel());

      SectorInfoPanelPosition =
          config.BindInOrder(
              "SectorInfoPanel",
              "sectorInfoPanelPosition",
              new Vector2(0f, -25f),
              "SectorInfoPanel position (relative to pivot/anchors).");

      SectorInfoPanelBackgroundColor =
          config.BindInOrder(
              "SectorInfoPanel",
              "sectorInfoPanelBackgroundColor",
              new Color(0f, 0f, 0f, 0.9f),
              "SectorInfoPanel background color.");

      SectorInfoPanelFontSize =
          config.BindInOrder(
              "SectorInfoPanel.Font",
              "sectorInfoPanelFontSize",
              16,
              "SectorInfoPanel font size.",
              new AcceptableValueRange<int>(2, 64));

      PositionValueXTextColor =
          config.BindInOrder(
              "SectorInfoPanel.PositionRow",
              "positionValueXTextColor",
              new Color(1f, 0.878f, 0.51f),
              "SectorInfoPanel.PositionRow.X value text color.");

      PositionValueYTextColor =
          config.BindInOrder(
              "SectorInfoPanel.PositionRow",
              "positionValueYTextColor",
              new Color(0.565f, 0.792f, 0.976f),
              "SectorInfoPanel.PositionRow.Y value text color.");

      PositionValueZTextColor =
          config.BindInOrder(
              "SectorInfoPanel.PositionRow",
              "positionValueZTextColor",
              new Color(0.647f, 0.839f, 0.655f),
              "SectorInfoPanel.PositionRow.Z value text color.");

      ShowZDOManagerContent =
          config.BindInOrder(
              "SectorInfoPanel.ZDOManagerContent",
              "showZDOManagerContent",
              false,
              "Show SectorInfoPanel.ZDOManager content.");

      ShowZDOManagerContent.OnSettingChanged(
          toggleOn => ZoneScouter.SectorInfoPanel?.ToggleZDOManagerContent(toggleOn));

      ShowSectorZdoCountGrid =
          config.BindInOrder(
              "SectorZdoCountGrid",
              "showSectorZdoCountGrid",
              false,
              "Show the SectorZdoCount grid in the SectorInfo panel.");

      SectorZdoCountGridSize =
          config.BindInOrder(
              "SectorZdoCountGrid",
              "sectorZdoCountGridSize",
              GridSize.ThreeByThree,
              "Size of the SectorZdoCount grid.");

      CellZdoCountBackgroundImageColor =
          config.BindInOrder(
              "SectorZdoCountGrid",
              "cellZdoCountBackgroundImageColor",
              Color.clear,
              "SectorZdoCountCell.ZdoCount.Background.Image color.");

      CellZdoCountTextFontSize =
          config.BindInOrder(
              "SectorZdoCountGrid",
              "cellZdoCountTextFontSize",
              16,
              "SectorZdoCountCell.ZdoCount.Text font size.",
              new AcceptableValueRange<int>(2, 64));

      CellZdoCountTextColor =
          config.BindInOrder(
              "SectorZdoCountGrid",
              "cellZdoCountTextColor",
              Color.white,
              "SectorZdoCountCell.ZdoCount.Text color.");

      CellSectorBackgroundImageColor =
          config.BindInOrder(
              "SectorZdoCountGrid",
              "cellSectorBackgroundImageColor",
              new Color(0.5f, 0.5f, 0.5f, 0.5f),
              "SectorZdoCountCell.Sector.Background.Image color.");

      CellSectorTextFontSize =
          config.BindInOrder(
              "SectorZdoCountGrid",
              "cellSectorTextFontSize",
              16,
              "SectorZdoCountCell.Sector.Text font size.",
              new AcceptableValueRange<int>(2, 64));

      CellSectorTextColor =
          config.BindInOrder(
              "SectorZdoCountGrid",
              "cellSectorTextColor",
              new Color(0.9f, 0.9f, 0.9f, 1f),
              "SectorZdoCountCell.Sector.Text color.");

      ShowSectorBoundaries =
          config.BindInOrder(
              "SectorBoundary",
              "showSectorBoundaries",
              false,
              "Shows sector boundaries using semi-transparent walls at each boundary.");

      SectorBoundaryColor =
          config.BindInOrder(
              "SectorBoundary",
              "sectorBoundaryColor",
              new Color(1f, 0f, 1f, 1f),
              "Color to use for the sector boundary walls.");
    }
  }
}