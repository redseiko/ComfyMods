namespace Pinnacle;

using System;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class ValueCell {
  public GameObject Container { get; private set; }
  public RectTransform RectTransform { get; private set; }
  public LayoutElement LayoutElement { get; private set; }

  public Image Background { get; private set; }
  public TMP_InputField InputField { get; private set; }

  public ValueCell(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();
    LayoutElement = Container.GetComponent<LayoutElement>();

    Background = Container.GetComponent<Image>();

    InputField = CreateValueInputField(Container.transform);
    InputField.SetTargetGraphic(Background);
  }

  static GameObject CreateContainer(Transform parentTransform) {
    GameObject container = new("Value", typeof(RectTransform));
    container.transform.SetParent(parentTransform, worldPositionStays: false);

    container.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UISpriteBuilder.CreateRoundedCornerSprite(64, 64, 8))
        .SetColor(new(0.5f, 0.5f, 0.5f, 0.5f));

    container.AddComponent<RectMask2D>();
    container.AddComponent<LayoutElement>();

    return container;
  }

  TMP_InputField CreateValueInputField(Transform parentTransform) {
    GameObject row = new("InputField", typeof(RectTransform));
    row.SetParent(parentTransform);

    row.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetSizeDelta(Vector2.zero)
        .SetPosition(Vector2.zero);

    TextMeshProUGUI label = UIBuilder.CreateTMPLabel(row.transform);

    label.SetName("Label")
        .SetRichText(false)
        .SetAlignment(TextAlignmentOptions.Left)
        .SetText("Label");

    TMP_InputField inputField = parentTransform.gameObject.AddComponent<TMP_InputField>();

    inputField
        .SetTextComponent(label)
        .SetTextViewport(row.GetComponent<RectTransform>())
        .SetOnFocusSelectAll(false)
        .SetDoubleClickDelay(0.2f);

    inputField
        .SetTransition(Selectable.Transition.ColorTint)
        .SetColors(InputFieldColorBlock.Value)
        .SetNavigationMode(Navigation.Mode.None);

    return inputField;
  }

  static readonly Lazy<ColorBlock> InputFieldColorBlock =
      new(() =>
        new() {
          normalColor = new Color(1f, 1f, 1f, 0.9f),
          highlightedColor = new Color(0.565f, 0.792f, 0.976f),
          disabledColor = new Color(0.2f, 0.2f, 0.2f, 0.8f),
          pressedColor = new Color(0.647f, 0.839f, 0.655f),
          selectedColor = new Color(1f, 0.878f, 0.51f),
          colorMultiplier = 1f,
          fadeDuration = 0.25f,
        });
}
