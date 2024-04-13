namespace EnRoute;

using ComfyLib;

using BepInEx.Configuration;

public static class PluginConfig {
  public static ConfigEntry<string> NearbyRPCMethodNames { get; private set; }

  public static void BindConfig(ConfigFile config) {
    NearbyRPCMethodNames =
        config.BindInOrder(
            "RouteManager",
            "nearbyRPCMethodNames",
            "FlashShield,Step,WNTCreateFragments",
            "Comma-separated list of RPC method names that should be routed only to nearby clients.");

    NearbyRPCMethodNames.OnSettingChanged(EnRoute.SetupNearbyRPCMethods);
  }
}
