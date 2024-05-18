namespace Contextual;

using UnityEngine;
using UnityEngine.EventSystems;

public sealed class ContextMenuController : MonoBehaviour {
  ContextMenu _contextMenu;
  Canvas _contextMenuCanvas;

  void Awake() {
    _contextMenu = new(transform);
    _contextMenuCanvas = _contextMenu.Container.GetComponentInParent<Canvas>();

    CreateMenuItem("Test One");
    CreateMenuItem("Not Test Two");
    CreateMenuItem("Three");

    HideMenu();
  }

  void Update() {
    if (ZInput.GetMouseButtonDown(1)) {
      if (_contextMenu.Container.activeSelf) {
        HideMenu();
      } else {
        ShowMenu(ZInput.mousePosition / _contextMenuCanvas.scaleFactor);
      }
    }
  }

  public void ShowMenu(Vector2 position) {
    _contextMenu.RectTransform.anchoredPosition = position;

    EventSystem.current.SetSelectedGameObject(_contextMenu.Container);
    _contextMenu.Container.SetActive(true);
  }

  public void HideMenu() {
    _contextMenu.Container.SetActive(false);
  }

  ContextMenuItem CreateMenuItem(string label) {
    ContextMenuItem menuItem = new(_contextMenu.RectTransform);
    menuItem.SetText(label);

    return menuItem;
  }
}
