namespace ColorfulPieces;

using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

public sealed class ColorPalettePanel {
  public GameObject Panel { get; private set; }
  public RectTransform RectTransform { get; private set; }

  public LabelButton AddSlotButton { get; private set; }
  public LabelButton RemoveSlotButton { get; private set; }

  public GameObject PaletteViewport { get; private set; }
  public ColorPaletteGrid PaletteGrid { get; private set; }
  public ScrollRect PaletteScrollRect { get; private set; }

  public ColorPalettePanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();

    AddSlotButton = CreateAddSlotButton(RectTransform);
    RemoveSlotButton = CreateRemoveSlotButton(RectTransform);

    PaletteViewport = CreatePaletteViewport(RectTransform);
    PaletteGrid = CreatePaletteGrid(PaletteViewport.transform);
    PaletteScrollRect = CreatePaletteScrollRect(PaletteViewport, PaletteGrid.Container);
  }

  static GameObject CreatePanel(Transform parentTransform) {
    GameObject panel = UIBuilder.CreatePanel(parentTransform);
    panel.name = "ColorPalettePanel";

    return panel;
  }

  static LabelButton CreateAddSlotButton(Transform parentTransform) {
    LabelButton addSlot = new(parentTransform);
    addSlot.Container.name = "AddSlot";

    addSlot.RectTransform
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.up)
        .SetPivot(Vector2.up)
        .SetPosition(new(20f, -20f))
        .SetSizeDelta(new(100f, 50f));

    addSlot.Label
        .SetFontSize(18f)
        .SetText("Add");

    return addSlot;
  }

  static LabelButton CreateRemoveSlotButton(Transform parentTransform) {
    LabelButton removeSlot = new(parentTransform);
    removeSlot.Container.name = "RemoveSlot";

    removeSlot.RectTransform
        .SetAnchorMin(Vector2.one)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.one)
        .SetPosition(new(-20f, -20f))
        .SetSizeDelta(new(60f, 50f));

    removeSlot.Label
        .SetFontSize(18f)
        .SetText("\U0001F5D1");

    return removeSlot;
  }

  static GameObject CreatePaletteViewport(Transform parentTransform) {
    GameObject paletteViewport = new("PaletteViewport", typeof(RectTransform));
    paletteViewport.transform.SetParent(parentTransform, worldPositionStays: false);

    paletteViewport.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.up)
        .SetPosition(new(20f, -85f))
        .SetSizeDelta(new(-37.5f, -105f));

    paletteViewport.AddComponent<RectMask2D>();

    return paletteViewport;
  }

  static ColorPaletteGrid CreatePaletteGrid(Transform parentTransform) {
    ColorPaletteGrid paletteGrid = new(parentTransform);

    paletteGrid.RectTransform
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.up)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    return paletteGrid;
  }

  static ScrollRect CreatePaletteScrollRect(GameObject viewport, GameObject content) {
    ScrollRect paletteScrollRect =
        viewport.AddComponent<ScrollRect>()
            .SetViewport(viewport.GetComponent<RectTransform>())
            .SetContent(content.GetComponent<RectTransform>())
            .SetHorizontal(false)
            .SetVertical(true)
            .SetScrollSensitivity(20f)
            .SetMovementType(ScrollRect.MovementType.Clamped);

    Scrollbar scrollbar = UIBuilder.CreateScrollbar(viewport.transform);
    scrollbar.direction = Scrollbar.Direction.BottomToTop;

    paletteScrollRect
        .SetVerticalScrollbar(scrollbar)
        .SetVerticalScrollbarVisibility(ScrollRect.ScrollbarVisibility.Permanent);

    return paletteScrollRect;
  }
}
