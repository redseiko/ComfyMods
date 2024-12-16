namespace ColorfulPieces;

using ComfyLib;

public static class ToggleColorPickerCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "toggle-color-picker",
        "(ColorfulPieces) toggle-color-picker",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    ShortcutUtils.OnToggleColorPickerShortcut();
    return true;
  }
}
