﻿namespace Pinnacle;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class LabelButton {
  public GameObject Container { get; private set; }
  public RectTransform RectTransform { get; private set; }
  public TMP_Text Label { get; private set; }
  public Button Button { get; private set; }

  public LabelButton(Transform parentTransform) {
    Container = CreateContainer(parentTransform);
    RectTransform = Container.GetComponent<RectTransform>();
    Label = CreateLabel(Container.transform);
    Button = CreateButton(Container);
  }

  static GameObject CreateContainer(Transform parentTransform) {
    GameObject container = new("Button", typeof(RectTransform));
    container.transform.SetParent(parentTransform, worldPositionStays: false);

    container.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIResources.GetSprite("button"))
        .SetColor(Color.white);

    container.GetComponent<RectTransform>()
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(120f, 45f));

    return container;
  }

  static TMP_Text CreateLabel(Transform parentTransform) {
    TMP_Text label = UIBuilder.CreateTMPLabel(parentTransform);

    label
        .SetFontSize(16f)
        .SetAlignment(TextAlignmentOptions.Center)
        .SetText("Button");

    label.rectTransform
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    return label;
  }

  static Button CreateButton(GameObject container) {
    Button button = container.AddComponent<Button>();

    button
        .SetTransition(Selectable.Transition.SpriteSwap)
        .SetSpriteState(
            new SpriteState() {
              disabledSprite = UIResources.GetSprite("button_disabled"),
              highlightedSprite = UIResources.GetSprite("button_highlight"),
              pressedSprite = UIResources.GetSprite("button_pressed"),
              selectedSprite = UIResources.GetSprite("button_highlight")
            });

    return button;
  }
}
