namespace ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public static class UIBuilder {
  public static TextMeshProUGUI CreateTMPLabel(Transform parentTransform) {
    TextMeshProUGUI label =
        UnityEngine.Object.Instantiate(UnifiedPopup.instance.bodyText, parentTransform, worldPositionStays: false);

    label.name = "Label";
    label.fontSize = 16f;
    label.richText = true;
    label.color = Color.white;
    label.enableAutoSizing = false;
    label.textWrappingMode = TextWrappingModes.NoWrap;
    label.overflowMode = TextOverflowModes.Overflow;
    label.text = string.Empty;

    return label;
  }

  public static TextMeshProUGUI CreateTMPHeaderLabel(Transform parentTransform) {
    TextMeshProUGUI label =
        UnityEngine.Object.Instantiate(UnifiedPopup.instance.headerText, parentTransform, worldPositionStays: false);

    label.name = "Label";
    label.fontSize = 32f;
    label.richText = true;
    label.enableAutoSizing = false;
    label.textWrappingMode = TextWrappingModes.NoWrap;
    label.overflowMode = TextOverflowModes.Overflow;
    label.text = string.Empty;

    return label;
  }

  public static GameObject CreatePanel(Transform parentTransform) {
    GameObject panel = new("Panel", typeof(RectTransform));
    panel.transform.SetParent(parentTransform, worldPositionStays: false);

    panel.GetComponent<RectTransform>()
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(275f, 350f));

    panel.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIResources.GetSprite("woodpanel_trophys"))
        .SetColor(Color.white);

    return panel;
  }

  public static Scrollbar CreateScrollbar(Transform parentTransform) {
    GameObject scrollbar = new("Scrollbar", typeof(RectTransform));
    scrollbar.transform.SetParent(parentTransform, worldPositionStays: false);

    scrollbar.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.right)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.one)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(10f, 0f));

    scrollbar.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIResources.GetSprite("Background"))
        .SetColor(Color.black)
        .SetRaycastTarget(true);

    GameObject area = new("SlidingArea", typeof(RectTransform));
    area.transform.SetParent(scrollbar.transform, worldPositionStays: false);

    area.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    GameObject handle = new("Handle", typeof(RectTransform));
    handle.transform.SetParent(area.transform, worldPositionStays: false);

    RectTransform handleRectTransform =
        handle.GetComponent<RectTransform>()
            .SetAnchorMin(new(0.5f, 0.5f))
            .SetAnchorMax(new(0.5f, 0.5f))
            .SetPivot(new(0.5f, 0.5f))
            .SetPosition(Vector2.zero)
            .SetSizeDelta(Vector2.zero);

    Image handleImage =
        handle.AddComponent<Image>()
            .SetType(Image.Type.Sliced)
            .SetSprite(UIResources.GetSprite("UISprite"))
            .SetColor(Color.white)
            .SetPixelsPerUnitMultiplier(0.7f);

    return scrollbar.AddComponent<Scrollbar>()
        .SetTargetGraphic(handleImage)
        .SetTransition(Selectable.Transition.ColorTint)
        .SetColors(ScrollbarColors)
        .SetDirection(Scrollbar.Direction.TopToBottom)
        .SetHandleRect(handleRectTransform);
  }

  public static readonly ColorBlock ScrollbarColors =
      new() {
        normalColor = Color.white,
        highlightedColor = new(1f, 0.75f, 0f),
        selectedColor = new(1f, 0.75f, 0f),
        pressedColor = new(1f, 0.67f, 0.11f),
        disabledColor = Color.gray,
        colorMultiplier = 1f,
        fadeDuration = 0.15f,
      };
}
