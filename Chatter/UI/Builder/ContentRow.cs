using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace ComfyLib {
  public class ContentRow {
    public GameObject Row { get; private set; }
    public TextMeshProUGUI Label { get; private set; }

    public ContentRow(Transform parentTransform) {
      GameObject row = new("Message", typeof(RectTransform));
      row.SetParent(parentTransform);

      row.GetComponent<RectTransform>()
          .SetSizeDelta(Vector2.zero);

      row.AddComponent<VerticalLayoutGroup>()
          .SetChildControl(width: true, height: true)
          .SetChildForceExpand(width: false, height: false);

      Label = CreateChildLabel(row.transform);
    }

    TextMeshProUGUI CreateChildLabel(Transform parentTransform) {
      TextMeshProUGUI label = UIBuilder.CreateLabel(parentTransform);
      label.name = "Label";

      label.alignment = TextAlignmentOptions.Left;
      label.enableWordWrapping = true;

      return label;
    }
  }
}
