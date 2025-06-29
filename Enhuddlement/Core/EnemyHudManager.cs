namespace Enhuddlement;

using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;

using ComfyLib;

using TMPro;

using UnityEngine;

using static PluginConfig;

public static class EnemyHudManager {
  public static readonly ConditionalWeakTable<EnemyHud.HudData, TextMeshProUGUI> HealthTextCache = new();

  public static void SetupPlayerHud(EnemyHud.HudData hudData) {
    SetupName(hudData, PlayerHudNameTextFontSize.Value, PlayerHudNameTextColor.Value);

    SetupHud(
        hudData,
        PlayerHudHealthTextFontSize.Value,
        PlayerHudHealthTextColor.Value,
        PlayerHudHealthBarWidth.Value,
        PlayerHudHealthBarHeight.Value);

    hudData.m_healthFast.SetColor(PlayerHudHealthBarColor.Value);
  }

  public static void SetupBossHud(EnemyHud.HudData hudData) {
    SetupName(hudData, BossHudNameTextFontSize.Value, BossHudNameTextColorTop.Value);

    SetupHud(
        hudData,
        BossHudHealthTextFontSize.Value,
        BossHudHealthTextFontColor.Value,
        BossHudHealthBarWidth.Value,
        BossHudHealthBarHeight.Value);

    hudData.m_healthFast.SetColor(BossHudHealthBarColor.Value);

    SetupNameGradient(hudData, BossHudNameTextColorTop.Value, BossHudNameTextColorBottom.Value);
  }

  public static void SetupEnemyHud(EnemyHud.HudData hudData) {
    SetupName(hudData, EnemyHudNameTextFontSize.Value, EnemyHudNameTextColor.Value);

    SetupHud(
        hudData,
        EnemyHudHealthTextFontSize.Value,
        EnemyHudHealthTextColor.Value,
        EnemyHudHealthBarWidth.Value,
        EnemyHudHealthBarHeight.Value);

    SetupAlerted(hudData);
    SetupAware(hudData);

    // Default to regular color as it can change in updater.
    hudData.m_healthFast.SetColor(EnemyHudHealthBarColor.Value);
  }

  public static void SetupName(EnemyHud.HudData hudData, int nameTextFontSize, Color nameTextColor) {
    hudData.m_name
        .SetColor(nameTextColor)
        .SetFontSize(nameTextFontSize)
        .SetTextWrappingMode(TextWrappingModes.NoWrap)
        .SetOverflowMode(TextOverflowModes.Overflow)
        .SetAlignment(TextAlignmentOptions.Bottom)
        .SetEnableAutoSizing(false);

    hudData.m_name.GetComponent<RectTransform>()
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 0f))
        .SetPosition(new(0f, 8f))
        .SetSizeDelta(new(hudData.m_name.preferredWidth, hudData.m_name.preferredHeight));
  }

  public static void SetupNameGradient(EnemyHud.HudData hudData, Color topColor, Color bottomColor) {
    hudData.m_name.enableVertexGradient = true;

    hudData.m_name.colorGradient =
        new VertexGradient() {
          topLeft = topColor,
          topRight = topColor,
          bottomLeft = bottomColor,
          bottomRight = bottomColor,
        };
  }

  public static void SetupHud(
      EnemyHud.HudData hudData,
      int healthTextFontSize,
      Color healthTextFontColor,
      float healthBarWidth,
      float healthBarHeight) {
    Transform healthTransform = hudData.m_gui.transform.Find("Health");
    healthTransform.GetComponent<RectTransform>()
        .SetAnchorMin(new(0.5f, 0.5f))
        .SetAnchorMax(new(0.5f, 0.5f))
        .SetPivot(new(0.5f, 1f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(healthBarWidth, healthBarHeight));

    SetupHealthBars(hudData, healthBarWidth, healthBarHeight);

    TextMeshProUGUI healthText = CreateHealthText(hudData, healthTransform, healthTextFontSize, healthTextFontColor);
    HealthTextCache.Add(hudData, healthText);

    SetupLevel(hudData, healthTransform);
  }

  public static void SetupHealthBars(EnemyHud.HudData hudData, float healthBarWidth, float healthBarHeight) {
    hudData.m_healthFast.m_width = healthBarWidth;
    hudData.m_healthFast.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.zero)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    hudData.m_healthFast.m_bar
        .SetAnchorMin(new(0f, 0.5f))
        .SetAnchorMax(new(0f, 0.5f))
        .SetPivot(new(0f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(healthBarWidth, healthBarHeight));

    hudData.m_healthFast.gameObject.SetActive(true);

    hudData.m_healthSlow.m_width = healthBarWidth;
    hudData.m_healthSlow.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.zero)
        .SetPosition(Vector2.zero)
        .SetSizeDelta(Vector2.zero);

    hudData.m_healthSlow.m_bar
        .SetAnchorMin(new(0f, 0.5f))
        .SetAnchorMax(new(0f, 0.5f))
        .SetPivot(new(0f, 0.5f))
        .SetPosition(Vector2.zero)
        .SetSizeDelta(new(healthBarWidth, healthBarHeight));

    hudData.m_healthSlow.gameObject.SetActive(true);

    // Turn these off as we'll just colorize the regular bar.
    hudData.m_healthFastFriendly.Ref()?.gameObject.SetActive(false);
  }

  public static void SetupAlerted(EnemyHud.HudData hudData) {
    if (hudData.m_alerted) {
      TextMeshProUGUI alertedText = hudData.m_alerted.GetComponent<TextMeshProUGUI>();

      hudData.m_alerted.SetParent(hudData.m_name.transform, worldPositionStays: false);
      hudData.m_alerted
          .SetAnchorMin(new(0.5f, 1f))
          .SetAnchorMax(new(0.5f, 1f))
          .SetPivot(new(0.5f, 0f))
          .SetPosition(Vector2.zero)
          .SetSizeDelta(alertedText.GetPreferredValues());

      hudData.m_alerted.gameObject.SetActive(!EnemyHudUseNameForStatus.Value);
    }
  }

  public static void SetupAware(EnemyHud.HudData hudData) {
    if (hudData.m_aware) {
      hudData.m_aware.SetParent(hudData.m_name.transform, worldPositionStays: false);
      hudData.m_aware.SetAnchorMin(new(0.5f, 1f))
          .SetAnchorMax(new(0.5f, 1f))
          .SetPivot(new(0.5f, 0f))
          .SetPosition(Vector2.zero)
          .SetSizeDelta(new(30f, 30f));

      hudData.m_aware.gameObject.SetActive(!EnemyHudUseNameForStatus.Value);
    }
  }

  public static TextMeshProUGUI CreateHealthText(
      EnemyHud.HudData hudData, Transform parentTransform, int healthTextFontSize, Color healthTextFontColor) {
    TextMeshProUGUI label = Object.Instantiate(hudData.m_name);
    label.transform.SetParent(parentTransform, worldPositionStays: false);
    label.name = "HealthText";

    label.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0.5f, 0.5f))
        .SetPosition(Vector2.zero);

    label.text = string.Empty;
    label.fontSize = healthTextFontSize;
    label.color = healthTextFontColor;
    label.alignment = TextAlignmentOptions.Center;
    label.enableAutoSizing = false;
    label.textWrappingMode = TextWrappingModes.NoWrap;
    label.overflowMode = TextOverflowModes.Overflow;

    return label;
  }

  public static void SetupLevel(EnemyHud.HudData hudData, Transform healthTransform) {
    if (!EnemyLevelUseVanillaStar.Value || hudData.m_character.m_level > (hudData.m_character.IsBoss() ? 1 : 3)) {
      CreateEnemyLevelText(hudData, healthTransform);

      hudData.m_level2.Ref()?.gameObject.SetActive(false);
      hudData.m_level3.Ref()?.gameObject.SetActive(false);
    } else {
      SetupEnemyLevelStars(hudData, healthTransform);
    }
  }

  public static TextMeshProUGUI CreateEnemyLevelText(EnemyHud.HudData hudData, Transform healthTransform) {
    TextMeshProUGUI label = Object.Instantiate(hudData.m_name);
    label.name = "LevelText";

    label.text = string.Empty;
    label.fontSize = Mathf.Clamp((int) hudData.m_name.fontSize, EnemyLevelTextMinFontSize.Value, 64f);
    label.color = new(1f, 0.85882f, 0.23137f, 1f);
    label.enableAutoSizing = false;
    label.overflowMode = TextOverflowModes.Overflow;

    if (EnemyLevelShowByName.Value) {
      label.transform.SetParent(hudData.m_name.transform, worldPositionStays: false);

      label.GetComponent<RectTransform>()
          .SetAnchorMin(new(1f, 0.5f))
          .SetAnchorMax(new(1f, 0.5f))
          .SetPivot(new(0f, 0.5f))
          .SetPosition(new(5f, 0f))
          .SetSizeDelta(new(100f, label.GetPreferredValues().y + 5f));

      label.alignment = TextAlignmentOptions.Left;
      label.textWrappingMode = TextWrappingModes.NoWrap;
    } else {
      label.transform.SetParent(healthTransform, worldPositionStays: false);

      Vector2 sizeDelta = healthTransform.GetComponent<RectTransform>().sizeDelta;
      sizeDelta.y = label.GetPreferredValues().y * 2f;

      label.GetComponent<RectTransform>()
          .SetAnchorMin(Vector2.zero)
          .SetAnchorMax(Vector2.zero)
          .SetPivot(Vector2.zero)
          .SetPosition(new(0f, 0f - sizeDelta.y - 2f))
          .SetSizeDelta(sizeDelta);

      label.alignment = TextAlignmentOptions.TopLeft;
      label.textWrappingMode = TextWrappingModes.Normal;
    }

    int stars = hudData.m_character.m_level - 1;

    label.SetText(
        stars <= EnemyLevelStarCutoff.Value
            ? string.Concat(Enumerable.Repeat(EnemyLevelStarSymbol.Value, stars))
            : $"{stars}{EnemyLevelStarSymbol.Value}");

    return label;
  }

  public static void SetupEnemyLevelStars(EnemyHud.HudData hudData, Transform healthTransform) {
    if (hudData.m_level2) {
      if (EnemyLevelShowByName.Value) {
        hudData.m_level2.SetParent(hudData.m_name.transform);
        hudData.m_level2
            .SetAnchorMin(new(1f, 0.5f))
            .SetAnchorMax(new(1f, 0.5f))
            .SetPivot(new(0f, 0.5f))
            .SetPosition(new(12f, 0f))
            .SetSizeDelta(Vector2.zero);
      } else {
        hudData.m_level2.SetParent(healthTransform, worldPositionStays: false);
        hudData.m_level2
            .SetAnchorMin(Vector2.zero)
            .SetAnchorMax(Vector2.zero)
            .SetPivot(Vector2.zero)
            .SetPosition(new(7.5f, -10f))
            .SetSizeDelta(Vector2.zero);
      }

      hudData.m_level2.gameObject.SetActive(hudData.m_character.GetLevel() == 2);
    }

    if (hudData.m_level3) {
      if (EnemyLevelShowByName.Value) {
        hudData.m_level3.SetParent(hudData.m_name.transform, worldPositionStays: false);
        hudData.m_level3
            .SetAnchorMin(new(1f, 0.5f))
            .SetAnchorMax(new(1f, 0.5f))
            .SetPivot(new(0f, 0.5f))
            .SetPosition(new(20f, 0f))
            .SetSizeDelta(Vector2.zero);
      } else {
        hudData.m_level3.SetParent(healthTransform, worldPositionStays: false);
        hudData.m_level3
            .SetAnchorMin(Vector2.zero)
            .SetAnchorMax(Vector2.zero)
            .SetPivot(Vector2.zero)
            .SetPosition(new(15.5f, -10f))
            .SetSizeDelta(Vector2.zero);
      }

      hudData.m_level3.gameObject.SetActive(hudData.m_character.GetLevel() == 3);
    }
  }

  static readonly List<Character> _keysToRemove = new(capacity: 12);

  public static void UpdateHuds(ref EnemyHud enemyHud, ref Player player, ref Sadle sadle, float dt) {
    Camera camera = Utils.GetMainCamera();

    if (!camera) {
      return;
    }

    Character sadleCharacter = sadle.Ref()?.GetCharacter();
    Character hoverCharacter = player.Ref()?.GetHoverCreature();

    _keysToRemove.Clear();

    int screenWidth = Screen.width;
    int screenHeight = Screen.height;

    foreach (KeyValuePair<Character, EnemyHud.HudData> pair in enemyHud.m_huds) {
      EnemyHud.HudData hudData = pair.Value;
      Character character = hudData.m_character;

      if (!character || !enemyHud.TestShow(character, isVisible: true) || character == sadleCharacter) {
        _keysToRemove.Add(character);
        Object.Destroy(hudData.m_gui);
        continue;
      }

      if (character == hoverCharacter) {
        hudData.m_hoverTimer = 0f;
      }

      hudData.m_hoverTimer += dt;

      if (character.IsPlayer()
          || character.IsBoss()
          || hudData.m_isMount
          || hudData.m_hoverTimer < enemyHud.m_hoverShowDuration) {
        hudData.m_gui.SetActive(true);
        hudData.m_name.text = Localization.m_instance.Localize(character.GetHoverName());

        if (character.IsPlayer()) {
          hudData.m_name.SetColor(
              character.IsPVPEnabled() ? PlayerHudNameTextPvPColor.Value : PlayerHudNameTextColor.Value);
        } else if (character.m_baseAI && !character.IsBoss()) {
          bool aware = character.m_baseAI.HaveTarget();
          bool alerted = character.m_baseAI.IsAlerted();

          if (EnemyHudUseNameForStatus.Value) {
            hudData.m_name.SetColor(
                (aware || alerted)
                    ? (alerted ? EnemyHudNameTextAlertedColor.Value : EnemyHudNameTextAwareColor.Value)
                    : EnemyHudHealthTextColor.Value);
          } else {
            hudData.m_alerted.gameObject.SetActive(alerted);
            hudData.m_aware.gameObject.SetActive(aware && !alerted);
          }
        }
      } else {
        hudData.m_gui.SetActive(false);
      }

      float currentHealth = character.GetHealth();
      float maxHealth = character.GetMaxHealth();
      float healthPercentage = currentHealth / maxHealth;

      if (ShowEnemyHealthValue.Value && HealthTextCache.TryGetValue(hudData, out TextMeshProUGUI healthText)) {
        if (EnemyHudHealthTextShowInfiniteHealth.Value
            && currentHealth > EnemyHudHealthTextInfiniteHealthThreshold.Value) {
          healthText.SetText("\u221E");
        } else if (EnemyHudHealthTextShowMaxHealth.Value) {
          healthText.SetText($"{currentHealth:N0} / {maxHealth:N0}");
        } else {
          healthText.SetText($"{currentHealth:N0}");
        }
      }

      hudData.m_healthSlow.SetValue(healthPercentage);
      hudData.m_healthFast.SetValue(healthPercentage);

      if (hudData.m_healthFastFriendly) {
        hudData.m_healthFast.SetColor(
            character.IsTamed()
                ? EnemyHudHealthBarTamedColor.Value
                : player && !BaseAI.IsEnemy(player, character)
                      ? EnemyHudHealthBarFriendlyColor.Value
                      : EnemyHudHealthBarColor.Value);
      }

      if (hudData.m_isMount && sadle) {
        float currentStamina = sadle.GetStamina();
        float maxStamina = sadle.GetMaxStamina();

        hudData.m_stamina.SetValue(currentStamina / maxStamina);
        hudData.m_healthText.text = $"{currentHealth:N0}";
        hudData.m_staminaText.text = $"{currentStamina:N0}";
      }

      if (hudData.m_gui.activeSelf && (FloatingBossHud.Value || !character.IsBoss())) {
        Vector3 position;

        if (character.IsPlayer()) {
          position = character.GetHeadPoint() + PlayerHudPositionOffset.Value;
        } else if (character.IsBoss()) {
          position = character.GetTopPoint() + BossHudPositionOffset.Value;
        } else if (hudData.m_isMount && player) {
          position = player.transform.transform.position - (player.transform.up * 0.5f);
        } else {
          position = character.GetTopPoint() + EnemyHudPositionOffset.Value;
        }

        Vector3 point = camera.WorldToScreenPointScaled(position);

        if (point.x < 0f || point.x > screenWidth || point.y < 0f || point.y > screenHeight || point.z > 0f) {
          hudData.m_gui.transform.position = point;
          hudData.m_gui.SetActive(true);
        } else {
          hudData.m_gui.SetActive(false);
        }
      }
    }

    for (int i = 0; i < _keysToRemove.Count; i++) {
      enemyHud.m_huds.Remove(_keysToRemove[i]);
    }

    _keysToRemove.Clear();
  }
}
