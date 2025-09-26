namespace GetOffMyLawn;

public static class LawnManager {
  public static void ToggleStatusEffectIndicator(bool toggleOn) {
    if (!Player.m_localPlayer) {
      return;
    }

    SEMan statusEffectManager = Player.m_localPlayer.m_seman;

    if (toggleOn) {
      statusEffectManager.AddStatusEffect(TargetPieceHealthStatusEffect.Instance, resetTime: true);
    } else {
      statusEffectManager.RemoveStatusEffect(TargetPieceHealthStatusEffect.Instance.NameHash());
    }
  }
}
