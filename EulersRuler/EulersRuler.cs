using System.Collections;
using System.Reflection;

using BepInEx;

using HarmonyLib;

using TMPro;

using UnityEngine;

using static EulersRuler.PluginConfig;

namespace EulersRuler {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class EulersRuler : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.eulersruler";
    public const string PluginName = "EulersRuler";
    public const string PluginVersion = "1.5.0";

    public static readonly Gradient HealthPercentGradient = CreateHealthPercentGradient();
    public static readonly Gradient StabilityPercentGradient = CreateStabilityPercentGradient();

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

    static TwoColumnPanel _placementGhostPanel;
    static TMP_Text _placementGhostNameTextLabel;
    static TMP_Text _placementGhostNameTextValue;
    static TMP_Text _placementGhostEulerTextLabel;
    static TMP_Text _placementGhostEulerTextValue;
    static TMP_Text _placementGhostQuaternionTextLabel;
    static TMP_Text _placementGhostQuaternionTextValue;

    Harmony _harmony;

    void Awake() {
      BindConfig(Config);

      IsModEnabled.SettingChanged += (sender, eventArgs) => {
        _hoverPiecePanel?.DestroyPanel();
        _placementGhostPanel?.DestroyPanel();

        if (IsModEnabled.Value && Hud.instance) {
          CreatePanels(Hud.instance);
        }
      };

      HoverPiecePanelPosition.SettingChanged +=
          (_, _) => _hoverPiecePanel?.SetPosition(HoverPiecePanelPosition.Value);

      HoverPiecePanelFontSize.SettingChanged +=
          (_, _) => _hoverPiecePanel?.SetFontSize(HoverPiecePanelFontSize.Value);

      PlacementGhostPanelPosition.SettingChanged +=
          (_, _) => _placementGhostPanel?.SetPosition(PlacementGhostPanelPosition.Value);

      PlacementGhostPanelFontSize.SettingChanged +=
          (_, _) => _placementGhostPanel?.SetFontSize(PlacementGhostPanelFontSize.Value);

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
    }

    void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    public static void CreatePanels(Hud hud) {
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

    public static IEnumerator UpdatePropertiesCoroutine() {
      WaitForSeconds waitInterval = new(seconds: 0.25f);

      while (true) {
        yield return waitInterval;

        if (!IsModEnabled.Value || !Player.m_localPlayer) {
          continue;
        }

        UpdateHoverPieceProperties(Player.m_localPlayer.m_hoveringPiece, HoverPiecePanelEnabledRows.Value);
        UpdatePlacementGhostProperties(Player.m_localPlayer.m_placementGhost, PlacementGhostPanelEnabledRows.Value);
      }
    }

    static void UpdateHoverPieceProperties(Piece piece, HoverPiecePanelRow enabledRows) {
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
                ColorUtility.ToHtmlStringRGB(HealthPercentGradient.Evaluate(healthPercent)),
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
              ColorUtility.ToHtmlStringRGB(StabilityPercentGradient.Evaluate(supportPrecent)),
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

    static void UpdatePlacementGhostProperties(GameObject placementGhost, PlacementGhostPanelRow enabledRows) {
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

    static Gradient CreateHealthPercentGradient() {
      Gradient gradient = new();

      gradient.SetKeys(
          new GradientColorKey[] {
            new GradientColorKey(new Color32(239, 83, 80, 255), 0.25f),
            new GradientColorKey(new Color32(255, 238, 88, 255), 0.5f),
            new GradientColorKey(new Color32(156, 204, 101, 255), 1),
          },
          new GradientAlphaKey[] {
            new GradientAlphaKey(1, 0),
            new GradientAlphaKey(1, 1),
          });

      return gradient;
    }

    static Gradient CreateStabilityPercentGradient() {
      Gradient gradient = new();

      gradient.SetKeys(
          new GradientColorKey[] {
            new GradientColorKey(new Color32(239, 83, 80, 255), 0.25f),
            new GradientColorKey(new Color32(255, 238, 88, 255), 0.5f),
            new GradientColorKey(new Color32(100, 181, 246, 255), 1),
          },
          new GradientAlphaKey[] {
            new GradientAlphaKey(1, 0),
            new GradientAlphaKey(1, 1),
          });

      return gradient;
    }
  }
}