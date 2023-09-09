using System;

using UnityEngine;

using static Chatter.PluginConfig;

namespace Chatter {
  public static class ChatMessageUtils {
    public static string GetChatMessageText(ChatMessage message) {
      return ChatMessageLayout.Value switch {
        MessageLayoutType.SingleRow =>
            string.Join(
                " ", GetTimestampText(message.Timestamp), GetUsernameText(message.Username), GetMessageText(message)),

        _ => GetMessageText(message)
      };
    }

    public static string GetUsernameText(string username) {
      if (username.Length == 0) {
        return string.Empty;
      }

      return ChatMessageLayout.Value switch {
        //MessageLayoutType.WithHeaderRow =>
        //    $"{ChatMessageUsernamePrefix.Value}{username}{ChatMessageUsernamePostfix.Value}",

        MessageLayoutType.SingleRow =>
            $"<color=#{ColorUtility.ToHtmlStringRGBA(ChatMessageTextDefaultColor.Value)}>[ {username} ]</color>",

        _ => username,
      };
    }

    public static string GetTimestampText(DateTime timestamp) {
      return ChatMessageLayout.Value switch {
        MessageLayoutType.SingleRow =>
            ChatMessageShowTimestamp.Value
                ? string.Format(
                        "<color=#{0}>{1:t}</color>",
                        ColorUtility.ToHtmlStringRGBA(ChatMessageTimestampColor.Value),
                        timestamp)
                : string.Empty,

        _ => timestamp.ToString("T"),
      };
    }

    public static string GetMessageText(ChatMessage message) {
      string text =
          message.MessageType switch {
            ChatMessageType.Ping => $"Ping! {message.Position}",
            _ => message.Text
          };

      return ChatMessageLayout.Value switch {
        MessageLayoutType.SingleRow =>
            string.Format(
                "<color=#{0}>{1}</color>",
                ColorUtility.ToHtmlStringRGBA(GetMessageTextColor(message.MessageType)),
                text),

        _ => text,
      };
    }

    public static Color GetMessageTextColor(ChatMessageType messageType) {
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

    public static bool ShouldShowMessage(ChatMessage message) {
      return message.MessageType switch {
        //ChatMessageType.Say => ShouldShowText(message.Text, SayTextFilterList.CachedValues),
        //ChatMessageType.Shout => ShouldShowText(message.Text, ShoutTextFilterList.CachedValues),
        //ChatMessageType.Whisper => ShouldShowText(message.Text, WhisperTextFilterList.CachedValues),
        //ChatMessageType.HudCenter => ShouldShowText(message.Text, HudCenterTextFilterList.CachedValues),
        //ChatMessageType.Text => ShouldShowText(message.Text, OtherTextFilterList.CachedValues),
        _ => true
      };
    }

    //static bool ShouldShowText(string text, List<string> filters) {
    //  if (filters.Count == 0) {
    //    return true;
    //  }

    //  return !filters.Any(f => text.IndexOf(f, 0, StringComparison.OrdinalIgnoreCase) != -1);
    //}
  }
}
