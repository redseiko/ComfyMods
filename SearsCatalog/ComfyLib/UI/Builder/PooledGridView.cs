namespace ComfyLib;

using System;

using UnityEngine;
using UnityEngine.UI;

public sealed class PooledGridView {
  public GameObject Container { get; private set; }
  public RectTransform RectTransform { get; private set; }

  public GameObject Viewport { get; private set; }
  public GameObject Content { get; private set; }
  public ScrollRect ScrollRect { get; private set; }

  public Vector2 ItemSize { get; private set; } = new(64f, 64f);
  public Vector2 ItemSpacing { get; private set; } = new(4f, 4f);
  public RectOffset Padding { get; private set; } = new(4, 4, 4, 4);

  int _columns = 1;
  int _totalRows = 0;
  int _maxVisibleRows = 0;
  Vector2 _stepSize;

  Func<Transform, RectTransform> _itemFactory;
  Action<int, GameObject> _onItemRender = OnItemRender;
  int _totalItems = 0;

  sealed class PooledItemState {
    public RectTransform RectTransform;
    public int Index = -1;

    public void SetActive(bool isActive) {
      if (RectTransform.gameObject.activeSelf != isActive) {
        RectTransform.gameObject.SetActive(isActive);
      }
    }
  }

  PooledItemState[] _itemStates = [];

  public PooledGridView(ScrollRect scrollRect) {
    Container = scrollRect.gameObject;
    RectTransform = Container.GetComponent<RectTransform>();

    Viewport = scrollRect.viewport.gameObject;
    Content = scrollRect.content.gameObject;
    ScrollRect = scrollRect;

    if (Content.TryGetComponent(out GridLayoutGroup gridLayoutGroup)) {
      ItemSize = gridLayoutGroup.cellSize;
      ItemSpacing = gridLayoutGroup.spacing;
      Padding = gridLayoutGroup.padding;
      gridLayoutGroup.enabled = false;
    }

    if (Content.TryGetComponent(out ContentSizeFitter contentSizeFitter)) {
      contentSizeFitter.enabled = false;
    }

    RectTransform contentRect = Content.GetComponent<RectTransform>();
    contentRect.anchorMin = new Vector2(0f, 1f);
    contentRect.anchorMax = new Vector2(1f, 1f);
    contentRect.pivot = new Vector2(0f, 1f);

    ScrollRect.onValueChanged.AddListener(OnScroll);
  }

  static void OnItemRender(int index, GameObject item) {
    // Default no-op
  }

  public PooledGridView SetItemSize(Vector2 size) {
    if (ItemSize == size) {
      return this;
    }

    ItemSize = size;
    UpdatePoolSize();
    RefreshTotalHeight();

    return this;
  }

  public PooledGridView SetSpacing(Vector2 spacing) {
    if (ItemSpacing == spacing) {
      return this;
    }

    ItemSpacing = spacing;
    UpdatePoolSize();
    RefreshTotalHeight();

    return this;
  }

  public PooledGridView SetPadding(RectOffset padding) {
    Padding = padding;
    UpdatePoolSize();
    RefreshTotalHeight();

    return this;
  }

  public void Setup(Func<Transform, RectTransform> itemFactory, Action<int, GameObject> onItemRender) {
    _itemFactory = itemFactory;
    _onItemRender = onItemRender;

    UpdatePoolSize();
  }

  public void UpdatePoolSize() {
    if (_itemFactory == null) {
      return;
    }

    _stepSize = ItemSize + ItemSpacing;

    float availableWidth = Viewport.GetComponent<RectTransform>().rect.width - Padding.left - Padding.right;
    if (availableWidth <= 0f) {
      availableWidth = RectTransform.rect.width - Padding.left - Padding.right;
    }

    _columns = Mathf.Max(1, Mathf.FloorToInt((availableWidth + ItemSpacing.x) / _stepSize.x));

    float viewportHeight = Viewport.GetComponent<RectTransform>().rect.height;
    if (viewportHeight <= 0f) {
      viewportHeight = RectTransform.rect.height;
    }

    _maxVisibleRows = Mathf.CeilToInt((viewportHeight - Padding.top - Padding.bottom + _stepSize.y * 2f) / _stepSize.y);

    int requiredPoolSize = (_maxVisibleRows + 1) * _columns;

    if (_itemStates.Length < requiredPoolSize) {
      int length = _itemStates.Length;
      Array.Resize(ref _itemStates, requiredPoolSize);

      for (int i = length; i < requiredPoolSize; i++) {
        RectTransform itemRectTransform = _itemFactory(Content.transform);
        itemRectTransform.anchorMin = new Vector2(0f, 1f);
        itemRectTransform.anchorMax = new Vector2(0f, 1f);
        itemRectTransform.pivot = new Vector2(0.5f, 0.5f);
        itemRectTransform.sizeDelta = ItemSize;

        _itemStates[i] = new PooledItemState {
          RectTransform = itemRectTransform,
          Index = -1,
        };
      }
    }

    Refresh();
  }

  void RefreshTotalHeight() {
    _totalRows = Mathf.CeilToInt((float) _totalItems / _columns);
    float totalHeight = Padding.top + Padding.bottom + (_totalRows * ItemSize.y) + (Mathf.Max(0, _totalRows - 1) * ItemSpacing.y);

    Content.GetComponent<RectTransform>()
        .SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(0f, totalHeight));
  }

  public void SetTotalItems(int count) {
    _totalItems = count;
    RefreshTotalHeight();

    for (int i = 0; i < _itemStates.Length; i++) {
      _itemStates[i].Index = -1;
    }

    Refresh();
  }

  public void Refresh() {
    OnScroll(Vector2.zero);
  }

  void OnScroll(Vector2 sliderValue) {
    if (_itemStates.Length <= 0 || _stepSize.y <= 0f || _columns <= 0) {
      return;
    }

    Vector2 scrollPosition = ScrollRect.content.anchoredPosition;

    int startRow = Mathf.Max(0, Mathf.FloorToInt((scrollPosition.y - Padding.top) / _stepSize.y));
    int endRow = Mathf.Min(startRow + _maxVisibleRows + 1, _totalRows);

    int startIndex = startRow * _columns;
    int endIndex = Mathf.Min(endRow * _columns, _totalItems);

    for (int i = startIndex; i < endIndex; i++) {
      PooledItemState state = _itemStates[i % _itemStates.Length];

      if (state.Index != i) {
        state.Index = i;

        int row = i / _columns;
        int col = i % _columns;

        state.RectTransform.anchoredPosition = new Vector2(
            Padding.left + (col * _stepSize.x) + (ItemSize.x / 2f),
            -(Padding.top + (row * _stepSize.y) + (ItemSize.y / 2f)));

        _onItemRender(i, state.RectTransform.gameObject);
      }

      state.SetActive(true);
    }

    for (int i = 0; i < _itemStates.Length; i++) {
      PooledItemState state = _itemStates[i];
      if (state.Index < startIndex || state.Index >= endIndex) {
        state.SetActive(false);
      }
    }
  }

  public GameObject GetItemGameObject(int index) {
    for (int i = 0; i < _itemStates.Length; i++) {
      if (_itemStates[i].Index == index && _itemStates[i].RectTransform.gameObject.activeSelf) {
        return _itemStates[i].RectTransform.gameObject;
      }
    }

    return null;
  }

  public void ScrollToItem(int index, float extraPadding = 0f) {
    if (_columns <= 0 || index < 0 || index >= _totalItems) {
      return;
    }

    int row = index / _columns;
    float rowTop = -(Padding.top + (row * _stepSize.y));
    float rowBottom = rowTop - ItemSize.y;

    float viewportHeight = ScrollRect.viewport.rect.height;
    Vector2 scrollPosition = ScrollRect.content.anchoredPosition;

    float visibleContentTop = -scrollPosition.y - extraPadding;
    float visibleContentBottom = -scrollPosition.y - viewportHeight + extraPadding;

    float scrollDelta =
        rowTop > visibleContentTop
            ? visibleContentTop - rowTop
            : rowBottom < visibleContentBottom
                ? visibleContentBottom - rowBottom
                : 0f;

    if (!Mathf.Approximately(scrollDelta, 0f)) {
      scrollPosition.y += scrollDelta;

      float maxScroll = Mathf.Max(0f, Content.GetComponent<RectTransform>().rect.height - viewportHeight);
      scrollPosition.y = Mathf.Clamp(scrollPosition.y, 0f, maxScroll);

      ScrollRect.content.anchoredPosition = scrollPosition;
      Refresh();
    }
  }
}
