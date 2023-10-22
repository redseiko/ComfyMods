using System;
using System.Linq;

using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

namespace EulersRuler {
  public static class PluginConfig {
    [Flags]
    public enum HoverPiecePanelRow {
      None = 0,
      Name = 1,
      Health = 2,
      Stability = 4,
      Euler = 8,
      Quaternion = 16,
    }

    [Flags]
    public enum PlacementGhostPanelRow {
      None = 0,
      Name = 1,
      Euler = 2,
      Quaternion = 4,
    }

    public static ConfigEntry<bool> IsModEnabled { get; private set; }

    public static ConfigEntry<Vector2> HoverPiecePanelPosition { get; private set; }
    public static ConfigEntry<HoverPiecePanelRow> HoverPiecePanelEnabledRows { get; private set; }
    public static ConfigEntry<int> HoverPiecePanelFontSize { get; private set; }

    public static ConfigEntry<bool> ShowHoverPieceHealthBar { get; private set; }

    public static ConfigEntry<Vector2> PlacementGhostPanelPosition { get; private set; }
    public static ConfigEntry<PlacementGhostPanelRow> PlacementGhostPanelEnabledRows { get; private set; }
    public static ConfigEntry<int> PlacementGhostPanelFontSize { get; private set; }

    public static void BindConfig(ConfigFile config) {
      IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      HoverPiecePanelPosition =
          config.BindInOrder(
              "HoverPiece.Panel",
              "hoverPiecePanelPosition",
              new Vector2(0f, 150f),
              "Position of the HoverPiece properties panel.");

      HoverPiecePanelEnabledRows =
          config.BindInOrder(
              "HoverPiece.Panel",
              "hoverPiecePanelEnabledRows",
              HoverPiecePanelRow.Name | HoverPiecePanelRow.Health | HoverPiecePanelRow.Stability,
              "Which rows to display on the HoverPiece properties panel.");

      HoverPiecePanelFontSize =
          config.BindInOrder(
              "HoverPiece.Panel",
              "hoverPiecePanelFontSize",
              18,
              "Font size for the HoverPiece properties panel.",
              new AcceptableValueRange<int>(6, 32));

      ShowHoverPieceHealthBar =
          config.BindInOrder(
              "HoverPiece.HealthBar",
              "showHoverPieceHealthBar",
              true,
              "Show the vanilla hover piece HealthBar.");

      PlacementGhostPanelPosition =
          config.BindInOrder(
              "PlacementGhost.Panel",
              "placementGhostPanelPosition",
              new Vector2(150f, 0f),
              "Position of the PlacementGhost properties panel.");

      PlacementGhostPanelEnabledRows =
          config.BindInOrder(
              "PlacementGhost.Panel",
              "placementGhostPanelEnabledRows",
              (PlacementGhostPanelRow) Enum.GetValues(typeof(PlacementGhostPanelRow)).Cast<int>().Sum(),
              "Which rows to display on the PlacementGhost properties panel.");

      PlacementGhostPanelFontSize =
          config.BindInOrder(
              "PlacementGhost.Panel",
              "placementGhostPanelFontSize",
              18,
              "Font size for the PlacementGhost properties panel.",
              new AcceptableValueRange<int>(6, 32));
    }
  }
}
