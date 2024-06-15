namespace SearsCatalog;

using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

public static class BuildHudController {
  public static int BuildHudColumns { get; set; } = 15;
  public static int BuildHudRows { get; set; } = 0;

  public static bool BuildHudNeedRefresh { get; set; } = false;
  public static bool BuildHudNeedIconLayoutRefresh { get; set; } = false;
  public static bool BuildHudNeedIconRecenter { get; set; } = false;

  public static RectTransform BuildHudPanelTransform { get; set; }
  public static Scrollbar BuildHudScrollbar { get; set; }
  public static ScrollRect BuildHudScrollRect { get; set; }

  public static GameObject BuildHudPanelResizer { get; set; }

  public static void CenterOnSelectedIndex() {
    if (!Player.m_localPlayer.Ref()?.m_buildPieces || !Hud.m_instance) {
      return;
    }

    Vector2Int gridIndex = Player.m_localPlayer.m_buildPieces.GetSelectedIndex();
    int index = (BuildHudColumns * gridIndex.y) + gridIndex.x;

    if (index < 0 || index >= Hud.m_instance.m_pieceIcons.Count) {
      return;
    }

    Hud.PieceIconData pieceIcon = Hud.m_instance.m_pieceIcons[index];

    if (!pieceIcon.m_go) {
      return;
    }

    ScrollRectUtils.EnsureVisibility(BuildHudScrollRect, pieceIcon.m_go.GetComponent<RectTransform>());
  }

  public static void ResizeBuildHudPanel(Vector2 sizeDelta) {
    float spacing = Hud.m_instance.m_pieceIconSpacing;

    BuildHudPanelColumns.Value = Mathf.Max(Mathf.FloorToInt((sizeDelta.x - 35f) / spacing), 1);
    BuildHudPanelRows.Value = Mathf.Max(Mathf.FloorToInt((sizeDelta.y - 70f) / spacing), 1);
  }

  public static void SetupBuildHudPanel() {
    if (Hud.m_instance && BuildHudPanelTransform) {
      BuildHudColumns = BuildHudPanelColumns.Value;
      BuildHudRows = 0;

      float spacing = Hud.m_instance.m_pieceIconSpacing;

      BuildHudPanelTransform.SetSizeDelta(
          new(BuildHudColumns * spacing + 35f, BuildHudPanelRows.Value * spacing + 70f));

      BuildHudNeedRefresh = true;
    }
  }

  public static void SetupPieceSelectionWindow(Hud hud) {
    Transform parentTransform = hud.m_pieceListRoot.parent;

    DefaultControls.Resources resources = new() {
      standard = UIResources.GetSprite("UISprite")
    };

    Scrollbar scrollbar = DefaultControls.CreateScrollbar(resources).GetComponent<Scrollbar>();
    scrollbar.transform.SetParent(parentTransform, worldPositionStays: false);

    scrollbar.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.right)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.one)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(10f, 0f));

    scrollbar.direction = Scrollbar.Direction.BottomToTop;
    scrollbar.GetComponent<Image>().SetColor(new(0f, 0f, 0f, 0.6f));
    scrollbar.handleRect.GetComponent<Image>().SetColor(new(1f, 1f, 1f, 0.9f));

    parentTransform.GetComponent<RectTransform>()
        .OffsetSizeDelta(new(10f, 0f));

    ScrollRect scrollRect = parentTransform.gameObject.AddComponent<ScrollRect>();
    scrollRect.content = hud.m_pieceListRoot;
    scrollRect.viewport = parentTransform.GetComponent<RectTransform>();
    scrollRect.verticalScrollbar = scrollbar;
    scrollRect.movementType = ScrollRect.MovementType.Clamped;
    scrollRect.inertia = false;
    scrollRect.scrollSensitivity = hud.m_pieceIconSpacing;
    scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

    GameObject panel = hud.m_pieceSelectionWindow.transform.parent.gameObject;

    RectTransform panelTransform = panel.GetComponent<RectTransform>();
    panelTransform.SetPosition(BuildHudPanelPosition.Value);

    PanelDragger panelDragger = panel.AddComponent<PanelDragger>();
    panelDragger.TargetRectTransform = panelTransform;
    panelDragger.OnPanelEndDrag += (_, position) => BuildHudPanelPosition.Value = position;

    PanelResizer panelResizer = UIBuilder.CreateResizer(panelTransform).AddComponent<PanelResizer>();
    panelResizer.TargetRectTransform = panelTransform;
    panelResizer.OnPanelEndResize += (_, sizeDelta) => ResizeBuildHudPanel(sizeDelta);

    BuildHudPanelTransform = panelTransform;
    BuildHudScrollbar = scrollbar;
    BuildHudScrollRect = scrollRect;
    BuildHudNeedRefresh = true;
  }

  public static void SetBuildPanelPosition(Vector2 position) {
    BuildHudPanelTransform.Ref()?.SetPosition(position);
  }
}
