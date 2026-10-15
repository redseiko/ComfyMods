namespace ComfySigns;

using ComfyLib;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(TextInput))]
static class TextInputPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(TextInput.Awake))]
  static void AwakePostfix(TextInput __instance) {
    if (IsModEnabled.Value && __instance.m_inputField) {
      SetupTextInput(__instance);
    }
  }

  static void SetupTextInput(TextInput textInput) {
    textInput.m_panel.GetOrAddComponent<TextInputPanelDragger>();
    textInput.m_inputField.richText = false;
  }
}
