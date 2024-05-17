namespace Contextual;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public sealed class ContextMenuController : MonoBehaviour {
  GameObject _contextMenu;
  Canvas _contextMenuCanvas;

  void Awake() {
    _contextMenu = CreateMenu(gameObject.transform);
    _contextMenuCanvas = _contextMenu.GetComponentInParent<Canvas>();

    CreateMenuItem(_contextMenu.transform, "Test One");
    CreateMenuItem(_contextMenu.transform, "Not Test Two");
    CreateMenuItem(_contextMenu.transform, "Three");

    _contextMenu.SetActive(false);
  }

  void Update() {
    if (ZInput.GetMouseButtonDown(1)) {
      if (_contextMenu.activeSelf) {
        HideMenu();
      } else {
        ShowMenu(ZInput.mousePosition / _contextMenuCanvas.scaleFactor);
      }
    }
  }

  public void ShowMenu(Vector2 position) {
    _contextMenu.GetComponent<RectTransform>().anchoredPosition = position;

    EventSystem.current.SetSelectedGameObject(_contextMenu);
    _contextMenu.SetActive(true);
  }

  public void HideMenu() {
    _contextMenu.SetActive(false);
  }

  static GameObject CreateMenu(Transform parentTransform) {
    GameObject menu = new("Menu", typeof(RectTransform));
    menu.transform.SetParent(parentTransform, worldPositionStays: false);

    menu.AddComponent<Image>()
        .SetSprite(UIResources.GetSprite("item_background"))
        .SetType(Image.Type.Sliced)
        .SetColor(new(0f, 0f, 0f, 0.565f))
        .SetRaycastTarget(true);

    menu.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.zero)
        .SetPivot(Vector2.up)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(200f, 0f));

    menu.AddComponent<VerticalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetPadding(left: 5, right: 5, top: 5, bottom: 5)
        .SetSpacing(5f);

    menu.AddComponent<ContentSizeFitter>()
        .SetHorizontalFit(ContentSizeFitter.FitMode.Unconstrained)
        .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

    return menu;
  }

  static MenuItem CreateMenuItem(Transform parentTransform, string label) {
    MenuItem menuItem = new(parentTransform);
    menuItem.SetText(label);

    return menuItem;
  }
}

public sealed class MenuItem {
  public readonly GameObject Container;
  public readonly RectTransform RectTransform;
  public readonly LayoutElement LayoutElement;
  public readonly Image Background;
  public readonly TMP_Text Label;
  public readonly Button Button;

  public MenuItem(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();
    LayoutElement = Container.GetComponent<LayoutElement>();
    Background = Container.GetComponent<Image>();
    Label = CreateLabel(Container.transform);
    Button = CreateButton(Container);
  }

  public void SetText(string text) {
    Label.text = text;
    Label.ForceMeshUpdate(ignoreActiveState: true);

    LayoutElement.SetPreferred(height: Label.preferredHeight + 10f);
  }

  static GameObject CreateContainer(Transform parentTransform) {
    GameObject container = new("MenuItem", typeof(RectTransform));
    container.transform.SetParent(parentTransform, worldPositionStays: true);

    container.AddComponent<Image>()
        .SetSprite(UIResources.GetSprite("item_background"))
        .SetType(Image.Type.Sliced)
        .SetColor(new(1f, 1f, 1f, 0.759f))
        .SetRaycastTarget(true);

    container.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    container.AddComponent<LayoutElement>()
        .SetFlexible(width: 1f)
        .SetPreferred(height: 35f);

    return container;
  }

  static TMP_Text CreateLabel(Transform parentTransform) {
    TMP_Text label = UIBuilder.CreateTMPLabel(parentTransform);
    label.transform.SetParent(parentTransform, worldPositionStays: false);

    label.rectTransform
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(-10f, 0f));

    label
        .SetFontSize(16f)
        .SetAlignment(TextAlignmentOptions.Left)
        .SetTextWrappingMode(TextWrappingModes.NoWrap)
        .SetOverflowMode(TextOverflowModes.Ellipsis)
        .SetText("...");

    return label;
  }

  static Button CreateButton(GameObject container) {
    Button button = container.AddComponent<Button>();

    button
        .SetTransition(Selectable.Transition.ColorTint)
        .SetColors(
            new ColorBlock() {
              normalColor = new(0.353f, 0.35f, 0.35f, 1f),
              highlightedColor = new(0.625f, 0.625f, 0.625f, 1f),
              pressedColor = new(0.890f, 0.890f, 0.890f, 1f),
              selectedColor = new(0.625f, 0.625f, 0.625f, 1f),
              disabledColor = new(0.345f, 0.345f, 0.345f, 0.5f),
              colorMultiplier = 1f,
              fadeDuration = 0.1f
            });

    return button;
  }
}
