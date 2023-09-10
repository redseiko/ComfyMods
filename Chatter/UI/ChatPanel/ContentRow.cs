using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Chatter {
  public class ContentRow {
    public ChatMessage Message { get; private set; }
    public GameObject Row { get; private set; }
    public TextMeshProUGUI Label { get; private set; }

    public ContentRow(ChatMessage message, Transform parentTransform) {
      Message = message;
      Row = CreateChildRow(parentTransform);
      Label = CreateChildLabel(Row.transform);
    }

    GameObject CreateChildRow(Transform parentTransform) {
      GameObject row = new("Message", typeof(RectTransform));
      row.SetParent(parentTransform);

      row.GetComponent<RectTransform>()
          .SetSizeDelta(Vector2.zero);

      row.AddComponent<VerticalLayoutGroup>()
          .SetChildControl(width: true, height: true)
          .SetChildForceExpand(width: true, height: false);

      return row;
    }

    TextMeshProUGUI CreateChildLabel(Transform parentTransform) {
      TextMeshProUGUI label = UIBuilder.CreateLabel(parentTransform);

      label.alignment = TextAlignmentOptions.Left;
      label.enableWordWrapping = true;

      return label;
    }
  }
}
