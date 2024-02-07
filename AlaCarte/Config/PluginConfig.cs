namespace AlaCarte;

using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public enum DialogType {
    Vanilla,
    Old
  }

  public static ConfigEntry<DialogType> MenuDialogType { get; private set; }
  public static ConfigEntry<Vector2> MenuDialogPosition { get; private set; }

  public static ConfigEntry<bool> DisableGamePauseOnMenu { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod (restart required).");

    MenuDialogType =
        config.BindInOrder(
            "MenuDialog",
            "menuDialogType",
            DialogType.Vanilla,
            "Which Menu.m_menuDialog GameObject to display.");

    MenuDialogPosition =
        config.BindInOrder(
            "MenuDialog",
            "menuDialogPosition",
            new Vector2(0f, 212f),
            "Menu.m_menuDialog.position");

    DisableGamePauseOnMenu =
        config.BindInOrder(
            "Pause",
            "disableGamePauseOnMenu",
            false,
            "Disables the Game 'pause' effect when the Menu is shown.");
  }
}
