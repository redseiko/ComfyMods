namespace Pinnacle;

using System.Collections.Generic;

using ComfyLib;

public static class ExportPinsCommand {
  [ComfyCommand]
  public static IEnumerable<Terminal.ConsoleCommand> Register() {
    return new Terminal.ConsoleCommand[] {
      new Terminal.ConsoleCommand(
        "pinnacle-exportpins-binary",
        "<filename> [name-filter-regex] -- export pins to a file in binary format.",
        ExportPinsToBinaryFile),

      new Terminal.ConsoleCommand(
        "pinnacle-exportpins-text",
        "<filename> [name-filter-regex] -- export pins to a file in plain text format.",
        ExportPinsToTextFile),
    };
  }

  public static object ExportPinsToBinaryFile(Terminal.ConsoleEventArgs args) {
    PinImportExport.ExportPinsToFile(args, PinImportExport.PinFileFormat.Binary);
    return true;
  }

  public static object ExportPinsToTextFile(Terminal.ConsoleEventArgs args) {
    PinImportExport.ExportPinsToFile(args, PinImportExport.PinFileFormat.PlainText);
    return true;
  }
}
