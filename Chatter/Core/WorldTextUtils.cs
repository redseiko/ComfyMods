using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Chatter {
  public static class WorldTextUtils {
    public static GameObject CreateWorldTextTemplate(Transform parentTransform) {
      GameObject root = new("WorldText", typeof(RectTransform));
      root.SetParent(parentTransform);

      root.GetComponent<RectTransform>()
          .SetAnchorMin(new(0.5f, 0f))
          .SetAnchorMax(new(0.5f, 0f))
          .SetPivot(new(0.5f, 0f))
          .SetPosition(Vector2.zero)
          .SetSizeDelta(new(400f, 100f));

      TextMeshProUGUI label = UIBuilder.CreateLabel(root.transform);
      label.name = "Text";

      label.rectTransform
          .SetAnchorMin(Vector2.zero)
          .SetAnchorMax(Vector2.one)
          .SetPivot(new(0.5f, 0.5f))
          .SetPosition(Vector2.zero)
          .SetSizeDelta(Vector2.zero);

      label.alignment = TextAlignmentOptions.Center;
      label.fontSize = 18f;

      return root;
    }
  }
}
