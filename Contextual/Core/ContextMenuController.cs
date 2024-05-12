namespace Contextual;

using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

public sealed class ContextMenuController : MonoBehaviour {
  GameObject _contextMenu;
  Canvas _contextMenuCanvas;

  void Awake() {
    _contextMenu = CreateMenu(gameObject.transform);
    _contextMenuCanvas = _contextMenu.GetComponentInParent<Canvas>();

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
    _contextMenu.SetActive(true);
  }

  public void HideMenu() {
    _contextMenu.SetActive(false);
  }

  static GameObject CreateMenu(Transform parentTransform) {
    GameObject menu = new("Menu", typeof(RectTransform));
    menu.transform.SetParent(parentTransform);

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
        .SetSizeDelta(new(200f, 400f));

    menu.AddComponent<VerticalLayoutGroup>()
        .SetChildControl(width: false, height: false)
        .SetChildForceExpand(width: true, height: false)
        .SetPadding(left: 5, right: 5, top: 5, bottom: 5)
        .SetSpacing(5f);

    //menu.AddComponent<ContentSizeFitter>()
    //    .SetHorizontalFit(ContentSizeFitter.FitMode.PreferredSize)
    //    .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

    return menu;
  }
}
