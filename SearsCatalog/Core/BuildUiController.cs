namespace SearsCatalog;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

public static class BuildUiController {
  public static RectTransform SelectionWindow { get; private set; }
  public static PanelDragger WindowDragger { get; private set; }
  public static PanelResizer WindowResizer { get; private set; }

  public static void SetupBuildUi(BuildUi buildUi) {
    RectTransform selectionWindow = (RectTransform) buildUi.transform.Find("bar/SelectionWindow");

    if (!selectionWindow.TryGetComponent(out PanelDragger panelDragger)) {
      panelDragger = selectionWindow.gameObject.AddComponent<PanelDragger>();
      panelDragger.OnPanelDragEnd.AddListener(HandlePanelDragEnd);
      panelDragger.enabled = false;
    }

    if (!selectionWindow.TryGetComponentInChildren(out PanelResizer panelResizer)) {
      GameObject resizer = UIBuilder.CreateResizer(selectionWindow);

      panelResizer = resizer.AddComponent<PanelResizer>();
      panelResizer.SetTargetRectTransform(selectionWindow);
      panelResizer.OnPanelResizeEnd.AddListener(HandlePanelResizeEnd);
      panelResizer.enabled = false;
    }

    buildUi.m_tagListContainer
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.up)
        .SetPivot(Vector2.zero)
        .SetPosition(Vector2.zero);

    ((RectTransform) buildUi.m_tagListContainer.Find("LayoutGroup"))
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.up)
        .SetPivot(Vector2.up)
        .SetPosition(Vector2.zero);

    SelectionWindow = selectionWindow;
    WindowDragger = panelDragger;
    WindowResizer = panelResizer;
  }

  static void HandlePanelDragEnd(Vector2 position) {
    BuildUiPanelPosition.Value = position;
  }

  static void HandlePanelResizeEnd(Vector2 sizeDelta) {
    BuildUiPanelSizeDelta.Value = sizeDelta;

    Hud.m_instance.m_buildUi.ConfigureButtonNavigation();
  }

  public static void DestroyBuildUi(BuildUi buildUi) {
    SelectionWindow = default;
    WindowDragger = default;
    WindowResizer = default;
  }

  public static void ShowBuildUi(BuildUi buildUi) {
    if (!SelectionWindow) {
      return;
    }

    if (IsModEnabled.Value) {
      SelectionWindow
          .SetSizeDelta(BuildUiPanelSizeDelta.Value)
          .SetPosition(BuildUiPanelPosition.Value);

      WindowDragger.enabled = BuildUiCanMovePanel.Value;
      WindowResizer.enabled = BuildUiCanResizePanel.Value;
    } else {
      SelectionWindow
          .SetSizeDelta(Vector2.zero)
          .SetPosition(Vector2.zero);

      WindowDragger.enabled = false;
      WindowResizer.enabled = false;
    }
  }
}
