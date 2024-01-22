using System;
using System.Collections.Generic;
using System.Linq;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using static Pinnacle.PluginConfig;

namespace Pinnacle {
  public sealed class PinListPanel {
    public GameObject Panel { get; private set; }

    public ValueCell PinNameFilter { get; private set; }

    public GameObject Viewport { get; private set; }
    public GameObject Content { get; private set; }

    public ScrollRect ScrollRect { get; private set; }
    public LabelCell PinStats { get; private set; }

    public PanelDragger PanelDragger { get; private set; }
    public PanelResizer PanelResizer { get; private set; }

    readonly PointerState _pointerState;

    public PinListPanel(Transform parentTransform) {
      Panel = CreateChildPanel(parentTransform);

      PanelDragger = CreateChildPanelDragger(Panel).AddComponent<PanelDragger>();
      PanelDragger.TargetRectTransform = Panel.RectTransform();

      PanelResizer = CreateChildPanelResizer(Panel).AddComponent<PanelResizer>();
      PanelResizer.TargetRectTransform = Panel.RectTransform();

      PinNameFilter = new(Panel.transform);
      PinNameFilter.Cell.LayoutElement().SetFlexible(width: 1f).SetPreferred(height: 30f);
      PinNameFilter.InputField.onValueChanged.AddListener(SetTargetPins);

      Viewport = CreateChildViewport(Panel.transform);
      Content = CreateChildContent(Viewport.transform);
      ScrollRect = CreateChildScrollRect(Panel, Viewport, Content);

      PinStats = new(Panel.transform);
      PinStats.Cell.GetComponent<HorizontalLayoutGroup>().SetPadding(left: 8, right: 8, top: 5, bottom: 5);
      PinStats.Cell.Image().SetColor(new(0.5f, 0.5f, 0.5f, 0.1f));
      PinStats.Cell.AddComponent<Outline>().SetEffectDistance(new(2f, -2f));

      _pointerState = Panel.AddComponent<PointerState>();
    }

    public bool HasFocus() {
      return Panel && Panel.activeInHierarchy && _pointerState.IsPointerHovered;
    }

    public readonly List<Minimap.PinData> TargetPins = new();
    
    static bool IsPinNameValid(Minimap.PinData pin, string filter) {
      return filter.Length == 0
          || (pin.m_name.Length > 0
              && pin.m_name.IndexOf(filter, 0, StringComparison.InvariantCultureIgnoreCase) >= 0);
    }

    public void SetTargetPins() {
      SetTargetPins(PinNameFilter.InputField.text);
    }

    public void SetTargetPins(string filter) {
      SetTargetPins(Minimap.m_instance.m_pins.Where(pin => IsPinNameValid(pin, filter)).ToList());
    }

    public void SetTargetPins(List<Minimap.PinData> pins) {
      TargetPins.Clear();
      TargetPins.AddRange(pins.OrderBy(p => p.m_type).ThenBy(p => p.m_name));

      foreach (Minimap.PinData pin in pins.Where(p => Mathf.Approximately(p.m_pos.y, 0f))) {
        pin.m_pos.y = Pinnacle.GetHeight(pin.m_pos);
      }

      RefreshPinListRows();

      PinStats.Label.SetText($"{TargetPins.Count} pins.");
    }

    readonly List<PinListRow> _rowCache = new();
    int _visibleRows = 0;
    float _rowPreferredHeight = 0f;
    LayoutElement _bufferBlock;
    bool _isRefreshing = false;
    int _previousRowIndex = -1;

    void RefreshPinListRows() {
      _isRefreshing = true;

      ScrollRect.onValueChanged.RemoveAllListeners();
      Content.RectTransform().SetPosition(Vector2.zero);
      ScrollRect.SetVerticalScrollPosition(1f);
      _previousRowIndex = -1;

      _rowCache.Clear();

      foreach (GameObject child in Content.Children()) {
        UnityEngine.Object.Destroy(child);
      }

      GameObject block = new("Block", typeof(RectTransform));
      block.SetParent(Content.transform);

      _bufferBlock = block.AddComponent<LayoutElement>();
      _bufferBlock.SetPreferred(height: 0f);

      PinListRow row = new(Content.transform);

      LayoutRebuilder.ForceRebuildLayoutImmediate(Panel.RectTransform());
      _rowPreferredHeight = LayoutUtility.GetPreferredHeight(row.Row.RectTransform());

      _visibleRows = Mathf.CeilToInt(Viewport.RectTransform().sizeDelta.y / _rowPreferredHeight);

      UnityEngine.Object.Destroy(row.Row);

      Content.RectTransform().SetSizeDelta(
          new(Viewport.RectTransform().sizeDelta.x, _rowPreferredHeight * TargetPins.Count));

      for (int i = 0; i < Mathf.Min(TargetPins.Count, _visibleRows); i++) {
        row = new(Content.transform);
        row.TogglePinPosition(PinListPanelShowPinPosition.Value);
        row.SetRowContent(TargetPins[i]);
        _rowCache.Add(row);
      }

      _previousRowIndex = -1;
      ScrollRect.SetVerticalScrollPosition(1f);

      ScrollRect.onValueChanged.AddListener(OnVerticalScroll);
      _isRefreshing = false;
    }

    void OnVerticalScroll(Vector2 scroll) {
      if (_isRefreshing || TargetPins.Count == 0 || _rowCache.Count == 0) {
        return;
      }

      float scrolledY = Content.RectTransform().anchoredPosition.y;

      int rowIndex =
          Mathf.Clamp(Mathf.CeilToInt(scrolledY / _rowPreferredHeight), 0, TargetPins.Count - _rowCache.Count);

      if (rowIndex == _previousRowIndex) {
        return;
      }

      if (rowIndex > _previousRowIndex) {
        PinListRow row = _rowCache[0];
        _rowCache.RemoveAt(0);
        row.Row.RectTransform().SetAsLastSibling();

        int index = Mathf.Clamp(rowIndex + _rowCache.Count, 0, TargetPins.Count - 1);
        row.SetRowContent(TargetPins[index]);
        _rowCache.Add(row);
      } else {
        PinListRow row = _rowCache[_rowCache.Count - 1];
        _rowCache.RemoveAt(_rowCache.Count - 1);
        row.Row.RectTransform().SetSiblingIndex(1);
        row.SetRowContent(TargetPins[rowIndex]);
        _rowCache.Insert(0, row);
      }

      _bufferBlock.SetPreferred(height: rowIndex * _rowPreferredHeight);
      _previousRowIndex = rowIndex;
    }

    GameObject CreateChildPanel(Transform parentTransform) {
      GameObject panel = new("PinList.Panel", typeof(RectTransform));
      panel.SetParent(parentTransform);

      panel.AddComponent<VerticalLayoutGroup>()
          .SetChildControl(width: true, height: true)
          .SetChildForceExpand(width: false, height: false)
          .SetPadding(left: 8, right: 8, top: 8, bottom: 8)
          .SetSpacing(8);

      panel.AddComponent<Image>()
          .SetType(Image.Type.Sliced)
          .SetSprite(UIBuilder.CreateSuperellipse(400, 400, 15))
          .SetColor(new(0f, 0f, 0f, 0.9f));

      return panel;
    }

    GameObject CreateChildViewport(Transform parentTransform) {
      GameObject viewport = new("PinList.Viewport", typeof(RectTransform));
      viewport.SetParent(parentTransform);

      viewport.AddComponent<RectMask2D>();

      viewport.AddComponent<LayoutElement>()
          .SetFlexible(width: 1f, height: 1f);

      viewport.AddComponent<Image>()
          .SetType(Image.Type.Sliced)
          .SetSprite(UIBuilder.CreateRoundedCornerSprite(128, 128, 8))
          .SetColor(new(0.5f, 0.5f, 0.5f, 0.1f));

      viewport.AddComponent<Outline>()
          .SetEffectDistance(new(2f, -2f));

      return viewport;
    }

    GameObject CreateChildContent(Transform parentTransform) {
      GameObject content = new("PinList.Content", typeof(RectTransform));
      content.SetParent(parentTransform);

      content.RectTransform()
          .SetAnchorMin(Vector2.up)
          .SetAnchorMax(Vector2.up)
          .SetPivot(Vector2.up);

      content.AddComponent<VerticalLayoutGroup>()
          .SetChildControl(width: true, height: true)
          .SetChildForceExpand(width: false, height: false)
          .SetSpacing(0f);

      content.AddComponent<Image>()
          .SetColor(Color.clear)
          .SetRaycastTarget(true);

      return content;
    }

    ScrollRect CreateChildScrollRect(GameObject panel, GameObject viewport, GameObject content) {
      return panel.AddComponent<ScrollRect>()
          .SetViewport(viewport.RectTransform())
          .SetContent(content.RectTransform())
          .SetHorizontal(false)
          .SetVertical(true)
          .SetScrollSensitivity(30f);
    }

    GameObject CreateChildPanelDragger(GameObject panel) {
      GameObject dragger = new("Dragger", typeof(RectTransform));
      dragger.SetParent(panel.transform);

      dragger.AddComponent<LayoutElement>()
          .SetIgnoreLayout(true);

      dragger.RectTransform()
          .SetAnchorMin(Vector2.zero)
          .SetAnchorMax(Vector2.one)
          .SetPivot(new(0.5f, 0.5f))
          .SetSizeDelta(Vector2.zero);

      dragger.AddComponent<Image>()
          .SetColor(Color.clear);

      return dragger;
    }

    GameObject CreateChildPanelResizer(GameObject panel) {
      GameObject resizer = new("Resizer", typeof(RectTransform));
      resizer.SetParent(panel.transform);

      resizer.AddComponent<LayoutElement>()
          .SetIgnoreLayout(true);

      resizer.RectTransform()
          .SetAnchorMin(Vector2.right)
          .SetAnchorMax(Vector2.right)
          .SetPivot(Vector2.right)
          .SetSizeDelta(new(42.5f, 42.5f))
          .SetPosition(new(15f, -15f));

      resizer.AddComponent<Image>()
          .SetType(Image.Type.Sliced)
          .SetSprite(UIResources.GetSprite("button"))
          .SetColor(new(1f, 1f, 1f, 0.95f));

      resizer.AddComponent<CanvasGroup>()
          .SetAlpha(0f);

      TMP_Text icon = UIBuilder.CreateTMPLabel(resizer.transform);
      icon.SetName("Icon");

      icon.gameObject.AddComponent<LayoutElement>()
          .SetIgnoreLayout(true);

      icon.gameObject.RectTransform()
          .SetAnchorMin(Vector2.zero)
          .SetAnchorMax(Vector2.one)
          .SetPivot(new(0.5f, 0.5f))
          .SetSizeDelta(Vector2.zero);

      icon.alignment = TextAlignmentOptions.Center;
      icon.fontSize = 24f;
      icon.text = "<rotate=-45>\u2194</rotate>";

      return resizer;
    }
  }
}
