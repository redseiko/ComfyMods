using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Chatter {
  public sealed class ToggleCell {
    public GameObject Cell { get; private set; }
    public Image Background { get; private set; }
    public TextMeshProUGUI Label { get; private set; }
    public Toggle Toggle { get; private set; }

    public ToggleCell(Transform parentTransform) {
      Cell = CreateChildCell(parentTransform);
      Background = Cell.Image();

      Label = CreateChildLabel(Cell.transform);

      Toggle = Cell.AddComponent<Toggle>();
      Toggle.onValueChanged.AddListener(isOn => OnToggleValueChanged(isOn));
      Toggle
          .SetNavigationMode(Navigation.Mode.None)
          .SetTargetGraphic(Background)
          .SetIsOn(false);

      Cell.AddComponent<DummyIgnoreDrag>();
    }

    GameObject CreateChildCell(Transform parentTransform) {
      GameObject cell = new("Toggle", typeof(RectTransform));
      cell.SetParent(parentTransform);

      cell.GetComponent<RectTransform>()
          .SetSizeDelta(Vector2.zero);

      cell.AddComponent<Image>()
          .SetType(Image.Type.Sliced)
          .SetSprite(UIResources.GetSprite("button"))
          .SetColor(new(1f, 1f, 1f, 0.95f));

      return cell;
    }

    TextMeshProUGUI CreateChildLabel(Transform parentTransform) {
      TextMeshProUGUI label = UIBuilder.CreateLabel(parentTransform);

      label.alignment = TextAlignmentOptions.Center;
      label.fontSize = 14f;
      label.color = new(1f, 1f, 1f, 0.9f);
      label.text = "Toggle";

      label.rectTransform
          .SetAnchorMin(Vector2.zero)
          .SetAnchorMax(Vector2.one)
          .SetSizeDelta(Vector2.zero)
          .SetPosition(Vector2.zero);

      return label;
    }

    void OnToggleValueChanged(bool isOn) {
      Background.color = isOn ? new(1f, 1f, 1f, 0.95f) : new(0.5f, 0.5f, 0.5f, 0.95f);
      Label.color = isOn ? Color.white : Color.gray;
    }
  }
}
