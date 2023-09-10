using ComfyLib;

using UnityEngine;

using static Chatter.PluginConfig;

namespace Chatter {
  public static class ChatTextInputUtils {
    public static string ChatTextInputDefaultPrefix { get; private set; } = "say ";

    public static void SetChatTextInputDefaultPrefix(ChatPanel chatPanel, Talker.Type talkerType) {
      ChatTextInputDefaultPrefix =
          talkerType switch {
            Talker.Type.Shout => "s ",
            Talker.Type.Whisper => "w ",
            _ => "say ",
          };

      string text =
          talkerType switch {
            Talker.Type.Shout => "/shout",
            Talker.Type.Whisper => "/whisper",
            _ => "/say ",
          };

      Color color =
          talkerType switch {
            Talker.Type.Shout => ChatMessageTextShoutColor.Value,
            Talker.Type.Whisper => ChatMessageTextWhisperColor.Value,
            _ => ChatMessageTextSayColor.Value
          };

      chatPanel.TextInput.InputField.textComponent.color = color;
      chatPanel.TextInput.InputFieldPlaceholder.text = text;
      chatPanel.TextInput.InputFieldPlaceholder.color = color.SetAlpha(0.3f);
    }
  }
}
