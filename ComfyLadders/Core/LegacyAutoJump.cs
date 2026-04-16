namespace ComfyLadders;

using UnityEngine;

public static class LegacyAutoJump {
  static bool _hasAutoJumped = false;

  // Mimics behaviour of legacy BetterLadders once-per-FixedUpdate logic.
  public static void OnFixedUpdate() {
    _hasAutoJumped = false;
  }

  // Mimics behaviour of legacy BetterLadders ladder-movement.
  public static void OnAutoJump(AutoJumpLedge autoJumpLedge, Character character) {
    if (_hasAutoJumped) {
      return;
    }

    _hasAutoJumped = true;

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
