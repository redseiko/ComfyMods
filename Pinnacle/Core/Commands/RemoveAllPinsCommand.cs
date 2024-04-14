namespace Pinnacle;

using ComfyLib;

public static class RemoveAllPinsCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "pinnacle-remove-all-pins",
        "(Pinnacle) Removes ALL pins.",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    if (Minimap.m_instance) {
      return false;
    }

    int count = Minimap.m_instance.m_pins.RemoveAll(pin => pin.m_save);
    args.Context.AddString($"Removed {count} pins.");

    return true;
  }
}
