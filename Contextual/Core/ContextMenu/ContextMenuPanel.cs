namespace Contextual;

using ComfyLib;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class ContextMenuPanel {
  public GameObject Container { get; private set; }
  public RectTransform RectTransform { get; private set; }
  public PointerClickHandler ClickHandler { get; private set; }

  public ContextMenuPanel(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();
    ClickHandler = Container.AddComponent<PointerClickHandler>();
  }

  static GameObject CreateContainer(Transform parentTransform) {
    GameObject container = new("ContextMenu", typeof(RectTransform));
    container.transform.SetParent(parentTransform, worldPositionStays: false);

    container.AddComponent<Image>()
        .SetSprite(UIResources.GetSprite("item_background"))
        .SetType(Image.Type.Sliced)
        .SetColor(new(0f, 0f, 0f, 0.95f))
        .SetRaycastTarget(true);

    container.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.zero)
        .SetPivot(Vector2.up)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(200f, 0f));

    container.AddComponent<VerticalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetPadding(left: 5, right: 5, top: 5, bottom: 5)
        .SetSpacing(5f);

    container.AddComponent<ContentSizeFitter>()
        .SetHorizontalFit(ContentSizeFitter.FitMode.PreferredSize)
        .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

    return container;
  }
}

public sealed class PointerClickHandler : MonoBehaviour {
  public RectTransform RectTransform { get; private set; }
  public UnityEvent OnLeftClickOutsideRect { get; private set; }

  void Awake() {
    RectTransform = GetComponent<RectTransform>();
    OnLeftClickOutsideRect = new();
  }

  void Update() {
    if (ZInput.GetButtonDown("MouseLeft")) {
      HandleLeftClick(ZInput.mousePosition);
    }
  }

  void HandleLeftClick(Vector2 position) {
    if (!RectTransformUtility.RectangleContainsScreenPoint(RectTransform, position)) {
      OnLeftClickOutsideRect.Invoke();
    }
  }
}
