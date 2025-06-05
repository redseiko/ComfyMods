namespace ColorfulPieces;

using ComfyLib;

using UnityEngine;

public sealed class ColorPalettePanel {
  public GameObject Panel { get; private set; }
  public RectTransform RectTransform { get; private set; }
  public ColorPaletteGrid PaletteGrid { get; private set; }
  public LabelButton AddSlotButton { get; private set; }

  public ColorPalettePanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();
    PaletteGrid = CreatePaletteGrid(RectTransform);
    AddSlotButton = CreateAddSlotButton(RectTransform);
  }

  static GameObject CreatePanel(Transform parentTransform) {
    GameObject panel = UIBuilder.CreatePanel(parentTransform);
    panel.name = "ColorPalettePanel";

    return panel;
  }

  static LabelButton CreateAddSlotButton(Transform parentTransform) {
    LabelButton addSlotButton = new(parentTransform);
    addSlotButton.Container.name = "AddSlot";

    addSlotButton.RectTransform
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.up)
        .SetPivot(Vector2.up)
        .SetPosition(new(20f, -20f))
        .SetSizeDelta(new(75f, 50f));

    addSlotButton.Label
        .SetFontSize(18f)
        .SetText("Add");

    return addSlotButton;
  }

  static ColorPaletteGrid CreatePaletteGrid(Transform parentTransform) {
    ColorPaletteGrid paletteGrid = new(parentTransform);

    paletteGrid.RectTransform
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.up)
        .SetPosition(new(20f, -80f))
        .SetSizeDelta(new(-40f, -100f));

    return paletteGrid;
  }
}
