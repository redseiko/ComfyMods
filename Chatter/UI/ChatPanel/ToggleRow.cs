using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

namespace Chatter {
  public class ToggleRow {
    public GameObject Row { get; private set; }
    public Image Background { get; private set; }

    public ToggleCell SayToggle { get; private set; }
    public ToggleCell ShoutToggle { get; private set; }
    public ToggleCell WhisperToggle { get; private set; }
    public ToggleCell PingToggle { get; private set; }
    public ToggleCell MessageHudToggle { get; private set; }
    public ToggleCell TextToggle { get; private set; }

    public ToggleRow(Transform parentTransform) {
      Row = CreateChildRow(parentTransform);
      Background = Row.Image();

      SayToggle = CreateChildToggle(Row.transform, "Say");
      ShoutToggle = CreateChildToggle(Row.transform, "Shout");
      WhisperToggle = CreateChildToggle(Row.transform, "Whisper");
      PingToggle = CreateChildToggle(Row.transform, "Ping");
      MessageHudToggle = CreateChildToggle(Row.transform, "Hud");
      TextToggle = CreateChildToggle(Row.transform, "Text");
    }

    GameObject CreateChildRow(Transform parentTransform) {
      GameObject row = new("ToggleRow", typeof(RectTransform));
      row.SetParent(parentTransform);

      row.GetComponent<RectTransform>()
          .SetAnchorMin(Vector2.zero)
          .SetAnchorMax(Vector2.right)
          .SetPivot(new(0.5f, 0f))
          .SetPosition(new(0f, -40f))
          .SetSizeDelta(new(0f, 35f));

      row.AddComponent<Image>()
        .SetType(Image.Type.Filled)
        .SetColor(new(0f, 0f, 0f, 0.5f));

      row.AddComponent<HorizontalLayoutGroup>()
          .SetChildControl(width: true, height: true)
          .SetChildForceExpand(width: false, height: false)
          .SetPadding(left:16, right: 16)
          .SetSpacing(6f)
          .SetChildAlignment(TextAnchor.MiddleRight);

      return row;
    }

    ToggleCell CreateChildToggle(Transform parentTransform, string toggleText) {
      ToggleCell toggle = new(parentTransform);
      toggle.Label.text = toggleText;

      toggle.Cell.AddComponent<LayoutElement>()
          .SetPreferred(width: toggle.Label.GetPreferredWidth() + 16f, height: toggle.Label.GetPreferredHeight() + 8f);

      return toggle;
    }
  }
}
