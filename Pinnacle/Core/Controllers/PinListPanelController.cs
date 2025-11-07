namespace Pinnacle;

using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

public static class PinListPanelController {
  public static PinListPanel PinListPanel { get; private set; }

  public static void TogglePanel() {
    TogglePanel(!PinListPanel?.Panel.Ref()?.activeSelf ?? false);
  }

  public static void TogglePanel(bool toggleOn) {
    if (!PinListPanel?.Panel) {
      PinListPanel = new(Minimap.m_instance.m_largeRoot.transform);

      PinListPanel.PanelRectTransform
          .SetAnchorMin(new(0f, 0.5f))
          .SetAnchorMax(new(0f, 0.5f))
          .SetPivot(new(0f, 0.5f))
          .SetPosition(PinListPanelPosition.Value)
          .SetSizeDelta(PinListPanelSizeDelta.Value);

      PinListPanel.PanelDragger.OnPanelEndDrag += OnPanelDragEnd;
      PinListPanel.PanelResizer.OnPanelEndResize += OnPanelResizeEnd;
    }

    if (toggleOn) {
      PinListPanel.Panel.SetActive(true);
      PinListPanel.SetTargetPins();
    } else {
      PinListPanel.PinNameFilter.InputField.DeactivateInputField();
      PinListPanel.Panel.SetActive(false);
    }
  }

  public static bool IsPanelValid() => PinListPanel?.Panel;

  public static bool HasFocus() {
    return PinListPanel != default && PinListPanel.HasFocus();
  }

  public static void SetBackgroundColor(Color color) {
    if (IsPanelValid()) {
      PinListPanel.PanelBackground.color = color;
    }
  }

  public static void SetPanelPosition(Vector2 position) {
    if (IsPanelValid()) {
      PinListPanel.PanelRectTransform.anchoredPosition = position;
    }
  }

  public static void SetPanelSize(Vector2 sizeDelta) {
    if (IsPanelValid()) {
      PinListPanel.PanelRectTransform.sizeDelta = sizeDelta;
      PinListPanel.SetTargetPins();
    }
  }

  public static void SetScrollRectMovementType(ScrollRect.MovementType movementType) {
    if (IsPanelValid()) {
      PinListPanel.ScrollRect.movementType = movementType;
    }
  }

  public static void SetScrollRectScrollSensitivity(float scrollSensitivity) {
    if (IsPanelValid()) {
      PinListPanel.ScrollRect.scrollSensitivity = scrollSensitivity;
    }
  }

  static void OnPanelDragEnd(object sender, Vector3 position) {
    PinListPanelPosition.Value = position;
  }

  static void OnPanelResizeEnd(object sender, Vector2 sizeDelta) {
    PinListPanelSizeDelta.Value = sizeDelta;
  }
}
