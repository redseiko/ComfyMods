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

  ColorPickerController(Transform parentTransform) {
    ColorPicker = CreateColorPicker(parentTransform);
  }

  ColorPickerPanel CreateColorPicker(Transform parentTransform) {
    ColorPickerPanel colorPicker = new(parentTransform);

    colorPicker.RectTransform
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(400f, 500f));

    colorPicker.ConfirmButton.AddOnClickListener(SelectColor);
    colorPicker.CloseButton.AddOnClickListener(CancelColor);
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
    ShowColorPicker();
  }

  public void ShowColorPicker() {
    ColorPicker.Panel.SetActive(true);
  }

  public void HideColorPicker() {
    ColorPicker.Panel.SetActive(false);
  }

  readonly List<Action<Color>> _onColorSelectCallbacks = [];
  readonly List<Action<IEnumerable<Color>>> _onPaletteColorsChangeCallbacks = [];

  void SelectColor() {
    HideColorPicker();

    foreach (Action<Color> callback in _onColorSelectCallbacks) {
      callback(ColorPicker.CurrentColor);
    }

    _onColorSelectCallbacks.Clear();
    _onPaletteColorsChangeCallbacks.Clear();
  }

  void CancelColor() {
    HideColorPicker();

    _onColorSelectCallbacks.Clear();
    _onPaletteColorsChangeCallbacks.Clear();
  }

  readonly List<Color> _currentPaletteColors = [];

  void SetPaletteColors(
      IEnumerable<Color> colors, Action<IEnumerable<Color>> onPaletteColorsChangeCallback = default) {
    if (colors == default) {
      ColorPicker.AddColorButton.Container.SetActive(false);
      ColorPicker.ColorPalette.Container.SetActive(false);
      return;
    }

    _currentPaletteColors.Clear();
    _currentPaletteColors.AddRange(colors);

    // TODO: redseiko - instead of clearing ALL slots, re-use existing ones first, then clear the remaining.
    ColorPicker.ColorPalette.ClearSlots();

    foreach (Color color in _currentPaletteColors) {
      AddPaletteSlot(color);
    }

    if (onPaletteColorsChangeCallback == default) {
      ColorPicker.AddColorButton.Container.SetActive(false);
    } else {
      ColorPicker.AddColorButton.Container.SetActive(true);
      _onPaletteColorsChangeCallbacks.Add(onPaletteColorsChangeCallback);
    }
  }

  void SelectPaletteColor(Color color) {
    ColorPicker.SetColor(color);
    SelectColor();
  }

  void AddPaletteColor() {
    Color color = ColorPicker.CurrentColor;
    _currentPaletteColors.Add(color);

    AddPaletteSlot(color);

    foreach (Action<IEnumerable<Color>> callback in _onPaletteColorsChangeCallbacks) {
      callback(_currentPaletteColors);
    }
  }

  void AddPaletteSlot(Color color) {
    PaletteSlot slot = ColorPicker.ColorPalette.AddSlot();
    slot.OnSelect.AddListener(SelectPaletteColor);
    slot.SetColor(color);
  }
}
