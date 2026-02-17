namespace EmDee;

using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

public sealed class EmDeePanel {
  public GameObject Panel { get; private set; }
  public RectTransform RectTransform { get; }

  public EmDeePanel(Transform parentTransform) {
    Panel = CreatePanel(parentTransform);
    RectTransform = Panel.GetComponent<RectTransform>();
  }

  public EmDeePanel ClearContent() {
    foreach (Transform childTransform in Panel.transform) {
      UnityEngine.Object.Destroy(childTransform.gameObject);
    }

    return this;
  }

  static GameObject CreatePanel(Transform parentTransform) {
    GameObject panel = UIBuilder.CreatePanel(parentTransform);
    panel.name = "Panel";

    panel.GetComponent<RectTransform>()
        .SetSizeDelta(new(400f, 600f));

    panel.AddComponent<VerticalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetPadding(left: 5, right: 5, top: 5, bottom: 5);

    return panel;
  }
}
