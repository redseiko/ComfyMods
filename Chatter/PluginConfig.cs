using BepInEx.Configuration;

using System.Collections.Generic;
using System.Linq;

using UnityEngine;

namespace Chatter {
  public class PluginConfig {
    public static ConfigFile Config { get; private set; } = default!;

    public static ConfigEntry<bool> IsModEnabled { get; private set; } = default!;

    // Behaviour
    public static ConfigEntry<int> HideChatPanelDelay { get; private set; } = default!;
    public static ConfigEntry<float> HideChatPanelAlpha { get; private set; } = default!;

    // Content
    public static ConfigEntry<bool> ShowMessageHudCenterMessages { get; private set; } = default!;
    public static ConfigEntry<bool> ShowChatPanelMessageDividers { get; private set; } = default!;

    // Layout
    public static ConfigEntry<Chatter.MessageLayoutType> ChatMessageLayout { get; private set; } = default!;
    public static ConfigEntry<bool> ChatMessageShowTimestamp { get; private set; } = default;

    // Style
    public static ConfigEntry<string> ChatMessageFont { get; private set; } = default!;
    public static ConfigEntry<int> ChatMessageFontSize { get; private set; } = default!;

    public static ConfigEntry<Color> ChatPanelBackgroundColor { get; private set; } = default!;
    public static ConfigEntry<Vector2> ChatPanelRectMaskSoftness { get; private set; } = default!;

    // Spacing
    public static ConfigEntry<float> ChatPanelContentSpacing { get; private set; } = default!;
    public static ConfigEntry<float> ChatPanelContentBodySpacing { get; private set; } = default!;
    public static ConfigEntry<float> ChatPanelContentSingleRowSpacing { get; private set; } = default!;

    // Panel
    public static ConfigEntry<Vector2> ChatPanelPosition { get; private set; } = default!;
    public static ConfigEntry<Vector2> ChatPanelSize { get; private set; } = default!;
    public static ConfigEntry<float> ChatContentWidthOffset { get; private set; } = default!;

    // Scrolling
    public static ConfigEntry<KeyboardShortcut> ScrollContentUpShortcut { get; private set; } = default!;
    public static ConfigEntry<KeyboardShortcut> ScrollContentDownShortcut { get; private set; } = default!;
    public static ConfigEntry<float> ScrollContentOffsetInterval { get; private set; } = default!;

    // Colors
    public static ConfigEntry<Color> ChatMessageTextDefaultColor { get; private set; } = default!;
    public static ConfigEntry<Color> ChatMessageTextSayColor { get; private set; } = default!;
    public static ConfigEntry<Color> ChatMessageTextShoutColor { get; private set; } = default!;
    public static ConfigEntry<Color> ChatMessageTextWhisperColor { get; private set; } = default!;
    public static ConfigEntry<Color> ChatMessageTextPingColor { get; private set; } = default!;
    public static ConfigEntry<Color> ChatMessageTextMessageHudColor { get; private set; } = default!;
    public static ConfigEntry<Color> ChatMessageTimestampColor { get; private set; } = default!;

    // Username
    public static ConfigEntry<string> ChatMessageUsernamePrefix { get; private set; } = default!;
    public static ConfigEntry<string> ChatMessageUsernamePostfix { get; private set; } = default!;
    public static ConfigEntry<string> DisallowChatMessageHudStartTexts { get; private set; } = default!;

    public static void BindConfig(ConfigFile config) {
      Config = config;
      IsModEnabled = config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      // Behaviour
      HideChatPanelDelay =
          config.Bind(
              "Behaviour",
              "hideChatPanelDelay",
              defaultValue: 10,
              new ConfigDescription(
                  "Delay (in seconds) before hiding the ChatPanel.", new AcceptableValueRange<int>(1, 180)));

      HideChatPanelAlpha =
          config.Bind(
              "Behaviour",
              "hideChatPanelAlpha",
              defaultValue: 0.2f,
              new ConfigDescription(
                  "Color alpha (in %) for the ChatPanel when hidden.", new AcceptableValueRange<float>(0f, 1f)));

      // Content
      ShowMessageHudCenterMessages =
          config.Bind(
              "Content",
              "showMessageHudCenterMessages",
              defaultValue: true,
              "Show messages from the MessageHud that display in the top-center (usually boss messages).");

      ShowChatPanelMessageDividers =
          config.Bind(
              "Content",
              "showChatPanelMessageDividers",
              defaultValue: true,
              "Show the horizontal dividers between groups of messages.");

      // Layout
      ChatMessageLayout =
          config.Bind(
              "Layout",
              "chatMessageLayout",
              Chatter.MessageLayoutType.WithHeaderRow,
              "Determines which layout to use when displaying a chat message.");

      ChatMessageShowTimestamp =
          config.Bind(
              "Layout",
              "chatMessageShowTimestamp",
              defaultValue: true,
              "Show a timestamp for each group of chat messages (except system/default).");

      // Style
      ChatPanelBackgroundColor =
          config.Bind(
              "Style",
              "chatPanelBackgroundColor",
              (Color) new Color32(0, 0, 0, 128),
              "The background color for the ChatPanel.");

      ChatPanelRectMaskSoftness =
          config.Bind(
              "Style", "chatPanelRectMaskSoftness", new Vector2(20f, 20f), "Softness of the ChatPanel's RectMask2D.");

      // Spacing
      ChatPanelContentSpacing =
          config.Bind(
              "Spacing",
              "chatPanelContentSpacing",
              10f,
              new ConfigDescription(
                  "Spacing (px) between `Content.Row` when using 'WithRowHeader` layout.",
                  new AcceptableValueRange<float>(-100, 100),
                  new ConfigurationManagerAttributes { Order = 3 }));

      ChatPanelContentBodySpacing =
          config.Bind(
              "Spacing",
              "chatPanelContentBodySpacing",
              5f,
              new ConfigDescription(
                  "Spacing (px) between `Content.Row.Body` when using 'WithRowHeader' layout.",
                  new AcceptableValueRange<float>(-100, 100),
                  new ConfigurationManagerAttributes { Order = 2 }));

      ChatPanelContentSingleRowSpacing =
          config.Bind(
              "Spacing",
              "chatPanelContentSingleRowSpacing",
              10f,
              new ConfigDescription(
                  "Spacing (in pixels) to use between rows when using 'SingleRow' layout.",
                  new AcceptableValueRange<float>(-100, 100),
                  new ConfigurationManagerAttributes { Order = 1 }));

      // Username
      ChatMessageUsernamePrefix =
          config.Bind(
              "Username",
              "chatMessageUsernamePrefix",
              defaultValue: string.Empty,
              new ConfigDescription(
                  "If non-empty, adds the text to the beginning of a ChatMesage username.",
                  acceptableValues: null,
                  new ConfigurationManagerAttributes { Order = 2 }));

      ChatMessageUsernamePostfix =
          config.Bind(
              "Username",
              "chatMessageUsernamePostfix",
              defaultValue: string.Empty,
              new ConfigDescription(
                  "If non-empty, adds the text to the end of a ChatMessage username.",
                  acceptableValues: null,
                  new ConfigurationManagerAttributes { Order = 1 }));

      DisallowChatMessageHudStartTexts =
          config.Bind(
              "Username",
              "disallowChatMessageHudStartTexts",
              defaultValue: string.Empty,
              new ConfigDescription(
                  "Start Text for HUD Messages that won\'t be added to the message queue. Separated by \',\'",
                  acceptableValues: null,
                  new ConfigurationManagerAttributes { Order = 1 }));

      // Scrolling
      ScrollContentUpShortcut =
          config.Bind(
              "Scrolling",
              "scrollContentUpShortcut",
              new KeyboardShortcut(KeyCode.PageUp),
              new ConfigDescription(
                  "Keyboard shortcut to scroll the ChatPanel content up.",
                  acceptableValues: null,
                  new ConfigurationManagerAttributes { Order = 3 }));

      ScrollContentDownShortcut =
          config.Bind(
              "Scrolling",
              "scrollContentDownShortcut",
              new KeyboardShortcut(KeyCode.PageDown),
              new ConfigDescription(
                  "Keyboard shortcut to scroll the ChatPanel content down.",
                  acceptableValues: null,
                  new ConfigurationManagerAttributes { Order = 2 }));

      ScrollContentOffsetInterval =
          config.Bind(
              "Scrolling",
              "scrollContentOffsetInterval",
              defaultValue: 200f,
              new ConfigDescription(
                  "Interval (in pixels) to scroll the ChatPanel content up/down.",
                  acceptableValues: null,
                  new ConfigurationManagerAttributes { Order = 1 }));

      // Colors
      ChatMessageTextDefaultColor =
          config.Bind(
              "Colors",
              "chatMessageTextDefaultColor",
              Color.white,
              new ConfigDescription(
                  "Color for default/system chat messages.",
                  acceptableValues: null,
                  new ConfigurationManagerAttributes { Order = 6 }));

      ChatMessageTextSayColor =
          config.Bind(
              "Colors",
              "chatMessageTextSayColor",
              Color.white,
              new ConfigDescription(
                  "Color for 'normal/say' chat messages.",
                  acceptableValues: null,
                  new ConfigurationManagerAttributes { Order = 5 }));

      ChatMessageTextShoutColor =
          config.Bind(
              "Colors",
              "chatMessageTextShoutColor",
              Color.yellow,
              new ConfigDescription(
                  "Color for 'shouting' chat messages.",
                  acceptableValues: null,
                  new ConfigurationManagerAttributes { Order = 4 }));

      ChatMessageTextWhisperColor =
          config.Bind(
              "Colors",
              "chatMessageTextWhisperColor",
              new Color(0.502f, 0f, 0.502f, 1f), // <color=purple> #800080
              new ConfigDescription(
                  "Color for 'whisper' chat messages.",
                  acceptableValues: null,
                  new ConfigurationManagerAttributes { Order = 3 }));

      ChatMessageTextPingColor =
          config.Bind(
              "Colors",
              "chatMessageTextPingColor",
              Color.cyan,
              new ConfigDescription(
                  "Color for 'ping' chat messages.",
                  acceptableValues: null,
                  new ConfigurationManagerAttributes { Order = 2 }));

      ChatMessageTextMessageHudColor =
          config.Bind(
              "Colors",
              "chatMessageTextMessageHudColor",
              new Color(1f, 0.647f, 0f, 1.0f), // <color=orange> #FFA500
              new ConfigDescription(
                  "Color for 'MessageHud' chat messages.",
                  acceptableValues: null,
                  new ConfigurationManagerAttributes { Order = 1 }));

      ChatMessageTimestampColor =
          config.Bind(
              "Colors",
              "chatMessageTimestampColor",
              (Color) new Color32(244, 246, 247, 255),
              new ConfigDescription(
                  "Color for any timestamp shown in the chat messages.",
                  acceptableValues: null,
                  new ConfigurationManagerAttributes { Order = 0 }));
    }

    public static float ContentRowSpacing {
      get {
        return ChatMessageLayout.Value switch {
          Chatter.MessageLayoutType.SingleRow => ChatPanelContentSingleRowSpacing.Value,
          _ => ChatPanelContentSpacing.Value,
        };
      }
    }

    public static float ContentRowBodySpacing {
      get {
        return ChatMessageLayout.Value switch {
          Chatter.MessageLayoutType.SingleRow => ChatPanelContentSingleRowSpacing.Value,
          _ => ChatPanelContentBodySpacing.Value,
        };
      }
    }

    static readonly Dictionary<string, Font> _fontCache = new();

    public static Font MessageFont {
      get {
        if (!_fontCache.TryGetValue(ChatMessageFont.Value, out Font font)) {
          font = Font.CreateDynamicFontFromOSFont(ChatMessageFont.Value, ChatMessageFontSize.Value);
          _fontCache[font.name] = font;
        }

        return font;
      }
    }

    public static void BindChatMessageFont(Font defaultFont) {
      foreach (Font font in Resources.FindObjectsOfTypeAll<Font>()) {
        _fontCache[font.name] = font;
      }

      string[] fontNames =
          _fontCache.Keys.OrderBy(f => f).Concat(Font.GetOSInstalledFontNames().OrderBy(f => f)).ToArray();

      ChatMessageFont =
          Config.Bind(
              "Style",
              "chatMessageFont",
              defaultFont.name,
              new ConfigDescription("The font to use for chat messages.", new AcceptableValueList<string>(fontNames)));

      ChatMessageFontSize =
          Config.Bind(
              "Style",
              "chatMessageFontSize",
              defaultFont.fontSize,
              new ConfigDescription("The font size to use for chat messages.", new AcceptableValueRange<int>(8, 64)));
    }

    public static void BindChatPanelSize(RectTransform chatWindowRectTransform) {
      ChatPanelPosition =
          Config.Bind(
              "Panel",
              "chatPanelPosition",
              chatWindowRectTransform.anchoredPosition,
              "The Vector2 position of the ChatPanel.");

      ChatPanelSize =
          Config.Bind(
              "Panel",
              "chatPanelSize",
              chatWindowRectTransform.sizeDelta - new Vector2(10f, 0f),
              "The size (width, height) of the ChatPanel.");

      ChatContentWidthOffset =
          Config.Bind(
              "Panel",
              "chatContentWidthOffset",
              -50f,
              new ConfigDescription(
                  "Offsets the width of a row in the ChatPanel content.",
                  new AcceptableValueRange<float>(-400, 400)));
    }
  }

  internal sealed class ConfigurationManagerAttributes {
    public int? Order;
  }
}