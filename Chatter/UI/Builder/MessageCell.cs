using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace ComfyLib {
  public class MessageCell {
    public GameObject Cell { get; private set; }
    public TextMeshProUGUI Label { get; private set; }

    public MessageCell(Transform parentTransform) {
      GameObject cell = new("Message", typeof(RectTransform));
      cell.SetParent(parentTransform);

      cell.GetComponent<RectTransform>()
          .SetSizeDelta(Vector2.zero);

      cell.AddComponent<VerticalLayoutGroup>()
          .SetChildControl(width: true, height: true)
          .SetChildForceExpand(width: false, height: false);

      Label = CreateChildLabel(cell.transform);
    }

    TextMeshProUGUI CreateChildLabel(Transform parentTransform) {
      TextMeshProUGUI label = UIBuilder.CreateLabel(parentTransform);
      label.name = "Message.Text";

      label.alignment = TextAlignmentOptions.Left;
      label.enableWordWrapping = true;

      return label;
    }
  }
}
