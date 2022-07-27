using BepInEx;

using HarmonyLib;

using System;
using System.Linq;
using System.Reflection;

using UnityEngine;
using UnityEngine.UI;

using static Chatter.PluginConfig;
using static UnityEngine.ColorUtility;

namespace Chatter {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class Chatter : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.chatter";
    public const string PluginName = "Chatter";
    public const string PluginVersion = "1.1.1";

    Harmony _harmony;

    public void Awake() {
      BindConfig(Config);

      IsModEnabled.OnSettingChanged(toggle => ToggleChatter(toggle));

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    public enum MessageLayoutType {
      WithHeaderRow,
      SingleRow
    }

    public class ContentRow {
      public GameObject Row { get; init; }
      public ChatMessage ChatMessage { get; init; }
      public GameObject Divider { get; init; }

      public ContentRow(GameObject row, ChatMessage message, GameObject divider) {
        Row = row;
        ChatMessage = message;
        Divider = divider;
      }
    }

    internal static readonly CircularQueue<ChatMessage> MessageHistory = new(50, _ => { });
    internal static readonly CircularQueue<ContentRow> MessageRows = new(50, DestroyMessageRow);

    internal static bool _isPluginConfigBound = false;

    internal static ChatPanel _chatPanel = null;
    internal static InputField _chatInputField = null;

    internal static bool _isCreatingChatMessage = false;
    static bool _isChatPanelVisible = false;

    public static void ToggleChatter(bool toggle) {
      ToggleVanillaChat(Chat.m_instance, !toggle);
      ToggleChatPanel(Chat.m_instance, toggle);

      if (Chat.m_instance) {
        Chat.m_instance.m_input = toggle && ChatPanel?.Panel ? ChatPanel.InputField : _chatInputField;
      }
    }

    static void ToggleChatPanel(Chat chat, bool toggle) {
      if (_chatPanel == null || !_chatPanel.Panel) {
        _chatPanel = CreateChatPanel(chat);
      }

      ChatPanel?.Panel.SetActive(toggle);

      if (toggle) {
        ChatPanel?.SetPanelPosition(ChatPanelPosition.Value);
        ChatPanel?.SetPanelSize(ChatPanelSize.Value);
        ChatPanel?.SetContentWidthOffset(ChatContentWidthOffset.Value);
      }
    }

    static ChatPanel CreateChatPanel(Chat chat) {
      if (!chat) {
        return null;
      }

      ChatPanel chatPanel = new(chat.m_chatWindow.transform.parent, chat.m_output);
      RectTransform panelRectTransform = chatPanel.Panel.GetComponent<RectTransform>();
      Outline panelOutline = chatPanel.Panel.GetComponent<Outline>();

      PanelDragger dragger = chatPanel.Grabber.GetComponentInChildren<PanelDragger>();
      dragger.TargetRectTransform = panelRectTransform;
      dragger.TargetOutline = panelOutline;
      dragger.OnEndDragAction = position => ChatPanelPosition.Value = position;

      PanelResizer resizer = chatPanel.Grabber.GetComponentInChildren<PanelResizer>();
      resizer.TargetRectTransform = panelRectTransform;
      resizer.TargetOutline = panelOutline;
      resizer.OnEndDragAction =
          size => {
            ChatPanelSize.Value = size;
            ChatPanelPosition.Value = panelRectTransform.anchoredPosition;
          };

      chatPanel.SayToggle.onValueChanged.AddListener(isOn => ToggleContentRows(isOn, ChatMessageType.Say));
      chatPanel.SayToggle.isOn = true;

      chatPanel.ShoutToggle.onValueChanged.AddListener(isOn => ToggleContentRows(isOn, ChatMessageType.Shout));
      chatPanel.ShoutToggle.isOn = true;

      chatPanel.PingToggle.onValueChanged.AddListener(isOn => ToggleContentRows(isOn, ChatMessageType.Ping));
      chatPanel.PingToggle.onValueChanged.Invoke(false);

      chatPanel.WhisperToggle.onValueChanged.AddListener(isOn => ToggleContentRows(isOn, ChatMessageType.Whisper));
      chatPanel.WhisperToggle.isOn = true;

      chatPanel.MessageHudToggle.onValueChanged.AddListener(
          isOn => ToggleContentRows(isOn, ChatMessageType.HudCenter));
      chatPanel.MessageHudToggle.isOn = true;

      chatPanel.TextToggle.onValueChanged.AddListener(isOn => ToggleContentRows(isOn, ChatMessageType.Text));
      chatPanel.TextToggle.isOn = true;

      return chatPanel;
    }

    static void ToggleVanillaChat(Chat chat, bool toggle) {
      if (chat) {
        chat.m_chatWindow.GetComponent<RectMask2D>().enabled = toggle;

        foreach (Image image in chat.m_chatWindow.GetComponentsInChildren<Image>(includeInactive: true)) {
          image.gameObject.SetActive(toggle);
        }

        chat.m_output.gameObject.SetActive(toggle);
      }
    }

    internal static void HideChatPanelDelegate(float hideTimer) {
      if (IsModEnabled.Value) {
        _isChatPanelVisible = hideTimer < HideChatPanelDelay.Value || Menu.IsVisible();

        if (_chatPanel?.CanvasGroup) {
          _chatPanel.CanvasGroup.alpha = _isChatPanelVisible ? 1f : HideChatPanelAlpha.Value;
          _chatPanel.CanvasGroup.blocksRaycasts = _isChatPanelVisible;
        }

        if (!_isChatPanelVisible) {
          _chatPanel.SetVerticalScrollPosition(0f);
        }
      }
    }

    internal static void EnableChatPanelDelegate() {
      if (IsModEnabled.Value && _chatPanel?.InputField) {
        _chatPanel.InputField.enabled = true;
      }
    }

    internal static bool DisableChatPanelDelegate(bool active) {
      if (IsModEnabled.Value && _chatPanel?.InputField) {
        _chatPanel.InputField.enabled = false;
        return true;
      }

      return active;
    }

    internal static void BindChatConfig(Chat chat, ChatPanel chatPanel) {
      if (_isPluginConfigBound) {
        return;
      }

      _isPluginConfigBound = true;

      BindChatMessageFont(chat.Ref()?.m_output.font);
      BindChatPanelSize(chat.Ref()?.m_chatWindow);

      ChatMessageFont.OnSettingChanged(font => ChatPanel?.SetFont(MessageFont));
      ChatMessageFontSize.OnSettingChanged(size => ChatPanel?.SetFontSize(size));

      ChatPanelBackgroundColor.OnSettingChanged(color => ChatPanel?.SetPanelBackgroundColor(color));
      ChatPanelRectMaskSoftness.OnSettingChanged(softness => ChatPanel?.SetPanelRectMaskSoftness(softness));

      ChatPanelPosition.OnSettingChanged(position => ChatPanel?.SetPanelPosition(position));
      ChatPanelSize.OnSettingChanged(size => ChatPanel?.SetPanelSize(size));
      ChatContentWidthOffset.OnSettingChanged(offset => ChatPanel?.SetContentWidthOffset(offset));

      ChatPanelContentSpacing.OnSettingChanged(_ => ChatPanel?.SetContentSpacing(ContentRowSpacing));
      ChatPanelContentBodySpacing.OnSettingChanged(_ => ChatPanel?.SetContentBodySpacing(ContentRowBodySpacing));
      ChatPanelContentSingleRowSpacing.OnSettingChanged(_ => {
        ChatPanel?.SetContentSpacing(ContentRowSpacing);
        ChatPanel?.SetContentBodySpacing(ContentRowBodySpacing);
      });

      ShowChatPanelMessageDividers.OnSettingChanged(ToggleChatPanelMessageDividers);

      ChatMessageLayout.OnSettingChanged(_ => RebuildMessageRows());
      ChatMessageShowTimestamp.OnSettingChanged(ToggleShowTimestamp);

      ChatMessageTextDefaultColor.OnSettingChanged(color => SetContentRowBodyTextColor(color, ChatMessageType.Text));
      ChatMessageTextMessageHudColor.OnSettingChanged(
          color => SetContentRowBodyTextColor(color, ChatMessageType.HudCenter));
      ChatMessageTextSayColor.OnSettingChanged(color => SetContentRowBodyTextColor(color, ChatMessageType.Say));
      ChatMessageTextShoutColor.OnSettingChanged(color => SetContentRowBodyTextColor(color, ChatMessageType.Shout));
      ChatMessageTextWhisperColor.OnSettingChanged(
          color => SetContentRowBodyTextColor(color, ChatMessageType.Whisper));
      ChatMessageTextPingColor.OnSettingChanged(color => SetContentRowBodyTextColor(color, ChatMessageType.Ping));
      ChatMessageTimestampColor.OnSettingChanged(SetTimestampTextColor);
    }

    public static ChatPanel ChatPanel {
      get => _chatPanel?.Panel ? _chatPanel : null;
    }

    public static bool IsChatPanelVisible {
      get => _chatPanel?.Panel ? _isChatPanelVisible : false;
    }

    static void DestroyMessageRow(ContentRow row) {
      Destroy(row.Row);
      Destroy(row.Divider);
    }

    static void RebuildMessageRows() {
      MessageRows.ClearItems();

      if (!_chatPanel?.Panel) {
        return;
      }

      foreach (ChatMessage message in MessageHistory) {
        CreateChatMessageRow(message);
      }
    }

    static void ToggleChatPanelMessageDividers(bool toggle) {
      if (ChatMessageLayout.Value == MessageLayoutType.SingleRow) {
        return;
      }

      foreach (ContentRow row in MessageRows) {
        row.Divider.Ref()?.SetActive(toggle);
      }
    }

    static void ToggleContentRows(bool toggle, ChatMessageType messageType) {
      foreach (ContentRow row in MessageRows.Where(row => row.ChatMessage?.MessageType == messageType)) {
        row.Row.Ref()?.SetActive(toggle);
        row.Divider.Ref()?.SetActive(toggle && ShouldShowDivider());
      }
    }

    static void SetContentRowBodyTextColor(Color color, ChatMessageType messageType) {
      if (ChatMessageLayout.Value == MessageLayoutType.SingleRow) {
        RebuildMessageRows();
        return;
      }

      foreach (
          Text text in MessageRows
              .Where(row => row.ChatMessage?.MessageType == messageType && row.Row)
              .SelectMany(row => row.Row.GetComponentsInChildren<Text>(includeInactive: true))
              .Where(text => text.name == ChatPanel.ContentRowBodyName)) {
        text.color = color;
      }
    }

    static void ToggleShowTimestamp(bool toggle) {
      if (ChatMessageLayout.Value == MessageLayoutType.SingleRow) {
        RebuildMessageRows();
        return;
      }

      foreach (
          GameObject cell in MessageRows
              .Where(row => row.Row)
              .SelectMany(row => row.Row.GetComponentsInChildren<Text>(includeInactive: true))
              .Where(text => text.name == ChatPanel.HeaderRightCellName)
              .Select(text => text.gameObject)) {
        cell.SetActive(toggle);
      }
    }

    static void SetTimestampTextColor(Color color) {
      if (ChatMessageLayout.Value == MessageLayoutType.SingleRow) {
        RebuildMessageRows();
        return;
      }

      foreach (
          Text text in MessageRows
              .Where(row => row.Row)
              .SelectMany(row => row.Row.GetComponentsInChildren<Text>(includeInactive: true))
              .Where(text => text.name == ChatPanel.HeaderRightCellName)) {
        text.color = color;
      }
    }

    [HarmonyPatch(typeof(Menu))]
    class MenuPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(Menu.Show))]
      static void ShowPostfix() {
        if (IsModEnabled.Value) {
          ChatPanel?.ToggleGrabber(true);
          ChatPanel?.SetPanelSize(ChatPanelSize.Value);
        }
      }

      [HarmonyPostfix]
      [HarmonyPatch(nameof(Menu.Hide))]
      static void HidePostfix() {
        if (IsModEnabled.Value) {
          ChatPanel?.ToggleGrabber(false);
          ChatPanel?.SetPanelSize(ChatPanelSize.Value);
          ChatPanel?.SetVerticalScrollPosition(0f);
        }
      }
    }

    [HarmonyPatch(typeof(MessageHud))]
    class MessageHudPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(MessageHud.ShowMessage))]
      static void ShowMessagePostfix(ref MessageHud.MessageType type, ref string text) {
        if (!IsModEnabled.Value
            || type != MessageHud.MessageType.Center
            || !ChatPanel?.Panel
            || !ShowMessageHudCenterMessages.Value) {
          return;
        }

        if (!DisallowChatMessageHudStartTexts.Value.IsNullOrWhiteSpace()) {
          string[] disallowedHudMessages = DisallowChatMessageHudStartTexts.Value.Split(',');
          foreach (string disallowedString in disallowedHudMessages) {
            if (text.StartsWith(disallowedString.Trim())) return;
          }
        }

        AddChatMessage(new() { MessageType = ChatMessageType.HudCenter, Timestamp = DateTime.Now, Text = text});
      }
    }

    public static void AddChatMessage(ChatMessage message) {
      MessageHistory.EnqueueItem(message);

      if (_chatPanel.Panel) {
        CreateChatMessageRow(message);
      }
    }

    static void CreateChatMessageRow(ChatMessage message) {
      if (ShouldCreateDivider(message)) {
        GameObject divider = _chatPanel.CreateMessageDivider(_chatPanel.Content.transform);
        GameObject row = _chatPanel.CreateChatMessageRow(_chatPanel.Content.transform);
        MessageRows.EnqueueItem(new(row, message, divider));

        bool active = IsMessageTypeActive(message.MessageType);
        divider.SetActive(active && ShouldShowDivider());
        row.SetActive(active);

        if (ShouldCreateRowHeader()) {
          (_, GameObject leftCell, GameObject rightCell) = 
              _chatPanel.CreateChatMessageRowHeader(
                  row.transform, GetUsernameText(message.Username), GetTimestampText(message.Timestamp));

          leftCell.GetComponent<Text>().SetColor(ChatMessageTextDefaultColor.Value);

          rightCell.GetComponent<Text>().SetColor(ChatMessageTimestampColor.Value);
          rightCell.SetActive(ChatMessageShowTimestamp.Value);
        }
      }

      _chatPanel.CreateChatMessageRowBody(MessageRows.LastItem.Row.transform, GetBodyText(message))
          .GetComponent<Text>()
          .SetColor(GetMessageTextColor(message.MessageType));
    }

    static bool ShouldCreateDivider(ChatMessage message) {
      return MessageRows.IsEmpty
          || MessageRows.LastItem?.ChatMessage?.MessageType != message.MessageType
          || MessageRows.LastItem?.ChatMessage?.SenderId != message.SenderId;
    }

    static bool ShouldCreateRowHeader() {
      return ChatMessageLayout.Value == MessageLayoutType.WithHeaderRow;
    }

    static bool ShouldShowDivider() {
      return ShowChatPanelMessageDividers.Value && ChatMessageLayout.Value == MessageLayoutType.WithHeaderRow;
    }

    static bool IsMessageTypeActive(ChatMessageType messageType) {
      return messageType switch {
        ChatMessageType.Text => _chatPanel.TextToggle.isOn,
        ChatMessageType.HudCenter => _chatPanel.MessageHudToggle.isOn,
        ChatMessageType.Say => _chatPanel.SayToggle.isOn,
        ChatMessageType.Shout => _chatPanel.ShoutToggle.isOn,
        ChatMessageType.Whisper => _chatPanel.WhisperToggle.isOn,
        ChatMessageType.Ping => _chatPanel.PingToggle.isOn,
        _ => true,
      };
    }

    static string GetUsernameText(string username) {
      if (username.Length == 0) {
        return string.Empty;
      }

      return ChatMessageLayout.Value switch {
        MessageLayoutType.WithHeaderRow =>
            $"{ChatMessageUsernamePrefix.Value}{username}{ChatMessageUsernamePostfix.Value}",
        MessageLayoutType.SingleRow =>
            $"<color=#{ToHtmlStringRGBA(ChatMessageTextDefaultColor.Value)}>[ {username} ]</color>",
        _ => username,
      };
    }

    static string GetTimestampText(DateTime timestamp) {
      return ChatMessageLayout.Value switch {
        MessageLayoutType.SingleRow => 
            ChatMessageShowTimestamp.Value
                ? string.Format(
                      "<color=#{0}>{1:t}</color>", ToHtmlStringRGBA(ChatMessageTimestampColor.Value), timestamp)
                : string.Empty,
        _ => timestamp.ToString("T"),
      };
    }

    static string GetBodyText(ChatMessage message) {
      return ChatMessageLayout.Value switch {
        MessageLayoutType.SingleRow =>
            string.Join(
                " ", GetTimestampText(message.Timestamp), GetUsernameText(message.Username), GetMessageText(message)),
        _ => GetMessageText(message)
      };
    }

    static string GetMessageText(ChatMessage message) {
      string text =
          message.MessageType switch {
            ChatMessageType.Ping => $"Ping! {message.Position}",
            _ => message.Text
          };

      return ChatMessageLayout.Value switch {
        MessageLayoutType.SingleRow =>
            string.Format("<color=#{0}>{1}</color>", ToHtmlStringRGBA(GetMessageTextColor(message.MessageType)), text),
        _ => text,
      };
    }

    static Color GetMessageTextColor(ChatMessageType messageType) {
      return messageType switch {
        ChatMessageType.Text => ChatMessageTextDefaultColor.Value,
        ChatMessageType.HudCenter => ChatMessageTextMessageHudColor.Value,
        ChatMessageType.Say => ChatMessageTextSayColor.Value,
        ChatMessageType.Shout => ChatMessageTextShoutColor.Value,
        ChatMessageType.Whisper => ChatMessageTextWhisperColor.Value,
        ChatMessageType.Ping => ChatMessageTextPingColor.Value,
        _ => ChatMessageTextDefaultColor.Value,
      };
    }
  }
}