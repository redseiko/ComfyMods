using ComfyLib;

using GUIFramework;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

namespace Chatter {
  public sealed class InputFieldCell {
    public GameObject Cell { get; private set; }
    public Image Background { get; private set; }

    public GuiInputField InputField { get; private set; }
    public TextMeshProUGUI InputFieldPlaceholder { get; private set; }

    public InputFieldCell(Transform parentTransform) {
      Cell = CreateChildCell(parentTransform);
      Background = Cell.GetComponent<Image>();

      InputField = CreateChildInputField(Cell.transform);
      InputFieldPlaceholder = InputField.placeholder.GetComponent<TextMeshProUGUI>();

      InputField
          .SetTargetGraphic(Background)
          .SetTransition(Selectable.Transition.ColorTint)
          .SetNavigationMode(Navigation.Mode.None);
    }

    GameObject CreateChildCell(Transform parentTransform) {
      GameObject cell = new("Cell", typeof(RectTransform));
      cell.SetParent(parentTransform);

      cell.AddComponent<Image>()
        .SetType(Image.Type.Sliced)
        .SetSprite(UIBuilder.CreateSuperellipse(128, 128, 10))
        .SetColor(new(0.1f, 0.1f, 0.1f, 0.3f));

      cell.AddComponent<RectMask2D>();

      return cell;
    }

    GuiInputField CreateChildInputField(Transform parentTransform) {
      GameObject row = new("Text Area", typeof(RectTransform));
      row.SetParent(parentTransform);

      row.GetComponent<RectTransform>()
          .SetAnchorMin(Vector2.zero)
          .SetAnchorMax(Vector2.one)
          .SetPivot(new(0.5f, 0.5f))
          .SetSizeDelta(new(-16f, -8f))
          .SetPosition(Vector2.zero);

      TextMeshProUGUI label = UIBuilder.CreateLabel(row.transform);
      label.SetName("Text");

      label.rectTransform
          .SetAnchorMin(Vector2.zero)
          .SetAnchorMax(Vector2.one)
          .SetSizeDelta(Vector2.zero)
          .SetPosition(Vector2.zero);

      label.textWrappingMode = TextWrappingModes.NoWrap;
      label.overflowMode = TextOverflowModes.Masking;
      label.richText = false;
      label.alignment = TextAlignmentOptions.Left;
      label.color = Color.white;
      label.text = string.Empty;

      TextMeshProUGUI placeholder = UIBuilder.CreateLabel(row.transform);
      placeholder.SetName("Placeholder");

      placeholder.rectTransform
          .SetAnchorMin(Vector2.zero)
          .SetAnchorMax(Vector2.one)
          .SetSizeDelta(Vector2.zero)
          .SetPosition(Vector2.zero);

      placeholder.textWrappingMode = TextWrappingModes.NoWrap;
      placeholder.overflowMode = TextOverflowModes.Masking;
      placeholder.richText = true;
      placeholder.alignment = TextAlignmentOptions.Left;
      placeholder.color = new(1f, 1f, 1f, 0.3f);
      placeholder.text = "...";

      GuiInputField inputField = parentTransform.gameObject.AddComponent<GuiInputField>();
      inputField.textViewport = row.GetComponent<RectTransform>();
      inputField.textComponent = label;
      inputField.placeholder = placeholder;
      inputField.onFocusSelectAll = false;

      inputField.OnInputLayoutChanged();
      inputField.MoveToStartOfLine(false, false);

      return inputField;
    }
  }
}
