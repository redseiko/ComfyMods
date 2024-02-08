namespace EulersRuler;

using TMPro;

using UnityEngine;

using static PluginConfig;

public static class PlacementGhostPanel {
  static TwoColumnPanel _placementGhostPanel;
  static TMP_Text _placementGhostNameTextLabel;
  static TMP_Text _placementGhostNameTextValue;
  static TMP_Text _placementGhostEulerTextLabel;
  static TMP_Text _placementGhostEulerTextValue;
  static TMP_Text _placementGhostQuaternionTextLabel;
  static TMP_Text _placementGhostQuaternionTextValue;

  public static void CreatePanel(Hud hud) {
    _placementGhostPanel =
        new TwoColumnPanel(hud.m_crosshair.transform)
            .AddPanelRow(out _placementGhostNameTextLabel, out _placementGhostNameTextValue)
            .AddPanelRow(out _placementGhostEulerTextLabel, out _placementGhostEulerTextValue)
            .AddPanelRow(out _placementGhostQuaternionTextLabel, out _placementGhostQuaternionTextValue)
            .SetPosition(PlacementGhostPanelPosition.Value)
            .SetAnchors(new Vector2(0, 0.5f), new Vector2(0, 0.5f), new Vector2(0, 0.5f))
            .SetFontSize(PlacementGhostPanelFontSize.Value);

    _placementGhostNameTextLabel.text = "Placing \u25a5";
    _placementGhostEulerTextLabel.text = "Euler \u29bf";
    _placementGhostQuaternionTextLabel.text = "Quaternion \u2318";
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

  public static void UpdateProperties(GameObject placementGhost, PlacementGhostPanelRow enabledRows) {
    if (!_placementGhostPanel?.Panel) {
      return;
    }

    if (!placementGhost
        || enabledRows == PlacementGhostPanelRow.None
        || !placementGhost.TryGetComponent(out Piece piece)) {
      _placementGhostPanel?.SetActive(false);
      return;
    }

    _placementGhostPanel.SetActive(true);

    UpdatePlacementGhostNameRow(piece, enabledRows.HasFlag(PlacementGhostPanelRow.Name));
    UpdatePlacementGhostEulerRow(placementGhost, enabledRows.HasFlag(PlacementGhostPanelRow.Euler));
    UpdatePlacementGhostQuaternionRow(placementGhost, enabledRows.HasFlag(PlacementGhostPanelRow.Quaternion));
  }

  static void UpdatePlacementGhostNameRow(Piece piece, bool isRowEnabled) {
    _placementGhostNameTextLabel.gameObject.SetActive(isRowEnabled);
    _placementGhostNameTextValue.gameObject.SetActive(isRowEnabled);

    if (isRowEnabled) {
      _placementGhostNameTextValue.text = $"<color=#FFCA28>{Localization.instance.Localize(piece.m_name)}</color>";
    }
  }

  static void UpdatePlacementGhostEulerRow(GameObject placementGhost, bool isRowEnabled) {
    _placementGhostEulerTextLabel.gameObject.SetActive(isRowEnabled);
    _placementGhostEulerTextValue.gameObject.SetActive(isRowEnabled);

    if (isRowEnabled) {
      _placementGhostEulerTextValue.text = $"<color=#CFD8DC>{placementGhost.transform.rotation.eulerAngles}</color>";
    }
  }

  static void UpdatePlacementGhostQuaternionRow(GameObject placementGhost, bool isRowEnabled) {
    _placementGhostQuaternionTextLabel.gameObject.SetActive(isRowEnabled);
    _placementGhostQuaternionTextValue.gameObject.SetActive(isRowEnabled);

    if (isRowEnabled) {
      _placementGhostQuaternionTextValue.text =
          $"<color=#D7CCC8>{placementGhost.transform.rotation:N2}</color>";
    }
  }
}
