namespace ColorfulPieces;

using System.Collections.Generic;

using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

public sealed class ColorPalette {
  public GameObject Container { get; private set; }
  public RectTransform RectTransform { get; private set; }
  public GridLayoutGroup LayoutGroup { get; private set; }
  public Image Background { get; private set; }
  public List<PaletteItem> PaletteItems { get; private set; } = [];

  public ColorPalette(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();
    LayoutGroup = Container.GetComponent<GridLayoutGroup>();
    Background = Container.GetComponent<Image>();
  }

  public const float ItemWidth = 30f;
  public const float ItemHeight = 30f;
  public const float ItemSpacing = 10f;

  public void GenerateRandomItems(int count) {
    for (int i = 0; i < count; i++) {
      PaletteItem item = new(Container.transform);
      item.SetColor(Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f));
      PaletteItems.Add(item);
    }
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

    container.AddComponent<GridLayoutGroup>()
        .SetStartCorner(GridLayoutGroup.Corner.UpperLeft)
        .SetStartAxis(GridLayoutGroup.Axis.Horizontal)
        .SetCellSize(new(30f, 30f))
        .SetSpacing(new(10f, 10f));

    container.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIResources.GetSprite("item_background"))
        .SetColor(new(0f, 0f, 0f, 0.1f));

    return container;
  }
}

public sealed class PaletteItem {
  public GameObject Container { get; private set; }
  public RectTransform RectTransform { get; private set; }
  public LayoutElement LayoutElement { get; private set; }
  public Image Background { get; private set; }

  public Image ColorImage { get; private set; }

  public PaletteItem(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();
    LayoutElement = Container.GetComponent<LayoutElement>();
    Background = Container.GetComponent<Image>();
    ColorImage = CreateColorImage(Container.transform);
  }

  public void SetColor(Color color) {
    ColorImage.color = color;
  }

  static GameObject CreateContainer(Transform parentTransform) {
    GameObject container = new("Item", typeof(RectTransform));
    container.transform.SetParent(parentTransform, worldPositionStays: false);

    container.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.up)
        .SetPivot(Vector2.up)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(30f, 30f));

    container.AddComponent<LayoutElement>()
        .SetPreferred(width: 30f, height: 30f);

    container.AddComponent<Image>()
        .SetType(Image.Type.Tiled)
        .SetSprite(UIBuilder.CreateCheckerboardSprite(30, 30, 15))
        .SetColor(Color.white);

    return container;
  }

  static Image CreateColorImage(Transform parentTransform) {
    GameObject color = new("Color", typeof(RectTransform));
    color.transform.SetParent(parentTransform, worldPositionStays: false);

    color.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    return color.AddComponent<Image>()
        .SetType(Image.Type.Simple)
        .SetColor(Color.white);
  }
}
