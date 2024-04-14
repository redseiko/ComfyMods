namespace Pinnacle;

using ComfyLib;

using static PinImportExport;

public static class ImportPinsCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "pinnacle-importpins-binary",
        "<filename> [name-filter-regex] -- import pins in binary format from file.",
        ImportPinsFromBinaryFile);
  }
}
