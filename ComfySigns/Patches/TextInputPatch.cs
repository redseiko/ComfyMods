namespace ComfySigns;

using ComfyLib;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(TextInput))]
static class TextInputPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(TextInput.Awake))]
  static void AwakePostfix(ref TextInput __instance) {
    if (IsModEnabled.Value && __instance.m_inputField) {
      __instance.m_panel.GetOrAddComponent<TextInputPanelDragger>();
      __instance.m_inputField.richText = false;
    }
  }
}
