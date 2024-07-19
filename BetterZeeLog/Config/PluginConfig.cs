namespace BetterZeeLog;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }
  public static ConfigEntry<bool> RemoveStackTraceForNonErrorLogType { get; private set; }
  public static ConfigEntry<bool> RemoveFailedToSendDataLogging { get; private set; }
  public static ConfigEntry<bool> RemoveContainerRequestOpenLogging { get; private set; }
  public static ConfigEntry<bool> CheckProjectFixedUpdateZeroVelocity { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod (restart required).");

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
            "Removes (NOPs out) 'Failed to send data' logging in ZSteamSocket.");

    RemoveContainerRequestOpenLogging =
        config.BindInOrder(
            "Logging",
            "removeContainerRequestOpenLogging",
            true,
            "Removes (NOPs out) 'Players wants to open/but im not the owner' logging in Container.RPC_RequestOpen().");

    CheckProjectFixedUpdateZeroVelocity =
        config.BindInOrder(
            "Quaternion.LookRotation",
            "checkProjectFixedUpdatedZeroVelocity",
            true,
            "Checks for zero `m_vel` in `Projectile.FixedUpdate()`.");
  }
}
