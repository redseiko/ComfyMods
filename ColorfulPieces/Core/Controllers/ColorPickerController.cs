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

  public ColorPickerPanel ColorPicker { get; private set; }
  public ColorPalettePanel ColorPalette { get; private set; }

  ColorPickerController(Transform parentTransform) {
    ColorPicker = CreateColorPicker(parentTransform);
    ColorPalette = ColorPicker.PalettePanel;
  }

  ColorPickerPanel CreateColorPicker(Transform parentTransform) {
    ColorPickerPanel colorPicker = new(parentTransform);

    colorPicker.RectTransform
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(400f, 570f));

    colorPicker.SelectButton.AddOnClickListener(SelectCurrentColor);
    colorPicker.CloseButton.AddOnClickListener(HideColorPicker);
    colorPicker.PalettePanel.AddSlotButton.AddOnClickListener(AddPaletteSlot);
    colorPicker.PalettePanel.RemoveSlotButton.AddOnClickListener(RemovePaletteSlot);

    colorPicker.Panel.AddComponent<ColorPickerPanelController>();

    return colorPicker;
  }

  public bool IsVisible() {
    return ColorPicker.Panel.activeSelf;
  }

  bool _selectColorOnClose = false;
  readonly List<Action<Color>> _selectColorCallbacks = [];
  readonly List<Action<float>> _selectEmissionColorFactorCallbacks = [];
  readonly List<Action<IEnumerable<Color>>> _changePaletteColorsCallbacks = [];
  readonly List<Color> _currentPaletteColors = [];

  public void ShowColorPicker(Action<Color> selectColorCallback) {
    ShowColorPicker(ColorPicker.CurrentColor, selectColorCallback);
  }

  public void ShowColorPicker(
      Color currentColor,
      Action<Color> selectColorCallback = default,
      bool selectColorOnClose = false,
      bool useColorAlpha = true,
      bool showEmissionColorFactor = false,
      float currentEmissionColorFactor = 0f,
      Action<float> selectEmissionColorFactorCallback = default,
      IEnumerable<Color> paletteColors = default,
      Action<IEnumerable<Color>> changePaletteColorsCallback = default) {
    SetupPicker(
        currentColor,
        selectColorCallback,
        selectColorOnClose,
        useColorAlpha,
        showEmissionColorFactor,
        currentEmissionColorFactor,
        selectEmissionColorFactorCallback);

    SetupPalette(paletteColors, changePaletteColorsCallback);

    ColorPicker.Panel.SetActive(true);
  }

  public void HideColorPicker() {
    if (_selectColorOnClose) {
      foreach (Action<Color> callback in _selectColorCallbacks) {
        callback(ColorPicker.CurrentColor);
      }

      foreach (Action<float> callback in _selectEmissionColorFactorCallbacks) {        
        callback(ColorPicker.CurrentEmissionColorFactor);
      }
    }

    _selectColorCallbacks.Clear();
    _selectEmissionColorFactorCallbacks.Clear();
    _changePaletteColorsCallbacks.Clear();
    _currentPaletteColors.Clear();

    ColorPicker.Panel.SetActive(false);
  }

  void SelectCurrentColor() {
    _selectColorOnClose = true;
    HideColorPicker();
  }

  void SetupPicker(
      Color currentColor,
      Action<Color> selectColorCallback,
      bool selectColorOnClose,
      bool useColorAlpha,
      bool showEmissionColorFactor,
      float currentEmissionColorFactor,
      Action<float> selectEmissionColorFactorCallback) {
    ColorPicker.SetUseColorAlpha(useColorAlpha);

    if (!useColorAlpha) {
      currentColor.a = 1f;
    }

    ColorPicker.SetColor(currentColor);

    if (selectColorCallback != default) {
      _selectColorCallbacks.Add(selectColorCallback);
    }

    ColorPicker.EmissionColorFactorSlider.Container.SetActive(showEmissionColorFactor);

    if (showEmissionColorFactor) {
      ColorPicker.SetEmissionColorFactor(currentEmissionColorFactor);

      if (selectEmissionColorFactorCallback != default) {
        _selectEmissionColorFactorCallbacks.Add(selectEmissionColorFactorCallback);
      }      
    }

    _selectColorOnClose = selectColorOnClose;
    ColorPicker.CloseButton.Container.SetActive(!selectColorOnClose);
  }

  void SetupPalette(IEnumerable<Color> paletteColors, Action<IEnumerable<Color>> changePaletteColorsCallback) {
    if (paletteColors == default) {
      _currentPaletteColors.Clear();
      _changePaletteColorsCallbacks.Clear();

      ColorPalette.Panel.SetActive(false);

      return;
    }

    _currentPaletteColors.Clear();
    _currentPaletteColors.AddRange(paletteColors);

    SetupPaletteSlots();

    if (changePaletteColorsCallback == default) {
      ColorPalette.AddSlotButton.Container.SetActive(false);
      ColorPalette.RemoveSlotButton.Container.SetActive(false);
    } else {
      ColorPalette.AddSlotButton.Container.SetActive(true);
      ColorPalette.RemoveSlotButton.Container.SetActive(true);

      _changePaletteColorsCallbacks.Add(changePaletteColorsCallback);
    }
  }

  void SetupPaletteSlots() {
    int colorCount = _currentPaletteColors.Count;
    int slotCount = ColorPalette.PaletteGrid.PaletteSlots.Count;

    if (colorCount < slotCount) {
      for (int i = slotCount - 1; i >= colorCount; i--) {
        ColorPalette.PaletteGrid.RemoveSlot(i);
      }
    } else if (colorCount > slotCount) {
      for (int i = slotCount; i < colorCount; i++) {
        PaletteSlot slot = ColorPalette.PaletteGrid.AddSlot();
        slot.OnSelect.AddListener(SelectPaletteSlot);
      }
    }

    for (int i = 0; i < colorCount; i++) {
      ColorPalette.PaletteGrid[i].SetColor(_currentPaletteColors[i]);
    }
  }

  // TODO: redseiko - make this not use hard-coded keys.
  void SelectPaletteSlot(PaletteSlot slot) {
    if (ZInput.GetKey(KeyCode.LeftShift, logWarning: false)) {
      ColorPicker.SetColor(slot.Color);
    } else if (ZInput.GetKey(KeyCode.LeftControl, logWarning: false)) {
      int slotIndex = ColorPalette.PaletteGrid.PaletteSlots.IndexOf(slot);

      if (slotIndex >= 0 && slotIndex < _currentPaletteColors.Count) {
        _currentPaletteColors.RemoveAt(slotIndex);
        SetupPaletteSlots();
      }
    } else {
      ColorPicker.SetColor(slot.Color);
      SelectCurrentColor();
    }
  }

  void AddPaletteSlot() {
    Color color = ColorPicker.CurrentColor;
    _currentPaletteColors.Add(color);

    SetupPaletteSlots();
    InvokeChangePaletteColors(_currentPaletteColors);
  }

  void RemovePaletteSlot() {
    if (_currentPaletteColors.Count <= 0) {
      return;
    }

    _currentPaletteColors.RemoveAt(_currentPaletteColors.Count - 1);

    SetupPaletteSlots();
    InvokeChangePaletteColors(_currentPaletteColors);
  }

  void InvokeChangePaletteColors(List<Color> colors) {
    foreach (Action<IEnumerable<Color>> callback in _changePaletteColorsCallbacks) {
      callback(colors);
    }
  }

  public sealed class ColorPickerPanelController : MonoBehaviour {
    void Update() {
      // Prevent GameCamera zooming when using mouse scroll-wheel.
      StoreGui.m_instance.m_hiddenFrames = 0;
    }

    void LateUpdate() {
      // Hide ColorPickerPanel when Escape is pressed.
      if (ZInput.GetKeyDown(KeyCode.Escape, logWarning: false)) {
        ColorPickerController.Instance.HideColorPicker();
      }
    }
  }
}
