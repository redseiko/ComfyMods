using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

using static Chatter.Chatter;

namespace Chatter {
  public static class PluginConfig {
    public static ConfigFile Config { get; private set; }
    public static ConfigEntry<bool> IsModEnabled { get; private set; }

    // Panel
    public static ConfigEntry<Vector2> ChatPanelPosition { get; private set; }
    public static ConfigEntry<Vector2> ChatPanelSizeDelta { get; private set; }

    //  // Behaviour
    //  public static ConfigEntry<int> HideChatPanelDelay { get; private set; }
    //  public static ConfigEntry<float> HideChatPanelAlpha { get; private set; }

    //  // Content
    //  public static ConfigEntry<bool> ShowMessageHudCenterMessages { get; private set; }
    //  public static ConfigEntry<bool> ShowChatPanelMessageDividers { get; private set; }

    //  // Defaults
    //  public static ConfigEntry<Talker.Type> ChatPanelDefaultMessageTypeToUse { get; private set; }
    //  public static ConfigEntry<ChatMessageType> ChatPanelContentRowTogglesToEnable { get; private set; }

    //  // Filters
    //  public static StringListConfigEntry SayTextFilterList { get; private set; }
    //  public static StringListConfigEntry ShoutTextFilterList { get; private set; }
    //  public static StringListConfigEntry WhisperTextFilterList { get; private set; }
    //  public static StringListConfigEntry HudCenterTextFilterList { get; private set; }
    //  public static StringListConfigEntry OtherTextFilterList { get; private set; }

    public enum MessageLayoutType {
      WithHeaderRow,
      SingleRow
    }

    // Layout
    public static ConfigEntry<MessageLayoutType> ChatMessageLayout { get; private set; }
    public static ConfigEntry<bool> ChatMessageShowTimestamp { get; private set; }

    //  // Style
    //  public static ConfigEntry<string> ChatMessageFont { get; private set; }
    //  public static ConfigEntry<int> ChatMessageFontSize { get; private set; }

    //  public static ConfigEntry<Color> ChatPanelBackgroundColor { get; private set; }
    //  public static ConfigEntry<Vector2> ChatPanelRectMaskSoftness { get; private set; }

    //  // Spacing
    //  public static ConfigEntry<float> ChatPanelContentSpacing { get; private set; }
    //  public static ConfigEntry<float> ChatPanelContentBodySpacing { get; private set; }
    //  public static ConfigEntry<float> ChatPanelContentSingleRowSpacing { get; private set; }

    //  // Panel
    //  public static ConfigEntry<float> ChatContentWidthOffset { get; private set; }

    //  // Scrolling
    //  public static ConfigEntry<KeyboardShortcut> ScrollContentUpShortcut { get; private set; }
    //  public static ConfigEntry<KeyboardShortcut> ScrollContentDownShortcut { get; private set; }
    //  public static ConfigEntry<float> ScrollContentOffsetInterval { get; private set; }

    // Colors
    public static ConfigEntry<Color> ChatMessageTextDefaultColor { get; private set; }
    public static ConfigEntry<Color> ChatMessageTextSayColor { get; private set; }
    public static ConfigEntry<Color> ChatMessageTextShoutColor { get; private set; }
    public static ConfigEntry<Color> ChatMessageTextWhisperColor { get; private set; }
    public static ConfigEntry<Color> ChatMessageTextPingColor { get; private set; }
    public static ConfigEntry<Color> ChatMessageTextMessageHudColor { get; private set; }
    public static ConfigEntry<Color> ChatMessageTimestampColor { get; private set; }

    //  // Username
    //  public static ConfigEntry<string> ChatMessageUsernamePrefix { get; private set; }
    //  public static ConfigEntry<string> ChatMessageUsernamePostfix { get; private set; }

    public static void BindConfig(ConfigFile config) {
      Config = config;

      IsModEnabled = config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      ChatPanelPosition =
          Config.BindInOrder(
              "ChatPanel",
              "chatPanelPosition",
              new Vector2(-10f, 100f),
              "The Vector2 position of the ChatPanel.");

      ChatPanelSizeDelta =
          Config.BindInOrder(
              "ChatPanel",
              "chatPanelSizeDelta",
              new Vector2(400f, 400f),
              "The size (width, height) of the ChatPanel.");

      ChatPanelPosition.SettingChanged +=
          (_, _) => (ChatterChatPanel?.Panel.transform as RectTransform).anchoredPosition = ChatPanelPosition.Value;

      ChatPanelSizeDelta.SettingChanged +=
          (_, _) => (ChatterChatPanel?.Panel.transform as RectTransform).sizeDelta = ChatPanelSizeDelta.Value;

      //    // Behaviour
      //    HideChatPanelDelay ??=
      //        config.Bind(
      //            "Behaviour",
      //            "hideChatPanelDelay",
      //            defaultValue: 10,
      //            new ConfigDescription(
      //                "Delay (in seconds) before hiding the ChatPanel.", new AcceptableValueRange<int>(1, 180)));

      //    HideChatPanelAlpha ??=
      //        config.Bind(
      //            "Behaviour",
      //            "hideChatPanelAlpha",
      //            defaultValue: 0.2f,
      //            new ConfigDescription(
      //                "Color alpha (in %) for the ChatPanel when hidden.", new AcceptableValueRange<float>(0f, 1f)));

      //    BindFilters(config);

      //    // Content
      //    ShowMessageHudCenterMessages ??=
      //        config.BindInOrder(
      //            "Content",
      //            "showMessageHudCenterMessages",
      //            defaultValue: true,
      //            "Show messages from the MessageHud that display in the top-center (usually boss messages).");

      //    ShowChatPanelMessageDividers ??=
      //        config.BindInOrder(
      //            "Content",
      //            "showChatPanelMessageDividers",
      //            defaultValue: true,
      //            "Show the horizontal dividers between groups of messages.");

      //    // Defaults
      //    ChatPanelDefaultMessageTypeToUse ??=
      //        config.BindInOrder(
      //            "Defaults",
      //            "chatPanelDefaultMessageTypeToUse",
      //            defaultValue: Talker.Type.Normal,
      //            "ChatPanel input default message type to use on game start. Ping value is ignored.");

      //    ChatPanelContentRowTogglesToEnable ??=
      //        config.BindInOrder(
      //            "Defaults",
      //            "chatPanelContentRowTogglesToEnable",
      //            defaultValue:
      //                ChatMessageType.Say
      //                | ChatMessageType.Shout
      //                | ChatMessageType.Whisper
      //                | ChatMessageType.HudCenter
      //                | ChatMessageType.Text,
      //            "ChatPanel content row toggles to enable on game start.");

      // Layout
      ChatMessageLayout =
          config.BindInOrder(
              "ChatMessage.Layout",
              "chatMessageLayout",
              MessageLayoutType.WithHeaderRow,
              "Determines which layout to use when displaying a chat message.");

      ChatMessageShowTimestamp =
          config.BindInOrder(
              "ChatMessage.Layout",
              "chatMessageShowTimestamp",
              defaultValue: true,
              "Show a timestamp for each group of chat messages (except system/default).");

      //    // Style
      //    ChatPanelBackgroundColor ??=
      //        config.Bind(
      //            "Style",
      //            "chatPanelBackgroundColor",
      //            (Color) new Color32(0, 0, 0, 128),
      //            "The background color for the ChatPanel.");

      //    ChatPanelRectMaskSoftness ??=
      //        config.Bind(
      //            "Style", "chatPanelRectMaskSoftness", new Vector2(20f, 20f), "Softness of the ChatPanel's RectMask2D.");

      //    // Spacing
      //    ChatPanelContentSpacing =
      //        config.BindInOrder(
      //            "Spacing",
      //            "chatPanelContentSpacing",
      //            defaultValue: 10f,
      //            "Spacing (px) between `Content.Row` when using 'WithRowHeader` layout.",
      //            new AcceptableValueRange<float>(-100, 100));

      //    ChatPanelContentBodySpacing =
      //        config.BindInOrder(
      //            "Spacing",
      //            "chatPanelContentBodySpacing",
      //            defaultValue: 5f,
      //            "Spacing (px) between `Content.Row.Body` when using 'WithRowHeader' layout.",
      //            new AcceptableValueRange<float>(-100, 100));

      //    ChatPanelContentSingleRowSpacing =
      //        config.BindInOrder(
      //            "Spacing",
      //            "chatPanelContentSingleRowSpacing",
      //            defaultValue: 10f,
      //            "Spacing (in pixels) to use between rows when using 'SingleRow' layout.",
      //            new AcceptableValueRange<float>(-100, 100));

      //    // Username
      //    ChatMessageUsernamePrefix =
      //        config.BindInOrder(
      //            "Username",
      //            "chatMessageUsernamePrefix",
      //            defaultValue: string.Empty,
      //            "If non-empty, adds the text to the beginning of a ChatMesage username.");

      //    ChatMessageUsernamePostfix =
      //        config.BindInOrder(
      //            "Username",
      //            "chatMessageUsernamePostfix",
      //            defaultValue: string.Empty,
      //            "If non-empty, adds the text to the end of a ChatMessage username.");

      //    // Scrolling
      //    ScrollContentUpShortcut =
      //        config.BindInOrder(
      //            "Scrolling",
      //            "scrollContentUpShortcut",
      //            new KeyboardShortcut(KeyCode.PageUp),
      //            "Keyboard shortcut to scroll the ChatPanel content up.");

      //    ScrollContentDownShortcut =
      //        config.BindInOrder(
      //            "Scrolling",
      //            "scrollContentDownShortcut",
      //            new KeyboardShortcut(KeyCode.PageDown),
      //            "Keyboard shortcut to scroll the ChatPanel content down.");

      //    ScrollContentOffsetInterval =
      //        config.BindInOrder(
      //            "Scrolling",
      //            "scrollContentOffsetInterval",
      //            defaultValue: 200f,
      //            "Interval (in pixels) to scroll the ChatPanel content up/down.");

      // Colors
      ChatMessageTextDefaultColor =
          config.BindInOrder(
              "ChatMessage.Text.Colors",
              "chatMessageTextDefaultColor",
              Color.white,
              "Color for default/system chat messages.");

      ChatMessageTextSayColor =
          config.BindInOrder(
              "ChatMessage.Text.Colors",
              "chatMessageTextSayColor",
              Color.white,
              "Color for 'normal/say' chat messages.");

      ChatMessageTextShoutColor =
          config.BindInOrder(
              "ChatMessage.Text.Colors",
              "chatMessageTextShoutColor",
              Color.yellow,
              "Color for 'shouting' chat messages.");

      ChatMessageTextWhisperColor =
          config.BindInOrder(
              "ChatMessage.Text.Colors",
              "chatMessageTextWhisperColor",
              new Color(0.502f, 0f, 0.502f, 1f), // <color=purple> #800080
              "Color for 'whisper' chat messages.");

      ChatMessageTextPingColor =
          config.BindInOrder(
              "ChatMessage.Text.Colors",
              "chatMessageTextPingColor",
              Color.cyan,
              "Color for 'ping' chat messages.");

      ChatMessageTextMessageHudColor =
          config.BindInOrder(
              "ChatMessage.Text.Colors",
              "chatMessageTextMessageHudColor",
              new Color(1f, 0.647f, 0f, 1.0f), // <color=orange> #FFA500
              "Color for 'MessageHud' chat messages.");

      ChatMessageTimestampColor =
          config.BindInOrder(
              "ChatMessage.Text.Colors",
              "chatMessageTimestampColor",
              (Color) new Color32(244, 246, 247, 255),
              "Color for any timestamp shown in the chat messages.");

      //    config.LateBindInOrder(config => BindChatMessageFont(config));

      //    BindMessageToggleConfig(config);
    }

    //  static void BindFilters(ConfigFile config) {
    //    // Filters
    //    SayTextFilterList =
    //        config.BindInOrder("Filters", "sayTextFilterList", "Filter list for Say message texts.", "\t");

    //    ShoutTextFilterList =
    //        config.BindInOrder("Filters", "shoutTextFilterList", "Filter list for Shout message texts.", "\t");

    //    WhisperTextFilterList =
    //        config.BindInOrder("Filters", "whisperTextFilterList", "Filter list for Whipser message texts.", "\t");

    //    HudCenterTextFilterList =
    //        config.BindInOrder(
    //            "Filters", "messageHudTextFilterList", "Filter list for MessageHud.Center message texts.", "\t");

    //    OtherTextFilterList =
    //        config.BindInOrder("Filters", "otherHudTextFilterList", "Filter list for all other message texts.", "\t");
    //  }

    //  public static float ContentRowSpacing {
    //    get {
    //      return ChatMessageLayout.Value switch {
    //        Chatter.MessageLayoutType.SingleRow => ChatPanelContentSingleRowSpacing.Value,
    //        _ => ChatPanelContentSpacing.Value,
    //      };
    //    }
    //  }

    //  public static float ContentRowBodySpacing {
    //    get {
    //      return ChatMessageLayout.Value switch {
    //        Chatter.MessageLayoutType.SingleRow => ChatPanelContentSingleRowSpacing.Value,
    //        _ => ChatPanelContentBodySpacing.Value,
    //      };
    //    }
    //  }

    //  public static Font MessageFont {
    //    get => UIResources.GetFont(ChatMessageFont.Value);
    //  }

    //  public static void BindChatMessageFont(ConfigFile config) {
    //    string[] fontNames =
    //        Resources.FindObjectsOfTypeAll<Font>()
    //            .Select(f => f.name)
    //            .OrderBy(f => f)
    //            .Concat(UIResources.OsFontMap.Value.Keys.OrderBy(f => f))
    //            .ToArray();

    //    ChatMessageFont ??=
    //        config.BindInOrder(
    //            "Style",
    //            "chatMessageFont",
    //            UIResources.AveriaSerifLibre,
    //            "The font to use for chat messages.",
    //            new AcceptableValueList<string>(fontNames));

    //    ChatMessageFontSize ??=
    //        config.BindInOrder(
    //            "Style",
    //            "chatMessageFontSize",
    //            18,
    //            "The font size to use for chat messages.",
    //            new AcceptableValueRange<int>(8, 64));
    //  }

    //  public static ConfigEntry<float> MessageToggleTextFontSize { get; private set; }
    //  public static ConfigEntry<Color> MessageToggleTextColorEnabled { get; private set; }
    //  public static ConfigEntry<Color> MessageToggleTextColorDisabled { get; private set; }

    //  public static void BindMessageToggleConfig(ConfigFile config) {
    //    MessageToggleTextFontSize =
    //        config.BindInOrder(
    //            "Style.MessageToggle.Text",
    //            "textFontSize",
    //            14f,
    //            "Style - MessageToggle.Text - text font size.",
    //            new AcceptableValueRange<float>(2f, 25f));

    //    MessageToggleTextColorEnabled =
    //        config.BindInOrder(
    //            "Style.MessageToggle.Text",
    //            "textColorEnabled",
    //            Color.white,
    //            "Style - MessageToggle.Text - text color when toggle is enabled.");

    //    MessageToggleTextColorDisabled =
    //        config.BindInOrder(
    //            "Style.MessageToggle.Text",
    //            "textColorDisabled",
    //            new Color(0.75f, 0.75f, 0.75f, 1f),
    //            "Style - MessageToggle.Text - text color when toggle is disabled.");
    //  }

    //public static void BindChatPanelSize(RectTransform chatWindowRectTransform) {
    //  ChatPanelPosition =
    //      Config.BindInOrder(
    //          "ChatPanel",
    //          "chatPanelPosition",
    //          chatWindowRectTransform.anchoredPosition,
    //          "The Vector2 position of the ChatPanel.");

    //  ChatPanelSize =
    //      Config.BindInOrder(
    //          "ChatPanel",
    //          "chatPanelSize",
    //          chatWindowRectTransform.sizeDelta - new Vector2(10f, 0f),
    //          "The size (width, height) of the ChatPanel.");

    //  ChatContentWidthOffset ??=
    //      Config.Bind(
    //          "Panel",
    //          "chatContentWidthOffset",
    //          -50f,
    //          new ConfigDescription(
    //              "Offsets the width of a row in the ChatPanel content.",
    //              new AcceptableValueRange<float>(-400, 400)));
    //}
  }

  //internal sealed class ConfigurationManagerAttributes {
  //  public int? Order;
  //}
}