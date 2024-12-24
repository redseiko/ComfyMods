namespace ColorfulPalettes;

using ComfyLib;

using UnityEngine;

public sealed class ConfigSelectPanel {
  public GameObject Panel { get; }
  public RectTransform RectTransform { get; }

  public LabelButton CloseButton { get; }

  public ConfigSelectPanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();

    CloseButton = CreateCloseButton(RectTransform);
  }

  static GameObject CreatePanel(Transform parentTransform) {
    GameObject panel = UIBuilder.CreatePanel(parentTransform);
    panel.name = "ConfigSelect";

    return panel;
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
}
