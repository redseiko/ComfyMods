namespace Pinnacle;

using System;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class PinListRow {
  public GameObject Row { get; private set; }

  public Image PinIcon { get; private set; }
  public TMP_Text PinName { get; private set; }

  public TMP_Text PositionX { get; private set; }
  public TMP_Text PositionY { get; private set; }
  public TMP_Text PositionZ { get; private set; }

  Minimap.PinData _targetPin;

  public PinListRow(Transform parentTransform) {
    Row = CreateChildRow(parentTransform);
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

  void OnRowClick() {
    PinnacleUtils.CenterMapOnOrTeleportTo(_targetPin);
  }

  public PinListRow SetRowContent(Minimap.PinData pin) {
    _targetPin = pin;

    PinIcon.SetSprite(pin.m_icon);
    PinName.SetText(GetLocalizedPinName(pin));

    PositionX.SetText($"{pin.m_pos.x:F0}");
    PositionY.SetText($"{pin.m_pos.y:F0}");
    PositionZ.SetText($"{pin.m_pos.z:F0}");

    return this;
  }

  string GetLocalizedPinName(Minimap.PinData pin) {
    if (pin.m_name.Length <= 0) {
      return $"<i>{pin.m_type}</i>";
    }

    if (pin.m_name[0] == '$') {
      return Localization.m_instance.Localize(pin.m_name);
    }

    return pin.m_name;
  }

  public void TogglePinPosition(bool toggleOn) {
    PositionX.gameObject.SetActive(toggleOn);
    PositionY.gameObject.SetActive(toggleOn);
    PositionZ.gameObject.SetActive(toggleOn);
  }

  GameObject CreateChildRow(Transform parentTransform) {
    GameObject row = new("PinList.Row", typeof(RectTransform));
    row.SetParent(parentTransform);

    row.AddComponent<HorizontalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetChildAlignment(TextAnchor.MiddleCenter)
        .SetPadding(left: 5, right: 10, top: 4, bottom: 4)
        .SetSpacing(2.5f);

    row.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIBuilder.CreateRoundedCornerSprite(400, 400, 5));

    row.AddComponent<Button>()
        .SetNavigationMode(Navigation.Mode.None)
        .SetTargetGraphic(row.Image())
        .SetColors(ButtonColorBlock.Value);

    row.AddComponent<ContentSizeFitter>()
        .SetHorizontalFit(ContentSizeFitter.FitMode.Unconstrained)
        .SetVerticalFit(ContentSizeFitter.FitMode.PreferredSize);

    row.AddComponent<ParentSizeFitter>();

    return row;
  }

  GameObject CreateChildPinIcon(Transform parentTransform) {
    GameObject icon = new("Pin.Icon", typeof(RectTransform));
    icon.SetParent(parentTransform);

    icon.AddComponent<LayoutElement>()
        .SetPreferred(width: 20f, height: 20f);

    icon.AddComponent<Image>()
        .SetType(Image.Type.Simple);

    return icon;
  }

  TMP_Text CreateChildPinName(Transform parentTransform) {
    TMP_Text name = UIBuilder.CreateTMPLabel(parentTransform);
    name.SetName("Pin.Name");

    name.alignment = TextAlignmentOptions.Left;
    name.textWrappingMode = TextWrappingModes.NoWrap;
    name.overflowMode = TextOverflowModes.Ellipsis;
    name.fontSize = 16f;

    return name;
  }

  TMP_Text CreateChildPinPositionValue(Transform parentTransform) {
    TMP_Text value = UIBuilder.CreateTMPLabel(parentTransform);
    value.SetName("Pin.Position.Value");

    value.alignment = TextAlignmentOptions.Right;
    value.text = "-99999";
    value.textWrappingMode = TextWrappingModes.NoWrap;
    value.overflowMode = TextOverflowModes.Ellipsis;
    value.fontSize = 14f;

    value.gameObject.AddComponent<LayoutElement>()
        .SetPreferred(width: value.GetPreferredValues().x);

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
