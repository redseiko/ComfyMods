namespace Contextual;

using ComfyLib;

using UnityEngine;
using UnityEngine.EventSystems;

public sealed class ContextMenuController :
    MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler {
  ContextMenu _contextMenu;
  Canvas _contextMenuCanvas;

  void Awake() {
    _contextMenu = new(transform);
    _contextMenuCanvas = _contextMenu.Container.GetComponentInParent<Canvas>();

    CreateMenuTitle("Test Context?");
    CreateMenuItem("Test One");
    CreateMenuItem("Not Test Two");
    CreateMenuItem("Three");

    HideMenu();
  }

  //void Update() {
  //  if (ZInput.GetButtonDown("MouseLeft")) {
  //    HandleLeftClick(ZInput.mousePosition);
  //  } else if (ZInput.GetButtonDown("MouseRight")) {
  //    HandleRightClick(ZInput.mousePosition);
  //  }
  //}

  public void OnPointerClick(PointerEventData eventData) {
    switch (eventData.button) {
      case PointerEventData.InputButton.Left:
        HandleLeftClick(eventData.position);
        break;

      case PointerEventData.InputButton.Right:
        HandleRightClick(eventData.position);
        break;
    }
  }

  public void OnPointerEnter(PointerEventData eventData) {
    // ???
  }

  public void OnPointerExit(PointerEventData eventData) {
    // ???
  }

  public void OnDeselect(BaseEventData eventData) {
    HideMenu();
  }

  void HandleLeftClick(Vector2 position) {
    Vector2 scaledPosition = position / _contextMenuCanvas.scaleFactor;

    if (_contextMenu.Container.activeSelf
        && !RectTransformUtility.RectangleContainsScreenPoint(_contextMenu.RectTransform, scaledPosition)) {
      HideMenu();
    }
  }

  void HandleRightClick(Vector2 position) {
    Vector2 scaledPosition = position / _contextMenuCanvas.scaleFactor;

    ShowMenu(scaledPosition);
  }

  public void ShowMenu(Vector2 position) {
    _contextMenu.RectTransform.anchoredPosition = position;
    _contextMenu.RectTransform.SetAsLastSibling();

    EventSystem.current.SetSelectedGameObject(_contextMenu.Container);
    _contextMenu.Container.SetActive(true);
  }

  public void HideMenu() {
    _contextMenu.Container.SetActive(false);
  }

  ContextMenuItem CreateMenuTitle(string label) {
    ContextMenuItem menuTitle = new(_contextMenu.RectTransform);

    // menuTitle.Label.SetColor(new(1f, 0.81f, 0f, 1f));
    menuTitle.Label.SetColor(new(1f, 0.72f, 0.36f, 1f));
    menuTitle.Background.SetColor(Color.clear);

    menuTitle.SetText(label);

    return menuTitle;
  }

  ContextMenuItem CreateMenuItem(string label) {
    ContextMenuItem menuItem = new(_contextMenu.RectTransform);
    menuItem.SetText(label);
    menuItem.AddOnClickListener(HideMenu);

    return menuItem;
  }
}
