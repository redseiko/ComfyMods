namespace Contextual;

using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

public sealed class ContextMenu {
  public readonly GameObject Container;
  public readonly RectTransform RectTransform;

  public ContextMenu(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();
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
