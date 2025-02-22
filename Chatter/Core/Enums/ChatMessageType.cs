namespace Chatter;

using System;

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
