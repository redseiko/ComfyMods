using BepInEx.Configuration;

using ComfyLib;

namespace PutMeDown {
  public static class PluginConfig {
    public static ConfigEntry<bool> IsModEnabled { get; private set; }

    public static ToggleStringListConfigEntry ItemsToIgnore { get; private set; }

    public static void BindConfig(ConfigFile config) {
      IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");

      ItemsToIgnore =
          new ToggleStringListConfigEntry(
              config,
              "AutoPickup",
              "itemsToIgnore",
              string.Empty,
              "Item (names) to ignore for auto-pickup behaviour.");

      AutoPickupController.SetItemsToIgnore(ItemsToIgnore.ToggledStringValues());
      ItemsToIgnore.SettingChanged += (_, values) => AutoPickupController.SetItemsToIgnore(values);
    }
  }
}
