namespace Chatter {
  public static class TerminalCommands {
    public static void ToggleCommands(bool toggleOn) {
      if (toggleOn) {
        new Terminal.ConsoleCommand(
            "shout",
            "Chatter: shout <message>",
            args => {
              if (args.FullLine.Length < 7) {
                Chatter.ChatterChatPanel?.SetChatTextInputPrefix(Talker.Type.Shout);
              } else if (Chat.m_instance) {
                Chat.m_instance.SendText(Talker.Type.Shout, args.FullLine.Substring(6));
              }
            });

        new Terminal.ConsoleCommand(
            "whisper",
            "Chatter: whisper <message>",
            args => {
              if (args.FullLine.Length < 9) {
                Chatter.ChatterChatPanel?.SetChatTextInputPrefix(Talker.Type.Whisper);
              } else if (Chat.m_instance) {
                Chat.m_instance.SendText(Talker.Type.Whisper, args.FullLine.Substring(8));
              }
            });
      } else {
        Terminal.commands.Remove("shout");
        Terminal.commands.Remove("whisper");
      }
    }
  }
}
