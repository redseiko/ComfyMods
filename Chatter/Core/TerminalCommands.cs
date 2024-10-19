namespace Chatter;

using System.Collections.Generic;

public static class TerminalCommands {
  static readonly List<Terminal.ConsoleCommand> _commands = [];

  public static void ToggleCommands(bool toggleOn) {
    DeregisterCommands(_commands);
    _commands.Clear();

    if (toggleOn) {
      _commands.AddRange(RegisterCommands());
    }

    UpdateCommandLists();
  }

  static Terminal.ConsoleCommand[] RegisterCommands() {
    return [
      new Terminal.ConsoleCommand(
          "shout",
          "(Chatter) shout <message>",
          RunShoutCommand),

      new Terminal.ConsoleCommand(
          "whisper",
          "(Chatter) whisper <message>",
          RunWhisperCommand),
    ];
  }

  public static void RunShoutCommand(Terminal.ConsoleEventArgs args) {
    if (args.FullLine.Length < 7) {
      ChatPanelController.ChatPanel?.SetChatTextInputPrefix(Talker.Type.Shout);
    } else if (Chat.m_instance) {
      Chat.m_instance.SendText(Talker.Type.Shout, args.FullLine.Substring(6));
    }
  }

  public static void RunWhisperCommand(Terminal.ConsoleEventArgs args) {
    if (args.FullLine.Length < 9) {
      ChatPanelController.ChatPanel?.SetChatTextInputPrefix(Talker.Type.Whisper);
    } else if (Chat.m_instance) {
      Chat.m_instance.SendText(Talker.Type.Whisper, args.FullLine.Substring(8));
    }
  }

  static void DeregisterCommands(List<Terminal.ConsoleCommand> commands) {
    foreach (Terminal.ConsoleCommand command in commands) {
      if (Terminal.commands[command.Command] == command) {
        Terminal.commands.Remove(command.Command);
      }
    }
  }

  static void UpdateCommandLists() {
    foreach (Terminal terminal in UnityEngine.Object.FindObjectsOfType<Terminal>(includeInactive: true)) {
      terminal.updateCommandList();
    }
  }
}
