namespace EulersRuler;

using UnityEngine;

using static PluginConfig;

public static class PlacementGhostPanel {
  static TwoColumnPanel _placementGhostPanel;

  public static TwoColumnPanel.LabelRow GhostNameRow { get; private set; }
  public static TwoColumnPanel.LabelRow GhostEulerRow { get; private set; }
  public static TwoColumnPanel.LabelRow GhostQuaternionRow { get; private set; }
  public static TwoColumnPanel.LabelRow GhostDistanceRow { get; private set; }

  public static void CreatePanel(Hud hud) {
    _placementGhostPanel = new TwoColumnPanel(hud.m_crosshair.transform);

    GhostNameRow = _placementGhostPanel.AddLabelRow();
    GhostNameRow.LeftLabel.text = "Placing \u25a5";
    GhostNameRow.RightLabel.color = new(1f, 0.79f, 0.15f);

    GhostEulerRow = _placementGhostPanel.AddLabelRow();
    GhostEulerRow.LeftLabel.text = "Euler \u29bf";
    GhostEulerRow.RightLabel.color = new(0.81f, 0.84f, 0.86f);

    GhostQuaternionRow = _placementGhostPanel.AddLabelRow();
    GhostQuaternionRow.LeftLabel.text = "Quaternion \u2318";
    GhostQuaternionRow.RightLabel.color = new(0.84f, 0.8f, 0.78f);

    GhostDistanceRow = _placementGhostPanel.AddLabelRow();
    GhostDistanceRow.LeftLabel.text = "Distance \u27A1";
    GhostDistanceRow.RightLabel.color = new(1f, 0.79f, 0.15f);

    _placementGhostPanel
        .SetPosition(PlacementGhostPanelPosition.Value)
        .SetAnchors(new Vector2(0f, 0.5f), new Vector2(0f, 0.5f), new Vector2(0f, 0.5f))
        .SetFontSize(PlacementGhostPanelFontSize.Value);
  }

  public static void DestroyPanel() {
    _placementGhostPanel?.DestroyPanel();
  }

  public static void SetPosition(Vector2 position) {
    _placementGhostPanel?.SetPosition(position);
  }

  public static void SetFontSize(int fontSize) {
    _placementGhostPanel?.SetFontSize(fontSize);
  }

  static PlacementGhostPanelRow _lastEnabledRows = PlacementGhostPanelRow.None;

  public static void UpdateProperties(GameObject placementGhost, PlacementGhostPanelRow enabledRows) {
    if (!_placementGhostPanel?.Panel) {
      return;
    }

    if (!placementGhost
        || enabledRows == PlacementGhostPanelRow.None
        || !placementGhost.TryGetComponent(out Piece piece)) {
      _placementGhostPanel.SetActive(false);
      return;
    }

    _placementGhostPanel.SetActive(true);

    SetupGhostRows(enabledRows);
    UpdateGhostRows(placementGhost, piece, enabledRows);
  }

  static void SetupGhostRows(PlacementGhostPanelRow enabledRows) {
    if (enabledRows == _lastEnabledRows) {
      return;
    }

    GhostNameRow.SetActive(enabledRows.HasFlag(PlacementGhostPanelRow.Name));
    GhostEulerRow.SetActive(enabledRows.HasFlag(PlacementGhostPanelRow.Euler));
    GhostQuaternionRow.SetActive(enabledRows.HasFlag(PlacementGhostPanelRow.Quaternion));
    GhostDistanceRow.SetActive(enabledRows.HasFlag(PlacementGhostPanelRow.Distance));

    _lastEnabledRows = enabledRows;
  }

  static void UpdateGhostRows(GameObject placementGhost, Piece piece, PlacementGhostPanelRow enabledRows) {
    if (enabledRows.HasFlag(PlacementGhostPanelRow.Name)) {
      UpdateGhostNameRow(piece, enabledRows.HasFlag(PlacementGhostPanelRow.PieceName));
    }

    if (enabledRows.HasFlag(PlacementGhostPanelRow.Euler)) {
      UpdateGhostEulerRow(placementGhost);
    }

    if (enabledRows.HasFlag(PlacementGhostPanelRow.Quaternion)) {
      UpdateGhostQuaternionRow(placementGhost);
    }

    if (enabledRows.HasFlag(PlacementGhostPanelRow.Distance)) {
      UpdateGhostDistanceRow(placementGhost, enabledRows.HasFlag(PlacementGhostPanelRow.Position));
    }
  }

  static void UpdateGhostNameRow(Piece piece, bool showPrefabName) {
    if (showPrefabName) {
      GhostNameRow.RightLabel.text =
          $"{Localization.instance.Localize(piece.m_name)} ({Utils.GetPrefabName(piece.gameObject)})";
    } else {
      GhostNameRow.RightLabel.text = Localization.instance.Localize(piece.m_name);
    }
  }

  static void UpdateGhostEulerRow(GameObject placementGhost) {
    GhostEulerRow.RightLabel.text = placementGhost.transform.rotation.eulerAngles.ToString();
  }

  static void UpdateGhostQuaternionRow(GameObject placementGhost) {
    GhostQuaternionRow.RightLabel.text = placementGhost.transform.rotation.ToString("N2");
  }

  static void UpdateGhostDistanceRow(GameObject placementGhost, bool showPosition) {
    float distance =
        Player.m_localPlayer
            ? Vector3.Distance(Player.m_localPlayer.transform.position, placementGhost.transform.position)
            : 0f;

    if (showPosition) {
      GhostDistanceRow.RightLabel.text = $"{distance:N2} \u2022 {placementGhost.transform.position:F0}";
    } else {
      GhostDistanceRow.RightLabel.text = distance.ToString("N2");
    }
  }
}
