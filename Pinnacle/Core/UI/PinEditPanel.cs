namespace Pinnacle;

using System.Collections;
using System.Collections.Generic;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

using static PluginConfig;

public sealed class PinEditPanel {
  public const long DefaultSharedPinOwnerId = long.MaxValue;

  public GameObject Panel { get; private set; }
  public RectTransform RectTransform { get; private set; }
  public CanvasGroup CanvasGroup { get; private set; }

  public LabelValueRow PinNameRow { get; private set; }

  public LabelRow PinIconSelectorRow { get; private set; }
  public PinIconSelector PinIconSelector { get; private set; }

  public LabelValueRow PinTypeRow { get; private set; }

  public LabelRow PinModifierRow { get; private set; }
  public ToggleCell PinCheckedToggle { get; private set; }
  public ToggleCell PinSharedToggle { get; private set; }

  public LabelRow PinPositionRow { get; private set; }
  public VectorCell PinPosition { get; private set; }

  // Styling.
  readonly List<TextMeshProUGUI> _rowLabels = [];

  // HasFocus.
  readonly HashSet<GameObject> _selectables = [];

  Coroutine _setActiveCoroutine;

  public PinEditPanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();
    CanvasGroup = Panel.GetComponent<CanvasGroup>();

    PinNameRow = CreatePinNameRow(Panel.transform);

    PinIconSelectorRow = CreatePinIconSelectorRow(Panel.transform);
    PinIconSelector = CreatePinIconSelector(PinIconSelectorRow.Row.transform);

    PinTypeRow = CreatePinTypeRow(Panel.transform);

    PinModifierRow = CreatePinModifierRow(Panel.transform);
    PinCheckedToggle = CreatePinCheckedToggle(PinModifierRow.Row.transform);
    PinSharedToggle = CreatePinSharedToggle(PinModifierRow.Row.transform);

    PinPositionRow = CreatePinPositionRow(Panel.transform);
    PinPosition = CreatePinPosition(PinPositionRow.Row.transform);

    CacheSelectables();
    CacheRowLabels();

    SetupPanelStyle();
  }

  void CacheSelectables() {
    foreach (Selectable selectable in Panel.GetComponentsInChildren<Selectable>()) {
      _selectables.Add(selectable.gameObject);
    }
  }

  void CacheRowLabels() {
    _rowLabels.Add(PinNameRow.Label);
    _rowLabels.Add(PinIconSelectorRow.Label);
    _rowLabels.Add(PinTypeRow.Label);
    _rowLabels.Add(PinModifierRow.Label);
    _rowLabels.Add(PinPositionRow.Label);
  }

  public void SetActive(bool toggle) {
    if (_setActiveCoroutine != null) {
      Minimap.m_instance.StopCoroutine(_setActiveCoroutine);
    }

    _setActiveCoroutine =
        Minimap.m_instance.StartCoroutine(
            LerpCanvasGroupAlpha(
                CanvasGroup, toggle ? 1f : 0f, PinEditPanelToggleLerpDuration.Value));
  }

  static IEnumerator LerpCanvasGroupAlpha(CanvasGroup canvasGroup, float targetAlpha, float lerpDuration) {
    float timeElapsed = 0f;
    float sourceAlpha = canvasGroup.alpha;

    canvasGroup.SetBlocksRaycasts(targetAlpha > 0f);

    while (timeElapsed < lerpDuration) {
      float t = timeElapsed / lerpDuration;
      t = t * t * (3f - (2f * t));

      canvasGroup.SetAlpha(Mathf.Lerp(sourceAlpha, targetAlpha, t));
      timeElapsed += Time.deltaTime;

      yield return null;
    }

    canvasGroup.SetAlpha(targetAlpha);
  }

  public void SetupPanelStyle() {
    float labelWidth = 0f;

    foreach (TextMeshProUGUI label in _rowLabels) {
      if (label.preferredWidth > labelWidth) {
        labelWidth = label.preferredWidth;
      }
    }

    foreach (TextMeshProUGUI label in _rowLabels) {
      label.layoutElement.SetPreferred(width: labelWidth);
    }

    PinNameRow.Value.LayoutElement.SetPreferred(width: 200f);
    PinTypeRow.Value.LayoutElement.SetPreferred(width: 200f);
  }

  public bool HasFocus() {
    GameObject selected = EventSystem.current.currentSelectedGameObject;

    return selected
        && _selectables.Contains(selected)
        && (!selected.TryGetComponent(out InputField inputField)
            || inputField.isFocused
            || ZInput.GetKeyDown(KeyCode.Return));
  }

  public Minimap.PinData TargetPin { get; private set; }

  public void SetTargetPin(Minimap.PinData pin) {
    TargetPin = pin;

    if (pin == null) {
      return;
    }

    PinNameRow.Value.InputField.SetTextWithoutNotify(pin.m_name);

    PinIconSelector.UpdateIcons(pin.m_type);
    PinTypeRow.Value.InputField.text = pin.m_type.ToString();

    PinCheckedToggle.Toggle.SetIsOnWithoutNotify(pin.m_checked);
    PinSharedToggle.Toggle.SetIsOnWithoutNotify(pin.m_ownerID != 0L);

    PinPosition.XValue.InputField.text = $"{pin.m_pos.x:F0}";
    PinPosition.YValue.InputField.text = $"{pin.m_pos.y:F0}";
    PinPosition.ZValue.InputField.text = $"{pin.m_pos.z:F0}";
  }

  public void ActivatePinNameInputField() {
    PinNameRow.Value.InputField.ActivateInputField();
  }

  void OnPinNameValueChange(string name) {
    if (TargetPin == null) {
      return;
    }

    TargetPin.m_name = name;

    if (TargetPin.m_NamePinData == null) {
      TargetPin.m_NamePinData = new(TargetPin);
      Minimap.m_instance.CreateMapNamePin(TargetPin, Minimap.m_instance.m_pinNameRootLarge);
    }

    TargetPin.m_NamePinData.PinNameText.SetText(name);

    PinIconManager.ProcessIconTagsModified(TargetPin);
  }

  void OnPinTypeValueChange(Minimap.PinType pinType) {
    Minimap.m_instance.m_selectedType = pinType;

    if (TargetPin == null) {
      return;
    }

    TargetPin.m_type = pinType;
    TargetPin.m_icon = Minimap.m_instance.GetSprite(pinType);
    TargetPin.m_iconElement.SetSprite(TargetPin.m_icon);

    PinIconSelector.UpdateIcons(pinType);
    PinTypeRow.Value.InputField.text = pinType.ToString();
  }

  void OnPinCheckedChange(bool pinChecked) {
    if (TargetPin == null) {
      return;
    }

    TargetPin.m_checked = pinChecked;
    TargetPin.m_checkedElement.SetActive(pinChecked);
  }

  void OnPinSharedChange(bool pinShared) {
    if (TargetPin == null) {
      return;
    }

    TargetPin.m_ownerID = TargetPin.m_ownerID == 0L ? DefaultSharedPinOwnerId : 0L;
  }

  void OnPinPositionValueChange(string value) {
    if (TargetPin == null) {
      return;
    }

    if (!float.TryParse(PinPosition.XValue.InputField.text, out float x)
        || !float.TryParse(PinPosition.YValue.InputField.text, out float y)
        || !float.TryParse(PinPosition.ZValue.InputField.text, out float z)) {
      PinPosition.XValue.InputField.text = $"{TargetPin.m_pos.x:F0}";
      PinPosition.YValue.InputField.text = $"{TargetPin.m_pos.y:F0}";
      PinPosition.ZValue.InputField.text = $"{TargetPin.m_pos.z:F0}";

      return;
    }

    TargetPin.m_pos = new(x, y, z);

    Vector2 mapImagePosition = GetMapImagePosition(TargetPin.m_pos);

    TargetPin.m_uiElement.SetPosition(mapImagePosition);
    TargetPin.m_NamePinData?.PinNameRectTransform.SetPosition(mapImagePosition);

    CenterMapHelper.CenterMapOnPosition(TargetPin.m_pos);
  }

  static Vector2 GetMapImagePosition(Vector3 mapPosition) {
    Minimap.m_instance.WorldToMapPoint(mapPosition, out float mx, out float my);
    return Minimap.m_instance.MapPointToLocalGuiPos(mx, my, Minimap.m_instance.m_mapImageLarge);
  }

  static GameObject CreatePanel(Transform parentTransform) {
    GameObject panel = new("PinEditPanel", typeof(RectTransform));
    panel.transform.SetParent(parentTransform, worldPositionStays: false);

    panel.AddComponent<VerticalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetPadding(left: 2, right: 2, top: 8, bottom: 8)
        .SetSpacing(4f);

    panel.AddComponent<ContentSizeFitter>()
        .SetHorizontalFit(ContentSizeFitter.FitMode.PreferredSize)
        .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

    panel.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UISpriteBuilder.CreateSuperellipse(200, 200, 10))
        .SetColor(new(0f, 0f, 0f, 0.9f));

    panel.AddComponent<Canvas>();
    panel.AddComponent<GraphicRaycaster>();

    panel.AddComponent<CanvasGroup>()
        .SetBlocksRaycasts(true);

    return panel;
  }

  LabelValueRow CreatePinNameRow(Transform parentTransform) {
    LabelValueRow pinNameRow = new(parentTransform);
    pinNameRow.Label.SetText("Name");

    pinNameRow.Value.LayoutElement
        .SetFlexible(width: 1f)
        .SetPreferred(height: pinNameRow.Value.InputField.preferredHeight + 8f);

    pinNameRow.Value.InputField.onEndEdit.AddListener(OnPinNameValueChange);

    return pinNameRow;
  }

  static LabelRow CreatePinIconSelectorRow(Transform parentTransform) {
    LabelRow pinIconSelectorRow = new(parentTransform);

    pinIconSelectorRow.Label
        .SetAlignment(TextAlignmentOptions.TopLeft)
        .SetText("Icon");

    return pinIconSelectorRow;
  }

  PinIconSelector CreatePinIconSelector(Transform parentTransform) {
    PinIconSelector pinIconSelector = new(parentTransform);

    pinIconSelector.OnPinIconClick.AddListener(OnPinTypeValueChange);

    return pinIconSelector;
  }

  static LabelValueRow CreatePinTypeRow(Transform parentTransform) {
    LabelValueRow pinTypeRow = new(parentTransform);

    pinTypeRow.Label.SetText("Type");
    pinTypeRow.Value.InputField.SetInteractable(false);

    return pinTypeRow;
  }

  static LabelRow CreatePinModifierRow(Transform parentTransform) {
    LabelRow pinModifierRow = new(parentTransform);

    pinModifierRow.Label.SetText("Modifier");

    return pinModifierRow;
  }

  ToggleCell CreatePinCheckedToggle(Transform parentTransform) {
    ToggleCell pinCheckedToggle = new(parentTransform);

    pinCheckedToggle.Label.SetText("Checked");
    pinCheckedToggle.Toggle.onValueChanged.AddListener(OnPinCheckedChange);

    return pinCheckedToggle;
  }

  ToggleCell CreatePinSharedToggle(Transform parentTransform) {
    ToggleCell pinSharedToggle = new(parentTransform);

    pinSharedToggle.Label.SetText("Shared");
    pinSharedToggle.Toggle.onValueChanged.AddListener(OnPinSharedChange);

    return pinSharedToggle;
  }

  static LabelRow CreatePinPositionRow(Transform parentTransform) {
    LabelRow pinPositionRow = new(parentTransform);

    pinPositionRow.Label.SetText("Position");

    return pinPositionRow;
  }

  VectorCell CreatePinPosition(Transform parentTransform) {
    VectorCell pinPosition = new(parentTransform);

    SetupPinPositionValueInputField(pinPosition.XValue.InputField, new(1f, 0.878f, 0.51f));
    SetupPinPositionValueInputField(pinPosition.YValue.InputField, new(0.565f, 0.792f, 0.976f));
    SetupPinPositionValueInputField(pinPosition.ZValue.InputField, new(0.647f, 0.839f, 0.655f));

    return pinPosition;
  }

  void SetupPinPositionValueInputField(TMP_InputField inputField, Color textColor) {
    inputField.textComponent.color = textColor;
    inputField.contentType = TMP_InputField.ContentType.DecimalNumber;
    inputField.characterValidation = TMP_InputField.CharacterValidation.Decimal;
    inputField.onEndEdit.AddListener(OnPinPositionValueChange);
  }
}
