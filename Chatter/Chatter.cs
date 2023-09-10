using System.Reflection;

using BepInEx;

using ComfyLib;

using HarmonyLib;

using UnityEngine;
using UnityEngine.UI;

using static Chatter.PluginConfig;
using static Chatter.ChatTextInputUtils;

namespace Chatter {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class Chatter : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.chatter";
    public const string PluginName = "Chatter";
    public const string PluginVersion = "2.0.0";

    Harmony _harmony;

    void Awake() {
      BindConfig(Config);

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }

    void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    public static CircularQueue<ChatMessage> MessageHistory { get; } = new(capacity: 50, _ => { });
    public static CircularQueue<ContentRow> MessageRows { get; } = new(capacity: 50, DestroyContentRow);

    public static bool IsChatMessageQueued { get; set; }
    public static ChatPanel ChatterChatPanel { get; private set; }

    public static void ToggleChatter(Chat chat, bool toggleOn) {
      ToggleVanillaChat(chat, !toggleOn);
      ToggleChatPanel(chat, toggleOn);
      //  TerminalCommands.ToggleCommands(toggle);

      // TODO: conditional restore to vanilla-references cached.
      chat.m_input = ChatterChatPanel.TextInput.InputField;
      //chat.m_chatWindow = ChatterChatPanel.Panel.GetComponent<RectTransform>();
    }

    // TODO: cache the vanilla-references before toggling.
    public static void ToggleVanillaChat(Chat chat, bool toggleOn) {
      foreach (Image image in chat.m_chatWindow.GetComponentsInChildren<Image>(includeInactive: true)) {
        image.gameObject.SetActive(toggleOn);
      }

      chat.m_chatWindow.GetComponent<RectMask2D>().enabled = toggleOn;
      chat.m_output.gameObject.SetActive(toggleOn);
    }

    public static void ToggleChatPanel(Chat chat, bool toggleOn) {
      if (!ChatterChatPanel?.Panel) {
        ChatterChatPanel = new(chat.m_chatWindow.parent);

        ChatterChatPanel.Panel.GetComponent<RectTransform>()
            .SetAnchorMin(Vector2.right)
            .SetAnchorMax(Vector2.right)
            .SetPivot(Vector2.right)
            .SetPosition(ChatPanelPosition.Value)
            .SetSizeDelta(ChatPanelSizeDelta.Value)
            .SetAsFirstSibling();

        ChatterChatPanel.PanelDragger.OnEndDragEvent += (_, position) => ChatPanelPosition.Value = position;
        ChatterChatPanel.TextInput.InputField.onSubmit.AddListener(_ => Chat.m_instance.SendInput());

        SetChatTextInputDefaultPrefix(ChatterChatPanel, ChatPanelDefaultMessageTypeToUse.Value);

        RebuildMessageRows();
      }

      ChatterChatPanel.Panel.SetActive(toggleOn);
    }

    public static void AddChatMessage(ChatMessage message) {
      MessageHistory.Enqueue(message);

      if (ChatterChatPanel?.Panel) {
        CreateContentRow(message, ChatterChatPanel.Content.transform);
      }
    }

    static void RebuildMessageRows() {
      MessageRows.ClearItems();

      if (ChatterChatPanel?.Panel) {
        foreach (ChatMessage message in MessageHistory) {
          CreateContentRow(message, ChatterChatPanel.Content.transform);
        }
      }
    }

    public static ContentRow CreateContentRow(ChatMessage message, Transform parentTransform) {
      ContentRow row = new(parentTransform);
      row.Label.text = ChatMessageUtils.GetChatMessageText(message);

      return row;
    }

    public static void DestroyContentRow(ContentRow row) {
      Destroy(row.Row);
    }

    public static void HideChatPanelDelegate(float hideTimer) {
      if (IsModEnabled.Value && ChatterChatPanel.Panel) {
        bool isVisible = (hideTimer < HideChatPanelDelay.Value || Menu.IsVisible()) && !Hud.IsUserHidden();

        if (isVisible == ChatterChatPanel.PanelCanvasGroup.blocksRaycasts) {
          return;
        }

        if (isVisible) {
          ChatterChatPanel.PanelCanvasGroup
              .SetAlpha(1f)
              .SetBlocksRaycasts(true);
        } else {
          ChatterChatPanel.PanelCanvasGroup
              .SetAlpha(Hud.IsUserHidden() ? 0f : HideChatPanelAlpha.Value)
              .SetBlocksRaycasts(false);

          ChatterChatPanel.SetContentVerticalScrollPosition(0f);
        }
      }
    }

    public static void EnableChatPanelDelegate() {
      if (IsModEnabled.Value) {
        ChatterChatPanel.TextInput.InputField.Ref()?.SetEnabled(true);
      }
    }

    public static bool DisableChatPanelDelegate(bool active) {
      if (IsModEnabled.Value) {
        ChatterChatPanel.TextInput.InputField.Ref()?.SetEnabled(false);
        return true;
      }

      return active;
    }

    //static ChatPanel CreateChatPanel(Chat chat) {
    //  if (!chat) {
    //    return null;
    //  }

    //  ChatPanel chatPanel = new(chat.m_chatWindow.transform.parent);
    //  RectTransform panelRectTransform = chatPanel.Panel.GetComponent<RectTransform>();
    //  Outline panelOutline = chatPanel.Panel.GetComponent<Outline>();

    //  PanelDragger dragger = chatPanel.Grabber.GetComponentInChildren<PanelDragger>();
    //  dragger.TargetRectTransform = panelRectTransform;
    //  dragger.TargetOutline = panelOutline;
    //  dragger.OnEndDragAction = position => ChatPanelPosition.Value = position;

    //  PanelResizer resizer = chatPanel.Grabber.GetComponentInChildren<PanelResizer>();
    //  resizer.TargetRectTransform = panelRectTransform;
    //  resizer.TargetOutline = panelOutline;
    //  resizer.OnEndDragAction =
    //      size => {
    //        ChatPanelSize.Value = size;
    //        ChatPanelPosition.Value = panelRectTransform.anchoredPosition;
    //      };

    //  SetupChatPanelContentRowToggles(chatPanel, ChatPanelContentRowTogglesToEnable.Value);

    //  return chatPanel;
    //}

    //static void SetupChatPanelContentRowToggles(ChatPanel chatPanel, ChatMessageType togglesToEnable) {
    //  chatPanel.SayToggle.onValueChanged.AddListener(isOn => ToggleContentRows(isOn, ChatMessageType.Say));
    //  chatPanel.ShoutToggle.onValueChanged.AddListener(isOn => ToggleContentRows(isOn, ChatMessageType.Shout));
    //  chatPanel.PingToggle.onValueChanged.AddListener(isOn => ToggleContentRows(isOn, ChatMessageType.Ping));
    //  chatPanel.WhisperToggle.onValueChanged.AddListener(isOn => ToggleContentRows(isOn, ChatMessageType.Whisper));
    //  chatPanel.MessageHudToggle.onValueChanged.AddListener(
    //      isOn => ToggleContentRows(isOn, ChatMessageType.HudCenter));
    //  chatPanel.TextToggle.onValueChanged.AddListener(isOn => ToggleContentRows(isOn, ChatMessageType.Text));

    //  chatPanel.SayToggle.SetIsOn(togglesToEnable.HasFlag(ChatMessageType.Say));
    //  chatPanel.ShoutToggle.SetIsOn(togglesToEnable.HasFlag(ChatMessageType.Shout));
    //  chatPanel.PingToggle.SetIsOn(togglesToEnable.HasFlag(ChatMessageType.Ping));
    //  chatPanel.WhisperToggle.SetIsOn(togglesToEnable.HasFlag(ChatMessageType.Whisper));
    //  chatPanel.MessageHudToggle.SetIsOn(togglesToEnable.HasFlag(ChatMessageType.HudCenter));
    //  chatPanel.TextToggle.SetIsOn(togglesToEnable.HasFlag(ChatMessageType.Text));
    //}

    //public static void BindChatConfig(Chat chat, ChatPanel chatPanel) {
    //  if (_isPluginConfigBound) {
    //    return;
    //  }

    //  _isPluginConfigBound = true;

    //  BindChatPanelSize(chat.Ref()?.m_chatWindow);

    //  ChatMessageFont.OnSettingChanged(font => ChatPanel?.SetFont(MessageFont));
    //  ChatMessageFontSize.OnSettingChanged(size => ChatPanel?.SetFontSize(size));

    //  ChatPanelBackgroundColor.OnSettingChanged(color => ChatPanel?.SetPanelBackgroundColor(color));
    //  ChatPanelRectMaskSoftness.OnSettingChanged(softness => ChatPanel?.SetPanelRectMaskSoftness(softness));

    //  ChatPanelPosition.OnSettingChanged(position => ChatPanel?.SetPanelPosition(position));
    //  ChatPanelSize.OnSettingChanged(size => ChatPanel?.SetPanelSize(size));
    //  ChatContentWidthOffset.OnSettingChanged(offset => ChatPanel?.SetContentWidthOffset(offset));

    //  ChatPanelContentSpacing.OnSettingChanged(_ => ChatPanel?.SetContentSpacing(ContentRowSpacing));
    //  ChatPanelContentBodySpacing.OnSettingChanged(_ => ChatPanel?.SetContentBodySpacing(ContentRowBodySpacing));
    //  ChatPanelContentSingleRowSpacing.OnSettingChanged(_ => {
    //    ChatPanel?.SetContentSpacing(ContentRowSpacing);
    //    ChatPanel?.SetContentBodySpacing(ContentRowBodySpacing);
    //  });

    //  ShowChatPanelMessageDividers.OnSettingChanged(ToggleChatPanelMessageDividers);

    //  ChatMessageLayout.OnSettingChanged(_ => RebuildMessageRows());
    //  ChatMessageShowTimestamp.OnSettingChanged(ToggleShowTimestamp);

    //  ChatMessageTextDefaultColor.OnSettingChanged(color => SetContentRowBodyTextColor(color, ChatMessageType.Text));
    //  ChatMessageTextMessageHudColor.OnSettingChanged(
    //      color => SetContentRowBodyTextColor(color, ChatMessageType.HudCenter));
    //  ChatMessageTextSayColor.OnSettingChanged(color => SetContentRowBodyTextColor(color, ChatMessageType.Say));
    //  ChatMessageTextShoutColor.OnSettingChanged(color => SetContentRowBodyTextColor(color, ChatMessageType.Shout));
    //  ChatMessageTextWhisperColor.OnSettingChanged(
    //      color => SetContentRowBodyTextColor(color, ChatMessageType.Whisper));
    //  ChatMessageTextPingColor.OnSettingChanged(color => SetContentRowBodyTextColor(color, ChatMessageType.Ping));
    //  ChatMessageTimestampColor.OnSettingChanged(SetTimestampTextColor);

    //  MessageToggleTextFontSize.SettingChanged += (_, _) => ChatPanel?.SetupMessageTypeToggles();
    //}

    //public static ChatPanel ChatPanel {
    //  get => _chatPanel?.Panel ? _chatPanel : null;
    //}

    //public static bool IsChatPanelVisible {
    //  get => _chatPanel?.Panel ? _isChatPanelVisible : false;
    //}

    //static void DestroyMessageRow(ContentRow row) {
    //  Destroy(row.Row);
    //  Destroy(row.Divider);
    //}

    //static void ToggleChatPanelMessageDividers(bool toggle) {
    //  if (ChatMessageLayout.Value == MessageLayoutType.SingleRow) {
    //    return;
    //  }

    //  foreach (ContentRow row in MessageRows) {
    //    row.Divider.Ref()?.SetActive(toggle);
    //  }
    //}

    //static void ToggleContentRows(bool toggle, ChatMessageType messageType) {
    //  foreach (ContentRow row in MessageRows.Where(row => row.ChatMessage?.MessageType == messageType)) {
    //    row.Row.Ref()?.SetActive(toggle);
    //    row.Divider.Ref()?.SetActive(toggle && ShouldShowDivider());
    //  }
    //}

    //static void SetContentRowBodyTextColor(Color color, ChatMessageType messageType) {
    //  if (ChatMessageLayout.Value == MessageLayoutType.SingleRow) {
    //    RebuildMessageRows();
    //    return;
    //  }

    //  foreach (
    //      TMP_Text text in MessageRows
    //          .Where(row => row.ChatMessage?.MessageType == messageType && row.Row)
    //          .SelectMany(row => row.Row.GetComponentsInChildren<TMP_Text>(includeInactive: true))
    //          .Where(text => text.name == ChatPanel.ContentRowBodyName)) {
    //    text.color = color;
    //  }
    //}

    //static void ToggleShowTimestamp(bool toggle) {
    //  if (ChatMessageLayout.Value == MessageLayoutType.SingleRow) {
    //    RebuildMessageRows();
    //    return;
    //  }

    //  foreach (
    //      GameObject cell in MessageRows
    //          .Where(row => row.Row)
    //          .SelectMany(row => row.Row.GetComponentsInChildren<TMP_Text>(includeInactive: true))
    //          .Where(text => text.name == ChatPanel.HeaderRightCellName)
    //          .Select(text => text.gameObject)) {
    //    cell.SetActive(toggle);
    //  }
    //}

    //static void SetTimestampTextColor(Color color) {
    //  if (ChatMessageLayout.Value == MessageLayoutType.SingleRow) {
    //    RebuildMessageRows();
    //    return;
    //  }

    //  foreach (
    //      TMP_Text text in MessageRows
    //          .Where(row => row.Row)
    //          .SelectMany(row => row.Row.GetComponentsInChildren<TMP_Text>(includeInactive: true))
    //          .Where(text => text.name == ChatPanel.HeaderRightCellName)) {
    //    text.color = color;
    //  }
    //}

    //[HarmonyPatch(typeof(MessageHud))]
    //class MessageHudPatch {
    //  [HarmonyPostfix]
    //  [HarmonyPatch(nameof(MessageHud.ShowMessage))]
    //  static void ShowMessagePostfix(ref MessageHud.MessageType type, ref string text) {
    //    if (!IsModEnabled.Value
    //        || type != MessageHud.MessageType.Center
    //        || !ChatPanel?.Panel
    //        || !ShowMessageHudCenterMessages.Value) {
    //      return;
    //    }

    //    AddChatMessage(new() { MessageType = ChatMessageType.HudCenter, Timestamp = DateTime.Now, Text = text});
    //  }
    //}

    //public static void AddChatMessage(ChatMessage message) {
    //  if (ShouldShowMessage(message)) {
    //    MessageHistory.EnqueueItem(message);
    //  } else {
    //    return;
    //  }

    //  if (_chatPanel.Panel) {
    //    CreateChatMessageRow(message);
    //  }
    //}



    //static void CreateChatMessageRow(ChatMessage message) {
    //  if (ShouldCreateDivider(message)) {
    //    GameObject divider = _chatPanel.CreateMessageDivider(_chatPanel.Content.transform);
    //    GameObject row = _chatPanel.CreateChatMessageRow(_chatPanel.Content.transform);
    //    MessageRows.EnqueueItem(new(row, message, divider));

    //    bool active = IsMessageTypeActive(message.MessageType);
    //    divider.SetActive(active && ShouldShowDivider());
    //    row.SetActive(active);

    //    if (ShouldCreateRowHeader()) {
    //      (_, TMP_Text leftCell, TMP_Text rightCell) = 
    //          _chatPanel.CreateChatMessageRowHeader(
    //              row.transform, GetUsernameText(message.Username), GetTimestampText(message.Timestamp));

    //      leftCell.color = ChatMessageTextDefaultColor.Value;

    //      rightCell.color = ChatMessageTimestampColor.Value;
    //      rightCell.gameObject.SetActive(ChatMessageShowTimestamp.Value);
    //    }
    //  }

    //  TMP_Text text = _chatPanel.CreateChatMessageRowBody(MessageRows.LastItem.Row.transform, GetBodyText(message));
    //  text.color = GetMessageTextColor(message.MessageType);
    //}

    //static bool ShouldCreateDivider(ChatMessage message) {
    //  return MessageRows.IsEmpty
    //      || MessageRows.LastItem?.ChatMessage?.MessageType != message.MessageType
    //      || MessageRows.LastItem?.ChatMessage?.SenderId != message.SenderId
    //      || MessageRows.LastItem?.ChatMessage?.Username != message.Username;
    //}

    //static bool ShouldCreateRowHeader() {
    //  return ChatMessageLayout.Value == MessageLayoutType.WithHeaderRow;
    //}

    //static bool ShouldShowDivider() {
    //  return ShowChatPanelMessageDividers.Value && ChatMessageLayout.Value == MessageLayoutType.WithHeaderRow;
    //}

    //static bool IsMessageTypeActive(ChatMessageType messageType) {
    //  return messageType switch {
    //    ChatMessageType.Text => _chatPanel.TextToggle.isOn,
    //    ChatMessageType.HudCenter => _chatPanel.MessageHudToggle.isOn,
    //    ChatMessageType.Say => _chatPanel.SayToggle.isOn,
    //    ChatMessageType.Shout => _chatPanel.ShoutToggle.isOn,
    //    ChatMessageType.Whisper => _chatPanel.WhisperToggle.isOn,
    //    ChatMessageType.Ping => _chatPanel.PingToggle.isOn,
    //    _ => true,
    //  };
    //}
  }
}