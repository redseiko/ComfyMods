namespace ComfyLib;

using System;

using UnityEngine;
using UnityEngine.UI;

public sealed class PooledGridView {
  public GameObject Container { get; private set; }
  public RectTransform RectTransform { get; private set; }

  public GameObject Viewport { get; private set; }
  public RectTransform ViewportRectTransform { get; private set; }

  public GameObject Content { get; private set; }
  public RectTransform ContentRectTransform { get; private set; }

  public ScrollRect ScrollRect { get; private set; }

  public Vector2 ItemSize { get; private set; } = new(64f, 64f);
  public Vector2 ItemSpacing { get; private set; } = new(4f, 4f);
  public RectOffset Padding { get; private set; } = new(4, 4, 4, 4);

  public int FirstVisibleIndex { get; private set; }
  public Navigation EdgeNavigation { get; set; }
  public float ScrollPadding { get; set; } = 0f;
  public Vector2 StepSize => _stepSize;

  int _columns = 1;
  int _totalRows = 0;
  int _maxVisibleRows = 0;
  float _totalHeight = 0f;
  Vector2 _stepSize;

  Func<Transform, RectTransform> _itemFactory;
  Action<int, GameObject> _onItemRender = OnItemRender;
  int _totalItems = 0;

  sealed class PooledItemState {
    public RectTransform RectTransform;
    public Selectable Selectable;
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
    ViewportRectTransform = scrollRect.viewport;

    Content = scrollRect.content.gameObject;
    ContentRectTransform = scrollRect.content;

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

    ContentRectTransform
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.up);

    ScrollRect.onValueChanged.AddListener(OnScroll);
  }

  static void OnItemRender(int index, GameObject item) {
    // ...
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
    _columns = Mathf.Max(1, Mathf.FloorToInt((GetAvailableWidth() + ItemSpacing.x) / _stepSize.x));
    _maxVisibleRows =
        Mathf.CeilToInt((GetViewportHeight() - Padding.top - Padding.bottom + _stepSize.y * 2f) / _stepSize.y);

    EnsurePoolCapacity((_maxVisibleRows + 2) * _columns);
    RefreshTotalHeight();
    Invalidate();
    Refresh();
  }

  float GetAvailableWidth() {
    float width = ViewportRectTransform.rect.width;

    if (width <= 0f) {
      width = RectTransform.rect.width;
    }

    return width - Padding.left - Padding.right;
  }

  float GetViewportHeight() {
    float height = ViewportRectTransform.rect.height;

    if (height <= 0f) {
      height = RectTransform.rect.height;
    }

    return height;
  }

  void EnsurePoolCapacity(int requiredPoolSize) {
    if (_itemStates.Length >= requiredPoolSize) {
      return;
    }

    int previousLength = _itemStates.Length;
    Array.Resize(ref _itemStates, requiredPoolSize);

    for (int i = previousLength; i < requiredPoolSize; i++) {
      _itemStates[i] = CreatePooledItem();
    }
  }

  PooledItemState CreatePooledItem() {
    RectTransform itemRectTransform = _itemFactory(Content.transform);

    itemRectTransform
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.up)
        .SetPivot(new Vector2(0.5f, 0.5f))
        .SetSizeDelta(ItemSize);

    return new PooledItemState {
      RectTransform = itemRectTransform,
      Selectable = itemRectTransform.GetComponent<Selectable>(),
      Index = -1,
    };
  }

  void RefreshTotalHeight() {
    _totalRows = Mathf.CeilToInt((float) _totalItems / _columns);

    _totalHeight =
        Padding.top + Padding.bottom + (_totalRows * ItemSize.y) + (Mathf.Max(0, _totalRows - 1) * ItemSpacing.y);

    ContentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(0f, _totalHeight));

    float maxScroll = Mathf.Max(0f, _totalHeight - GetViewportHeight());
    Vector2 scrollPosition = ScrollRect.content.anchoredPosition;

    if (scrollPosition.y > maxScroll) {
      scrollPosition.y = maxScroll;
      ScrollRect.content.anchoredPosition = scrollPosition;
    }
  }

  public void Invalidate() {
    for (int i = 0; i < _itemStates.Length; i++) {
      _itemStates[i].Index = -1;
    }
  }

  public void SetTotalItems(int count) {
    _totalItems = count;
    RefreshTotalHeight();
    Invalidate();
    Refresh();
  }

  public void Refresh() {
    OnScroll(Vector2.zero);
  }

  Vector2 GetItemAnchoredPosition(int index) {
    int row = index / _columns;
    int column = index % _columns;

    return new Vector2(
        Padding.left + (column * _stepSize.x) + (ItemSize.x / 2f),
        -(Padding.top + (row * _stepSize.y) + (ItemSize.y / 2f)));
  }

  void OnScroll(Vector2 sliderValue) {
    if (_itemStates.Length <= 0 || _stepSize.y <= 0f || _columns <= 0) {
      return;
    }

    CalculateVisibleItemRange(out int startIndex, out int endIndex);

    for (int i = startIndex; i < endIndex; i++) {
      PooledItemState state = _itemStates[i % _itemStates.Length];

      if (state.Index != i) {
        state.Index = i;
        state.RectTransform.anchoredPosition = GetItemAnchoredPosition(i);

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

    UpdateItemNavigation(startIndex, endIndex);
  }

  void CalculateVisibleItemRange(out int startIndex, out int endIndex) {
    Vector2 scrollPosition = ScrollRect.content.anchoredPosition;

    int firstVisibleRow = Mathf.Max(0, Mathf.FloorToInt((scrollPosition.y - Padding.top) / _stepSize.y));
    FirstVisibleIndex = Mathf.Min(firstVisibleRow * _columns, Mathf.Max(0, _totalItems - 1));

    int startRow = Mathf.Max(0, firstVisibleRow - 1);
    int endRow = Mathf.Min(startRow + _maxVisibleRows + 2, _totalRows);

    startIndex = startRow * _columns;
    endIndex = Mathf.Min(endRow * _columns, _totalItems);
  }

  Selectable GetItemSelectable(int index) {
    return _itemStates[index % _itemStates.Length].Selectable;
  }

  void UpdateItemNavigation(int startIndex, int endIndex) {
    if (_columns <= 0 || _totalItems <= 0) {
      return;
    }

    for (int i = startIndex; i < endIndex; i++) {
      PooledItemState state = _itemStates[i % _itemStates.Length];
      Selectable selectable = state.Selectable;

      if (!selectable) {
        continue;
      }

      Navigation navigation = selectable.navigation;
      navigation.mode = Navigation.Mode.Explicit;

      int column = i % _columns;
      int row = i / _columns;

      navigation.selectOnLeft =
          column > 0 && i - 1 >= startIndex
              ? GetItemSelectable(i - 1)
              : EdgeNavigation.selectOnLeft;

      navigation.selectOnRight =
          column < _columns - 1 && i + 1 < endIndex
              ? GetItemSelectable(i + 1)
              : EdgeNavigation.selectOnRight;

      navigation.selectOnUp =
          row > 0 && i - _columns >= startIndex
              ? GetItemSelectable(i - _columns)
              : (row == 0 ? EdgeNavigation.selectOnUp : null);

      navigation.selectOnDown =
          i + _columns < endIndex
              ? GetItemSelectable(i + _columns)
              : default;

      selectable.navigation = navigation;
    }
  }

  public void UpdateNavigation() {
    if (_itemStates.Length <= 0 || _stepSize.y <= 0f || _columns <= 0) {
      return;
    }

    CalculateVisibleItemRange(out int startIndex, out int endIndex);
    UpdateItemNavigation(startIndex, endIndex);
  }

  public bool TryGetGridItem(int index, out GameObject gameObject) {
    if (index >= 0 && index < _totalItems && _itemStates.Length > 0) {
      PooledItemState itemState = _itemStates[index % _itemStates.Length];

      if (itemState.Index == index && itemState.RectTransform.gameObject.activeSelf) {
        gameObject = itemState.RectTransform.gameObject;
        return true;
      }
    }

    gameObject = default;
    return false;
  }

  public bool TryGetFirstVisibleItem(out GameObject gameObject) {
    return TryGetGridItem(FirstVisibleIndex, out gameObject) || TryGetGridItem(0, out gameObject);
  }

  public void ScrollToItem(int index) {
    if (_columns <= 0 || index < 0 || index >= _totalItems) {
      return;
    }

    int row = index / _columns;
    float rowTopY = Padding.top + (row * _stepSize.y);
    float rowBottomY = rowTopY + ItemSize.y;

    float viewportHeight = GetViewportHeight();
    float maxPadding = Mathf.Max(0f, (viewportHeight - ItemSize.y) / 2f);
    float effectivePadding = Mathf.Clamp(ScrollPadding, 0f, maxPadding);

    Vector2 scrollPosition = ScrollRect.content.anchoredPosition;
    float currentScrollY = scrollPosition.y;

    float safeTopY = currentScrollY + effectivePadding;
    float safeBottomY = currentScrollY + viewportHeight - effectivePadding;
    float targetScrollY = currentScrollY;

    if (rowTopY < safeTopY) {
      targetScrollY = (row * _stepSize.y) - effectivePadding;
    } else if (rowBottomY > safeBottomY) {
      targetScrollY = rowBottomY + effectivePadding + Padding.bottom - viewportHeight;
    }

    float maxScroll = Mathf.Max(0f, _totalHeight - viewportHeight);
    targetScrollY = Mathf.Clamp(targetScrollY, 0f, maxScroll);

    if (!Mathf.Approximately(targetScrollY, currentScrollY)) {
      scrollPosition.y = targetScrollY;
      ScrollRect.content.anchoredPosition = scrollPosition;

      Refresh();
    }
  }
}
