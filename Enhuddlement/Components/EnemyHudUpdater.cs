using System.Collections.Generic;
using System.Runtime.CompilerServices;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using static Enhuddlement.PluginConfig;

namespace Enhuddlement {
  public static class EnemyHudUpdater {
    static readonly List<Character> _keysToRemove = new(capacity: 12);

    public static void UpdateHuds(
        ref EnemyHud enemyHud,
        ref Player player,
        ref Sadle sadle,
        float dt,
        ConditionalWeakTable<EnemyHud.HudData, TextMeshProUGUI> healthTextCache) {
      Camera camera = Utils.GetMainCamera();

      if (!camera) {
        return;
      }

      Character sadleCharacter = sadle.Ref()?.GetCharacter();
      Character hoverCharacter = player.Ref()?.GetHoverCreature();

      _keysToRemove.Clear();

      foreach (KeyValuePair<Character, EnemyHud.HudData> pair in enemyHud.m_huds) {
        EnemyHud.HudData hudData = pair.Value;
        Character character = hudData.m_character;

        if (!character || !enemyHud.TestShow(character, isVisible: true) || character == sadleCharacter) {
          _keysToRemove.Add(character);
          UnityEngine.Object.Destroy(hudData.m_gui);
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

        if (ShowEnemyHealthValue.Value && healthTextCache.TryGetValue(hudData, out TextMeshProUGUI healthText)) {
          healthText.SetText($"{currentHealth:N0} / {maxHealth:N0}");
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

          if (point.x < 0f || point.x > Screen.width || point.y < 0f || point.y > Screen.height || point.z > 0f) {
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
}
