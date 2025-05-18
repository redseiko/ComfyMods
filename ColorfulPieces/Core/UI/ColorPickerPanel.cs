namespace ColorfulPieces;

using ComfyLib;

using GUIFramework;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class ColorPickerPanel {
  public GameObject Panel { get; private set; }
  public RectTransform RectTransform { get; private set; }

  public TextMeshProUGUI TitleLabel { get; private set; }

  public LabelButton SelectButton { get; private set; }
  public LabelButton CloseButton { get; private set; }

  public Image CheckerboardColorImage { get; private set; }
  public GuiInputField HexValueInputField { get; }

  public ColorSlider RedSlider { get; private set; }
  public ColorSlider GreenSlider { get; private set; }
  public ColorSlider BlueSlider { get; private set; }
  public ColorSlider AlphaSlider { get; private set; }

  public ColorSlider HueSlider { get; private set; }
  public ColorSlider SaturationSlider { get; private set; }
  public ColorSlider BrightnessSlider { get; private set; }

  public Texture2D BrightnessTexture { get; private set; }
  public Texture2D SaturationTexture { get; private set; }

  public ColorSlider EmissionColorFactorSlider { get; private set; }

  public ColorPalettePanel PalettePanel { get; private set; }

  public Color CurrentColor { get; private set; } = Color.white;
  public string CurrentHexValue { get; private set; } = string.Empty;

  public bool UseColorAlpha { get; private set; } = true;

  public float CurrentEmissionColorFactor { get; private set; } = 0f;

  public ColorPickerPanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();

    TitleLabel = CreateTitleLabel(RectTransform);

    SelectButton = CreateSelectButton(RectTransform);
    CloseButton = CreateCloseButton(RectTransform);

    CheckerboardColorImage = CreateCheckerboardColorImage(RectTransform);

    HexValueInputField = CreateHexValueInputField(RectTransform);
    HexValueInputField.OnInputSubmit.AddListener(OnHexValueChanged);

    RedSlider = CreateRGBSlider(RectTransform, "R", Color.red);
    RedSlider.RectTransform.SetPosition(new(20f, -125f));

    GreenSlider = CreateRGBSlider(RectTransform, "G", Color.green);
    GreenSlider.RectTransform.SetPosition(new(20f, -165f));

    BlueSlider = CreateRGBSlider(RectTransform, "B", Color.blue);
    BlueSlider.RectTransform.SetPosition(new(20f, -205f));

    AlphaSlider = CreateRGBSlider(RectTransform, "A", Color.white);
    AlphaSlider.RectTransform.SetPosition(new(20f, -245f));

    HueSlider = CreateHSVSlider(RectTransform, "H", CreateHueSprite(360));
    HueSlider.RectTransform.SetPosition(new(20f, -285f));

    SaturationSlider = CreateHSVSlider(RectTransform, "S", CreateSprite(100));
    SaturationSlider.RectTransform.SetPosition(new(20f, -325f));
    SaturationTexture = SaturationSlider.Background.sprite.texture;

    BrightnessSlider = CreateHSVSlider(RectTransform, "V", CreateSprite(100));
    BrightnessSlider.RectTransform.SetPosition(new(20f, -365f));
    BrightnessTexture = BrightnessSlider.Background.sprite.texture;

    EmissionColorFactorSlider = CreateEmissionColorFactorSlider(RectTransform);
    EmissionColorFactorSlider.RectTransform.SetPosition(new(20f, -410f));
    EmissionColorFactorSlider.Slider.onValueChanged.AddListener(OnEmissionColorFactorSliderChanged);

    PalettePanel = CreatePalettePanel(RectTransform);

    SetColor(Color.cyan);
    SetEmissionColorFactor(0.4f);
  }

  public void SetColor(Color color) {
    SetCurrentColor(color);
    SetRGBSliderValues(CurrentColor);
    SetHSVSliderValues(CurrentColor);
    SetHexValue(CurrentHexValue);
    SetColorTextures(CurrentColor);
  }

  public void SetUseColorAlpha(bool useColorAlpha) {
    UseColorAlpha = useColorAlpha;
    AlphaSlider.SetInteractable(useColorAlpha);
  }

  public void SetEmissionColorFactor(float factor) {
    CurrentEmissionColorFactor = factor;
    EmissionColorFactorSlider.SetValueWithoutNotify(factor);
  }

  void OnEmissionColorFactorSliderChanged(float factor) {
    CurrentEmissionColorFactor = factor;
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
      if (!UseColorAlpha) {
        color.a = AlphaSlider.PercentValue;
      }

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

  void SetCurrentColor(Color color) {
    CurrentColor = color;

    CurrentHexValue =
        color.a < 1f
            ? "#" + ColorUtility.ToHtmlStringRGBA(color)
            : "#" + ColorUtility.ToHtmlStringRGB(color);
  }

  static GameObject CreatePanel(Transform parentTransform) {
    GameObject panel = UIBuilder.CreatePanel(parentTransform);
    panel.name = "ColorPickerPanel";

    panel.AddComponent<Canvas>();
    panel.AddComponent<GraphicRaycaster>();
    panel.AddComponent<PanelDragger>();

    return panel;
  }

  static TextMeshProUGUI CreateTitleLabel(Transform parentTransform) {
    TextMeshProUGUI titleLabel = UIBuilder.CreateTMPHeaderLabel(parentTransform);
    titleLabel.name = "Title";

    titleLabel.rectTransform
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 1f))
        .SetPosition(new(0f, -20f))
        .SetSizeDelta(new(0f, 40f));

    titleLabel
        .SetAlignment(TextAlignmentOptions.Top)
        .SetText("Color Picker");

    return titleLabel;
  }

  static LabelButton CreateSelectButton(Transform parentTransform) {
    LabelButton selectButton = new(parentTransform);
    selectButton.Container.name = "Select";

    selectButton.RectTransform
        .SetAnchorMin(Vector2.right)
        .SetAnchorMax(Vector2.right)
        .SetPivot(Vector2.right)
        .SetPosition(new(-20f, 20f))
        .SetSizeDelta(new(125f, 42.5f));

    selectButton.Label
        .SetFontSize(18f)
        .SetText("Select");

    selectButton.Container.AddComponent<IgnoreDragHandler>();

    return selectButton;
  }

  static LabelButton CreateCloseButton(Transform parentTransform) {
    LabelButton closeButton = new(parentTransform);
    closeButton.Container.name = "Close";

    closeButton.RectTransform
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.zero)
        .SetPivot(Vector2.zero)
        .SetPosition(new(20f, 20f))
        .SetSizeDelta(new(100f, 42.5f));

    closeButton.Label
        .SetFontSize(18f)
        .SetText("Close");

    closeButton.Container.AddComponent<IgnoreDragHandler>();

    return closeButton;
  }

  static Image CreateCheckerboardColorImage(Transform parentTransform) {
    GameObject checkerboard = new("Checkerboard", typeof(RectTransform));
    checkerboard.transform.SetParent(parentTransform, worldPositionStays: false);

    checkerboard.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.up)
        .SetPivot(Vector2.up)
        .SetPosition(new(20f, -70f))
        .SetSizeDelta(new(200f, 40f));

    checkerboard.AddComponent<Image>()
        .SetType(Image.Type.Tiled)
        .SetSprite(UIBuilder.CreateCheckerboardSprite(40, 40, 20))
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
        .SetPosition(new(-20f, -70f))
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

  ColorSlider CreateRGBSlider(Transform parentTransform, string sliderLabel, Color fillColor) {
    ColorSlider rgbSlider = CreateColorSlider(parentTransform);

    rgbSlider.NameLabel.text = sliderLabel;
    rgbSlider.FillImage.SetColor(fillColor);
    rgbSlider.Slider.onValueChanged.AddListener(OnRGBSliderValueChanged);

    return rgbSlider;
  }

  ColorSlider CreateHSVSlider(Transform parentTransform, string sliderLabel, Sprite backgroundSprite) {
    ColorSlider hsvSlider = CreateColorSlider(parentTransform);

    hsvSlider.NameLabel.text = sliderLabel;
    hsvSlider.Slider.onValueChanged.AddListener(OnHSVSliderValueChanged);

    hsvSlider.Background
        .SetSprite(backgroundSprite)
        .SetColor(Color.white);

    return hsvSlider;
  }

  static ColorSlider CreateColorSlider(Transform parentTransform) {
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

    colorSlider.ValueInputField.characterLimit = 4;

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

  static ColorSlider CreateEmissionColorFactorSlider(Transform parentTransform) {
    ColorSlider factorSlider = new(parentTransform);
    factorSlider.Container.name = "FactorSlider";

    factorSlider.RectTransform
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.up)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(-40f, 45f));

    factorSlider.ShowValueAsPercent = false;

    factorSlider.NameLabel
        .SetAlignment(TextAlignmentOptions.TopLeft);

    factorSlider.SliderRectTransform
        .SetPivot(Vector2.zero)
        .SetPosition(new(0f, -20f))
        .SetSizeDelta(new(-100f, -15f));

    factorSlider.Slider
        .SetInteractable(true)
        .SetWholeNumbers(false)
        .SetMinValue(0f)
        .SetMaxValue(0.8f)
        .SetValue(0.4f);

    factorSlider.ValueInputField.characterLimit = 6;

    factorSlider.ValueInputField.GetComponent<RectTransform>()
        .SetPivot(Vector2.one)
        .SetPosition(new(0f, -7.5f))
        .SetSizeDelta(new(85f, 35f));

    factorSlider.NameLabel.SetText("Emission Color Factor");
    factorSlider.FillImage.SetColor(Color.gray);

    return factorSlider;
  }

  static ColorPalettePanel CreatePalettePanel(Transform parentTransform) {
    ColorPalettePanel palettePanel = new(parentTransform);

    palettePanel.RectTransform
        .SetAnchorMin(Vector2.right)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.up)
        .SetPosition(new(5f, 0f))
        .SetSizeDelta(new(285f, 0f));

    return palettePanel;
  }
}
