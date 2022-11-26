﻿using BepInEx.Configuration;

using ComfyLib;

namespace Entitlement {
  public static class PluginConfig {
    public static ConfigEntry<bool> IsModEnabled { get; private set; }
    public static ConfigEntry<bool> ShowEnemyHealthValue { get; private set; }

    public static ConfigEntry<string> EnemyLevelStarSymbol { get; private set; }

    public static void BindConfig(ConfigFile config) {
      IsModEnabled = config.BindInOrder("_Global", "isModEnabled", true, "Globally enable or disable this mod.");
      ShowEnemyHealthValue = config.BindInOrder("EnemyHud", "showEnemyHealthValue", true, "Show enemy health values.");

      EnemyLevelStarSymbol =
          config.BindInOrder(
              "EnemyLevel",
              "enemyLevelStarSymbol",
              "\u2605",
              "Symbol to use for 'star' for enemy levels.",
              new AcceptableValueList<string>("\u2605", "\u272a", "\u2735", "\u272d", "\u272b"));

      BindEnemyHudConfig(config);
      BindBossHudConfig(config);
    }

    public static ConfigEntry<int> EnemyHudNameTextFontSize { get; private set; }

    public static ConfigEntry<int> EnemyHudHealthTextFontSize { get; private set; }
    public static ConfigEntry<float> EnemyHudHealthBarWidth { get; private set; }
    public static ConfigEntry<float> EnemyHudHealthBarHeight { get; private set; }

    public static void BindEnemyHudConfig(ConfigFile config) {
      EnemyHudNameTextFontSize =
          config.BindInOrder(
              "EnemyHud.Name",
              "nameTextFontSize",
              16,
              "EnemyHud.Name text font size (vanilla: 16).",
              new AcceptableValueRange<int>(0, 32));

      EnemyHudHealthTextFontSize =
          config.BindInOrder(
              "EnemyHud.HealthBar",
              "healthTextFontSize",
              14,
              "EnemyHud.HealthText font size.",
              new AcceptableValueRange<int>(0, 32));

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
    }

    public static ConfigEntry<bool> FloatingBossHud { get; private set; }

    public static ConfigEntry<int> BossHudNameTextFontSize { get; private set; }
    public static ConfigEntry<bool> BossHudNameUseGradientEffect { get; private set; }

    public static ConfigEntry<int> BossHudHealthTextFontSize { get; private set; }
    public static ConfigEntry<float> BossHudHealthBarWidth { get; private set; }
    public static ConfigEntry<float> BossHudHealthBarHeight { get; private set; }

    public static void BindBossHudConfig(ConfigFile config) {
      FloatingBossHud =
          config.BindInOrder(
              "BossHud", "floatingBossHud", true, "If set, each BossHud will float over the target enemy.");

      BossHudNameTextFontSize =
          config.BindInOrder(
              "BossHud.Name",
              "nameTextFontSize",
              32,
              "BossHud.Name text font size (vanilla: 32).",
              new AcceptableValueRange<int>(0, 64));

      BossHudNameUseGradientEffect =
          config.BindInOrder(
              "BossHud.Name",
              "useGradientEffect",
              true,
              "If true, adds a vertical Gradient effect to the BossHud.Name text.");

      BossHudHealthTextFontSize =
          config.BindInOrder(
              "BossHud.HealthBar",
              "healthTextFontSize",
              24,
              "BossHud.HealthText font size.",
              new AcceptableValueRange<int>(0, 64));

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
    }
  }
}