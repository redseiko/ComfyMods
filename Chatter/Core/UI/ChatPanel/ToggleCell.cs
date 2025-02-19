namespace Chatter;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

public sealed class ToggleCell {
  public GameObject Cell { get; private set; }
  public Image Background { get; private set; }
  public TextMeshProUGUI Label { get; private set; }
  public Toggle Toggle { get; private set; }

  public ToggleCell(Transform parentTransform) {
    Cell = CreateChildCell(parentTransform);
    Background = Cell.GetComponent<Image>();

    Label = CreateChildLabel(Cell.transform);

    Toggle = Cell.AddComponent<Toggle>();
    Toggle.onValueChanged.AddListener(OnToggleValueChanged);
    Toggle
        .SetNavigationMode(Navigation.Mode.None)
        .SetTargetGraphic(Background)
        .SetIsOnWithoutNotify(false);

    Cell.AddComponent<DummyIgnoreDrag>();

    OnToggleValueChanged(false);
  }

  GameObject CreateChildCell(Transform parentTransform) {
    GameObject cell = new("Toggle", typeof(RectTransform));
    cell.SetParent(parentTransform);

    cell.GetComponent<RectTransform>()
        .SetSizeDelta(Vector2.zero);

    cell.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIResources.GetSprite("button"))
        .SetColor(BackgroundColorOn);

    return cell;
  }

  TextMeshProUGUI CreateChildLabel(Transform parentTransform) {
    TextMeshProUGUI label = UIBuilder.CreateLabel(parentTransform);

    label
        .SetAlignment(TextAlignmentOptions.Center)
        .SetFontSize(14f)
        .SetColor(LabelColorOn)
        .SetText("Toggle");

    label.rectTransform
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetSizeDelta(Vector2.zero)
        .SetPosition(Vector2.zero);

    return label;
  }

  public static readonly Color BackgroundColorOn = new(1f, 1f, 1f, 0.95f);
  public static readonly Color BackgroundColorOff = new(0.5f, 0.5f, 0.5f, 0.95f);

  public static readonly Color LabelColorOn = Color.white;
  public static readonly Color LabelColorOff = Color.gray;

  public void SetToggleIsOn(bool isOn) {
    Toggle.isOn = isOn;
  }

  public bool GetToggleIsOn() {
    return Toggle.isOn;
  }

  void OnToggleValueChanged(bool isOn) {
    Background.color = isOn ? BackgroundColorOn : BackgroundColorOff;
    Label.color = isOn ? LabelColorOn : LabelColorOff;
  }
}
