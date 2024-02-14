namespace PutMeDown;

using System.Collections.Generic;
using System.Linq;

using BepInEx.Configuration;

using ComfyLib;

public static class PluginConfig {
  public static ConfigEntry<bool> IsModEnabled { get; private set; }

  public static ToggleStringListConfigEntry ItemsToIgnore { get; private set; }

  public static ConfigEntry<KeyboardShortcut> ItemsToIgnoreCycleShortcut { get; private set; }
  public static ToggleStringListConfigEntry ItemsToIgnoreAlt1 { get; private set; }
  public static ToggleStringListConfigEntry ItemsToIgnoreAlt2 { get; private set; }

  [ComfyConfig]
  public static void BindConfig(ConfigFile config) {
    IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");
  }

  [ComfyConfig(LateBind = true)]
  public static void BindAutoPickupConfig(ConfigFile config) {
    _itemsToIgnoreConfigs.Clear();
    _itemsToIgnoreConfigIndex = -1;

    ItemsToIgnore =
        new ToggleStringListConfigEntry(
            config,
            "AutoPickup",
            "itemsToIgnore",
            "Wood=0,Stone=0",
            "Items to ignore for auto-pickup behaviour (normal config).",
            autoCompleteFunc: GetItemDropSearchOptions);

    _itemsToIgnoreConfigs.Add(ItemsToIgnore);
    ItemsToIgnore.SettingChanged += OnItemsToIgnoreChanged;

    ItemsToIgnoreCycleShortcut =
        config.BindInOrder(
            "AutoPickup.Alt",
            "itemsToIgnoreCycleShortcut",
            new KeyboardShortcut(UnityEngine.KeyCode.None),
            "Shortcut to cycle through ItemsToIgnore normal and alternate configs.");

    ItemsToIgnoreAlt1 =
        new ToggleStringListConfigEntry(
            config,
            "AutoPickup.Alt",
            "itemsToIgnoreAlt1",
            "Wood=0,Stone=0",
            "Items to ignore for auto-pickup behaviour (alternate config 1).",
            autoCompleteFunc: GetItemDropSearchOptions);

    _itemsToIgnoreConfigs.Add(ItemsToIgnoreAlt1);
    ItemsToIgnoreAlt1.SettingChanged += OnItemsToIgnoreChanged;

    ItemsToIgnoreAlt2 =
        new ToggleStringListConfigEntry(
            config,
            "AutoPickup.Alt",
            "itemsToIgnoreAlt2",
            "Wood=0,Stone=0",
            "Items to ignore for auto-pickup behaviour (alternate config 2).",
            autoCompleteFunc: GetItemDropSearchOptions);

    _itemsToIgnoreConfigs.Add(ItemsToIgnoreAlt2);
    ItemsToIgnoreAlt2.SettingChanged += OnItemsToIgnoreChanged;

    CycleItemsToIgnoreConfig();
  }

  static readonly List<ToggleStringListConfigEntry> _itemsToIgnoreConfigs = new();
  static int _itemsToIgnoreConfigIndex = -1;
  static ToggleStringListConfigEntry _itemsToIgnoreConfig = default;

  public static void CycleItemsToIgnoreConfig() {
    _itemsToIgnoreConfigIndex++;

    if (_itemsToIgnoreConfigIndex < 0 || _itemsToIgnoreConfigIndex >= _itemsToIgnoreConfigs.Count) {
      _itemsToIgnoreConfigIndex = 0;
    }

    _itemsToIgnoreConfig = _itemsToIgnoreConfigs[_itemsToIgnoreConfigIndex];
    AutoPickupController.SetItemsToIgnore(_itemsToIgnoreConfig.ToggledStringValues());
  }

  static void OnItemsToIgnoreChanged(object sender, IEnumerable<string> itemsToIgnore) {
    if (sender == _itemsToIgnoreConfig) {
      AutoPickupController.SetItemsToIgnore(itemsToIgnore);
    }
  }

  public static IEnumerable<ToggleStringListConfigEntry.SearchOption> GetItemDropSearchOptions() {
    return ObjectDB.m_instance.m_items
        .Where(
            item =>
                item.TryGetComponent(out ItemDrop itemDrop)
                && itemDrop.m_autoPickup
                && itemDrop.m_itemData.m_shared.m_icons.Length > 0
                && !string.IsNullOrEmpty(itemDrop.m_itemData.m_shared.m_description))
        .Select(
            item =>
                new ToggleStringListConfigEntry.SearchOption(
                    item.name,
                    Localization.m_instance.Localize(item.GetComponent<ItemDrop>().m_itemData.m_shared.m_name)
                        + $" ({item.name})"));
  }
}
