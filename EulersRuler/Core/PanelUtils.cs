namespace EulersRuler;

using System.Collections;

using UnityEngine;

using static PluginConfig;

public static class PanelUtils {
  public static readonly Gradient HealthPercentGradient = CreateHealthPercentGradient();
  public static readonly Gradient StabilityPercentGradient = CreateStabilityPercentGradient();

  public static void CreatePanels(Hud hud) {
    HoverPiecePanel.CreatePanel(hud);
    PlacementGhostPanel.CreatePanel(hud);
  }

  public static void TogglePanels(bool toggleOn) {
    HoverPiecePanel.DestroyPanel();
    PlacementGhostPanel.DestroyPanel();

    if (toggleOn && Hud.m_instance) {
      CreatePanels(Hud.m_instance);
    }
  }

  public static IEnumerator UpdatePropertiesCoroutine() {
    WaitForSeconds waitInterval = new(seconds: 0.25f);

    while (true) {
      yield return waitInterval;

      if (!IsModEnabled.Value || !Player.m_localPlayer) {
        continue;
      }

      HoverPiecePanel.UpdateProperties(Player.m_localPlayer.m_hoveringPiece, HoverPiecePanelEnabledRows.Value);
      PlacementGhostPanel.UpdateProperties(Player.m_localPlayer.m_placementGhost, PlacementGhostPanelEnabledRows.Value, Player.m_localPlayer.m_hoveringPiece);
    }
  }

  public static Gradient CreateHealthPercentGradient() {
    Gradient gradient = new();

    gradient.SetKeys(
        colorKeys: [
          new(new(0.937f, 0.325f, 0.314f, 1f), 0.25f),
          new(new(1f, 0.933f, 0.345f, 1f), 0.5f),
          new(new(0.612f, 0.8f, 0.396f, 1f), 1f) ],
        alphaKeys: [
          new(1f, 0f),
          new(1f, 1f) ]);

    return gradient;
  }

  public static Gradient CreateStabilityPercentGradient() {
    Gradient gradient = new();

    gradient.SetKeys(
        colorKeys: [
          new(new(0.937f, 0.325f, 0.314f, 1f), 0.25f),
          new(new(1f, 0.933f, 0.345f, 1f), 0.5f),
          new(new(0.392f, 0.71f, 0.965f, 1f), 1f) ],
        alphaKeys: [
          new(1f, 0f),
          new(1f, 1f) ]);

    return gradient;
  }
}
