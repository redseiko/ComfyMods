using System.Reflection;

using BepInEx;

using ComfyLib;

using HarmonyLib;

using UnityEngine;

using static Chatter.PluginConfig;

namespace Chatter {
  [BepInPlugin(PluginGuid, PluginName, PluginVersion)]
  public class Chatter : BaseUnityPlugin {
    public const string PluginGuid = "redseiko.valheim.chatter";
    public const string PluginName = "Chatter";
    public const string PluginVersion = "1.5.0";

    Harmony _harmony;

    void Awake() {
      BindConfig(Config);

      _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGuid);
    }

    void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    public static bool IsChatMessageQueued { get; set; }

    public static ChatPanel ChatterChatPanel { get; private set; }

    public static void ToggleChatter(Chat chat, bool toggleOn) {
      //  ToggleVanillaChat(Chat.m_instance, !toggle);
      ToggleChatPanel(chat, toggleOn);
      //  TerminalCommands.ToggleCommands(toggle);

      //  if (Chat.m_instance) {
      //    Chat.m_instance.m_input = toggle && ChatPanel?.Panel ? ChatPanel.InputField : _chatInputField;
      //  }
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

        ChatterChatPanel.TextInput.InputField.onSubmit.AddListener(OnChatTextInput);
      }

      ChatterChatPanel.Panel.SetActive(toggleOn);
    }

    public static void AddChatMessage(ChatMessage message) {
      if (!ChatterChatPanel?.Panel) {
        return;
      }

      MessageCell contentMessage = ChatterChatPanel.CreateContentMessage();
      contentMessage.Label.text = ChatMessageUtils.GetChatMessageText(message);
    }

    public static void OnChatTextInput(string input) {
      Chat.m_instance.m_input.text = ChatterChatPanel.TextInput.InputField.text;
      Chat.m_instance.SendInput();
      ChatterChatPanel.TextInput.InputField.text = string.Empty;
    }

    //internal static readonly CircularQueue<ChatMessage> MessageHistory = new(50, _ => { });
    //internal static readonly CircularQueue<ContentRow> MessageRows = new(50, DestroyMessageRow);

    //internal static bool _isPluginConfigBound = false;

    //internal static ChatPanel _chatPanel = null;
    //internal static InputField _chatInputField = null;

    //internal static bool _isCreatingChatMessage = false;
    //static bool _isChatPanelVisible = false;

    //public static string ChatInputTextDefaultPrefix { get; private set; } = "say ";

    //public static void SetChatInputTextDefaultPrefix(Talker.Type talkerType) {
    //  ChatInputTextDefaultPrefix =
    //      talkerType switch {
    //        Talker.Type.Shout => "s ",
    //        Talker.Type.Whisper => "w ",
    //        _ => "say ",
    //      };

    //  string text =
    //      talkerType switch {
    //        Talker.Type.Shout => "/shout",
    //        Talker.Type.Whisper => "/whisper",
    //        _ => "/say ",
    //      };

    //  Color color =
    //      talkerType switch {
    //        Talker.Type.Shout => ChatMessageTextShoutColor.Value,
    //        Talker.Type.Whisper => ChatMessageTextWhisperColor.Value,
    //        _ => ChatMessageTextSayColor.Value
    //      };

    //  ChatPanel?.InputField.textComponent.SetColor(color);
    //  ChatPanel?.InputField.placeholder.GetComponent<Text>()
    //      .SetText(text)
    //      .SetColor(color.SetAlpha(0.3f));
    //}



    //static void ToggleChatPanel(Chat chat, bool toggle) {
    //  if (_chatPanel == null || !_chatPanel.Panel) {
    //    _chatPanel = CreateChatPanel(chat);
    //  }

    //  ChatPanel?.Panel.SetActive(toggle);
    //  _isChatPanelVisible = toggle;

    //  if (toggle) {
    //    ChatPanel?.SetPanelPosition(ChatPanelPosition.Value);
    //    ChatPanel?.SetPanelSize(ChatPanelSize.Value);
    //    ChatPanel?.SetContentWidthOffset(ChatContentWidthOffset.Value);

    //    SetChatInputTextDefaultPrefix(ChatPanelDefaultMessageTypeToUse.Value);
    //  }
    //}

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

    //static void ToggleVanillaChat(Chat chat, bool toggle) {
    //  if (chat) {
    //    chat.m_chatWindow.GetComponent<RectMask2D>().enabled = toggle;

    //    foreach (Image image in chat.m_chatWindow.GetComponentsInChildren<Image>(includeInactive: true)) {
    //      image.gameObject.SetActive(toggle);
    //    }

    //    chat.m_output.gameObject.SetActive(toggle);
    //  }
    //}

    //internal static void HideChatPanelDelegate(float hideTimer) {
    //  if (IsModEnabled.Value && _chatPanel?.Panel) {
    //    bool isVisible = (hideTimer < HideChatPanelDelay.Value || Menu.IsVisible()) && !Hud.IsUserHidden();

    //    if (isVisible == _isChatPanelVisible && _chatPanel.CanvasGroup.isActiveAndEnabled) {
    //      return;
    //    }

    //    _isChatPanelVisible = isVisible;

    //    if (_isChatPanelVisible) {
    //      _chatPanel.CanvasGroup.alpha = 1f;
    //      _chatPanel.CanvasGroup.blocksRaycasts = true;
    //    } else {
    //      _chatPanel.CanvasGroup.alpha = Hud.IsUserHidden() ? 0f : HideChatPanelAlpha.Value;
    //      _chatPanel.CanvasGroup.blocksRaycasts = false;
    //      _chatPanel.SetVerticalScrollPosition(0f);
    //    }
    //  }
    //}

    //public static void EnableChatPanelDelegate() {
    //  if (IsModEnabled.Value && _chatPanel?.InputField) {
    //    _chatPanel.InputField.enabled = true;
    //  }
    //}

    //public static bool DisableChatPanelDelegate(bool active) {
    //  if (IsModEnabled.Value && _chatPanel?.InputField) {
    //    _chatPanel.InputField.enabled = false;
    //    return true;
    //  }

    //  return active;
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

    //static void RebuildMessageRows() {
    //  MessageRows.ClearItems();

    //  if (!_chatPanel?.Panel) {
    //    return;
    //  }

    //  foreach (ChatMessage message in MessageHistory) {
    //    CreateChatMessageRow(message);
    //  }
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

    //[HarmonyPatch(typeof(Menu))]
    //class MenuPatch {

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