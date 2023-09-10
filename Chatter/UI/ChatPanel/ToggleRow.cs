using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

namespace Chatter {
  public class ToggleRow {
    public GameObject Row { get; private set; }

    public ToggleCell SayToggle { get; private set; }
    public ToggleCell ShoutToggle { get; private set; }
    public ToggleCell WhisperToggle { get; private set; }
    public ToggleCell PingToggle { get; private set; }
    public ToggleCell MessageHudToggle { get; private set; }
    public ToggleCell TextToggle { get; private set; }

    public ToggleRow(Transform parentTransform) {
      Row = CreateChildRow(parentTransform);

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
          .SetSizeDelta(Vector2.zero);

      row.AddComponent<HorizontalLayoutGroup>()
          .SetChildControl(width: true, height: true)
          .SetChildForceExpand(width: false, height: false)
          .SetSpacing(8f)
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
