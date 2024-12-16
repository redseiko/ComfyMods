namespace ColorfulPieces;

using ComfyLib;

using GUIFramework;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class ColorPickerPanel {
  public GameObject Panel { get; }
  public RectTransform RectTransform { get; }

  public LabelButton ConfirmButton { get; }
  public LabelButton CloseButton { get; }

  public Image CheckerboardColorImage { get; private set; }
  public GuiInputField HexValueInputField { get; }

  public ColorSlider RedSlider { get; }
  public ColorSlider GreenSlider { get; }
  public ColorSlider BlueSlider { get; }
  public ColorSlider AlphaSlider { get; }

  public ColorSlider HueSlider { get; }
  public ColorSlider SaturationSlider { get; }
  public ColorSlider BrightnessSlider { get; }

  public Texture2D BrightnessTexture { get; }
  public Texture2D SaturationTexture { get; }

  public Color CurrentColor { get; private set; } = Color.white;
  public string CurrentHexValue { get; private set; } = string.Empty;

  public ColorPickerPanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();

    ConfirmButton = CreateConfirmButton(RectTransform);
    CloseButton = CreateCloseButton(RectTransform);

    CheckerboardColorImage = CreateCheckerboardColorImage(RectTransform);

    HexValueInputField = CreateHexValueInputField(RectTransform);
    HexValueInputField.OnInputSubmit.AddListener(OnHexValueChanged);

    RedSlider = CreateRGBSlider(RectTransform);
    RedSlider.RectTransform.SetPosition(new(20f, -75f));
    RedSlider.NameLabel.text = "R";
    RedSlider.FillImage.SetColor(Color.red);
    RedSlider.Slider.onValueChanged.AddListener(OnRGBSliderValueChanged);

    GreenSlider = CreateRGBSlider(RectTransform);
    GreenSlider.RectTransform.SetPosition(new(20f, -115f));
    GreenSlider.NameLabel.text = "G";
    GreenSlider.FillImage.SetColor(Color.green);
    GreenSlider.Slider.onValueChanged.AddListener(OnRGBSliderValueChanged);

    BlueSlider = CreateRGBSlider(RectTransform);
    BlueSlider.RectTransform.SetPosition(new(20f, -155f));
    BlueSlider.NameLabel.text = "B";
    BlueSlider.FillImage.SetColor(Color.blue);
    BlueSlider.Slider.onValueChanged.AddListener(OnRGBSliderValueChanged);

    AlphaSlider = CreateRGBSlider(RectTransform);
    AlphaSlider.RectTransform.SetPosition(new(20f, -195f));
    AlphaSlider.NameLabel.text = "A";
    AlphaSlider.FillImage.SetColor(Color.white);
    AlphaSlider.Slider.onValueChanged.AddListener(OnRGBSliderValueChanged);

    HueSlider = CreateHSVSlider(RectTransform);
    HueSlider.RectTransform.SetPosition(new(20f, -235f));
    HueSlider.NameLabel.text = "H";
    HueSlider.Slider.onValueChanged.AddListener(OnHSVSliderValueChanged);

    HueSlider.Background
        .SetSprite(CreateHueSprite(360))
        .SetColor(Color.white);

    SaturationSlider = CreateHSVSlider(RectTransform);
    SaturationSlider.RectTransform.SetPosition(new(20f, -275f));
    SaturationSlider.NameLabel.text = "S";
    SaturationSlider.Slider.onValueChanged.AddListener(OnHSVSliderValueChanged);

    SaturationSlider.Background
        .SetSprite(CreateSprite(100))
        .SetColor(Color.white);

    SaturationTexture = SaturationSlider.Background.sprite.texture;

    BrightnessSlider = CreateHSVSlider(RectTransform);
    BrightnessSlider.RectTransform.SetPosition(new(20f, -315f));
    BrightnessSlider.NameLabel.text = "V";
    BrightnessSlider.Slider.onValueChanged.AddListener(OnHSVSliderValueChanged);

    BrightnessSlider.Background
        .SetSprite(CreateSprite(100))
        .SetColor(Color.white);

    BrightnessTexture = BrightnessSlider.Background.sprite.texture;

    SetCurrentColor(Color.cyan);
    UpdatePanel();
  }

  void OnRGBSliderValueChanged(float value) {
    Color color =
        new(RedSlider.PercentValue, GreenSlider.PercentValue, BlueSlider.PercentValue, AlphaSlider.PercentValue);

    SetCurrentColor(color);
    SetHSVSliderValues(CurrentColor);
    SetHexValue(CurrentHexValue);
    SetColorTextures(CurrentColor);
  }

  void OnHSVSliderValueChanged(float value) {
    Color color = Color.HSVToRGB(HueSlider.PercentValue, SaturationSlider.PercentValue, BrightnessSlider.PercentValue);
    color.a = AlphaSlider.PercentValue;

    SetCurrentColor(color);
    SetRGBSliderValues(CurrentColor);
    SetHexValue(CurrentHexValue);
    SetColorTextures(CurrentColor);
  }

  void OnHexValueChanged(string value) {
    if (ColorUtility.TryParseHtmlString(value, out Color color)) {
      SetCurrentColor(color);
      SetRGBSliderValues(CurrentColor);
      SetHSVSliderValues(CurrentColor);
      SetColorTextures(CurrentColor);
    }

    SetHexValue(CurrentHexValue);
  }

  void SetRGBSliderValues(Color color) {
    RedSlider.SetValueWithoutNotify(color.r);
    GreenSlider.SetValueWithoutNotify(color.g);
    BlueSlider.SetValueWithoutNotify(color.b);
    AlphaSlider.SetValueWithoutNotify(color.a);
  }

  void SetHSVSliderValues(Color color) {
    Color.RGBToHSV(color, out float hue, out float saturation, out float brightness);

    HueSlider.SetValueWithoutNotify(hue);
    SaturationSlider.SetValueWithoutNotify(saturation);
    BrightnessSlider.SetValueWithoutNotify(brightness);
  }

  void SetHexValue(string hexValue) {
    HexValueInputField.SetTextWithoutNotify(hexValue);
  }

  void SetColorTextures(Color color) {
    CheckerboardColorImage.color = color;

    Color.RGBToHSV(color, out float hue, out float saturation, out float brightness);
    ResetSaturationTexture(SaturationTexture, hue, brightness);
    ResetBrightnessTexture(BrightnessTexture, hue, saturation);
  }

  public void SetCurrentColor(Color color) {
    CurrentColor = color;
    CurrentHexValue = "#" + (color.a < 1f ? ColorUtility.ToHtmlStringRGBA(color) : ColorUtility.ToHtmlStringRGB(color));
  }

  public void UpdatePanel() {
    SetRGBSliderValues(CurrentColor);
    SetHSVSliderValues(CurrentColor);
    SetHexValue(CurrentHexValue);
    SetColorTextures(CurrentColor);
  }

  static GameObject CreatePanel(Transform parentTransform) {
    GameObject panel = UIBuilder.CreatePanel(parentTransform);
    panel.name = "ColorPickerPanel";

    return panel;
  }

  static LabelButton CreateConfirmButton(Transform parentTransform) {
    LabelButton confirmButton = new(parentTransform);
    confirmButton.Container.name = "Confirm";

    confirmButton.RectTransform
    .SetAnchorMin(Vector2.zero)
    .SetAnchorMax(Vector2.zero)
    .SetPivot(Vector2.zero)
    .SetPosition(new(20f, 20f))
    .SetSizeDelta(new(100f, 42.5f));

    confirmButton.Label
        .SetFontSize(18f)
        .SetText("Pick");

    return confirmButton;
  }

  static LabelButton CreateCloseButton(Transform parentTransform) {
    LabelButton closeButton = new(parentTransform);
    closeButton.Container.name = "Close";

    closeButton.RectTransform
        .SetAnchorMin(Vector2.right)
        .SetAnchorMax(Vector2.right)
        .SetPivot(Vector2.right)
        .SetPosition(new(-20f, 20f))
        .SetSizeDelta(new(100f, 42.5f));

    closeButton.Label
        .SetFontSize(18f)
        .SetText("Close");

    return closeButton;
  }

  static Image CreateCheckerboardColorImage(Transform parentTransform) {
    GameObject checkerboard = new("Checkerboard", typeof(RectTransform));
    checkerboard.transform.SetParent(parentTransform, worldPositionStays: false);

    checkerboard.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.up)
        .SetPivot(Vector2.up)
        .SetPosition(new(20f, -20f))
        .SetSizeDelta(new(200f, 40f));

    checkerboard.AddComponent<Image>()
        .SetType(Image.Type.Tiled)
        .SetSprite(CreateCheckerboardSprite(40, 40, 20))
        .SetColor(Color.white);

    GameObject color = new("Color", typeof(RectTransform));
    color.transform.SetParent(checkerboard.transform, worldPositionStays: false);

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

  static GuiInputField CreateHexValueInputField(Transform parentTransform) {
    GuiInputField inputField = UIBuilder.CreateInputField(parentTransform);
    inputField.name = "HexValue";

    inputField.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.one)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.one)
        .SetPosition(new(-20f, -20f))
        .SetSizeDelta(new(150f, 40f));

    inputField.textComponent
        .SetCharacterSpacing(5f);

    inputField.placeholder.GetComponent<TextMeshProUGUI>()
        .SetCharacterSpacing(5f)
        .SetText("#FFFFFF");

    inputField.SetTextWithoutNotify("#FFFFFF");

    inputField.onFocusSelectAll = false;
    inputField.m_DoubleClickDelay = 0.2f;

    return inputField;
  }

  static ColorSlider CreateRGBSlider(Transform parentTransform) {
    ColorSlider colorSlider = new(parentTransform);

    colorSlider.RectTransform
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.up)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(-40f, 25f));

    colorSlider.ShowValueAsPercent = true;

    colorSlider.Slider
        .SetInteractable(true)
        .SetWholeNumbers(false)
        .SetMinValue(0f)
        .SetMaxValue(1f)
        .SetValue(1f);

    colorSlider.FillImage.SetColor(Color.clear);

    return colorSlider;
  }

  static ColorSlider CreateHSVSlider(Transform parentTransform) {
    ColorSlider colorSlider = new(parentTransform);

    colorSlider.RectTransform
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.up)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(-40f, 25f));

    colorSlider.ShowValueAsPercent = true;

    colorSlider.Slider
        .SetInteractable(true)
        .SetWholeNumbers(false)
        .SetMinValue(0f)
        .SetMaxValue(1f)
        .SetValue(1f);

    colorSlider.FillImage.SetColor(Color.clear);

    return colorSlider;
  }

  static Sprite CreateHueSprite(int width) {
    Texture2D hueTexture = new(width, 1) {
      name = "HueTexture"
    };

    ResetHueTexture(hueTexture);

    Sprite hueSprite = Sprite.Create(hueTexture, new(0f, 0f, width, 1), new(0.5f, 0.5f));
    hueSprite.name = "HueSprite";

    return hueSprite;
  }

  static Sprite CreateSprite(int width) {
    Texture2D texture = new(width, 1) {
      name = "Texture"
    };

    Sprite sprite = Sprite.Create(texture, new(0f, 0f, width, 1), new(0.5f, 0.5f));
    sprite.name = "Sprite";

    return sprite;
  }

  static void ResetHueTexture(Texture2D hueTexture) {
    for (int i = 0, width = hueTexture.width; i < width; i++) {
      hueTexture.SetPixel(i, 0, Color.HSVToRGB(Mathf.InverseLerp(0, width, i), 1f, 1f));
    }

    hueTexture.Apply();
  }

  static void ResetSaturationTexture(Texture2D saturationTexture, float hue, float brightness) {
    for (int i = 0, width = saturationTexture.width; i < width; i++) {
      saturationTexture.SetPixel(i, 0, Color.HSVToRGB(hue, Mathf.InverseLerp(0, width, i), brightness));
    }

    saturationTexture.Apply();
  }

  static void ResetBrightnessTexture(Texture2D brightnessTexture, float hue, float saturation) {
    for (int i = 0, width = brightnessTexture.width; i < width; i++) {
      brightnessTexture.SetPixel(i, 0, Color.HSVToRGB(hue, saturation, Mathf.InverseLerp(0, width, i)));
    }

    brightnessTexture.Apply();
  }

  static Sprite CreateCheckerboardSprite(int width, int height, int length = 10) {
    Texture2D texture = new(width, height) {
      name = "CheckerboardTexture"
    };

    for (int x = 0; x < width; x++) {
      for (int y = 0; y < height; y++) {
        texture.SetPixel(x, y, ((x / length + y / length) % 2) == 1 ? Color.black : Color.white);
      }
    }

    texture.SetWrapMode(TextureWrapMode.Repeat);
    texture.Apply();

    Sprite sprite = Sprite.Create(texture, new(0f, 0f, width, height), new(0.5f, 0.5f));
    sprite.name = "CheckerboardSprite";

    return sprite;
  }
}
