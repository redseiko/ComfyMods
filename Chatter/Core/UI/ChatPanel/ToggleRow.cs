namespace Chatter;

using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

public sealed class ToggleRow {
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
    Background = Row.GetComponent<Image>();

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
        .SetPosition(new(0f, -55f))
        .SetSizeDelta(new(0f, 45f));

    row.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UISpriteBuilder.CreateRoundedCornerSprite(256, 256, 16))
        .SetColor(new(0.5f, 0.5f, 0.5f, 0.25f));

    row.AddComponent<HorizontalLayoutGroup>()
        .SetChildControl(width: true, height: true)
        .SetChildForceExpand(width: false, height: false)
        .SetSpacing(5f)
        .SetPadding(5, 5, 4, 4)
        .SetChildAlignment(TextAnchor.MiddleRight);

    return row;
  }

  ToggleCell CreateChildToggle(Transform parentTransform, string toggleText) {
    ToggleCell toggle = new(parentTransform);
    toggle.Label.text = toggleText;

    toggle.Cell.AddComponent<LayoutElement>()
        .SetPreferred(width: toggle.Label.GetPreferredWidth() + 25f)
        .SetFlexible(height: 0.9f);

    return toggle;
  }
}
