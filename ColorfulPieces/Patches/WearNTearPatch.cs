namespace ColorfulPieces;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(WearNTear))]
static class WearNTearPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(WearNTear.Awake))]
  static void AwakePostfix(WearNTear __instance) {
    if (IsModEnabled.Value) {
      __instance.gameObject.AddComponent<PieceColor>();
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(WearNTear.Highlight))]
  static bool HighlightPrefix(WearNTear __instance) {
    if (IsModEnabled.Value && __instance.TryGetComponent(out PieceColor pieceColor)) {
      Color color = GetSupportColor(__instance.GetSupportColorValue());
      pieceColor.OverrideColors(color, color * 0.4f);

      __instance.CancelInvoke(nameof(WearNTear.ResetHighlight));
      __instance.Invoke(nameof(WearNTear.ResetHighlight), 0.2f);

      return false;
    }

    return true;
  }

  static Color GetSupportColor(float supportColorValue) {
    Color color = new(0.6f, 0.8f, 1f);

    if (supportColorValue >= 0f) {
      color = Color.Lerp(PieceStabilityMinColor.Value, PieceStabilityMaxColor.Value, supportColorValue);

      Color.RGBToHSV(color, out float h, out _, out _);

      float s = Mathf.Lerp(1f, 0.5f, supportColorValue);
      float v = Mathf.Lerp(1.2f, 0.9f, supportColorValue);

      color = Color.HSVToRGB(h, s, v);
    }

    return color;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(WearNTear.ResetHighlight))]
  static bool ResetHighlightPrefix(WearNTear __instance) {
    if (IsModEnabled.Value && __instance.TryGetComponent(out PieceColor pieceColor)) {
      pieceColor.UpdateColors(forceUpdate: true);
      return false;
    }

    return true;
  }
}
