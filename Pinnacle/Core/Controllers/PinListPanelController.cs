namespace Pinnacle;

using System.Collections.Generic;

using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

public static class PinListPanelController {
  public static PinListPanel PinListPanel { get; private set; }

  static readonly Dictionary<GameObject, PinListRow> _listItems = [];

  public static void TogglePanel() {
    TogglePanel(!IsPanelValid() || !PinListPanel.Panel.activeSelf);
  }

  public static void TogglePanel(bool toggleOn) {
    if (!IsPanelValid()) {
      PinListPanel = new(Minimap.s_instance.m_largeRoot.transform);

      PinListPanel.RectTransform
          .SetAnchorMin(new(0f, 0.5f))
          .SetAnchorMax(new(0f, 0.5f))
          .SetPivot(new(0f, 0.5f))
          .SetPosition(PinListPanelPosition.Value)
          .SetSizeDelta(PinListPanelSizeDelta.Value);

      PinListPanel.PinScrollView
          .SetItemHeight(26f)
          .SetSpacing(2f)
          .SetScrollSensitivity(PinListPanelScrollRectScrollSensitivity.Value)
          .SetMovementType(PinListPanelScrollRectMovementType.Value)
          .Setup(CreateRow, RenderRow);

      PinListPanel.PinNameFilter.InputField.onValueChanged.AddListener(OnSearchValueChanged);
      PinListPanel.RefreshButton.Button.onClick.AddListener(RefreshTargetPins);

      PinListPanel.PanelDragger.OnPanelEndDrag += OnPanelDragEnd;
      PinListPanel.PanelResizer.OnPanelEndResize += OnPanelResizeEnd;
    }

    if (toggleOn) {
      PinListPanel.Panel.SetActive(true);

      LayoutRebuilder.ForceRebuildLayoutImmediate(PinListPanel.RectTransform);
      PinListPanel.PinScrollView.UpdatePoolSize();

      RefreshTargetPins();
    } else {
      PinListPanel.PinNameFilter.InputField.DeactivateInputField();
      PinListPanel.Panel.SetActive(false);
    }
  }

  static RectTransform CreateRow(Transform parent) {
    PinListRow row = new(parent);
    _listItems[row.Row] = row;

    row.Init(OnRowClicked);

    return row.RectTransform;
  }

  static void RenderRow(int index, GameObject rowObj) {
    if (!_listItems.TryGetValue(rowObj, out PinListRow row)) {
      return;
    }

    if (index >= 0 && index < PinSearchFilter.FilteredPins.Count) {
      Minimap.PinData pin = PinSearchFilter.FilteredPins[index];
      row.SetRowContent(pin, showCoordinates: PinListPanelShowPinPosition.Value);
    }
  }

  static void OnRowClicked(Minimap.PinData pin) {
    if (pin != null) {
      PinnacleUtils.CenterMapOnOrTeleportTo(pin);
    }
  }

  public static void OnSearchValueChanged(string searchText) {
    PinSearchFilter.ApplySearch(searchText);

    if (IsPanelValid()) {
      PinListPanel.PinScrollView.SetTotalItems(PinSearchFilter.FilteredPins.Count);
      PinListPanel.PinScrollView.ScrollToTop();
      PinListPanel.PinStats.Label.SetText($"{PinSearchFilter.FilteredPins.Count} pins.");
    }
  }

  public static void RefreshTargetPins() {
    SetTargetPins(Minimap.s_instance ? Minimap.s_instance.m_pins : []);
  }

  public static void SetTargetPins(IEnumerable<Minimap.PinData> pins) {
    PinSearchFilter.SetSourcePins(pins ?? []);

    string filter = IsPanelValid() ? PinListPanel.PinNameFilter.InputField.text : string.Empty;
    PinSearchFilter.ApplySearch(filter);

    if (IsPanelValid()) {
      PinListPanel.PinScrollView.SetTotalItems(PinSearchFilter.FilteredPins.Count);
      PinListPanel.PinStats.Label.SetText($"{PinSearchFilter.FilteredPins.Count} pins.");
    }
  }

  public static bool IsPanelValid() => PinListPanel != null && PinListPanel.Panel;

  public static bool HasFocus() {
    return IsPanelValid() && PinListPanel.HasFocus();
  }

  public static void SetBackgroundColor(Color color) {
    if (IsPanelValid()) {
      PinListPanel.Background.color = color;
    }
  }

  public static void SetPanelPosition(Vector2 position) {
    if (IsPanelValid()) {
      PinListPanel.RectTransform.anchoredPosition = position;
    }
  }

  public static void SetPanelSize(Vector2 sizeDelta) {
    if (IsPanelValid()) {
      PinListPanel.RectTransform.sizeDelta = sizeDelta;
      LayoutRebuilder.ForceRebuildLayoutImmediate(PinListPanel.RectTransform);
      PinListPanel.PinScrollView.UpdatePoolSize();
    }
  }

  public static void SetScrollRectMovementType(ScrollRect.MovementType movementType) {
    if (IsPanelValid()) {
      PinListPanel.PinScrollView.SetMovementType(movementType);
    }
  }

  public static void SetScrollRectScrollSensitivity(float scrollSensitivity) {
    if (IsPanelValid()) {
      PinListPanel.PinScrollView.SetScrollSensitivity(scrollSensitivity);
    }
  }

  public static void OnShowPinPositionChanged(bool showPinPosition) {
    if (IsPanelValid()) {
      PinListPanel.PinScrollView.Refresh();
    }
  }

  static void OnPanelDragEnd(object sender, Vector3 position) {
    PinListPanelPosition.Value = position;
  }

  static void OnPanelResizeEnd(object sender, Vector2 sizeDelta) {
    PinListPanelSizeDelta.Value = sizeDelta;

    if (IsPanelValid()) {
      LayoutRebuilder.ForceRebuildLayoutImmediate(PinListPanel.RectTransform);
      PinListPanel.PinScrollView.UpdatePoolSize();
    }
  }

  public static void Reset() {
    _listItems.Clear();
    PinListPanel = null;
  }
}
