namespace EulersRuler;

using System.Text;
using UnityEngine;

using static PluginConfig;

public static class PlacementGhostPanel {
  static TwoColumnPanel _placementGhostPanel;

  public static TwoColumnPanel.LabelRow GhostNameRow { get; private set; }
  public static TwoColumnPanel.LabelRow GhostEulerRow { get; private set; }
  public static TwoColumnPanel.LabelRow GhostQuaternionRow { get; private set; }
  public static TwoColumnPanel.LabelRow GhostDistanceRow { get; private set; }
  public static TwoColumnPanel.LabelRow GhostStabilityRow { get; private set; }

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

    GhostStabilityRow = _placementGhostPanel.AddLabelRow();
    GhostStabilityRow.LeftLabel.text = "Stability \u2616";
    GhostStabilityRow.RightLabel.color = new(1f, 0.79f, 0.15f);

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

  public static void UpdateProperties(GameObject placementGhost, PlacementGhostPanelRow enabledRows, Piece hoverPiece) {
    if (!_placementGhostPanel?.Panel) {
      return;
    }

    if (!placementGhost
        || enabledRows == PlacementGhostPanelRow.None
        || !placementGhost.TryGetComponent(out Piece placementPiece)) {
      _placementGhostPanel.SetActive(false);
      return;
    }

    _placementGhostPanel.SetActive(true);

    SetupGhostRows(enabledRows);
    UpdateGhostRows(placementGhost, placementPiece, enabledRows, hoverPiece);
  }

  static void SetupGhostRows(PlacementGhostPanelRow enabledRows) {
    if (enabledRows == _lastEnabledRows) {
      return;
    }

    GhostNameRow.SetActive(enabledRows.HasFlag(PlacementGhostPanelRow.Name));
    GhostEulerRow.SetActive(enabledRows.HasFlag(PlacementGhostPanelRow.Euler));
    GhostQuaternionRow.SetActive(enabledRows.HasFlag(PlacementGhostPanelRow.Quaternion));
    GhostDistanceRow.SetActive(enabledRows.HasFlag(PlacementGhostPanelRow.Distance));
    GhostStabilityRow.SetActive(enabledRows.HasFlag(PlacementGhostPanelRow.Stability));

    _lastEnabledRows = enabledRows;
  }

  static void UpdateGhostRows(GameObject placementGhost, Piece placementPiece, PlacementGhostPanelRow enabledRows, Piece hoverPiece) {
    if (enabledRows.HasFlag(PlacementGhostPanelRow.Name)) {
      UpdateGhostNameRow(placementPiece, enabledRows.HasFlag(PlacementGhostPanelRow.PieceName));
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

    if (enabledRows.HasFlag(PlacementGhostPanelRow.Stability)) {
      UpdateGhostStabilityRow(placementGhost, placementPiece, hoverPiece);
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

  static void UpdateGhostStabilityRow(GameObject placementGhost, Piece placementPiece, Piece hoverPiece) {
    // If we aren't hovering over anything or can't get their WearNTears, no need to show the stability row
    if (!hoverPiece
        || !hoverPiece.m_nview
        || !hoverPiece.m_nview.IsValid()
        || !hoverPiece.TryGetComponent(out WearNTear hoverWearNTear)
        || !placementGhost.TryGetComponent(out WearNTear placementWearNTear)) {
      GhostStabilityRow.SetActive(false);
      return;
    }
    else {
      GhostStabilityRow.SetActive(true);
    }

    // Find out how much support the parent has left
    float parentSupport = hoverWearNTear.GetSupport();

    // Find out how much support this piece needs and consumes
    placementWearNTear.GetMaterialProperties(out float maxSupport, out float minSupport, out float horizontalLoss, out float verticalLoss);

    // Compute roughly how many meters of this object can be snapped to the parent, both vertically and horizontally.
    int metersCanPlaceHoriz = 0, metersCanPlaceVert = 0;
    if (parentSupport > minSupport) {
      // It would be ideal to list the number of objects we can snap horizontally and vertically, but the game's
      // stability calculation factors in the width/height of objects and we aren't caculating the size of either piece yet.
      // This simple calculation is relatively accurate when both objects are 1 meter wide/tall but badly off for larger pieces,
      // especially when snapping a wide piece to a tall piece.  But if we call the result "meters" it's pretty close!
      // An accurate calculation would require calculating the dimensions of each piece, the angle of intersection,
      // their centers of mass, and whether any other pieces also provide support (like placing the capstone in an arch).
      //
      // The 2m and 4m core wood parts are good test cases.  You can stack 6 poles, 24m total, matching our numbers.
      // This code thinks you can snap 13m worth of horizontal beams, though, but in the game the 3rd 4m beam shatters.
      // But you can put six 2m beams in game, though!
      //
      // Once we have measurements of piece sizes, the accurate calculation is the ending block of the main loop
      // in WearNTear.UpdateSupport() starting from the call to FindSupportPoint, using ACos on the pieces sizes.

      // How much support remains after adding a piece in each direction
      float supportPercentLeftAfterEachPieceHoriz = 1f - horizontalLoss;
      float supportPercentLeftAfterEachPieceVert = 1f - verticalLoss;

      // Bad approximation for the starting support - subtracts one iteration 
      float supportAvailableFromParent = Mathf.Min(maxSupport, parentSupport);
      float supportAvailableHoriz = supportAvailableFromParent * supportPercentLeftAfterEachPieceHoriz;
      float supportAvailableVert = supportAvailableFromParent * supportPercentLeftAfterEachPieceVert;

      // Calculate how many iterations we can repeat adding pieces till they collapse.
      metersCanPlaceHoriz = Mathf.FloorToInt(Mathf.Log(minSupport / supportAvailableHoriz) / Mathf.Log(supportPercentLeftAfterEachPieceHoriz));
      metersCanPlaceVert = Mathf.FloorToInt(Mathf.Log(minSupport / supportAvailableVert) / Mathf.Log(supportPercentLeftAfterEachPieceVert));
    }

    metersCanPlaceHoriz = Mathf.Max(metersCanPlaceHoriz, 0);
    metersCanPlaceVert = Mathf.Max(metersCanPlaceVert, 0);

    const string ColorPlain = "#FAFAFA";
    const string ColorError = "#F05250";
    const string ColorOk = "#9CCC63";
    StringBuilder sb = new StringBuilder(160);

    sb.AppendFormat("<color={0}>{1}: about </color>", ColorPlain, placementWearNTear.m_materialType.ToString());

    sb.AppendFormat("<color={0}>{1}{2}</color>",
      metersCanPlaceHoriz > 1 ? ColorOk : ColorError,
      metersCanPlaceHoriz,
      metersCanPlaceHoriz > 0 ? "m" : "");

    sb.AppendFormat("<color={0}> horiz, </color>", ColorPlain);

    sb.AppendFormat("<color={0}>{1}{2}</color>",
      metersCanPlaceVert > 1 ? ColorOk : ColorError,
      metersCanPlaceVert,
      metersCanPlaceVert > 0 ? "m" : "");

    sb.AppendFormat("<color={0}> vert</color>", ColorPlain);

    GhostStabilityRow.RightLabel.text = sb.ToString();
  }
}
