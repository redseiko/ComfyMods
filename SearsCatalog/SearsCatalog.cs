using System.Reflection;

using BepInEx;

using ComfyLib;

using HarmonyLib;

using UnityEngine;
using UnityEngine.UI;

using static SearsCatalog.PluginConfig;

namespace SearsCatalog {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public sealed class SearsCatalog : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.searscatalog";
    public const string PluginName = "SearsCatalog";
    public const string PluginVersion = "1.4.0";

    public static Harmony HarmonyInstance { get; private set; }

    void Awake() {
      BindConfig(Config);

      HarmonyInstance = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }

    void OnDestroy() {
      HarmonyInstance?.UnpatchSelf();
    }

    public static int BuildHudColumns { get; set; } = 15;
    public static int BuildHudRows { get; set; } = 0;

    public static bool BuildHudNeedRefresh { get; set; } = false;
    public static bool BuildHudNeedIconLayoutRefresh { get; set; } = false;
    public static bool BuildHudNeedIconRecenter { get; set; } = false;

    public static RectTransform BuildHudPanelTransform { get; set; }
    public static Scrollbar BuildHudScrollbar { get; set; }
    public static ScrollRect BuildHudScrollRect { get; set; }

    public static GameObject BuildHudPanelResizer { get; set; }

    public static void SetupBuildHudPanel() {
      if (Hud.m_instance && BuildHudPanelTransform) {
        BuildHudColumns = BuildHudPanelColumns.Value;
        BuildHudRows = 0;
         
        float spacing = Hud.m_instance.m_pieceIconSpacing;

        BuildHudPanelTransform.SetSizeDelta(
            new(BuildHudColumns * spacing + 35f, BuildHudPanelRows.Value * spacing + 70f));

        BuildHudNeedRefresh = true;

        SetupPieceSelectionWindow();
      }
    }

    static RectTransform _tabBorderRectTransform;
    static RectTransform _inputHelpLeftRectTransform;
    static RectTransform _inputHelpRightRectTransform;

    public static void SetupPieceSelectionWindow() {
      if (!Hud.m_instance || !BuildHudPanelTransform) {
        return;
      }

      float width = BuildHudPanelTransform.sizeDelta.x;
      float height = BuildHudPanelTransform.sizeDelta.y;

      Hud.m_instance.m_pieceCategoryRoot
          .RectTransform()
          .SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width + CategoryRootSizeWidthOffset.Value);

      SetupTabBorder(Hud.m_instance, width);

      float xOffset = width + InputHelpSizeDeltaOffset.Value.x;
      float yOffset = (height / 2f) + InputHelpSizeDeltaOffset.Value.y;

      if (!_inputHelpLeftRectTransform) {
        _inputHelpLeftRectTransform =
             Hud.m_instance.m_pieceSelectionWindow.transform
                .Find("InputHelp/MK hints/Left").Ref()?
                .GetComponent<RectTransform>();
      }

      _inputHelpLeftRectTransform.Ref()?.SetPosition(new(xOffset / -2f, yOffset));

      if (!_inputHelpRightRectTransform) {
        _inputHelpRightRectTransform =
            Hud.m_instance.m_pieceSelectionWindow.transform
                .Find("InputHelp/MK hints/Right").Ref()?
                .GetComponent<RectTransform>();
      }

      _inputHelpRightRectTransform.Ref()?.SetPosition(new(xOffset / 2f, yOffset));
    }

    static void SetupTabBorder(Hud hud, float width) {
      if (!_tabBorderRectTransform) {
        _tabBorderRectTransform = hud.m_pieceSelectionWindow.transform.Find("TabBorder").RectTransform();
      }

      if (!_tabBorderRectTransform) {
        _tabBorderRectTransform = hud.m_pieceCategoryRoot.transform.Find("TabBorder").RectTransform();
      }

      _tabBorderRectTransform.Ref()?
          .SetSizeWithCurrentAnchors(RectTransform.Axis.Horizontal, width + TabBorderSizeWidthOffset.Value);
    }

    public static void ResizeBuildHudPanel(Vector2 sizeDelta) {
      float spacing = Hud.m_instance.m_pieceIconSpacing;

      BuildHudPanelColumns.Value = Mathf.Max(Mathf.FloorToInt((sizeDelta.x - 35f) / spacing), 1);
      BuildHudPanelRows.Value = Mathf.Max(Mathf.FloorToInt((sizeDelta.y - 70f) / spacing), 1);
    }

    public static void CenterOnSelectedIndex() {
      if (!Player.m_localPlayer.Ref()?.m_buildPieces || !Hud.m_instance) {
        return;
      }

      Vector2Int gridIndex = Player.m_localPlayer.m_buildPieces.GetSelectedIndex();
      int index = (BuildHudColumns * gridIndex.y) + gridIndex.x;

      if (index >= Hud.m_instance.m_pieceIcons.Count) {
        return;
      }

      Hud.PieceIconData pieceIcon = Hud.m_instance.m_pieceIcons[index];

      if (!pieceIcon.m_go) {
        return;
      }

      BuildHudScrollRect.EnsureVisibility(pieceIcon.m_go.GetComponent<RectTransform>());
    }
  }
}
