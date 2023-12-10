using BepInEx.Configuration;

using ComfyLib;

namespace TorchesAndResin {
  public static class PluginConfig {
    public static ConfigEntry<bool> IsModEnabled { get; private set; }

    public static ConfigEntry<int> TorchStartingFuel { get; private set; }

    public static void BindConfig(ConfigFile config) {
      IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      TorchStartingFuel =
          config.BindInOrder(
              "Fuel",
              "torchStartingFuel",
              10000,
              "Value to set eligible torches' starting fuel to.",
              new AcceptableValueRange<int>(10000, 99999));
    }
  }
}
