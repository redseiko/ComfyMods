namespace ComfyLib;

using System;

using UnityEngine;
using UnityEngine.UI;

public sealed class PooledListView {
  public GameObject Container { get; private set; }
  public RectTransform RectTransform { get; private set; }
  public Image Background { get; private set; }

  public GameObject Viewport { get; private set; }
  public GameObject Content { get; private set; }
  public RectTransform ContentRectTransform { get; private set; }
  public ScrollRect ScrollRect { get; private set; }

  public float ItemHeight { get; private set; } = 24f;
  public float ItemSpacing { get; private set; } = 4f;

  float _stepSize = 24f;
  int _maxVisibleItems = 0;

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

  public PooledListView(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();
    Background = Container.GetComponent<Image>();

    Viewport = CreateViewport(Container.transform);
    Content = CreateContent(Viewport.transform);
    ContentRectTransform = Content.GetComponent<RectTransform>();
    ScrollRect = CreateScrollRect(Container, Viewport, Content);

    ScrollRect.onValueChanged.AddListener(OnScroll);
  }

  static void OnItemRender(int index, GameObject item) {
    // ...
  }

  public PooledListView SetItemHeight(float height) {
    if (Mathf.Approximately(ItemHeight, height)) {
      return this;
    }

    ItemHeight = height;

    UpdatePoolSize();
    RefreshTotalHeight();

    return this;
  }

  public PooledListView SetSpacing(float spacing) {
    if (Mathf.Approximately(ItemSpacing, spacing)) {
      return this;
    }

    ItemSpacing = spacing;

    UpdatePoolSize();
    RefreshTotalHeight();

    return this;
  }

  public PooledListView SetScrollSensitivity(float scrollSensitivity) {
    ScrollRect.scrollSensitivity = scrollSensitivity;
    return this;
  }

  public PooledListView SetMovementType(ScrollRect.MovementType movementType) {
    ScrollRect.movementType = movementType;
    return this;
  }

  public void Setup(Func<Transform, RectTransform> listItemFactory, Action<int, GameObject> onListItemRender) {
    _itemFactory = listItemFactory;
    _onItemRender = onListItemRender;

    UpdatePoolSize();
  }

  public void UpdatePoolSize() {
    if (_itemFactory == null) {
      return;
    }

    _stepSize = ItemHeight + ItemSpacing;

    if (_stepSize <= 0f) {
      return;
    }

    float height = RectTransform.rect.height;

    if (height <= 0f && RectTransform.parent is RectTransform parentRectTransform) {
      LayoutRebuilder.ForceRebuildLayoutImmediate(parentRectTransform);
      height = RectTransform.rect.height;
    }

    if (height <= 0f) {
      return;
    }

    _maxVisibleItems = Mathf.CeilToInt((height + _stepSize * 2f) / _stepSize);

    if (_itemStates.Length >= _maxVisibleItems) {
      return;
    }

    int length = _itemStates.Length;
    Array.Resize(ref _itemStates, _maxVisibleItems);

    for (int i = length; i < _maxVisibleItems; i++) {
      RectTransform listItemRectTransform = _itemFactory(Content.transform);

      _itemStates[i] = new PooledItemState {
        RectTransform = listItemRectTransform,
        Index = -1,
      };
    }

    Refresh();
  }

  void RefreshTotalHeight() {
    float totalHeight = (_totalItems * ItemHeight) + (Mathf.Max(0, _totalItems - 1) * ItemSpacing);
    ContentRectTransform.SetSizeWithCurrentAnchors(RectTransform.Axis.Vertical, Mathf.Max(0f, totalHeight));
  }

  public void SetTotalItems(int count) {
    _totalItems = count;
    RefreshTotalHeight();

    foreach (PooledItemState state in _itemStates) {
      state.Index = -1;
    }

    Refresh();
  }

  public void Refresh() {
    OnScroll(Vector2.zero);
  }

  public void ScrollToTop() {
    ScrollRect.velocity = Vector2.zero;
    ScrollRect.verticalNormalizedPosition = 1f;
    ScrollRect.content.anchoredPosition = Vector2.zero;

    Refresh();
  }

  void OnScroll(Vector2 sliderValue) {
    if (_stepSize <= 0f) {
      return;
    }

    float height = RectTransform.rect.height;
    if (height > 0f) {
      int neededItems = Mathf.CeilToInt((height + _stepSize * 2f) / _stepSize);

      if (neededItems > _itemStates.Length) {
        UpdatePoolSize();
      } else {
        _maxVisibleItems = neededItems;
      }
    }

    if (_itemStates.Length <= 0) {
      return;
    }

    Vector2 scrollPosition = ScrollRect.content.anchoredPosition;

    int startIndex = Mathf.Max(0, Mathf.FloorToInt(scrollPosition.y / _stepSize));
    int endIndex = Mathf.Min(startIndex + _maxVisibleItems, _totalItems);

    for (int i = startIndex; i < endIndex; i++) {
      PooledItemState state = _itemStates[i % _itemStates.Length];

      if (state.Index != i) {
        state.Index = i;
        state.RectTransform.anchoredPosition = new(0f, -(i * _stepSize) - (ItemHeight / 2f));
        _onItemRender(i, state.RectTransform.gameObject);
      }

      state.SetActive(true);
    }

    foreach (PooledItemState state in _itemStates) {
      if (state.Index < startIndex || state.Index >= endIndex) {
        state.SetActive(false);
      }
    }
  }

  static GameObject CreateContainer(Transform parentTransform) {
    GameObject container = new("PooledListView", typeof(RectTransform));
    container.transform.SetParent(parentTransform, worldPositionStays: false);

    container.GetComponent<RectTransform>()
        .SetSizeDelta(Vector2.zero);

    container.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIResources.GetSprite("item_background"))
        .SetColor(new(0f, 0f, 0f, 0.5f));

    return container;
  }

  static GameObject CreateViewport(Transform parentTransform) {
    GameObject viewport = new("Viewport", typeof(RectTransform));
    viewport.transform.SetParent(parentTransform, worldPositionStays: false);

    viewport.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(-10f, -10f));

    viewport.AddComponent<RectMask2D>();

    return viewport;
  }

  static GameObject CreateContent(Transform parentTransform) {
    GameObject content = new("Content", typeof(RectTransform));
    content.transform.SetParent(parentTransform, worldPositionStays: false);

    content.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.up)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    return content;
  }

  static ScrollRect CreateScrollRect(GameObject container, GameObject viewport, GameObject content) {
    ScrollRect scrollrect =
        container.AddComponent<ScrollRect>()
            .SetViewport(viewport.GetComponent<RectTransform>())
            .SetContent(content.GetComponent<RectTransform>())
            .SetHorizontal(false)
            .SetVertical(true)
            .SetScrollSensitivity(1000f)
            .SetMovementType(ScrollRect.MovementType.Clamped);

    Scrollbar scrollbar = UIBuilder.CreateScrollbar(container.transform);

    scrollbar
        .SetDirection(Scrollbar.Direction.BottomToTop);

    scrollrect
        .SetVerticalScrollbar(scrollbar)
        .SetVerticalScrollbarVisibility(ScrollRect.ScrollbarVisibility.AutoHide);

    return scrollrect;
  }
}
