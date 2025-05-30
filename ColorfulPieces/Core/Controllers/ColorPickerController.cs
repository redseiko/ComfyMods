namespace ColorfulPieces;

using System;
using System.Collections.Generic;

using ComfyLib;

using UnityEngine;

public sealed class ColorPickerController {
  static ColorPickerController _instance;

  public static ColorPickerController Instance {
    get {
      if (!_instance?.ColorPicker?.Panel) {
        ColorfulPieces.LogInfo($"Creating new ColorPicker.");
        _instance = new(UnifiedPopup.instance.gameObject.transform);
        _instance.HideColorPicker();
      }

      return _instance;
    }
  }

  public static bool HasVisibleInstance() {
    GameObject panel = _instance?.ColorPicker?.Panel;
    return panel && panel.activeSelf;
  }

  public ColorPickerPanel ColorPicker { get; }
  public ColorPaletteGrid ColorPalette { get; }

  ColorPickerController(Transform parentTransform) {
    ColorPicker = CreateColorPicker(parentTransform);
    ColorPalette = ColorPicker.ColorPalette;
  }

  ColorPickerPanel CreateColorPicker(Transform parentTransform) {
    ColorPickerPanel colorPicker = new(parentTransform);

    colorPicker.RectTransform
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(400f, 530f));

    colorPicker.ConfirmButton.AddOnClickListener(SelectColor);
    colorPicker.CloseButton.AddOnClickListener(HideColorPicker);
    colorPicker.AddColorButton.AddOnClickListener(AddPaletteColor);

    return colorPicker;
  }

  public bool IsVisible() {
    return ColorPicker.Panel.activeSelf;
  }

  public void ShowColorPicker(Action<Color> onColorSelectCallback) {
    ShowColorPicker(ColorPicker.CurrentColor, onColorSelectCallback);
  }

  public void ShowColorPicker(
      Color currentColor,
      Action<Color> onColorSelectCallback = default,
      IEnumerable<Color> paletteColors = default,
      Action<IEnumerable<Color>> onPaletteColorsChangeCallback = default) {
    ColorPicker.SetColor(currentColor);

    if (onColorSelectCallback != default) {
      _onColorSelectCallbacks.Add(onColorSelectCallback);
    }

    SetPaletteColors(paletteColors, onPaletteColorsChangeCallback);

    ColorPicker.Panel.SetActive(true);
  }

  public void HideColorPicker() {
    _onColorSelectCallbacks.Clear();
    _onPaletteColorsChangeCallbacks.Clear();

    ColorPicker.Panel.SetActive(false);
  }

  readonly List<Action<Color>> _onColorSelectCallbacks = [];
  readonly List<Action<IEnumerable<Color>>> _onPaletteColorsChangeCallbacks = [];

  void SelectColor() {
    foreach (Action<Color> callback in _onColorSelectCallbacks) {
      callback(ColorPicker.CurrentColor);
    }

    HideColorPicker();
  }

  readonly List<Color> _currentPaletteColors = [];

  void SetPaletteColors(
      IEnumerable<Color> colors, Action<IEnumerable<Color>> onPaletteColorsChangeCallback = default) {
    if (colors == default) {
      _currentPaletteColors.Clear();
      _onPaletteColorsChangeCallbacks.Clear();

      ColorPicker.AddColorButton.Container.SetActive(false);
      ColorPalette.Container.SetActive(false);

      return;
    }

    _currentPaletteColors.Clear();
    _currentPaletteColors.AddRange(colors);

    SetupPaletteSlots();

    if (onPaletteColorsChangeCallback == default) {
      ColorPicker.AddColorButton.Container.SetActive(false);
    } else {
      ColorPicker.AddColorButton.Container.SetActive(true);
      _onPaletteColorsChangeCallbacks.Add(onPaletteColorsChangeCallback);
    }
  }

  // TODO: redseiko - make this not use hard-coded keys.
  void SelectPaletteSlot(PaletteSlot slot) {
    if (ZInput.GetKey(KeyCode.LeftShift, logWarning: false)) {
      ColorPicker.SetColor(slot.Color);
    } else if (ZInput.GetKey(KeyCode.LeftControl, logWarning: false)) {
      int slotIndex = ColorPalette.PaletteSlots.IndexOf(slot);

      if (slotIndex >= 0 && slotIndex < _currentPaletteColors.Count) {
        _currentPaletteColors.RemoveAt(slotIndex);
        SetupPaletteSlots();
      }
    } else {
      ColorPicker.SetColor(slot.Color);
      SelectColor();
    }
  }

  void AddPaletteColor() {
    Color color = ColorPicker.CurrentColor;
    _currentPaletteColors.Add(color);

    SetupPaletteSlots();

    foreach (Action<IEnumerable<Color>> callback in _onPaletteColorsChangeCallbacks) {
      callback(_currentPaletteColors);
    }
  }

  void SetupPaletteSlots() {
    int colorCount = _currentPaletteColors.Count;
    int slotCount = ColorPalette.PaletteSlots.Count;

    if (colorCount < slotCount) {
      for (int i = slotCount - 1; i >= colorCount; i--) {
        ColorPalette.RemoveSlot(i);
      }
    } else if (colorCount > slotCount) {
      for (int i = slotCount; i < colorCount; i++) {
        PaletteSlot slot = ColorPalette.AddSlot();
        slot.OnSelect.AddListener(SelectPaletteSlot);
      }
    }

    for (int i = 0; i < colorCount; i++) {
      ColorPalette[i].SetColor(_currentPaletteColors[i]);
    }
  }
}
