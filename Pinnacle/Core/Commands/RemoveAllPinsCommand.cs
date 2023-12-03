using System.Collections.Generic;

using ComfyLib;

namespace Pinnacle {
  public static class RemoveAllPinsCommand {
    [ComfyCommand]
    public static IEnumerable<Terminal.ConsoleCommand> Register() {
      yield return new Terminal.ConsoleCommand(
          "pinnacle-removeallpins",
          "Pinnacle: removes ALL pins.",
          args => RemoveAllPins(args));
    }

    static void RemoveAllPins(Terminal.ConsoleEventArgs args) {
      if (Minimap.m_instance) {
        return;
      }

      int count = Minimap.m_instance.m_pins.RemoveAll(pin => pin.m_save);
      args.Context.AddString($"Removed {count} pins.");
    }
  }
}
