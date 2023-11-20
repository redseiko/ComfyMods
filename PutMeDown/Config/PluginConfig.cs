using System.Collections.Generic;
using System.Linq;

using BepInEx.Configuration;

using ComfyLib;

namespace PutMeDown {
  public static class PluginConfig {
    public static ConfigEntry<bool> IsModEnabled { get; private set; }

    public static ToggleStringListConfigEntry ItemsToIgnore { get; private set; }

    [ComfyConfig]
    public static void BindConfig(ConfigFile config) {
      IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");
    }

    [ComfyConfig(LateBind = true)]
    public static void BindAutoPickupConfig(ConfigFile config) {
      ItemsToIgnore =
          new ToggleStringListConfigEntry(
              config,
              "AutoPickup",
              "itemsToIgnore",
              "Wood=0,Stone=0",
              "Items to ignore for auto-pickup behaviour.",
              autoCompleteFunc: GetItemDropNames);

      AutoPickupController.SetItemsToIgnore(ItemsToIgnore.ToggledStringValues());
      ItemsToIgnore.SettingChanged += (_, values) => AutoPickupController.SetItemsToIgnore(values);
    }

    public static IEnumerable<string> GetItemDropNames() {
      return ObjectDB.m_instance.m_items
          .Where(
              item =>
                  item.TryGetComponent(out ItemDrop itemDrop)
                  && itemDrop.m_autoPickup
                  && itemDrop.m_itemData.m_shared.m_icons.Length > 0
                  && !string.IsNullOrEmpty(itemDrop.m_itemData.m_shared.m_description))
          .Select(item => item.name);
    }
  }
}
