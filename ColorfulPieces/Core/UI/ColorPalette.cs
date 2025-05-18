namespace ColorfulPieces;

using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

public sealed class ColorPalette {
  public GameObject Container { get; private set; }
  public RectTransform RectTransform { get; private set; }
  public Image Background { get; private set; }

  public ColorPalette(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();
    Background = Container.GetComponent<Image>();
  }

  static GameObject CreateContainer(Transform parentTransform) {
    GameObject container = new("Palette", typeof(RectTransform));
    container.transform.SetParent(parentTransform, worldPositionStays: false);

    container.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.right)
        .SetPivot(Vector2.zero)
        .SetPosition(new(20f, 20f))
        .SetSizeDelta(new(-40f, 140f));

    container.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIResources.GetSprite("item_background"))
        .SetColor(new(0f, 0f, 0f, 0.5f));

    return container;
  }
}
