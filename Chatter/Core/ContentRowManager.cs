namespace Chatter;

using System.Linq;

using ComfyLib;

using TMPro;

using UnityEngine;

using static PluginConfig;

public static class ContentRowManager {
  public static CircularQueue<ContentRow> MessageRows { get; } = new(capacity: 50, DestroyContentRow);

  public static void CreateContentRow(ChatMessage message) {
    if (!ChatPanelController.ChatPanel?.Content) {
      return;
    }

    if (ShouldCreateContentRow(message)) {
      ContentRow row = new(message, ChatMessageLayout.Value, ChatPanelController.ChatPanel.Content.transform);
      MessageRows.EnqueueItem(row);

      SetupContentRow(row);

      bool isMessageTypeActive = ChatPanelController.ChatPanel.IsMessageTypeToggleActive(message.MessageType);
      row.Divider.Ref()?.SetActive(isMessageTypeActive && ShowChatPanelMessageDividers.Value);
      row.Row.SetActive(isMessageTypeActive);
    }

    TMP_Text bodyLabel = MessageRows.LastItem.AddBodyLabel(message);
    bodyLabel.font = UIResources.GetFontAssetByName(ChatMessageFontAsset.Value);
    bodyLabel.fontSize = ChatMessageFontSize.Value;
  }

  public static bool ShouldCreateContentRow(ChatMessage message) {
    return ChatMessageLayout.Value == MessageLayoutType.SingleRow
        || MessageRows.IsEmpty
        || MessageRows.LastItem == null
        || MessageRows.LastItem.Message?.MessageType != message.MessageType
        || MessageRows.LastItem.Message?.SenderId != message.SenderId
        || MessageRows.LastItem.Message?.Username != message.Username;
  }

  public static void DestroyContentRow(ContentRow row) {
    if (row.Row) {
      UnityEngine.Object.Destroy(row.Row);
    }

    if (row.Divider) {
      UnityEngine.Object.Destroy(row.Divider);
    }
  }

  public static void RebuildContentRows() {
    MessageRows.ClearItems();

    if (ChatPanelController.ChatPanel?.Panel) {
      foreach (ChatMessage message in ChatMessageUtils.MessageHistory) {
        CreateContentRow(message);
      }
    }
  }

  public static void SetupContentRow(ContentRow row) {
    if (row.LayoutType == MessageLayoutType.WithHeaderRow) {
      row.HeaderLeftLabel.font = UIResources.GetFontAssetByName(ChatMessageFontAsset.Value);
      row.HeaderLeftLabel.fontSize = ChatMessageFontSize.Value;
      row.HeaderLeftLabel.text = ChatMessageUtils.GetUsernameText(row.Message.Username);
      row.HeaderLeftLabel.color = ChatMessageUsernameColor.Value;

      row.HeaderRightLabel.font = UIResources.GetFontAssetByName(ChatMessageFontAsset.Value);
      row.HeaderRightLabel.fontSize = ChatMessageFontSize.Value;
      row.HeaderRightLabel.text = ChatMessageUtils.GetTimestampText(row.Message.Timestamp);
      row.HeaderRightLabel.color = ChatMessageTimestampColor.Value;
      row.HeaderRightLabel.gameObject.SetActive(ChatMessageShowTimestamp.Value);

      row.RowLayoutGroup.SetSpacing(ChatPanelContentRowSpacing.Value);
    } else if (row.LayoutType == MessageLayoutType.SingleRow) {
      row.RowLayoutGroup.SetSpacing(ChatPanelContentSingleRowSpacing.Value);
    }
  }

  public static void ToggleContentRows(bool toggleOn, ChatMessageType messageType) {
    foreach (ContentRow row in MessageRows) {
      if (row.Message.MessageType == messageType) {
        row.Row.Ref()?.SetActive(toggleOn);
        row.Divider.Ref()?.SetActive(toggleOn);
      }
    }
  }

  public static void SetContentRowSpacing(float spacing) {
    foreach (ContentRow row in MessageRows) {
      row?.RowLayoutGroup.Ref()?.SetSpacing(spacing);
    }
  }

  public static void SetMessageTextColor(Color color, ChatMessageType messageType) {
    if (ChatMessageLayout.Value == MessageLayoutType.SingleRow) {
      RebuildContentRows();
      return;
    }

    foreach (
        TMP_Text tmpText in MessageRows
            .Where(row => row.Message?.MessageType == messageType && row.Row)
            .SelectMany(row => row.Row.GetComponentsInChildren<TMP_Text>(includeInactive: true))
            .Where(label => label.name == "BodyLabel")) {
      tmpText.color = color;
    }
  }

  public static void SetUsernameTextColor(Color color) {
    if (ChatMessageLayout.Value == MessageLayoutType.SingleRow) {
      RebuildContentRows();
      return;
    }

    foreach (ContentRow row in MessageRows) {
      row?.HeaderLeftLabel.Ref()?.SetColor(color);
    }
  }

  public static void SetTimestampTextColor(Color color) {
    if (ChatMessageLayout.Value == MessageLayoutType.SingleRow) {
      RebuildContentRows();
      return;
    }

    foreach (ContentRow row in MessageRows) {
      row?.HeaderRightLabel.Ref()?.SetColor(color);
    }
  }

  public static void ToggleShowTimestamp(bool toggleOn) {
    if (ChatMessageLayout.Value == MessageLayoutType.SingleRow) {
      RebuildContentRows();
      return;
    }

    foreach (ContentRow row in MessageRows) {
      row?.HeaderRightLabel?.gameObject.SetActive(toggleOn);
    }
  }

  public static void ToggleMessageDividers(bool toggleOn) {
    foreach (ContentRow row in MessageRows) {
      row?.Divider.Ref()?.SetActive(toggleOn);
    }
  }
}
