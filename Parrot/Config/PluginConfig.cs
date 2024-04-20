namespace Parrot;

using BepInEx.Configuration;

public static class PluginConfig {
  public static ConfigEntry<bool> SuppressChatMessageGamertag { get; private set; }
  public static ConfigEntry<bool> SuppressSayGamertag { get; private set; }

  public static void BindConfig(ConfigFile config) {
    SuppressChatMessageGamertag =
        config.Bind(
            "RPC.ChatMessage",
            "suppressChatMessageGamertag",
            true,
            "If set, will suppress UserINfo.Gamertag from ChatMessage RPCs.");

    SuppressSayGamertag =
        config.Bind(
            "RPC.Say",
            "suppressSayGamertag",
            true,
            "If set, will suppress UserInfo.Gamertag from Say RPCs.");
  }
}
