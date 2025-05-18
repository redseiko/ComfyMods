namespace ComfyLib;

using GUIFramework;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class ColorSlider {
  public GameObject Container { get; private set; }
  public RectTransform RectTransform { get; private set; }

  public TextMeshProUGUI NameLabel { get; private set; }
  public GuiInputField ValueInputField { get; private set; }

  public GameObject SliderContainer { get; private set; }
  public RectTransform SliderRectTransform { get; private set; }

  public Image Background { get; private set; }

  public GameObject Fill { get; private set; }
  public Image FillImage { get; private set; }

  public GameObject Handle { get; private set; }
  public Image HandleImage { get; private set; }

  public Slider Slider { get; private set; }

  public ColorSlider(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();

    NameLabel = CreateNameLabel(RectTransform);
    ValueInputField = CreateValueInputField(RectTransform);

    ValueInputField.OnInputSubmit.AddListener(OnValueInputFieldChanged);

    SliderContainer = CreateSliderContainer(RectTransform);
    SliderRectTransform = SliderContainer.GetComponent<RectTransform>();

    Background = CreateBackground(SliderRectTransform);
    Fill = CreateFill(SliderRectTransform);
    FillImage = Fill.GetComponent<Image>();

    Handle = CreateHandle(SliderRectTransform);
    HandleImage = Handle.GetComponent<Image>();

    Slider = SliderContainer.AddComponent<Slider>();

    Slider
        .SetDirection(Slider.Direction.LeftToRight)
        .SetFillRect(Fill.GetComponent<RectTransform>())
        .SetHandleRect(Handle.GetComponent<RectTransform>())
        .SetTargetGraphic(FillImage)
        .SetTransition(Selectable.Transition.None);

    Slider.onValueChanged.AddListener(SetValueLabelText);
  }

  public bool ShowValueAsPercent { get; set; } = false;

  public float RawValue => Slider.value;
  public float PercentValue => (Slider.value / Slider.maxValue);

  public void SetValueWithoutNotify(float value) {
    Slider.SetValueWithoutNotify(value);
    SetValueLabelText(value);
  }

  public void SetValueLabelText(float value) {
    ValueInputField.SetTextWithoutNotify(ShowValueAsPercent ? $"{value:F2}" : $"{value:0.####}");
  }

  void OnValueInputFieldChanged(string text) {
    if (float.TryParse(text, out float result)
        && Mathf.Clamp(result, Slider.minValue, Slider.maxValue) != Slider.value) {
      Slider.SetValue(result);
    } else {
      SetValueLabelText(Slider.value);
    }
  }

  public void SetInteractable(bool interactable) {
    Slider.SetInteractable(interactable);
    Handle.SetActive(interactable);
    ValueInputField.SetInteractable(interactable);
  }

  static GameObject CreateContainer(Transform parentTransform) {
    GameObject container = new("ColorSlider", typeof(RectTransform));
    container.transform.SetParent(parentTransform, worldPositionStays: false);

    container.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.right)
        .SetPivot(Vector2.zero)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(0f, 25f));

    return container;
  }

  static TextMeshProUGUI CreateNameLabel(Transform parentTransform) {
    TextMeshProUGUI label = UIBuilder.CreateTMPLabel(parentTransform);
    label.name = "Name";

    label.rectTransform
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.up)
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(new(20f, 0f))
        .SetSizeDelta(new(40f, 0f));

    label
        .SetFontSize(20f)
        .SetAlignment(TextAlignmentOptions.MidlineLeft);

    label.text = "X";

    return label;
  }

  static GuiInputField CreateValueInputField(Transform parentTransform) {
    GuiInputField inputField = UIBuilder.CreateInputField(parentTransform);
    inputField.name = "Value";

    inputField.GetComponent<RectTransform>()
        .SetAnchorMin(new(1f, 0.5f))
        .SetAnchorMax(new(1f, 0.5f))
        .SetPivot(new(1f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(65f, 35f));

    inputField.textComponent
        .SetFontSize(18f);

    inputField.placeholder.GetComponent<TextMeshProUGUI>()
        .SetFontSize(18f)
        .SetText("0");

    inputField.SetTextWithoutNotify("0");

    inputField.characterValidation = TMP_InputField.CharacterValidation.Decimal;
    inputField.onFocusSelectAll = false;
    inputField.m_DoubleClickDelay = 0.2f;

    return inputField;
  }

  static GameObject CreateSliderContainer(Transform parentTransform) {
    GameObject area = new("Slider", typeof(RectTransform));
    area.transform.SetParent(parentTransform, worldPositionStays: false);

    area.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0f, 0.5f))
        .SetPosition(new(25f, 0f))
        .SetSizeDelta(new(-105f, 0f));

    return area;
  }

  static Image CreateBackground(Transform parentTransform) {
    GameObject background = new("Background", typeof(RectTransform));
    background.transform.SetParent(parentTransform, worldPositionStays: false);

    Image image = background.AddComponent<Image>();
    image.SetColor(new(0.271f, 0.271f, 0.271f, 1f));

    background.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    return image;
  }

  static GameObject CreateFill(Transform parentTransform) {
    GameObject fill = new("Fill", typeof(RectTransform));
    fill.transform.SetParent(parentTransform, worldPositionStays: false);

    fill.AddComponent<Image>()
        .SetColor(Color.white);

    fill.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    return fill;
  }

  static GameObject CreateHandle(Transform parentTransform) {
    GameObject handle = new("Handle", typeof(RectTransform));
    handle.transform.SetParent(parentTransform, worldPositionStays: false);

    handle.AddComponent<Image>()
        .SetColor(new(0f, 0f, 0f, 0.95f));

    handle.GetComponent<RectTransform>()
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(10f, 10f));

    return handle;
  }
}
