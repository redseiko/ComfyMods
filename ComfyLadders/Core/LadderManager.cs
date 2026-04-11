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

  // Mimics behaviour of legacy BetterLadders mod.
  public static void OnAutoJumpLegacy(AutoJumpLedge autoJumpLedge, Character character) {
    float characterY = character.transform.rotation.eulerAngles.y;
    float ladderY = autoJumpLedge.transform.rotation.eulerAngles.y;

    if (Mathf.Abs(Mathf.DeltaAngle(ladderY, characterY)) > 12f) {
      return;
    }

    Vector3 characterPosition = character.transform.position;
    characterPosition.y += (character.m_running ? 0.08f : 0.06f);

    character.transform.position = characterPosition + character.transform.forward * 0.08f;
  }
}
