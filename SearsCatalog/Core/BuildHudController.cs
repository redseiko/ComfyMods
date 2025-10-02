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

    BuildHudNeedRefresh = true;
  }

  public static void SetupBuildHudPanel() {
    if (Hud.m_instance && BuildHudPanelTransform) {
      BuildHudColumns = BuildHudPanelColumns.Value;
      BuildHudRows = 0;

      float spacing = Hud.m_instance.m_pieceIconSpacing;

      BuildHudPanelTransform.SetSizeDelta(
          new(BuildHudColumns * spacing + 35f, BuildHudPanelRows.Value * spacing + 70f));

      BuildHudNeedRefresh = true;

      TriggerJotunnCategoryRebuild(Player.m_localPlayer);
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

    scrollRect
        .SetContent(hud.m_pieceListRoot)
        .SetViewport(parentTransform.GetComponent<RectTransform>())
        .SetVerticalScrollbar(scrollbar)
        .SetVerticalScrollbarVisibility(ScrollRect.ScrollbarVisibility.Permanent)
        .SetMovementType(ScrollRect.MovementType.Clamped)
        .SetInertia(false)
        .SetScrollSensitivity(BuildHudPanelScrollSensitivity.Value); ;

    if (!hud.m_pieceListRoot.TryGetComponent(out Image image)) {
      image = hud.m_pieceListRoot.gameObject.AddComponent<Image>();
      image.color = Color.clear;
    }

    SetupCategories(hud);
    SetupTabBorder(hud);
    SetupInputHelp(hud);

    GameObject panel = hud.m_pieceSelectionWindow.transform.parent.gameObject;

    RectTransform panelTransform = panel.GetComponent<RectTransform>();
    panelTransform.SetPosition(BuildHudPanelPosition.Value);

    PanelDragger panelDragger = panel.AddComponent<PanelDragger>();
    panelDragger.TargetRectTransform = panelTransform;
    panelDragger.OnPanelEndDrag += HandlePanelEndDrag;

    PanelResizer panelResizer = UIBuilder.CreateResizer(panelTransform).AddComponent<PanelResizer>();
    panelResizer.TargetRectTransform = panelTransform;
    panelResizer.OnPanelEndResize += HandlePanelEndResize;

    BuildHudPanelTransform = panelTransform;
    BuildHudScrollbar = scrollbar;
    BuildHudScrollRect = scrollRect;
    BuildHudNeedRefresh = true;
  }

  static void SetupCategories(Hud hud) {
    hud.m_pieceCategoryRoot.GetComponent<RectTransform>()
        .SetPosition(CategoriesRectPosition.Value)
        .SetSizeDelta(CategoriesRectSizeDelta.Value);
  }

  static void SetupTabBorder(Hud hud) {
    Transform tabBorderTransform = hud.m_pieceCategoryRoot.transform.Find("TabBorder");
    tabBorderTransform.gameObject.SetActive(TabBorderIsEnabled.Value);

    tabBorderTransform.GetComponent<RectTransform>()
        .SetPosition(TabBorderRectPosition.Value)
        .SetSizeDelta(TabBorderRectSizeDelta.Value);
  }

  static void SetupInputHelp(Hud hud) {
    Transform inputHelpTransform = hud.m_pieceSelectionWindow.transform.Find("InputHelp");
    inputHelpTransform.gameObject.SetActive(InputHelpIsEnabled.Value);

    inputHelpTransform.GetComponent<RectTransform>()
        .SetPosition(InputHelpRectPosition.Value)
        .SetSizeDelta(InputHelpRectSizeDelta.Value);
  }

  static void TriggerJotunnCategoryRebuild(Player player) {
    if (player && player.m_buildPieces) {
      player.SetPlaceMode(player.m_buildPieces);
    }
  }

  static void HandlePanelEndDrag(object sender, Vector3 position) {
    BuildHudPanelPosition.Value = position;
  }
  
  static void HandlePanelEndResize(object sender, Vector2 sizeDelta) {
    ResizeBuildHudPanel(sizeDelta);
    TriggerJotunnCategoryRebuild(Player.m_localPlayer);
  }

  public static void SetBuildPanelPosition(Vector2 position) {
    if (BuildHudPanelTransform) {
      BuildHudPanelTransform.anchoredPosition = position;
    }
  }

  public static void SetBuildPanelScrollSensitivity(float scrollSensitivity) {
    if (BuildHudScrollRect) {
      BuildHudScrollRect.scrollSensitivity = scrollSensitivity;
    }
  }

  public static void HandleCategoriesConfigChange() {
    Hud hud = Hud.m_instance;

    if (!hud) {
      return;
    }

    SetupCategories(hud);
    SetupTabBorder(hud);
    SetupInputHelp(hud);

    TriggerJotunnCategoryRebuild(Player.m_localPlayer);
  }
}
