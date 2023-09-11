using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using static Chatter.PluginConfig;

namespace Chatter {
  public class ContentRow {
    public ChatMessage Message { get; private set; }
    public MessageLayoutType LayoutType { get; private set; }

    public GameObject Row { get; private set; }

    public GameObject Divider { get; private set; }
    public GameObject Header { get; private set; }
    public TextMeshProUGUI HeaderLeftLabel { get; private set; }
    public TextMeshProUGUI HeaderRightLabel { get; private set; }

    public ContentRow(ChatMessage message, MessageLayoutType layoutType, Transform parentTransform) {
      Message = message;
      LayoutType = layoutType;

      if (layoutType == MessageLayoutType.WithHeaderRow) {
        Divider = CreateDivider(parentTransform);
      }

      Row = CreateChildRow(parentTransform);

      if (layoutType == MessageLayoutType.WithHeaderRow) {
        (Header, HeaderLeftLabel, HeaderRightLabel) = CreateHeader(Row.transform);
      }
    }

    public void SetupContentRow() {
      if (LayoutType == MessageLayoutType.WithHeaderRow) {
        Divider.gameObject.SetActive(ShowChatPanelMessageDividers.Value);

        HeaderLeftLabel.text = ChatMessageUtils.GetUsernameText(Message.Username);
        HeaderLeftLabel.color = ChatMessageUsernameColor.Value;

        HeaderRightLabel.text = ChatMessageUtils.GetTimestampText(Message.Timestamp);
        HeaderRightLabel.color = ChatMessageTimestampColor.Value;
        HeaderRightLabel.gameObject.SetActive(ChatMessageShowTimestamp.Value);
      }
    }

    public TextMeshProUGUI AddBodyLabel(ChatMessage message) {
      TextMeshProUGUI bodyLabel = CreateChildBodyLabel(Row.transform);
      bodyLabel.text = ChatMessageUtils.GetContentRowBodyText(message);

      if (LayoutType == MessageLayoutType.WithHeaderRow) {
        bodyLabel.color = ChatMessageUtils.GetMessageTextColor(message.MessageType);
      }

      return bodyLabel;
    }

    GameObject CreateChildRow(Transform parentTransform) {
      GameObject row = new("Message", typeof(RectTransform));
      row.SetParent(parentTransform);

      row.GetComponent<RectTransform>()
          .SetSizeDelta(Vector2.zero);

      row.AddComponent<VerticalLayoutGroup>()
          .SetChildControl(width: true, height: true)
          .SetChildForceExpand(width: true, height: false)
          .SetSpacing(2f);

      return row;
    }

    GameObject CreateDivider(Transform parentTransform) {
      GameObject divider = new("Divider", typeof(RectTransform));
      divider.SetParent(parentTransform);

      divider.AddComponent<Image>()
          .SetSprite(UIBuilder.CreateRect(10, 10, Color.white))
          .SetType(Image.Type.Filled)
          .SetColor(new(1f, 1f, 1f, 0.0625f))
          .SetRaycastTarget(true)
          .SetMaskable(true);

      divider.AddComponent<LayoutElement>()
          .SetFlexible(width: 1f)
          .SetPreferred(height: 1f);

      return divider;
    }

    (GameObject header, TextMeshProUGUI leftCell, TextMeshProUGUI rightCell) CreateHeader(Transform parentTransform) {
      GameObject header = new("Header", typeof(RectTransform));
      header.SetParent(parentTransform);

      header.AddComponent<HorizontalLayoutGroup>()
          .SetChildControl(width: true, height: true)
          .SetChildForceExpand(width: false, height: false)
          .SetPadding(left: 0, right: 0, top: 0, bottom: 0);

      TextMeshProUGUI leftLabel = UIBuilder.CreateLabel(header.transform);
      leftLabel.name = "HeaderLeftLabel";
      leftLabel.text = "LeftLabel";
      leftLabel.alignment = TextAlignmentOptions.Left;

      GameObject spacer = new("Spacer", typeof(RectTransform));
      spacer.SetParent(header.transform);
      spacer.AddComponent<LayoutElement>().SetFlexible(width: 1f);

      TextMeshProUGUI rightLabel = UIBuilder.CreateLabel(header.transform);
      rightLabel.name = "HeaderRightLabel";
      rightLabel.text = "RightLabel";
      rightLabel.alignment = TextAlignmentOptions.Right;

      return (header, leftLabel, rightLabel);
    }

    TextMeshProUGUI CreateChildBodyLabel(Transform parentTransform) {
      TextMeshProUGUI label = UIBuilder.CreateLabel(parentTransform);
      label.name = "BodyLabel";

      label.alignment = TextAlignmentOptions.Left;
      label.enableWordWrapping = true;

      return label;
    }
  }
}
