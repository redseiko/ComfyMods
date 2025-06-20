namespace Contextual;

using ComfyLib;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;

public sealed class ContextMenuController :
    MonoBehaviour, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler, IDeselectHandler {
  RectTransform _parentRectTransform;
  Canvas _parentCanvas;
  ContextMenu _contextMenu;

  void Awake() {
    _parentRectTransform = (RectTransform) transform;
    _parentCanvas = _parentRectTransform.GetComponentInParent<Canvas>();
    _contextMenu = new(_parentRectTransform);

    HideMenu();
  }

  public void OnPointerClick(PointerEventData eventData) {
    switch (eventData.button) {
      case PointerEventData.InputButton.Left:
        HandleLeftClick(eventData.position);
        break;

      case PointerEventData.InputButton.Right:
        HandleRightClick(eventData);
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

    Contextual.LogInfo($"RightClick: {position:F2}, {scaledPosition:F2}, {localPoint:F2}");

    ShowMenu(localPoint);
  }

  public void ShowMenu(Vector2 position) {
    _contextMenu.RectTransform.localPosition = position;
    _contextMenu.RectTransform.SetAsLastSibling();

    EventSystem.current.SetSelectedGameObject(_contextMenu.Container);
    _contextMenu.Container.SetActive(true);
  }

  public void HideMenu() {
    _contextMenu.Container.SetActive(false);
  }

  public ContextMenuItem AddMenuTitle(string label) {
    ContextMenuItem menuTitle = new(_contextMenu.RectTransform);

    menuTitle.Label.SetColor(new(1f, 0.72f, 0.36f, 1f));
    menuTitle.Background.SetColor(Color.clear);

    menuTitle.SetText(label);

    //Contextual.LogInfo($"MenuTitle localScale is: {menuTitle.RectTransform.localScale}");
    //menuTitle.RectTransform.localScale = Vector3.one;

    return menuTitle;
  }

  public ContextMenuItem AddMenuItem(string label, UnityAction onClickCallback = default) {
    ContextMenuItem menuItem = new(_contextMenu.RectTransform);

    menuItem.SetText(label);
    menuItem.AddOnClickListener(HideMenu);

    if (onClickCallback != default) {
      menuItem.AddOnClickListener(onClickCallback);
    }

    //Contextual.LogInfo($"MenuTitle localScale is: {menuItem.RectTransform.localScale}");
    //menuItem.RectTransform.localScale = Vector3.one;

    return menuItem;
  }

  public void SetMenuWidth(float width) {
    _contextMenu.RectTransform.SetSizeDelta(new(width, 0f));
  }
}
