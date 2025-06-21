namespace Contextual;

using System.Collections.Generic;

using ComfyLib;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public sealed class ContextMenuController : MonoBehaviour, IPointerClickHandler, IDeselectHandler {
  RectTransform _parentRectTransform;
  Canvas _parentCanvas;
  ContextMenuPanel _contextMenu;

  public List<ContextMenuItem> MenuTitles { get; } = [];
  public List<ContextMenuItem> MenuItems { get; } = [];

  void Awake() {
    _parentRectTransform = (RectTransform) transform;
    _parentCanvas = _parentRectTransform.GetComponentInParent<Canvas>();
    _contextMenu = new(_parentRectTransform);
    _contextMenu.ClickHandler.OnLeftClickOutsideRect.AddListener(HideMenu);

    HideMenu();
  }

  public void OnPointerClick(PointerEventData eventData) {
    switch (eventData.button) {
      case PointerEventData.InputButton.Left:
        HandleLeftClick(eventData);
        break;

      case PointerEventData.InputButton.Right:
        HandleRightClick(eventData);
        break;
    }
  }

  public void OnDeselect(BaseEventData eventData) {
    HideMenu();
  }

  void HandleLeftClick(PointerEventData eventData) {
    Vector2 position = eventData.position;
    Vector2 scaledPosition = position / _parentCanvas.scaleFactor;

    if (_contextMenu.Container.activeSelf
        && !RectTransformUtility.RectangleContainsScreenPoint(_contextMenu.RectTransform, scaledPosition)) {
      HideMenu();
    }
  }

  void HandleRightClick(PointerEventData eventData) {
    Vector2 position = eventData.position;
    Vector2 scaledPosition = position / _parentCanvas.scaleFactor;

    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        _parentRectTransform, scaledPosition, null, out Vector2 localPoint);

    ShowMenu(localPoint);
  }

  public void ShowMenu(Vector2 position) {
    _contextMenu.RectTransform
        .SetLocalPosition(position)
        .SetAsLastSibling();

    EventSystem.current.SetSelectedGameObject(
        MenuItems.Count > 0 ? MenuItems[0].Button.gameObject : _contextMenu.Container);

    _contextMenu.Container.SetActive(true);
  }

  public void HideMenu() {
    _contextMenu.Container.SetActive(false);
  }

  public void ResetMenu() {
    foreach (ContextMenuItem menuTitle in MenuTitles) {
      Destroy(menuTitle.Container);
    }

    foreach (ContextMenuItem menuItem in MenuItems) {
      Destroy(menuItem.Container);
    }

    MenuTitles.Clear();
    MenuItems.Clear();
  }

  public ContextMenuItem AddMenuTitle(string label) {
    ContextMenuItem menuTitle = new(_contextMenu.RectTransform);

    menuTitle.Label.SetColor(new(1f, 0.72f, 0.36f, 1f));
    menuTitle.Background.SetColor(Color.clear);
    menuTitle.SetText(label);

    MenuTitles.Add(menuTitle);

    return menuTitle;
  }

  public ContextMenuItem AddMenuItem(string label, UnityAction onClickCallback = default) {
    ContextMenuItem menuItem = new(_contextMenu.RectTransform);

    menuItem.SetText(label);
    menuItem.AddOnClickListener(HideMenu);

    if (onClickCallback != default) {
      menuItem.AddOnClickListener(onClickCallback);
    }

    MenuItems.Add(menuItem);

    return menuItem;
  }

  public void SetMenuWidth(float width) {
    _contextMenu.RectTransform.SetSizeDelta(new(width, 0f));
  }
}
