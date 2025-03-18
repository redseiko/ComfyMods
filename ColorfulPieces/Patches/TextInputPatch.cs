namespace ColorfulPieces;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(TextInput))]
static class TextInputPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(TextInput.IsVisible))]
  static void IsVisiblePostfix(ref bool __result) {
    if (!__result && IsModEnabled.Value && ColorPickerController.HasVisibleInstance()) {
      __result = true;
    }
  }
}
