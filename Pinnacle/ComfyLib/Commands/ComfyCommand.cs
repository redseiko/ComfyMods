using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace ComfyLib {
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = true)]
  public class ComfyCommand : Attribute {
    // ...
  }

  public static class ComfyCommandUtils {
    static readonly List<Terminal.ConsoleCommand> _commands = new();

    public static void ToggleCommands(bool toggleOn) {
      DeregisterCommands(_commands);
      _commands.Clear();

      if (toggleOn) {
        _commands.AddRange(RegisterCommands(Assembly.GetExecutingAssembly()));
      }

      UpdateCommandLists();
    }

    static void UpdateCommandLists() {
      foreach (Terminal terminal in UnityEngine.Object.FindObjectsOfType<Terminal>(includeInactive: true)) {
        terminal.updateCommandList();
      }
    }

    static void DeregisterCommands(List<Terminal.ConsoleCommand> commands) {
      foreach (Terminal.ConsoleCommand command in commands) {
        if (Terminal.commands[command.Command] == command) {
          Terminal.commands.Remove(command.Command);
        }
      }
    }

    static IEnumerable<Terminal.ConsoleCommand> RegisterCommands(Assembly assembly) {
      return assembly.GetTypes()
          .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
          .Where(method => method.GetCustomAttributes(typeof(ComfyCommand), inherit: false).Length > 0)
          .Where(method => IsRegisterCommandMethod(method) || IsRegisterCommandsMethod(method))
          .SelectMany(method => RegisterCommands(method));
    }

    static IEnumerable<Terminal.ConsoleCommand> RegisterCommands(MethodInfo method) {
      if (IsRegisterCommandMethod(method)) {
        yield return (Terminal.ConsoleCommand) method.Invoke(null, null);
      } else if (IsRegisterCommandsMethod(method)) {
        foreach (Terminal.ConsoleCommand command in (IEnumerable<Terminal.ConsoleCommand>) method.Invoke(null, null)) {
          yield return command;
        }
      }
    }

    static bool IsRegisterCommandMethod(MethodInfo method) {
      return method.GetParameters().Length == 0 && typeof(Terminal.ConsoleCommand).IsAssignableFrom(method.ReturnType);
    }

    static bool IsRegisterCommandsMethod(MethodInfo method) {
      return method.GetParameters().Length == 0
          && typeof(IEnumerable<Terminal.ConsoleCommand>).IsAssignableFrom(method.ReturnType);
    }
  }
}
