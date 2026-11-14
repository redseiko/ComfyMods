namespace BetterZeeLog;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<bool> RemoveStackTraceForNonErrorLogType { get; private set; }
  public static ConfigEntry<bool> RemoveFailedToSendDataLogging { get; private set; }
  public static ConfigEntry<bool> RemoveContainerRequestOpenLogging { get; private set; }
  public static ConfigEntry<bool> RemovePlayerPlacePieceLogging { get; private set; }
  public static ConfigEntry<bool> RemovePlayerTryPlacePieceLogging { get; private set; }
  public static ConfigEntry<bool> RemoveArmorStandSetVisualItemLogging { get; private set; }
  public static ConfigEntry<bool> RemoveItemStandSetVisualItemLogging { get; private set; }
  public static ConfigEntry<bool> RemovePickableItemSetupItemLogging { get; private set; }

  public static ConfigEntry<bool> CheckProjectFixedUpdateZeroVelocity { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod (restart required).");

    RemoveStackTraceForNonErrorLogType =
        config.BindInOrder(
            "Logging",
            "removeStackTraceForNonErrorLogType",
            true,
            "Disables the stack track for 'Info' and 'Warning' log types.");

    RemoveFailedToSendDataLogging =
        config.BindInOrder(
            "Logging",
            "removeFailedToSendDataLogging",
            true,
            "Removes 'Failed to send data' logging in ZSteamSocket.SendQueuedPackages().");

    RemoveContainerRequestOpenLogging =
        config.BindInOrder(
            "Logging",
            "removeContainerRequestOpenLogging",
            true,
            "Removes (NOPs out) 'Players wants to open/but im not the owner' logging in Container.RPC_RequestOpen().");

    RemovePlayerPlacePieceLogging =
        config.BindInOrder(
            "Logging",
            "removePlayerPlacePieceLogging",
            true,
            "Removes 'placed at height'/'placed at world height' logging in Player.PlacePiece().");

    RemovePlayerTryPlacePieceLogging =
        config.BindInOrder(
            "Logging",
            "removePlayerTryPlacePieceLogging",
            true,
            "Removes 'Placed' logging in Player.TryPlacePiece().");

    RemoveArmorStandSetVisualItemLogging =
        config.BindInOrder(
            "Logging",
            "removeArmorStandSetVisualItemLogging",
            true,
            "Removes 'Missing item prefab' logging in ArmorStand.SetVisualItem().");

    RemoveItemStandSetVisualItemLogging =
        config.BindInOrder(
            "Logging",
            "removeItemStandSetVisualItemLogging",
            true,
            "Removes 'Missing item prefab' and 'Failed to get attach prefab for item' logging in "
                + "ItemStand.SetVisualItem().");

    RemovePickableItemSetupItemLogging =
        config.BindInOrder(
            "Logging",
            "removePickableItemSetupItemLogging",
            true,
            "Removes 'Failed to get attach prefab for item' logging in PickableItem.SetupItem().");

    CheckProjectFixedUpdateZeroVelocity =
        config.BindInOrder(
            "Quaternion.LookRotation",
            "checkProjectFixedUpdatedZeroVelocity",
            true,
            "Checks for zero `m_vel` in `Projectile.FixedUpdate()`.");
  }
}
