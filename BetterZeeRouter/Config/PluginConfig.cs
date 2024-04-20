namespace BetterZeeRouter;

using BepInEx.Configuration;

public static class PluginConfig {
  public static ConfigEntry<bool> SetTargetHandlerShouldCheckDistance { get; private set; }
  public static ConfigEntry<float> SetTargetHandlerDistanceCheckRange { get; private set; }

  public static void BindConfig(ConfigFile config) {
    SetTargetHandlerShouldCheckDistance =
        config.Bind(
            "SetTargetHandler",
            "shouldCheckDistance",
            true,
            "SetTargetHandler: if true, it only will process the RPC if targetZDO is within range.");

    SetTargetHandlerDistanceCheckRange =
        config.Bind(
            "SetTargetHandler",
            "distanceCheckRange",
            10420f,
            "SetTargetHandler: the range to use for the targetZDO distance check.");
  }
}
