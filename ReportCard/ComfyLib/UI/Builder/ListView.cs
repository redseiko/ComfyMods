namespace ComfyLib;

using UnityEngine;
using UnityEngine.UI;

public sealed class ListView {
  public GameObject View { get; private set; }
  public Image Background { get; private set; }
  public GameObject Viewport { get; private set; }
  public GameObject Content { get; private set; }
  public HorizontalOrVerticalLayoutGroup ContentLayoutGroup { get; private set; }
  public ScrollRect ScrollRect { get; private set; }

  public ListView(Transform parentTransform) {
    View = CreateView(parentTransform);
    Background = View.GetComponent<Image>();
    Viewport = CreateChildViewport(View.transform);
    Content = CreateChildContent(Viewport.transform);
    ContentLayoutGroup = Content.GetComponent<HorizontalOrVerticalLayoutGroup>();
    ScrollRect = CreateChildScrollRect(View, Viewport, Content);
  }

  static GameObject CreateView(Transform parentTransform) {
    GameObject view = new("ListView", typeof(RectTransform));
    view.transform.SetParent(parentTransform, worldPositionStays: false);

    view.GetComponent<RectTransform>()
        .SetSizeDelta(Vector2.zero);

    view.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIResources.GetSprite("item_background"))
        .SetColor(new(0f, 0f, 0f, 0.5f));

    return view;
  }

  static GameObject CreateChildViewport(Transform parentTransform) {
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

  static GameObject CreateChildContent(Transform parentTransform) {
    GameObject content = new("Content", typeof(RectTransform));
    content.transform.SetParent(parentTransform, worldPositionStays: false);

    content.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.up)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    content.AddComponent<VerticalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetSpacing(0f);

    content.AddComponent<ContentSizeFitter>()
        .SetHorizontalFit(ContentSizeFitter.FitMode.Unconstrained)
        .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

    return content;
  }

  static ScrollRect CreateChildScrollRect(GameObject view, GameObject viewport, GameObject content) {
    ScrollRect scrollrect =
        view.AddComponent<ScrollRect>()
            .SetViewport(viewport.GetComponent<RectTransform>())
            .SetContent(content.GetComponent<RectTransform>())
            .SetHorizontal(false)
            .SetVertical(true)
            .SetScrollSensitivity(20f)
            .SetMovementType(ScrollRect.MovementType.Clamped);

    Scrollbar scrollbar = UIBuilder.CreateScrollbar(view.transform);
    scrollbar.direction = Scrollbar.Direction.BottomToTop;

    scrollrect.verticalScrollbar = scrollbar;
    scrollrect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

    return scrollrect;
  }
}
