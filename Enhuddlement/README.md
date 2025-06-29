# Enhuddlement

*EnemyHud enhancement and customization.*

![Splash](https://i.imgur.com/BpyBnON.png)

## Features

### HealthBar customization

  * Change the width/height/colors of the Healthbar
  * Display health values as text within the HealthBar

### HealthText value customization

  * Config-options under `[EnemyHud.HealthText]`:
    * `showMaxHealth` -- show max health value.
    * `showInfiniteHealth` -- show infinite symbol when current health exceeds threshold.
    * `infiniteHealthThreshold` -- threshold to show the infinite symbol for current health.
  * Note: these take effect on all PlayerHud, BossHud and EnemyHud health texts.

### Floating BossHud

  * Separate name and HealthBars follow each boss
  * Add a color gradient for the name

### Enemy level display

  * Display enemy (and boss) levels as stars (with a number)
  * Position next to name or below HealthBar

### Enemy aware/alert status display

  * Color enemy name for aware/alert status

### Show local PlayerHud

  * Display name and HealthBar over your own character

### Player PvP status indicator

  * Color player names when their PvP toggle is enabled

## Configuration

Changes to most settings will only occur when the EnemyHud is rebuilt (distance > 10m enemies, 100m bosses).

Use a [ConfigurationManager](https://valheim.thunderstore.io/package/Azumatt/Official_BepInEx_ConfigurationManager/) to
modify settings in-game.

![Configuration](https://imgur.com/sBDKAKf.png)

## Compatability

### BetterUI / BetterUI Reforged

  * Enhuddlment will un-patch BetterUI's EnemyHud patches to avoid mod conflicts.
