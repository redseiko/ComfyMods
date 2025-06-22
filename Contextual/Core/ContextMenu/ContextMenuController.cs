namespace Contextual;

using System.Collections.Generic;

using ComfyLib;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public sealed class ContextMenuTrigger : MonoBehaviour, IPointerClickHandler {
  public ContextMenuController ContextMenu { get; private set; }
  public RectTransform RectTransform { get; private set; }

  public void SetContextMenu(ContextMenuController contextMenu) {
    ContextMenu = contextMenu;
  }

  void Awake() {
    RectTransform = GetComponent<RectTransform>();
  }

  public void OnPointerClick(PointerEventData eventData) {
    switch (eventData.button) {
      case PointerEventData.InputButton.Right:
        HandleRightClick(eventData);
        break;
    }
  }

  void HandleRightClick(PointerEventData eventData) {
    Vector2 position = eventData.position;
    RectTransformUtility.ScreenPointToLocalPointInRectangle(RectTransform, position, null, out Vector2 localPoint);

    ContextMenu.ShowMenu(localPoint);
  }
}

public sealed class ContextMenuController : MonoBehaviour {
  RectTransform _parentRectTransform;
  ContextMenuPanel _contextMenu;

  public List<ContextMenuItem> MenuTitles { get; } = [];
  public List<ContextMenuItem> MenuItems { get; } = [];

  void Awake() {
    _parentRectTransform = (RectTransform) transform;
    _contextMenu = new(_parentRectTransform);
    _contextMenu.ClickHandler.OnLeftClickOutsideRect.AddListener(HideMenu);

    HideMenu();
  }

  //public void OnPointerClick(PointerEventData eventData) {
  //  switch (eventData.button) {
  //    case PointerEventData.InputButton.Left:
  //      HandleLeftClick(eventData);
  //      break;
  //  }
  //}

  //void HandleLeftClick(PointerEventData eventData) {
  //  Vector2 position = eventData.position;

  //  if (_contextMenu.Container.activeSelf
  //      && !RectTransformUtility.RectangleContainsScreenPoint(_contextMenu.RectTransform, position)) {
  //    HideMenu();
  //  }
  //}

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
