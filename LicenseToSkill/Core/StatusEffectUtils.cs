namespace LicenseToSkill;

using UnityEngine;

using static PluginConfig;

public static class StatusEffectUtils {
  public const float DefaultHardDeathCooldown = 600f;

  public static float GetConfigHardDeathCooldown() {
    return HardDeathCooldownOverride.Value * 60f;
  }

  public static void SetHardDeathCoolDown() {
    if (Player.m_localPlayer) {
      UpdateHardDeathCooldownTimer(Player.m_localPlayer);
    }
  }

  public static void UpdateHardDeathCooldownTimer(Player player) {
    if (player.TryGetStatusEffect(SEMan.s_statusEffectSoftDeath, out StatusEffect softDeath)) {
      softDeath.m_ttl = GetConfigHardDeathCooldown() - player.m_timeSinceDeath;
    }

    player.m_hardDeathCooldown = IsModEnabled.Value ? GetConfigHardDeathCooldown() : DefaultHardDeathCooldown;
  }

  public static bool TryGetStatusEffect(this Player player, int nameHash, out StatusEffect statusEffect) {
    foreach (StatusEffect effect in player.m_seman.m_statusEffects) {
      if (effect && effect.NameHash() == nameHash) {
        statusEffect = effect;
        return true;
      }
    }

    statusEffect = default;
    return false;
  }

  public static float GetSkillLossFactor() {
    return Mathf.Min(SkillLossPercentOverride.Value * 0.01f, Game.m_skillReductionRate);
  }
}
