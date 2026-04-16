namespace ComfyLadders;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

public static class LadderManager {
  public const int AshlandSteepstairHash = -571355724;  // Ashland_Steepstair
  public const int GoblinStepladderHash = 70596228;     // goblin_stepladder
  public const int GraustenStoneLadderHash = 913259269; // Piece_grausten_stone_ladder
  public const int WoodStepladderHash = -354552934;     // wood_stepladder

  public const int IsEligibleOverrideHash = 1026132609; // AutoJumpLedge.m_isEligibleOverride
  public const int IsEligibleOverrideUseVanilla = 1;

  public static bool IsEligibleLadder(AutoJumpLedge autoJumpLedge) {
    if (!autoJumpLedge.TryGetComponentInParent(out ZNetView netView) || !netView.IsValid()) {
      return false;
    }

    if (ZDOExtraData.GetInt(netView.m_zdo.m_uid, IsEligibleOverrideHash, out int isEligibleOverride)
        && isEligibleOverride == IsEligibleOverrideUseVanilla) {
      return false;
    }

    int prefabHash = netView.m_zdo.m_prefab;

    if (prefabHash == AshlandSteepstairHash) {
      return AshlandSteepstairIsEligible.Value;
    }

    if (prefabHash == GoblinStepladderHash) {
      return GoblinStepladderIsEligible.Value;
    }

    if (prefabHash == GraustenStoneLadderHash) {
      return GraustenStoneLadderIsEligible.Value;
    }

    if (prefabHash == WoodStepladderHash) {
      return WoodStepladderIsEligible.Value;
    }

    return false;
  }
}
