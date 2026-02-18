namespace EmDee;

using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

public sealed class EmDeePanel {
  public GameObject Panel { get; }
  public RectTransform RectTransform { get; }
  public ListView MarkdownList { get; }
  public LabelButton CloseButton { get; }

  public EmDeePanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();
    MarkdownList = CreateMarkdownList(RectTransform);
    CloseButton = CreateCloseButton(RectTransform);
  }

  public EmDeePanel ResetMarkdownList() {
    foreach (Transform transform in MarkdownList.Content.transform) {
      UnityEngine.Object.Destroy(transform.gameObject);
    }

    return this;
  }

  static GameObject CreatePanel(Transform parentTransform) {
    GameObject panel = UIBuilder.CreatePanel(parentTransform);
    panel.name = "EmDee";

    panel.GetComponent<RectTransform>()
        .SetSizeDelta(new(600f, 600f));

    return panel;
  }

  static ListView CreateMarkdownList(Transform parentTransform) {
    ListView listView = new(parentTransform);
    listView.Container.name = "Markdown";

    listView.RectTransform
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.up)
        .SetPosition(new(20f, -20f))
        .SetSizeDelta(new(-40f, -95f));

    return listView;
  }

  static LabelButton CreateCloseButton(Transform parentTransform) {
    LabelButton closeButton = new(parentTransform);
    closeButton.Button.name = "Close";

    closeButton.Container.GetComponent<RectTransform>()
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
