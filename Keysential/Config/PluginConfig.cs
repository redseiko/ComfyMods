using BepInEx.Configuration;

using UnityEngine;

namespace Keysential {
  public static class PluginConfig {
    public static ConfigEntry<string> GlobalKeysOverrideList { get; private set; }
    public static ConfigEntry<string> GlobalKeysAllowedList { get; private set; }

    public static ConfigEntry<bool> VendorKeyManagerEnabled { get; private set; }
    public static ConfigEntry<Vector3> VendorKeyManagerPosition { get; private set; }

    public static void BindConfig(ConfigFile config) {
      GlobalKeysOverrideList =
          config.Bind(
              "ZoneSystem",
              "globalKeysOverrideList",
              string.Empty,
              "If set, server will maintain this constant list of comma-delimited global keys.");

      GlobalKeysAllowedList =
          config.Bind(
              "ZoneSystem",
              "globalKeysAllowedList",
              string.Empty,
              "If set, server will only accept these global keys (comma-delimited) in RPC_SetGlobalKey().");

      VendorKeyManagerEnabled =
          config.Bind(
              "VendorKeyManager",
              "vendorKeyManagerEnabled",
              false,
              "If true, will start the VendorKeyManager coroutine at the vendor position.");

      VendorKeyManagerPosition =
          config.Bind(
              "VendorKeyManager",
              "vendorKeyManagerPosition",
              Vector3.zero,
              "If non-zero, used as the vendor position for the VendorKeyManager coroutine.");
    }
  }
}
