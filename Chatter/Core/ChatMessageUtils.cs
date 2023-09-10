using System;
using System.Linq;

using UnityEngine;

using static Chatter.PluginConfig;

using static UnityEngine.ColorUtility;

namespace Chatter {
  public static class ChatMessageUtils {
    public static ChatMessageType GetChatMessageType(Talker.Type talkerType) {
      return talkerType switch {
        Talker.Type.Normal => ChatMessageType.Say,
        Talker.Type.Shout => ChatMessageType.Shout,
        Talker.Type.Whisper => ChatMessageType.Whisper,
        Talker.Type.Ping => ChatMessageType.Ping,
        _ => ChatMessageType.Text
      };
    }

    public static string GetChatMessageText(ChatMessage message) {
      return ChatMessageLayout.Value switch {
        MessageLayoutType.SingleRow =>
            JoinIgnoringEmpty(
                GetTimestampText(message.Timestamp), GetUsernameText(message.Username), GetMessageText(message)),

        _ => GetMessageText(message)
      };
    }

    static string JoinIgnoringEmpty(params string[] values) {
      return string.Join(" ", values.Where(value => !string.IsNullOrEmpty(value)));
    }

    public static string GetUsernameText(string username) {
      if (username.Length == 0) {
        return string.Empty;
      }

      return ChatMessageLayout.Value switch {
        MessageLayoutType.WithHeaderRow =>
            $"{ChatMessageUsernamePrefix.Value}{username}{ChatMessageUsernamePostfix.Value}",

        MessageLayoutType.SingleRow =>
            $"[ <color=#{ToHtmlStringRGBA(ChatMessageUsernameColor.Value)}>{username}</color> ]",

        _ => username,
      };
    }

    public static string GetTimestampText(DateTime timestamp) {
      if (!ChatMessageShowTimestamp.Value) {
        return string.Empty;
      }

      return ChatMessageLayout.Value switch {
        MessageLayoutType.SingleRow =>
            ChatMessageShowTimestamp.Value
                ? $"<color=#{ToHtmlStringRGBA(ChatMessageTimestampColor.Value)}>{timestamp:t}</color>"
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
            $"<color=#{ToHtmlStringRGBA(GetMessageTextColor(message.MessageType))}>{text}</color>",

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
