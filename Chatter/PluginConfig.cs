using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

using BepInEx.Configuration;

using ComfyLib;

using HarmonyLib;

using TMPro;

using UnityEngine;

using static Chatter.Chatter;

namespace Chatter {
  public static class PluginConfig {
    public static ConfigFile Config { get; private set; }
    public static ConfigEntry<bool> IsModEnabled { get; private set; }

    // Panel
    public static ConfigEntry<Vector2> ChatPanelPosition { get; private set; }
    public static ConfigEntry<Vector2> ChatPanelSizeDelta { get; private set; }
    public static ConfigEntry<Color> ChatPanelBackgroundColor { get; private set; }

    // Behaviour
    public static ConfigEntry<int> HideChatPanelDelay { get; private set; }
    public static ConfigEntry<float> HideChatPanelAlpha { get; private set; }

    // Scrolling
    public static ConfigEntry<KeyboardShortcut> ScrollContentUpShortcut { get; private set; }
    public static ConfigEntry<KeyboardShortcut> ScrollContentDownShortcut { get; private set; }
    public static ConfigEntry<float> ScrollContentOffsetInterval { get; private set; }

    // Content
    public static ConfigEntry<bool> ShowMessageHudCenterMessages { get; private set; }
    public static ConfigEntry<bool> ShowChatPanelMessageDividers { get; private set; }

    // Spacing
    public static ConfigEntry<float> ChatPanelContentSpacing { get; private set; }
    public static ConfigEntry<float> ChatPanelContentRowSpacing { get; private set; }
    public static ConfigEntry<float> ChatPanelContentSingleRowSpacing { get; private set; }

    // Defaults
    public static ConfigEntry<Talker.Type> ChatPanelDefaultMessageTypeToUse { get; private set; }
    public static ConfigEntry<ChatMessageType> ChatPanelContentRowTogglesToEnable { get; private set; }

    // Layout
    public static ConfigEntry<MessageLayoutType> ChatMessageLayout { get; private set; }
    public static ConfigEntry<bool> ChatMessageShowTimestamp { get; private set; }

    // Colors
    public static ConfigEntry<Color> ChatMessageTextDefaultColor { get; private set; }
    public static ConfigEntry<Color> ChatMessageTextSayColor { get; private set; }
    public static ConfigEntry<Color> ChatMessageTextShoutColor { get; private set; }
    public static ConfigEntry<Color> ChatMessageTextWhisperColor { get; private set; }
    public static ConfigEntry<Color> ChatMessageTextPingColor { get; private set; }
    public static ConfigEntry<Color> ChatMessageTextMessageHudColor { get; private set; }
    public static ConfigEntry<Color> ChatMessageUsernameColor { get; private set; }
    public static ConfigEntry<Color> ChatMessageTimestampColor { get; private set; }

    // Username
    public static ConfigEntry<string> ChatMessageUsernamePrefix { get; private set; }
    public static ConfigEntry<string> ChatMessageUsernamePostfix { get; private set; }

    public static void BindConfig(ConfigFile config) {
      Config = config;

      IsModEnabled = config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      IsModEnabled.OnSettingChanged(toggleOn => ToggleChatter(toggleOn));

      ChatPanelPosition =
          Config.BindInOrder(
              "ChatPanel",
              "chatPanelPosition",
              new Vector2(-10f, 125f),
              "The Vector2 position of the ChatPanel.");

      ChatPanelPosition.OnSettingChanged(position => ChatterChatPanel?.PanelRectTransform.SetPosition(position));

      ChatPanelSizeDelta =
          Config.BindInOrder(
              "ChatPanel",
              "chatPanelSizeDelta",
              new Vector2(500f, 500f),
              "The size (width, height) of the ChatPanel.");

      ChatPanelSizeDelta.OnSettingChanged(sizeDelta => ChatterChatPanel?.PanelRectTransform.SetSizeDelta(sizeDelta));

      ChatPanelBackgroundColor =
          config.BindInOrder(
              "ChatPanel",
              "chatPanelBackgroundColor",
              new Color(0f, 0f, 0f, 0.125f),
              "The background color for the ChatPanel.");

      ChatPanelBackgroundColor.OnSettingChanged(color => ChatterChatPanel?.PanelBackground.SetColor(color));

      // Behaviour
      HideChatPanelDelay =
          config.BindInOrder(
              "ChatPanel.Behaviour",
              "hideChatPanelDelay",
              defaultValue: 5,
              "Delay (in seconds) before hiding the ChatPanel.",
              new AcceptableValueRange<int>(1, 180));

      HideChatPanelAlpha =
          config.BindInOrder(
              "ChatPanel.Behaviour",
              "hideChatPanelAlpha",
              defaultValue: 0f,
              "Color alpha (in %) for the ChatPanel when hidden.",
              new AcceptableValueRange<float>(0f, 1f));

      // Scrolling
      ScrollContentUpShortcut =
          config.BindInOrder(
              "ChatPanel.Scrolling",
              "scrollContentUpShortcut",
              new KeyboardShortcut(KeyCode.PageUp),
              "Keyboard shortcut to scroll the ChatPanel content up.");

      ScrollContentDownShortcut =
          config.BindInOrder(
              "ChatPanel.Scrolling",
              "scrollContentDownShortcut",
              new KeyboardShortcut(KeyCode.PageDown),
              "Keyboard shortcut to scroll the ChatPanel content down.");

      ScrollContentOffsetInterval =
          config.BindInOrder(
              "ChatPanel.Scrolling",
              "scrollContentOffsetInterval",
              defaultValue: 200f,
              "Interval (in pixels) to scroll the ChatPanel content up/down.",
              new AcceptableValueRange<float>(-1000f, 1000f));

      // Content
      ShowMessageHudCenterMessages =
          config.BindInOrder(
              "ChatPanel.Content",
              "showMessageHudCenterMessages",
              defaultValue: true,
              "Show messages from the MessageHud that display in the top-center (usually boss messages).");

      ShowChatPanelMessageDividers =
          config.BindInOrder(
              "ChatPanel.Content",
              "showChatPanelMessageDividers",
              defaultValue: true,
              "Show the horizontal dividers between groups of messages.");

      ShowChatPanelMessageDividers.OnSettingChanged(toggleOn => ContentRowManager.ToggleMessageDividers(toggleOn));

      // Spacing
      ChatPanelContentSpacing =
          config.BindInOrder(
              "ChatPanel.Spacing",
              "chatPanelContentSpacing",
              defaultValue: 10f,
              "Spacing (px) between `Content.Row` when using 'WithRowHeader` layout.",
              new AcceptableValueRange<float>(-100, 100));

      ChatPanelContentSpacing.OnSettingChanged(() => ChatterChatPanel?.SetContentSpacing());

      ChatPanelContentRowSpacing =
          config.BindInOrder(
              "ChatPanel.Spacing",
              "chatPanelContentRowSpacing",
              defaultValue: 2f,
              "Spacing (px) between `Content.Row.Body` when using 'WithRowHeader' layout.",
              new AcceptableValueRange<float>(-100, 100));

      ChatPanelContentRowSpacing.OnSettingChanged(spacing => ContentRowManager.SetContentRowSpacing(spacing));

      ChatPanelContentSingleRowSpacing =
          config.BindInOrder(
              "ChatPanel.Spacing",
              "chatPanelContentSingleRowSpacing",
              defaultValue: 10f,
              "Spacing (in pixels) to use between rows when using 'SingleRow' layout.",
              new AcceptableValueRange<float>(-100, 100));

      ChatPanelContentSingleRowSpacing.OnSettingChanged(() => ChatterChatPanel?.SetContentSpacing());

    // Defaults
    ChatPanelDefaultMessageTypeToUse =
          config.BindInOrder(
              "ChatPanel.Defaults",
              "chatPanelDefaultMessageTypeToUse",
              defaultValue: Talker.Type.Normal,
              "ChatPanel input default message type to use on game start. Ping value is ignored.");

      ChatPanelContentRowTogglesToEnable =
          config.BindInOrder(
              "ChatPanel.Defaults",
              "chatPanelContentRowTogglesToEnable",
              defaultValue:
                  ChatMessageType.Say
                  | ChatMessageType.Shout
                  | ChatMessageType.Whisper
                  | ChatMessageType.HudCenter
                  | ChatMessageType.Text,
              "ChatPanel content row toggles to enable on game start.");

      // Layout
      ChatMessageLayout =
          config.BindInOrder(
              "ChatMessage.Layout",
              "chatMessageLayout",
              MessageLayoutType.WithHeaderRow,
              "Determines which layout to use when displaying a chat message.");

      ChatMessageLayout.OnSettingChanged(() => ContentRowManager.RebuildContentRows());

      ChatMessageShowTimestamp =
          config.BindInOrder(
              "ChatMessage.Layout",
              "chatMessageShowTimestamp",
              defaultValue: true,
              "Show a timestamp for each group of chat messages (except system/default).");

      ChatMessageShowTimestamp.OnSettingChanged(toggleOn => ContentRowManager.ToggleShowTimestamp(toggleOn));

      // Colors
      ChatMessageTextDefaultColor =
          config.BindInOrder(
              "ChatMessage.Text.Colors",
              "chatMessageTextDefaultColor",
              Color.white,
              "Color for default/system chat messages.");

      ChatMessageTextDefaultColor.OnSettingChanged(
          color => ContentRowManager.SetMessageTextColor(color, ChatMessageType.Text));

      ChatMessageTextSayColor =
          config.BindInOrder(
              "ChatMessage.Text.Colors",
              "chatMessageTextSayColor",
              Color.white,
              "Color for 'normal/say' chat messages.");

      ChatMessageTextSayColor.OnSettingChanged(
          color => ContentRowManager.SetMessageTextColor(color, ChatMessageType.Say));

      ChatMessageTextShoutColor =
          config.BindInOrder(
              "ChatMessage.Text.Colors",
              "chatMessageTextShoutColor",
              Color.yellow,
              "Color for 'shouting' chat messages.");

      ChatMessageTextShoutColor.OnSettingChanged(
          color => ContentRowManager.SetMessageTextColor(color, ChatMessageType.Shout));

      ChatMessageTextWhisperColor =
          config.BindInOrder(
              "ChatMessage.Text.Colors",
              "chatMessageTextWhisperColor",
              new Color(0.502f, 0f, 0.502f, 1f), // <color=purple> #800080
              "Color for 'whisper' chat messages.");

      ChatMessageTextWhisperColor.OnSettingChanged(
          color => ContentRowManager.SetMessageTextColor(color, ChatMessageType.Whisper));

      ChatMessageTextPingColor =
          config.BindInOrder(
              "ChatMessage.Text.Colors",
              "chatMessageTextPingColor",
              Color.cyan,
              "Color for 'ping' chat messages.");

      ChatMessageTextPingColor.OnSettingChanged(
          color => ContentRowManager.SetMessageTextColor(color, ChatMessageType.Ping));

      ChatMessageTextMessageHudColor =
          config.BindInOrder(
              "ChatMessage.Text.Colors",
              "chatMessageTextMessageHudColor",
              new Color(1f, 0.807f, 0f, 1.0f),
              "Color for 'MessageHud' chat messages.");

      ChatMessageTextMessageHudColor.OnSettingChanged(
          color => ContentRowManager.SetMessageTextColor(color, ChatMessageType.HudCenter));

      ChatMessageUsernameColor =
          config.BindInOrder(
              "ChatMessage.Text.Colors",
              "chatMessageUsernameColor",
              new Color(1f, 0.647f, 0f), // <color=orange> #FFA500
              "Color for the username shown in chat messages.");

      ChatMessageUsernameColor.OnSettingChanged(color => ContentRowManager.SetUsernameTextColor(color));

      ChatMessageTimestampColor =
          config.BindInOrder(
              "ChatMessage.Text.Colors",
              "chatMessageTimestampColor",
              (Color) new Color32(244, 246, 247, 255),
              "Color for any timestamp shown in the chat messages.");

      ChatMessageTimestampColor.OnSettingChanged(color => ContentRowManager.SetTimestampTextColor(color));

      // Username
      ChatMessageUsernamePrefix =
          config.BindInOrder(
              "ChatMessage.WithHeaderRow",
              "chatMessageUsernamePrefix",
              defaultValue: string.Empty,
              "If non-empty, adds the text to the beginning of a ChatMesage username in 'WithHeaderRow' mode.");

      ChatMessageUsernamePostfix =
          config.BindInOrder(
              "ChatMessage.WithHeaderRow",
              "chatMessageUsernamePostfix",
              defaultValue: string.Empty,
              "If non-empty, adds the text to the end of a ChatMessage username in 'WithHeaderRow' mode.");

      BindFilters(config);

      LateBindConfig(BindChatMessageFontConfig);
    }

    // Filters
    public static StringListConfigEntry SayTextFilterList { get; private set; }
    public static StringListConfigEntry ShoutTextFilterList { get; private set; }
    public static StringListConfigEntry WhisperTextFilterList { get; private set; }
    public static StringListConfigEntry HudCenterTextFilterList { get; private set; }
    public static StringListConfigEntry OtherTextFilterList { get; private set; }

    static void BindFilters(ConfigFile config) {
      // Filters
      SayTextFilterList =
          config.BindInOrder("Filters", "sayTextFilterList", "Filter list for Say message texts.", "\t");

      ShoutTextFilterList =
          config.BindInOrder("Filters", "shoutTextFilterList", "Filter list for Shout message texts.", "\t");

      WhisperTextFilterList =
          config.BindInOrder("Filters", "whisperTextFilterList", "Filter list for Whipser message texts.", "\t");

      HudCenterTextFilterList =
          config.BindInOrder(
              "Filters", "messageHudTextFilterList", "Filter list for MessageHud.Center message texts.", "\t");

      OtherTextFilterList =
          config.BindInOrder("Filters", "otherHudTextFilterList", "Filter list for all other message texts.", "\t");
    }

    // Fonts
    public static ConfigEntry<string> ChatMessageFontAsset { get; private set; }
    public static ConfigEntry<float> ChatMessageFontSize { get; private set; }

    public static void BindChatMessageFontConfig(ConfigFile config) {
      string[] fontNames =
          Resources.FindObjectsOfTypeAll<TMP_FontAsset>()
              .Select(f => f.name)
              .OrderBy(f => f)
              .Distinct()
              .ToArray();

      ChatMessageFontAsset =
          config.BindInOrder(
              "ChatMessage.Text.Font",
              "chatMessageTextFontAsset",
              "Valheim-AveriaSansLibre",
              "FontAsset (TMP) to use for ChatMessage text.",
              new AcceptableValueList<string>(fontNames));

      ChatMessageFontAsset.OnSettingChanged(
          fontName => ChatterChatPanel?.SetContentFontAsset(UIResources.GetFontAssetByName(fontName)));

      ChatMessageFontSize =
          config.BindInOrder(
              "ChatMessage.Text.Font",
              "chatMessageTextFontSize",
              16f,
              "The font size to use for chat messages.",
              new AcceptableValueRange<float>(6f, 64f));

      ChatMessageFontSize.OnSettingChanged(fontSize => ChatterChatPanel?.SetContentFontSize(fontSize));
    }

    public static void LateBindConfig(Action<ConfigFile> lateBindConfigAction) {
      _fejdStartupBindConfigQueue.Enqueue(lateBindConfigAction);
    }

    static readonly Queue<Action<ConfigFile>> _fejdStartupBindConfigQueue = new();

    [HarmonyPatch(typeof(FejdStartup))]
    static class FejdStartupPatch {
      [HarmonyPostfix]
      [HarmonyPatch(nameof(FejdStartup.Awake))]
      static void AwakePostfix() {
        while (_fejdStartupBindConfigQueue.Count > 0) {
          _fejdStartupBindConfigQueue.Dequeue()?.Invoke(Config);
        }
      }
    }
  }
}