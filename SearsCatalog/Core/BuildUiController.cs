namespace SearsCatalog;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

public static class BuildUiController {
  public static RectTransform SelectionWindow { get; private set; }
  public static PanelDragger PanelDragger { get; private set; }
  public static PanelResizer PanelResizer { get; private set; }

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
    PanelDragger = panelDragger;
    PanelResizer = panelResizer;
  }

  static void HandlePanelDragEnd(Vector2 position) {
    BuildUiPanelPosition.Value = position;
  }

  static void HandlePanelResizeEnd(Vector2 sizeDelta) {
    BuildUiPanelSizeDelta.Value = sizeDelta;
  }

  public static void DestroyBuildUi(BuildUi buildUi) {
    SelectionWindow = default;
    PanelDragger = default;
    PanelResizer = default;
  }

  public static void ShowBuildUi(BuildUi buildUi) {
    if (!SelectionWindow) {
      return;
    }

    if (IsModEnabled.Value) {
      SelectionWindow
          .SetSizeDelta(BuildUiPanelSizeDelta.Value)
          .SetPosition(BuildUiPanelPosition.Value);

      PanelDragger.enabled = BuildUiCanMovePanel.Value;
      PanelResizer.enabled = BuildUiCanResizePanel.Value;
    } else {
      SelectionWindow
          .SetSizeDelta(Vector2.zero)
          .SetPosition(Vector2.zero);

      PanelDragger.enabled = false;
      PanelResizer.enabled = false;
    }
  }
}
