using System;

using UnityEngine;

namespace Chatter {
  [Flags]
  public enum ChatMessageType {
    None = 0,
    Text = 1,
    HudCenter = 2,
    Say = 4,
    Shout = 8,
    Whisper = 16,
    Ping = 32
  }

  public class ChatMessage {
    public ChatMessageType MessageType { get; set; } = ChatMessageType.Text;
    public DateTime Timestamp { get; set; } = DateTime.MinValue;
    public long SenderId { get; set; } = 0L;
    public Vector3 Position { get; set; } = Vector3.zero;
    public Talker.Type TalkerType { get; set; } = Talker.Type.Normal;
    public string Username { get; set; } = string.Empty;
    public string Text { get; set; } = string.Empty;
  }
}