namespace EmDee;

using System.IO;

using ComfyLib;

public sealed class TestMarkdownCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "test-markdown",
        "test-markdown --filename=<filename>",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    return Run(new ComfyArgs(args));
  }

  public static bool Run(ComfyArgs args) {
    if (!args.TryGetValue("filename", out string filenameArg)) {
      return false;
    }

    if (!File.Exists(filenameArg)) {
      return false;
    }

    string inputText = File.ReadAllText(filenameArg);
    EmDeeManager.Instance.RenderMarkdown(inputText);

    return true;
  }
}
