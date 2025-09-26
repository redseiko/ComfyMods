namespace GetOffMyLawn;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

public sealed class TargetPieceHealthStatusEffect : StatusEffect {
  public static readonly TargetPieceHealthStatusEffect Instance = CreateStatusEffect();

  public static TargetPieceHealthStatusEffect CreateStatusEffect() {
    TargetPieceHealthStatusEffect statusEffect = CreateInstance<TargetPieceHealthStatusEffect>();
    statusEffect.name = nameof(TargetPieceHealthStatusEffect);

    statusEffect.m_name = "GOML";
    statusEffect.m_icon = Resources.FindObjectsOfTypeAll<Sprite>().FirstByNameOrThrow("hammer_icon_gold");
    statusEffect.m_ttl = 0f;

    return statusEffect;
  }

  string _targetPieceHealthText = string.Empty;

  public override void SetLevel(int itemLevel, float skillLevel) {
    base.SetLevel(itemLevel, skillLevel);

    _targetPieceHealthText = $"{TargetPieceHealth.Value}";
    m_isNew = true;
  }

  public override string GetIconText() {
    return _targetPieceHealthText;
  }
}
