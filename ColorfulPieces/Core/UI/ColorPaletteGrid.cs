namespace ColorfulPieces;

using System.Collections.Generic;

using ComfyLib;

using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public sealed class ColorPaletteGrid {
  public GameObject Container { get; private set; }
  public RectTransform RectTransform { get; private set; }
  public GridLayoutGroup LayoutGroup { get; private set; }
  public Image Background { get; private set; }

  public List<PaletteSlot> PaletteSlots { get; } = [];
  public PaletteSlot this[int index] { get => PaletteSlots[index]; }

  public ColorPaletteGrid(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();
    LayoutGroup = Container.GetComponent<GridLayoutGroup>();
    Background = Container.GetComponent<Image>();
  }

  public void ClearSlots() {
    for (int i = PaletteSlots.Count - 1; i >= 0; i--) {
      Object.Destroy(PaletteSlots[i].Container);
    }

    PaletteSlots.Clear();
  }

  public PaletteSlot AddSlot() {
    PaletteSlot slot = new(RectTransform);
    PaletteSlots.Add(slot);

    return slot;
  }

  public void RemoveSlot(int slotIndex) {
    if (slotIndex >= 0 && slotIndex < PaletteSlots.Count) {
      Object.Destroy(PaletteSlots[slotIndex].Container);
      PaletteSlots.RemoveAt(slotIndex);
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
        .SetChildAlignment(TextAnchor.UpperLeft)
        .SetCellSize(new(50f, 50f))
        .SetSpacing(new(10f, 10f));

    container.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIResources.GetSprite("item_background"))
        .SetColor(new(0f, 0f, 0f, 0.1f));

    container.AddComponent<ContentSizeFitter>()
        .SetHorizontalFit(ContentSizeFitter.FitMode.Unconstrained)
        .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

    return container;
  }
}

public sealed class PaletteSlot {
  public GameObject Container { get; private set; }
  public RectTransform RectTransform { get; private set; }
  public LayoutElement LayoutElement { get; private set; }

  public LabelButton SelectButton { get; private set; }
  public ColorImage ColorPreview { get; private set; }

  public Color Color { get; private set; }
  public string ColorHex { get; private set; }

  public UnityEvent<PaletteSlot> OnSelect { get; private set; } = new();

  public PaletteSlot(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();
    LayoutElement = Container.GetComponent<LayoutElement>();

    SelectButton = CreateSelectButton(RectTransform);
    SelectButton.Button.onClick.AddListener(OnSelectButtonClick);

    ColorPreview = CreateColorPreview(SelectButton.RectTransform);

    SetColor(Color.white);
  }

  public void SetColor(Color color) {
    Color = color;
    ColorHex = "#" + (color.a < 1f ? ColorUtility.ToHtmlStringRGBA(color) : ColorUtility.ToHtmlStringRGB(color));

    ColorPreview.SetColor(Color);
  }

  void OnSelectButtonClick() {
    OnSelect.Invoke(this);
  }

  static GameObject CreateContainer(Transform parentTransform) {
    GameObject container = new("Slot", typeof(RectTransform));
    container.transform.SetParent(parentTransform, worldPositionStays: false);

    container.GetComponent<RectTransform>()
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(50f, 50f));

    container.AddComponent<LayoutElement>()
        .SetPreferred(width: 50f, height: 50f);

    return container;
  }

  static LabelButton CreateSelectButton(Transform parentTransform) {
    LabelButton selectSlot = new(parentTransform);
    selectSlot.Container.name = "Select";

    selectSlot.RectTransform
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    selectSlot.Label.SetText(string.Empty);

    selectSlot.Container.AddComponent<IgnoreDragHandler>();

    return selectSlot;
  }

  static ColorImage CreateColorPreview(Transform parentTransform) {
    ColorImage colorPreview = new(parentTransform);
    colorPreview.Container.name = "ColorPreview";

    colorPreview.RectTransform
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(30f, 30f));

    return colorPreview;
  }
}

public sealed class ColorImage {
  public GameObject Container { get; private set; }
  public RectTransform RectTransform { get; private set; }

  public Image Background { get; private set; }
  public Image Image { get; private set; }

  public ColorImage(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();
    Background = Container.GetComponent<Image>();
    Image = CreateImage(RectTransform);
  }

  public void SetColor(Color color) {
    Image.color = color;
  }

  static GameObject CreateContainer(Transform parentTransform) {
    GameObject container = new("ColorImage", typeof(RectTransform));
    container.transform.SetParent(parentTransform, worldPositionStays: false);

    container.GetComponent<RectTransform>()
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(30f, 30f));

    container.AddComponent<Image>()
        .SetType(Image.Type.Tiled)
        .SetSprite(UIBuilder.CreateCheckerboardSprite(30, 30, 15))
        .SetColor(Color.white);

    return container;
  }

  static Image CreateImage(Transform parentTransform) {
    GameObject color = new("Image", typeof(RectTransform));
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
