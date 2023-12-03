using System.Collections.Generic;

using ComfyLib;

using static Pinnacle.PinImportExport;

namespace Pinnacle {
  public static class ExportPinsCommand {
    [ComfyCommand]
    public static IEnumerable<Terminal.ConsoleCommand> Register() {
      yield return new Terminal.ConsoleCommand(
          "pinnacle-exportpins-binary",
          "<filename> [name-filter-regex] -- export pins to a file in binary format.",
          args => ExportPinsToFile(args, PinFileFormat.Binary));

      yield return new Terminal.ConsoleCommand(
          "pinnacle-exportpins-text",
          "<filename> [name-filter-regex] -- export pins to a file in plain text format.",
          args => ExportPinsToFile(args, PinFileFormat.PlainText));
    }
  }
}
