﻿namespace ReportCard;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<bool> OpenStatsPanelOnCharacterSelect { get; private set; }

  public static ConfigEntry<bool> ExploredStatsPanelShowRawValues { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod (restart required).");

    OpenStatsPanelOnCharacterSelect =
        config.BindInOrder(
            "StatsPanel",
            "openStatsPanelOnCharacterSelect",
            true,
            "If set, opens the StatsPanel on the Character Select screen.");

    ExploredStatsPanelShowRawValues =
        config.BindInOrder(
            "ExploredStatsPanel",
            "showRawValues",
            false,
            "If set, will show raw values in the ExploredStatsPanel.");
  }
}
