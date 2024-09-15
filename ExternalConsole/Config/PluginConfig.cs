namespace ExternalConsole;

using BepInEx.Configuration;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }
  public static ConfigEntry<string> ExternalInputFilename { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled = config.Bind("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

    ExternalInputFilename =
        config.Bind(
            "ExternalInput",
            "externalInputFilename",
            "external-input.txt",
            "Filename for the text file to use for external input.");
  }
}
