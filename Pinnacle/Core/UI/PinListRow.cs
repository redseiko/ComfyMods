namespace Pinnacle;

using System;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class PinListRow {
  public GameObject Row { get; private set; }
  public RectTransform RectTransform { get; private set; }

  public Image PinIcon { get; private set; }
  public TextMeshProUGUI PinName { get; private set; }

  public TextMeshProUGUI PositionX { get; private set; }
  public TextMeshProUGUI PositionY { get; private set; }
  public TextMeshProUGUI PositionZ { get; private set; }

  public Minimap.PinData TargetPin { get; private set; }

  Action<Minimap.PinData> _onRowClicked;

  public PinListRow(Transform parentTransform) {
    Row = CreateChildRow(parentTransform);
    RectTransform = Row.GetComponent<RectTransform>();
    Row.Button().onClick.AddListener(OnRowClick);

    PinIcon = CreateChildPinIcon(Row.transform).Image();
    PinName = CreateChildPinName(Row.transform);

    UIBuilder.CreateRowSpacer(Row.transform);

    PositionX = CreateChildPinPositionValue(Row.transform);
    PositionX.color = new(1f, 0.878f, 0.51f);
    PositionY = CreateChildPinPositionValue(Row.transform);
    PositionY.color = new(0.565f, 0.792f, 0.976f);
    PositionZ = CreateChildPinPositionValue(Row.transform);
    PositionZ.color = new(0.647f, 0.839f, 0.655f);
  }

  public void Init(Action<Minimap.PinData> onRowClicked) {
    _onRowClicked = onRowClicked;
  }

  void OnRowClick() {
    if (TargetPin != null) {
      _onRowClicked?.Invoke(TargetPin);
    }
  }

  public PinListRow SetRowContent(Minimap.PinData pin, bool showCoordinates) {
    TargetPin = pin;

    if (pin.m_icon) {
      PinIcon.sprite = pin.m_icon;
      PinIcon.gameObject.SetActive(true);
    } else if (Minimap.s_instance) {
      PinIcon.sprite = Minimap.s_instance.GetSprite(pin.m_type);
      PinIcon.gameObject.SetActive(PinIcon.sprite);
    } else {
      PinIcon.gameObject.SetActive(false);
    }

    PinName.text = GetLocalizedPinName(pin);

    PositionX.text = $"{pin.m_pos.x:F0}";
    PositionY.text = $"{pin.m_pos.y:F0}";
    PositionZ.text = $"{pin.m_pos.z:F0}";

    TogglePinPosition(showCoordinates);

    return this;
  }

  public PinListRow SetRowContent(Minimap.PinData pin) {
    return SetRowContent(pin, showCoordinates: PositionX.gameObject.activeSelf);
  }

  static string GetLocalizedPinName(Minimap.PinData pin) {
    if (string.IsNullOrEmpty(pin.m_name)) {
      return $"<i>{PinNameUtils.GetFallbackPinName(pin)}</i>";
    }

    if (pin.m_name[0] == '$' && Localization.m_instance != null) {
      return Localization.m_instance.Localize(pin.m_name);
    }

    return pin.m_name;
  }

  public void TogglePinPosition(bool toggleOn) {
    PositionX.gameObject.SetActive(toggleOn);
    PositionY.gameObject.SetActive(toggleOn);
    PositionZ.gameObject.SetActive(toggleOn);
  }

  static GameObject CreateChildRow(Transform parentTransform) {
    GameObject row = new("PinList.Row", typeof(RectTransform));
    row.SetParent(parentTransform, worldPositionStays: false);

    row.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(0f, 26f));

    row.AddComponent<HorizontalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetChildAlignment(TextAnchor.MiddleCenter)
        .SetPadding(left: 5, right: 10, top: 2, bottom: 2)
        .SetSpacing(4f);

    row.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UISpriteBuilder.CreateRoundedCornerSprite(400, 400, 5));

    row.AddComponent<Button>()
        .SetNavigationMode(Navigation.Mode.None)
        .SetTargetGraphic(row.Image())
        .SetColors(ButtonColorBlock.Value);

    return row;
  }

  static GameObject CreateChildPinIcon(Transform parentTransform) {
    GameObject icon = new("Pin.Icon", typeof(RectTransform));
    icon.SetParent(parentTransform, worldPositionStays: false);

    icon.AddComponent<LayoutElement>()
        .SetPreferred(width: 20f, height: 20f);

    icon.AddComponent<Image>()
        .SetType(Image.Type.Simple);

    return icon;
  }

  static TextMeshProUGUI CreateChildPinName(Transform parentTransform) {
    TextMeshProUGUI name = UIBuilder.CreateTMPLabel(parentTransform);
    name.SetName("Pin.Name");

    name.alignment = TextAlignmentOptions.Left;
    name.textWrappingMode = TextWrappingModes.NoWrap;
    name.overflowMode = TextOverflowModes.Ellipsis;
    name.fontSize = 16f;

    return name;
  }

  static TextMeshProUGUI CreateChildPinPositionValue(Transform parentTransform) {
    TextMeshProUGUI value = UIBuilder.CreateTMPLabel(parentTransform);
    value.SetName("Pin.Position.Value");

    value.alignment = TextAlignmentOptions.Right;
    value.text = "-99999";
    value.textWrappingMode = TextWrappingModes.NoWrap;
    value.overflowMode = TextOverflowModes.Ellipsis;
    value.fontSize = 14f;

    value.gameObject.AddComponent<LayoutElement>()
        .SetPreferred(width: 55f);

    return value;
  }

  static readonly Lazy<ColorBlock> ButtonColorBlock =
      new(() =>
        new() {
          normalColor = new Color(0f, 0f, 0f, 0.01f),
          highlightedColor = new Color32(50, 161, 217, 128),
          disabledColor = new Color(0f, 0f, 0f, 0.1f),
          pressedColor = new Color32(50, 161, 217, 192),
          selectedColor = new Color32(50, 161, 217, 248),
          colorMultiplier = 1f,
          fadeDuration = 0f,
        });
}
