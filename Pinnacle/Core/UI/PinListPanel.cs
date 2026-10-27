namespace Pinnacle;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class PinListPanel {
  public GameObject Panel { get; private set; }
  public RectTransform RectTransform { get; private set; }
  public CanvasGroup CanvasGroup { get; private set; }
  public Image Background { get; private set; }

  public ValueCell PinNameFilter { get; private set; }
  public PooledListView PinScrollView { get; private set; }

  public LabelCell PinStats { get; private set; }
  public LabelButton RefreshButton { get; private set; }

  public PanelDragger PanelDragger { get; private set; }
  public PanelResizer PanelResizer { get; private set; }

  readonly PointerState _pointerState;

  public PinListPanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();
    CanvasGroup = Panel.GetComponent<CanvasGroup>();
    Background = Panel.GetComponent<Image>();

    PanelDragger = CreateDragger(Panel.transform).AddComponent<PanelDragger>();
    PanelDragger.TargetRectTransform = RectTransform;

    PanelResizer = CreateResizer(Panel.transform).AddComponent<PanelResizer>();
    PanelResizer.TargetRectTransform = RectTransform;

    PinNameFilter = new(Panel.transform);
    PinNameFilter.Container.LayoutElement().SetFlexible(width: 1f).SetPreferred(height: 30f);

    PinScrollView = CreatePinScrollView(Panel.transform);

    PinStats = new(Panel.transform);
    PinStats.Container.GetComponent<HorizontalLayoutGroup>().SetPadding(left: 8, right: 8, top: 2, bottom: 2);
    PinStats.Background.SetColor(new(0.5f, 0.5f, 0.5f, 0.1f));
    PinStats.Container.AddComponent<Outline>().SetEffectDistance(new(2f, -2f));

    RefreshButton = CreateRefreshButton(PinStats.Container.transform);

    _pointerState = Panel.AddComponent<PointerState>();
  }

  public bool HasFocus() {
    return Panel && Panel.activeInHierarchy && _pointerState.IsPointerHovered;
  }

  static GameObject CreatePanel(Transform parentTransform) {
    GameObject panel = new("PinListPanel", typeof(RectTransform));
    panel.transform.SetParent(parentTransform, worldPositionStays: false);

    panel.AddComponent<VerticalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetPadding(left: 8, right: 8, top: 8, bottom: 8)
        .SetSpacing(8);

    panel.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UISpriteBuilder.CreateSuperellipse(400, 400, 15))
        .SetColor(new(0f, 0f, 0f, 0.9f));

    panel.AddComponent<Canvas>();
    panel.AddComponent<GraphicRaycaster>();

    panel.AddComponent<CanvasGroup>()
        .SetBlocksRaycasts(true);

    return panel;
  }

  static PooledListView CreatePinScrollView(Transform parentTransform) {
    PooledListView listView = new(parentTransform);
    listView.Container.name = "PinScrollView";

    listView.Container.AddComponent<LayoutElement>()
        .SetFlexible(width: 1f, height: 1f);

    listView.Background
        .SetType(Image.Type.Sliced)
        .SetSprite(UISpriteBuilder.CreateRoundedCornerSprite(128, 128, 8))
        .SetColor(new(0.5f, 0.5f, 0.5f, 0.1f));

    return listView;
  }

  static GameObject CreateDragger(Transform parentTransform) {
    GameObject dragger = new("Dragger", typeof(RectTransform));
    dragger.transform.SetParent(parentTransform, worldPositionStays: false);

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

  static GameObject CreateResizer(Transform parentTransform) {
    GameObject resizer = new("Resizer", typeof(RectTransform));
    resizer.transform.SetParent(parentTransform, worldPositionStays: false);

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

  static LabelButton CreateRefreshButton(Transform parentTransform) {
    LabelButton refresh = new(parentTransform);

    refresh.RectTransform
        .SetSizeDelta(new(100f, 35f));

    refresh.Container.AddComponent<LayoutElement>()
        .SetPreferred(width: 100f, height: 35f);

    refresh.Label
        .SetFontSize(14f)
        .SetText("Refresh");

    refresh.Button.SetNavigationMode(Navigation.Mode.None);

    return refresh;
  }
}
