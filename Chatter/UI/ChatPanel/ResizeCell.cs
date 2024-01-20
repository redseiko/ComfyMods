using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Chatter {
  public sealed class ResizeCell {
    public GameObject Cell { get; private set; }
    public Image Background { get; private set; }
    public TMP_Text Label { get; private set; }

    public ResizeCell(Transform parentTransform) {
      Cell = CreateChildCell(parentTransform);
      Background = Cell.GetComponent<Image>();
      Label = CreateChildLabel(Cell.transform);
    }

    GameObject CreateChildCell(Transform parentTransform) {
      GameObject cell = new("Resizer", typeof(RectTransform));
      cell.SetParent(parentTransform);

      cell.RectTransform()
          .SetAnchorMin(new(0f, 0.5f))
          .SetAnchorMax(new(0f, 0.5f))
          .SetPivot(new(0f, 0.5f))
          .SetSizeDelta(new(42.5f, 42.5f))
          .SetPosition(new(5f, 0f));

      cell.AddComponent<Image>()
          .SetType(Image.Type.Sliced)
          .SetSprite(UIResources.GetSprite("button"))
          .SetColor(new(1f, 1f, 1f, 0.95f));

      return cell;
    }

    TMP_Text CreateChildLabel(Transform parentTransform) {
      TMP_Text label = UIBuilder.CreateLabel(parentTransform);

      label.GetComponent<RectTransform>()
          .SetAnchorMin(Vector2.zero)
          .SetAnchorMax(Vector2.one)
          .SetPivot(new(0.5f, 0.5f))
          .SetSizeDelta(Vector2.zero);

      label.alignment = TextAlignmentOptions.Center;
      label.fontSize = 24f;
      label.text = "<rotate=45>\u2194</rotate>";

      return label;
    }
  }
}
