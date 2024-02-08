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
      PlacementGhostPanel.UpdateProperties(Player.m_localPlayer.m_placementGhost, PlacementGhostPanelEnabledRows.Value);
    }
  }

  public static Gradient CreateHealthPercentGradient() {
    Gradient gradient = new();

    gradient.SetKeys(
        new GradientColorKey[] {
          new(new Color32(239, 83, 80, 255), 0.25f),
          new(new Color32(255, 238, 88, 255), 0.5f),
          new(new Color32(156, 204, 101, 255), 1),
        },
        new GradientAlphaKey[] {
          new(1, 0),
          new(1, 1),
        });

    return gradient;
  }

  public static Gradient CreateStabilityPercentGradient() {
    Gradient gradient = new();

    gradient.SetKeys(
        new GradientColorKey[] {
          new(new Color32(239, 83, 80, 255), 0.25f),
          new(new Color32(255, 238, 88, 255), 0.5f),
          new(new Color32(100, 181, 246, 255), 1),
        },
        new GradientAlphaKey[] {
          new(1, 0),
          new(1, 1),
        });

    return gradient;
  }
}
