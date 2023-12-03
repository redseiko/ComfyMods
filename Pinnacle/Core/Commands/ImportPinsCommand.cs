using System.Collections.Generic;

using ComfyLib;

using static Pinnacle.PinImportExport;

namespace Pinnacle {
  public static class ImportPinsCommand {
    [ComfyCommand]
    public static IEnumerable<Terminal.ConsoleCommand> Register() {
      yield return new Terminal.ConsoleCommand(
          "pinnacle-importpins-binary",
          "<filename> [name-filter-regex] -- import pins in binary format from file.",
          args => ImportPinsFromBinaryFile(args));
    }
  }
}
