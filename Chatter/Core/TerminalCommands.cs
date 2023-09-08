namespace Chatter {
  //public static class TerminalCommands {
  //  static Terminal.ConsoleCommand _sayCommand;
  //  static Terminal.ConsoleCommand _shoutCommand;
  //  static Terminal.ConsoleCommand _whisperCommand;

  //  public static void CacheCommands() {
  //    _sayCommand ??= Terminal.commands["say"];
  //    _shoutCommand ??= Terminal.commands["s"];
  //    _whisperCommand ??= Terminal.commands["w"];
  //  }

  //  public static void ToggleCommands(bool toggleOn) {
  //    if (toggleOn) {
  //      CacheCommands();

  //      new Terminal.ConsoleCommand(
  //          "say",
  //          "Chatter: say <message>",
  //          args => {
  //            if (args.FullLine.Length < 5) {
  //              Chatter.SetChatInputTextDefaultPrefix(Talker.Type.Normal);
  //            } else if (Chat.m_instance) {
  //              Chat.m_instance.SendText(Talker.Type.Normal, args.FullLine.Substring(4));
  //            }
  //          });

  //      new Terminal.ConsoleCommand(
  //          "s",
  //          "Chatter: shout <message>",
  //          args => {
  //            if (args.FullLine.Length < 3) {
  //              Chatter.SetChatInputTextDefaultPrefix(Talker.Type.Shout);
  //            } else if (Chat.m_instance) {
  //              Chat.m_instance.SendText(Talker.Type.Shout, args.FullLine.Substring(2));
  //            }
  //          });

  //      new Terminal.ConsoleCommand(
  //          "shout",
  //          "Chatter: shout <message>",
  //          args => {
  //            if (args.FullLine.Length < 7) {
  //              Chatter.SetChatInputTextDefaultPrefix(Talker.Type.Shout);
  //            } else if (Chat.m_instance) {
  //              Chat.m_instance.SendText(Talker.Type.Shout, args.FullLine.Substring(6));
  //            }
  //          });

  //      new Terminal.ConsoleCommand(
  //          "w",
  //          "Chatter: whisper <message>",
  //          args => {
  //            if (args.FullLine.Length < 3) {
  //              Chatter.SetChatInputTextDefaultPrefix(Talker.Type.Whisper);
  //            } else if (Chat.m_instance) {
  //              Chat.m_instance.SendText(Talker.Type.Whisper, args.FullLine.Substring(2));
  //            }
  //          });

  //      new Terminal.ConsoleCommand(
  //          "whisper",
  //          "Chatter: whisper <message>",
  //          args => {
  //            if (args.FullLine.Length < 9) {
  //              Chatter.SetChatInputTextDefaultPrefix(Talker.Type.Whisper);
  //            } else if (Chat.m_instance) {
  //              Chat.m_instance.SendText(Talker.Type.Whisper, args.FullLine.Substring(8));
  //            }
  //          });
  //    } else {
  //      Terminal.commands["say"] = _sayCommand;
  //      Terminal.commands["s"] = _shoutCommand;
  //      Terminal.commands["w"] = _whisperCommand;

  //      Terminal.commands.Remove("shout");
  //      Terminal.commands.Remove("whisper");
  //    }
  //  }
  //}
}
