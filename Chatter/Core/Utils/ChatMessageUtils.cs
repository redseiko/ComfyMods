namespace Chatter;

using System;
using System.Collections.Generic;
using System.Linq;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

using static UnityEngine.ColorUtility;

public static class ChatMessageUtils {
  public static readonly CircularQueue<ChatMessage> MessageHistory = new(capacity: 50, static _ => { });

  public static bool IsChatMessageQueued { get; set; }
  public static bool IsChatMessageFiltered { get; set; }

  public static bool AddChatMessage(ChatMessage message) {
    if (ShouldShowText(message.MessageType, message.Text)) {
      MessageHistory.EnqueueItem(message);
      ContentRowManager.CreateContentRow(message);

      return true;
    }

    return false;
  }

  public static ChatMessageType GetChatMessageType(Talker.Type talkerType) {
    return talkerType switch {
      Talker.Type.Normal => ChatMessageType.Say,
      Talker.Type.Shout => ChatMessageType.Shout,
      Talker.Type.Whisper => ChatMessageType.Whisper,
      Talker.Type.Ping => ChatMessageType.Ping,
      _ => ChatMessageType.Text
    };
  }

  public static string GetContentRowBodyText(ChatMessage message) {
    return ChatMessageLayout.Value switch {
      MessageLayoutType.SingleRow =>
          JoinIgnoringEmpty(
              ChatMessageShowTimestamp.Value ? GetTimestampText(message.Timestamp) : string.Empty,
              GetUsernameText(message.Username),
              GetMessageText(message)),

      _ => GetMessageText(message)
    };
  }

  static string JoinIgnoringEmpty(params string[] values) {
    return string.Join(" ", values.Where(static value => !string.IsNullOrEmpty(value)));
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

  public static bool ShouldShowText(ChatMessageType messageType, string text) {
    return messageType switch {
      ChatMessageType.Say => ShouldShowText(text, SayTextFilterList.CachedValues),
      ChatMessageType.Shout => ShouldShowText(text, ShoutTextFilterList.CachedValues),
      ChatMessageType.Whisper => ShouldShowText(text, WhisperTextFilterList.CachedValues),
      ChatMessageType.HudCenter => ShouldShowText(text, HudCenterTextFilterList.CachedValues),
      ChatMessageType.Text => ShouldShowText(text, OtherTextFilterList.CachedValues),
      _ => true
    };
  }

  public static bool ShouldShowText(string text, List<string> filters) {
    if (filters.Count == 0) {
      return true;
    }

    for (int i = 0, count = filters.Count; i < count; i++) {
      if (text.IndexOf(filters[i], 0, StringComparison.OrdinalIgnoreCase) >= 0) {
        return false;
      }
    }

    return true;
  }
}
