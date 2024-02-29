namespace ComfyLib;

using BepInEx.Configuration;

public sealed class ToggleSliderListConfigEntry {
  public readonly ConfigEntry<string> BaseConfigEntry;

  public ToggleSliderListConfigEntry(
      ConfigFile config,
      string section,
      string key,
      string defaultValue,
      string description) {
    BaseConfigEntry = config.BindInOrder(section, key, defaultValue, description);
  }

  public sealed class ToggleSliderListItem {
    public string ItemName;
    public bool IsToggled;
    public float SliderValue;
  }

  public void Drawer(ConfigEntryBase configEntry) {

  }
}
