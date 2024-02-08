namespace EulersRuler;

using TMPro;

using UnityEngine;

using static PluginConfig;

public static class HoverPiecePanel {
  static TwoColumnPanel _hoverPiecePanel;
  static TMP_Text _pieceNameTextLabel;
  static TMP_Text _pieceNameTextValue;
  static TMP_Text _pieceHealthTextLabel;
  static TMP_Text _pieceHealthTextValue;
  static TMP_Text _pieceStabilityTextLabel;
  static TMP_Text _pieceStabilityTextValue;
  static TMP_Text _pieceEulerTextLabel;
  static TMP_Text _pieceEulerTextValue;
  static TMP_Text _pieceQuaternionTextLabel;
  static TMP_Text _pieceQuaternionTextValue;

  public static void CreatePanel(Hud hud) {
    _hoverPiecePanel =
        new TwoColumnPanel(hud.m_crosshair.transform)
            .AddPanelRow(out _pieceNameTextLabel, out _pieceNameTextValue)
            .AddPanelRow(out _pieceHealthTextLabel, out _pieceHealthTextValue)
            .AddPanelRow(out _pieceStabilityTextLabel, out _pieceStabilityTextValue)
            .AddPanelRow(out _pieceEulerTextLabel, out _pieceEulerTextValue)
            .AddPanelRow(out _pieceQuaternionTextLabel, out _pieceQuaternionTextValue)
            .SetPosition(HoverPiecePanelPosition.Value)
            .SetAnchors(new Vector2(0.5f, 0f), new Vector2(0.5f, 0f), new Vector2(0.5f, 0f))
            .SetFontSize(HoverPiecePanelFontSize.Value);

    _pieceNameTextLabel.text = "Piece \u25c8";
    _pieceHealthTextLabel.text = "Health \u2661";
    _pieceStabilityTextLabel.text = "Stability \u2616";
    _pieceEulerTextLabel.text = "Euler \u29bf";
    _pieceQuaternionTextLabel.text = "Quaternion \u2318";
  }

  public static void DestroyPanel() {
    _hoverPiecePanel?.DestroyPanel();
  }

  public static void SetPosition(Vector2 position) {
    _hoverPiecePanel?.SetPosition(position);
  }

  public static void SetFontSize(int fontSize) {
    _hoverPiecePanel?.SetFontSize(fontSize);
  }

  public static void UpdateProperties(Piece piece, HoverPiecePanelRow enabledRows) {
    if (!_hoverPiecePanel?.Panel) {
      return;
    }

    if (!piece || !piece.m_nview || !piece.m_nview.IsValid() || !piece.TryGetComponent(out WearNTear wearNTear)) {
      _hoverPiecePanel?.SetActive(false);
      return;
    }

    _hoverPiecePanel.SetActive(true);

    UpdateHoverPieceNameRow(piece, enabledRows.HasFlag(HoverPiecePanelRow.Name));
    UpdateHoverPieceHealthRow(wearNTear, enabledRows.HasFlag(HoverPiecePanelRow.Health));
    UpdateHoverPieceStabilityRow(wearNTear, enabledRows.HasFlag(HoverPiecePanelRow.Stability));
    UpdateHoverPieceEulerRow(wearNTear, enabledRows.HasFlag(HoverPiecePanelRow.Euler));
    UpdateHoverPieceQuaternionRow(wearNTear, enabledRows.HasFlag(HoverPiecePanelRow.Quaternion));
  }

  static void UpdateHoverPieceNameRow(Piece piece, bool isRowEnabled) {
    _pieceNameTextLabel.gameObject.SetActive(isRowEnabled);
    _pieceNameTextValue.gameObject.SetActive(isRowEnabled);

    if (isRowEnabled) {
      _pieceNameTextValue.text = $"<color=#FFCA28>{Localization.instance.Localize(piece.m_name)}</color>";
    }
  }

  static void UpdateHoverPieceHealthRow(WearNTear wearNTear, bool isRowEnabled) {
    _pieceHealthTextLabel.gameObject.SetActive(isRowEnabled);
    _pieceHealthTextValue.gameObject.SetActive(isRowEnabled);

    if (isRowEnabled) {
      float health = wearNTear.m_nview.m_zdo.GetFloat(ZDOVars.s_health, wearNTear.m_health);
      float healthPercent = Mathf.Clamp01(health / wearNTear.m_health);

      _pieceHealthTextValue.text =
          string.Format(
              "<color=#{0}>{1}</color> /<color={2}>{3}</color> (<color=#{0}>{4:0%}</color>)",
              ColorUtility.ToHtmlStringRGB(PanelUtils.HealthPercentGradient.Evaluate(healthPercent)),
              Mathf.Abs(health) > 1E9 ? health.ToString("G5") : health.ToString("N0"),
              "#FAFAFA",
              wearNTear.m_health,
              healthPercent);
    }
  }

  static void UpdateHoverPieceStabilityRow(WearNTear wearNTear, bool isRowEnabled) {
    _pieceStabilityTextLabel.gameObject.SetActive(isRowEnabled);
    _pieceStabilityTextValue.gameObject.SetActive(isRowEnabled);

    if (isRowEnabled) {
      float support = wearNTear.GetSupport();
      float maxSupport = wearNTear.GetMaxSupport();
      float supportPrecent = Mathf.Clamp01(support / maxSupport);

      _pieceStabilityTextValue.text =
          string.Format(
            "<color=#{0}>{1:N0}</color> /<color={2}>{3}</color> (<color=#{0}>{4:0%}</color>)",
            ColorUtility.ToHtmlStringRGB(PanelUtils.StabilityPercentGradient.Evaluate(supportPrecent)),
            support,
            "#FAFAFA",
            maxSupport,
            supportPrecent);
    }
  }

  static void UpdateHoverPieceEulerRow(WearNTear wearNTear, bool isRowEnabled) {
    _pieceEulerTextLabel.gameObject.SetActive(isRowEnabled);
    _pieceEulerTextValue.gameObject.SetActive(isRowEnabled);

    if (isRowEnabled) {
      _pieceEulerTextValue.text = $"<color=#CFD8DC>{wearNTear.transform.rotation.eulerAngles}</color>";
    }
  }

  static void UpdateHoverPieceQuaternionRow(WearNTear wearNTear, bool isRowEnabled) {
    _pieceQuaternionTextLabel.gameObject.SetActive(isRowEnabled);
    _pieceQuaternionTextValue.gameObject.SetActive(isRowEnabled);

    if (isRowEnabled) {
      _pieceQuaternionTextValue.text = $"<color=#D7CCC8>{wearNTear.transform.rotation:N2}</color>";
    }
  }
}
