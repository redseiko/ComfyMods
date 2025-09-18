namespace TorchesAndResin;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ConfigEntry<int> TorchStartingFuel { get; private set; }
  public static ConfigEntry<bool> CandleHoverTextShowFuel { get; private set; }
  public static ConfigEntry<bool> CandleAlwaysToggleOn { get; private set; }
  public static ConfigEntry<bool> CandleUpdateStateNeverWet { get; private set; }

  public static void BindConfig(ConfigFile config) {
    IsModEnabled =
        config.BindInOrder(
            "_Global",
            "isModEnabled",
            true,
            "Globally enable or disable this mod.");

    TorchStartingFuel =
        config.BindInOrder(
            "Fuel",
            "torchStartingFuel",
            10000,
            "Value to set eligible torches' starting fuel to.",
            new AcceptableValueRange<int>(10000, 99999));

    CandleHoverTextShowFuel =
        config.BindInOrder(
            "Candle",
            "candleHoverTextShowFuel",
            true,
            "When true, will show the remaining fuel for candles in its hover-text.");

    CandleAlwaysToggleOn =
        config.BindInOrder(
            "Candle",
            "candleAlwaysToggleOn",
            true,
            "When true, eligible candles will always be toggled-on.");

    CandleUpdateStateNeverWet =
        config.BindInOrder(
            "Candle",
            "candleUpdateStateNeverWet",
            true,
            "When true, eligible over-fueld candles will not be wet or underwater when updating state.");
  }
}
