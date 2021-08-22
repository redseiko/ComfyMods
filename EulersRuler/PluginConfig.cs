using BepInEx.Configuration;
using System;
using System.Linq;
using UnityEngine;

namespace EulersRuler {
  class PluginConfig {
    [Flags]
    internal enum HoverPiecePanelRow {
      None = 0,
      Name = 1,
      Health = 2,
      Stability = 4,
      Euler = 8,
      Quaternion = 16,
    }

    [Flags]
    internal enum PlacementGhostPanelRow {
      None = 0,
      Name = 1,
      Euler = 2,
      Quaternion = 4,
    }

    internal static ConfigEntry<bool> _isModEnabled;

    internal static ConfigEntry<Vector2> _hoverPiecePanelPosition;
    internal static ConfigEntry<HoverPiecePanelRow> _hoverPiecePanelEnabledRows;
    internal static ConfigEntry<int> _hoverPiecePanelFontSize;
    internal static ConfigEntry<bool> _showHoverPieceHealthBar;

    internal static ConfigEntry<Vector2> _placementGhostPanelPosition;
    internal static ConfigEntry<PlacementGhostPanelRow> _placementGhostPanelEnabledRows;
    internal static ConfigEntry<int> _placementGhostPanelFontSize;

    internal static void CreateConfig(ConfigFile config) {
      _isModEnabled = config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      _hoverPiecePanelPosition =
          config.Bind(
              "HoverPiecePanel",
              "hoverPiecePanelPosition",
              new Vector2(0, 225),
              "Position of the HoverPiece properties panel.");

      _hoverPiecePanelEnabledRows =
          config.Bind(
              "HoverPiecePanel",
              "hoverPiecePanelEnabledRows",
              HoverPiecePanelRow.Name | HoverPiecePanelRow.Health | HoverPiecePanelRow.Stability,
              "Which rows to display on the HoverPiece properties panel.");

      _hoverPiecePanelFontSize =
          config.Bind(
            "HoverPiecePanel",
            "hoverPiecePanelFontSize",
            18,
            new ConfigDescription(
                "Font size for the HoverPiece properties panel.",
                new AcceptableValueRange<int>(6, 32)));

      _showHoverPieceHealthBar =
          config.Bind("HoverPiecePanel", "showHoverPieceHealthBar", true, "Show the vanilla hover piece HealthBar.");

      _placementGhostPanelPosition =
          config.Bind(
              "PlacementGhostPanel",
              "placementGhostPanelPosition",
              new Vector2(100, 0),
              "Position of the PlacementGhost properties panel.");

      _placementGhostPanelEnabledRows =
          config.Bind(
              "PlacementGhostPanel",
              "placementGhostPanelEnabledRows",
              (PlacementGhostPanelRow) Enum.GetValues(typeof(PlacementGhostPanelRow)).Cast<int>().Sum(),
              "Which rows to display on the PlacementGhost properties panel.");

      _placementGhostPanelFontSize =
          config.Bind(
            "PlacementGhostPanel",
            "placementGhostPanelFontSize",
            18,
            new ConfigDescription(
                "Font size for the PlacementGhost properties panel.",
                new AcceptableValueRange<int>(6, 32)));
    }
  }
}
