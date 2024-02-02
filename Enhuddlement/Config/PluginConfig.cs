using BepInEx.Configuration;

using ComfyLib;

using UnityEngine;

namespace Enhuddlement {
  public static class PluginConfig {
    public static ConfigEntry<bool> IsModEnabled { get; private set; }
    public static ConfigEntry<bool> ShowEnemyHealthValue { get; private set; }

    public static void BindConfig(ConfigFile config) {
      IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");
      ShowEnemyHealthValue = config.BindInOrder("EnemyHud", "showEnemyHealthValue", true, "Show enemy health values.");

      BindPlayerHudConfig(config);
      BindBossHudConfig(config);
      BindEnemyHudConfig(config);
      BindEnemyLevelConfig(config);
    }

    public static ConfigEntry<bool> PlayerHudShowLocalPlayer { get; private set; }
    public static ConfigEntry<Vector3> PlayerHudPositionOffset { get; private set; }

    public static ConfigEntry<int> PlayerHudNameTextFontSize { get; private set; }
    public static ConfigEntry<Color> PlayerHudNameTextColor { get; private set; }
    public static ConfigEntry<Color> PlayerHudNameTextPvPColor { get; private set; }

    public static ConfigEntry<int> PlayerHudHealthTextFontSize { get; private set; }
    public static ConfigEntry<Color> PlayerHudHealthTextColor { get; private set; }

    public static ConfigEntry<float> PlayerHudHealthBarWidth { get; private set; }
    public static ConfigEntry<float> PlayerHudHealthBarHeight { get; private set; }
    public static ConfigEntry<Color> PlayerHudHealthBarColor { get; private set; }

    public static void BindPlayerHudConfig(ConfigFile config) {
      PlayerHudShowLocalPlayer =
          config.BindInOrder(
              "PlayerHud",
              "showLocalPlayer",
              false,
              "If true, shows a PlayerHud for the local player.");

      PlayerHudPositionOffset =
          config.BindInOrder(
              "PlayerHud.Position",
              "positionOffset",
              new Vector3(0f, 0.3f, 0f),
              "PlayerHud position offset from head point.");

      PlayerHudNameTextFontSize =
          config.BindInOrder(
              "PlayerHud.Name",
              "nameTextFontSize",
              20,
              "PlayerHud.Name text font size.",
              new AcceptableValueRange<int>(0, 32));

      PlayerHudNameTextColor =
          config.BindInOrder(
              "PlayerHud.Name",
              "nameTextColor",
              new Color(1f, 0.7176f, 0.3603f, 1f),
              "PlayerHud.Name text color.");

      PlayerHudNameTextPvPColor =
          config.BindInOrder(
              "PlayerHud.Name",
              "nameTextPvPColor",
              Color.red,
              "PlayerHud.Name text color when player has PvP enabled.");

      PlayerHudHealthTextFontSize =
          config.BindInOrder(
              "PlayerHud.HealthText",
              "healthTextFontSize",
              16,
              "PlayerHud.HealthText text font size.",
              new AcceptableValueRange<int>(0, 32));

      PlayerHudHealthTextColor =
          config.BindInOrder(
              "PlayerHud.HealthText",
              "healthTextColor",
              Color.white,
              "PlayerHud.HealthText text color.");

      PlayerHudHealthBarWidth =
          config.BindInOrder(
              "PlayerHud.HealthBar",
              "healthBarWidth",
              125f,
              "PlayerHud.HealthBar width (vanilla: 100).",
              new AcceptableValueRange<float>(0f, 1200f));

      PlayerHudHealthBarHeight =
          config.BindInOrder(
              "PlayerHud.HealthBar",
              "healthBarHeight",
              22f,
              "PlayerHud.HealthBar height (vanilla: 5).",
              new AcceptableValueRange<float>(0f, 90f));

      PlayerHudHealthBarColor =
          config.BindInOrder(
              "PlayerHud.HealthBar",
              "healthBarColor",
              new Color(0.2638f, 1f, 0.125f, 1f),
              "PlayerHud.HealthBar fast color for regular players.");
    }

    public static ConfigEntry<bool> FloatingBossHud { get; private set; }
    public static ConfigEntry<Vector3> BossHudPositionOffset { get; private set; }

    public static ConfigEntry<int> BossHudNameTextFontSize { get; private set; }
    public static ConfigEntry<Color> BossHudNameTextColorTop { get; private set; }
    public static ConfigEntry<Color> BossHudNameTextColorBottom { get; private set; }

    public static ConfigEntry<int> BossHudHealthTextFontSize { get; private set; }
    public static ConfigEntry<Color> BossHudHealthTextFontColor { get; private set; }

    public static ConfigEntry<float> BossHudHealthBarWidth { get; private set; }
    public static ConfigEntry<float> BossHudHealthBarHeight { get; private set; }
    public static ConfigEntry<Color> BossHudHealthBarColor { get; private set; }

    public static void BindBossHudConfig(ConfigFile config) {
      FloatingBossHud =
          config.BindInOrder(
              "BossHud", "floatingBossHud", true, "If set, each BossHud will float over the target enemy.");

      BossHudPositionOffset =
          config.BindInOrder(
              "BossHud.Position",
              "positionOffset",
              new Vector3(0f, 1f, 0f),
              "BossHud position offset from top point.");

      BossHudNameTextFontSize =
          config.BindInOrder(
              "BossHud.Name",
              "nameTextFontSize",
              32,
              "BossHud.Name text font size (vanilla: 32).",
              new AcceptableValueRange<int>(0, 64));

      BossHudNameTextColorTop =
          config.BindInOrder(
              "BossHud.Name",
              "nameTextColorTop",
              Color.red,
              "BossHud.Name text top-color for vertical gradient.");

      BossHudNameTextColorBottom =
          config.BindInOrder(
              "BossHud.Name",
              "nameTextColorBottom",
              Color.black,
              "BossHud.Name text bottom color for vertical gradient.");

      BossHudHealthTextFontSize =
          config.BindInOrder(
              "BossHud.HealthText",
              "healthTextFontSize",
              24,
              "BossHud.HealthText font size.",
              new AcceptableValueRange<int>(0, 64));

      BossHudHealthTextFontColor =
          config.BindInOrder(
              "BossHud.HealthText",
              "healthTextColor",
              Color.white,
              "BossHud.HealthText text color.");

      BossHudHealthBarWidth =
          config.BindInOrder(
              "BossHud.HealthBar",
              "healthBarWidth",
              300f,
              "BossHud.HealthBar width (vanilla: 600).",
              new AcceptableValueRange<float>(0f, 1200f));

      BossHudHealthBarHeight =
          config.BindInOrder(
              "BossHud.HealthBar",
              "healthBarHeight",
              30f,
              "BossHud.HealthBar height (vanilla: 15).",
              new AcceptableValueRange<float>(0f, 90f));

      BossHudHealthBarColor =
          config.BindInOrder(
              "BossHud.HealthBar",
              "healthBarColor",
              new Color(1f, 0f, 0.3931f, 1f),
              "BossHud.HealthBar fast color.");
    }

    public static ConfigEntry<Vector3> EnemyHudPositionOffset { get; private set; }

    public static ConfigEntry<int> EnemyHudNameTextFontSize { get; private set; }
    public static ConfigEntry<Color> EnemyHudNameTextColor { get; private set; }

    public static ConfigEntry<bool> EnemyHudUseNameForStatus { get; private set; }
    public static ConfigEntry<Color> EnemyHudNameTextAlertedColor { get; private set; }
    public static ConfigEntry<Color> EnemyHudNameTextAwareColor { get; private set; }

    public static ConfigEntry<Color> EnemyHudHealthTextColor { get; private set; }
    public static ConfigEntry<int> EnemyHudHealthTextFontSize { get; private set; }

    public static ConfigEntry<float> EnemyHudHealthBarWidth { get; private set; }
    public static ConfigEntry<float> EnemyHudHealthBarHeight { get; private set; }

    public static ConfigEntry<Color> EnemyHudHealthBarColor { get; private set; }
    public static ConfigEntry<Color> EnemyHudHealthBarFriendlyColor { get; private set; }
    public static ConfigEntry<Color> EnemyHudHealthBarTamedColor { get; private set; }

    public static void BindEnemyHudConfig(ConfigFile config) {
      EnemyHudPositionOffset =
          config.BindInOrder(
              "EnemyHud.Position",
              "positionOffset",
              new Vector3(0f, 0.1f, 0f),
              "EnemyHud position offset from top point.");

      EnemyHudNameTextFontSize =
          config.BindInOrder(
              "EnemyHud.Name",
              "nameTextFontSize",
              16,
              "EnemyHud.Name text font size (vanilla: 16).",
              new AcceptableValueRange<int>(0, 32));

      EnemyHudNameTextColor =
          config.BindInOrder(
              "EnemyHud.Name",
              "nameTextColor",
              Color.white,
              "EnemyHud.Name text color (vanilla: white).");

      EnemyHudUseNameForStatus =
          config.BindInOrder(
              "EnemyHud.Name.Status",
              "useNameForStatus",
              true,
              "Use the EnemyHud.Name text color for alerted/aware status.");

      EnemyHudNameTextAlertedColor =
          config.BindInOrder(
              "EnemyHud.Name.Status",
              "nameTextAlertedColor",
              Color.red,
              "EnemyHud.Name text color for alerted status.");

      EnemyHudNameTextAwareColor =
          config.BindInOrder(
              "EnemyHud.Name.Status",
              "nameTextAwareColor",
              Color.yellow,
              "EnemyHud.Name text color for aware status.");

      EnemyHudHealthTextFontSize =
          config.BindInOrder(
              "EnemyHud.HealthText",
              "healthTextFontSize",
              14,
              "EnemyHud.HealthText text font size.",
              new AcceptableValueRange<int>(0, 32));

      EnemyHudHealthTextColor =
          config.BindInOrder(
              "EnemyHud.HealthText",
              "healthTextColor",
              Color.white,
              "EnemyHud.HealthText text color.");

      EnemyHudHealthBarWidth =
          config.BindInOrder(
              "EnemyHud.HealthBar",
              "healthBarWidth",
              125f,
              "EnemyHud.HealthBar width (vanilla: 100).",
              new AcceptableValueRange<float>(0f, 1200f));

      EnemyHudHealthBarHeight =
          config.BindInOrder(
              "EnemyHud.HealthBar",
              "healthBarHeight",
              22f,
              "EnemyHud.HealthBar height (vanilla: 5).",
              new AcceptableValueRange<float>(0f, 90f));

      EnemyHudHealthBarColor =
          config.BindInOrder(
              "EnemyHud.HealthBar",
              "healthBarColor",
              new Color(1f, 0.333f, 0.333f, 1f),
              "EnemyHud.HealthBar fast color for regular mobs.");

      EnemyHudHealthBarFriendlyColor =
          config.BindInOrder(
              "EnemyHud.HealthBar",
              "healthBarFriendlyColor",
              new Color(0.2638f, 1f, 0.125f, 1f),
              "EnemyHud.HealthBar fast color for friendly (but not tamed) mobs.");

      EnemyHudHealthBarTamedColor =
          config.BindInOrder(
              "EnemyHud.HealthBar",
              "healthBarTamedColor",
              Color.green,
              "EnemyHud.HealthBar fast color for tamed mobs.");
    }

    public static ConfigEntry<bool> EnemyLevelShowByName { get; private set; }
    public static ConfigEntry<bool> EnemyLevelUseVanillaStar { get; private set; }
    public static ConfigEntry<string> EnemyLevelStarSymbol { get; private set; }
    public static ConfigEntry<int> EnemyLevelStarCutoff { get; private set; }
    public static ConfigEntry<int> EnemyLevelTextMinFontSize { get; private set; }

    public static void BindEnemyLevelConfig(ConfigFile config) {
      EnemyLevelShowByName =
          config.BindInOrder(
              "EnemyLevel",
              "enemyLevelShowByName",
              false,
              "If true, shows the enemy level after the name, otherwise below healthbar.");

      EnemyLevelUseVanillaStar =
          config.BindInOrder(
              "EnemyLevel",
              "enemyLevelUseVanillaStar",
              false,
              "If true, uses the vanilla 'star' image for 1* and 2* monsters.");

      EnemyLevelStarSymbol =
          config.BindInOrder(
              "EnemyLevel",
              "enemyLevelStarSymbol",
              "\u2605",
              "Symbol to use for 'star' for enemy levels above vanilla 2*.",
              new AcceptableValueList<string>("\u2605", "\u2606", "\u2734", "\u2733", "\u2756", "\u2716"));

      EnemyLevelStarCutoff =
          config.BindInOrder(
              "EnemyLevel",
              "enemyLevelStarCutoff",
              2,
              "When showing enemy levels using stars, max stars to show before switching to 'X\u2605' format.",
              new AcceptableValueRange<int>(0, 10));

      EnemyLevelTextMinFontSize =
          config.BindInOrder(
              "EnemyLevel",
              "enemyLevelMinFontSize",
              20,
              "Sets a minimum font size for the enemy level text which is inherited from enemy name text font size.",
              new AcceptableValueRange<int>(0, 32));
    }
  }
}
