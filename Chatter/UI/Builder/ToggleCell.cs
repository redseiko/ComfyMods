using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Chatter {
  public class ToggleCell {
    public GameObject Cell { get; private set; }
    public Image Background { get; private set; }
    public TextMeshProUGUI Label { get; private set; }
    public Toggle Toggle { get; private set; }

    public ToggleCell(Transform parentTransform) {
      Cell = CreateChildCell(parentTransform);
      Background = Cell.Image();

      Label = CreateChildLabel(Cell.transform);

      Toggle = Cell.AddComponent<Toggle>();
      Toggle.targetGraphic = Background;
      Toggle.onValueChanged.AddListener(isOn => OnToggleValueChanged(isOn));

      Toggle.SetIsOn(false);
    }

    GameObject CreateChildCell(Transform parentTransform) {
      GameObject cell = new("Toggle", typeof(RectTransform));
      cell.SetParent(parentTransform);

      cell.GetComponent<RectTransform>()
          .SetSizeDelta(Vector2.zero);

      cell.AddComponent<Image>()
          .SetType(Image.Type.Sliced)
          .SetSprite(UIBuilder.CreateSuperellipse(64, 64, 8))
          .SetColor(Color.clear)
          .SetRaycastTarget(true);

      return cell;
    }

    TextMeshProUGUI CreateChildLabel(Transform parentTransform) {
      TextMeshProUGUI label = UIBuilder.CreateLabel(parentTransform);

      label.alignment = TextAlignmentOptions.Center;
      label.text = "Toggle";

      label.rectTransform
          .SetAnchorMin(Vector2.zero)
          .SetAnchorMax(Vector2.one)
          .SetSizeDelta(Vector2.zero)
          .SetPosition(Vector2.zero);

      return label;
    }

    void OnToggleValueChanged(bool isOn) {
      Background.color = isOn ? new(0.8f, 0.8f, 0.8f, 0.5f) : new(0.5f, 0.5f, 0.5f, 0.5f);
      Label.color = isOn ? Color.white : Color.gray;
    }
  }
}
