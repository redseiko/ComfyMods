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

    colorPicker.ConfirmButton.Button.onClick.AddListener(OnConfirmButtonClick);
    colorPicker.CloseButton.Button.onClick.AddListener(OnCloseButtonClick);

    return colorPicker;
  }

  public bool IsVisible() {
    return ColorPicker.Panel.activeSelf;
  }

  public void ShowColorPicker(Color currentColor, Action<Color> onColorSelectedCallback = default) {
    ColorPicker.SetCurrentColor(currentColor);
    ShowColorPicker(onColorSelectedCallback);
  }

  public void ShowColorPicker(Action<Color> onColorSelectedCallback = default) {
    if (onColorSelectedCallback != default) {
      _onColorSelectedCallbacks.Enqueue(onColorSelectedCallback);
    }

    ShowColorPicker();
  }

  public void ShowColorPicker() {
    ColorPicker.Panel.SetActive(true);
  }

  public void HideColorPicker() {
    ColorPicker.Panel.SetActive(false);
  }

  readonly Queue<Action<Color>> _onColorSelectedCallbacks = new();

  void OnConfirmButtonClick() {
    while (_onColorSelectedCallbacks.Count > 0) {
      Action<Color> callback = _onColorSelectedCallbacks.Dequeue();
      callback?.Invoke(ColorPicker.CurrentColor);
    }

    HideColorPicker();
  }

  void OnCloseButtonClick() {
    _onColorSelectedCallbacks.Clear();
    HideColorPicker();
  }
}
