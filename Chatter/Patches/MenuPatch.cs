using HarmonyLib;

namespace Chatter {
  [HarmonyPatch(typeof(Menu))]
  static class MenuPatch {
    //  [HarmonyPostfix]
    //  [HarmonyPatch(nameof(Menu.Show))]
    //  static void ShowPostfix() {
    //    if (IsModEnabled.Value) {
    //      ChatPanel?.ToggleGrabber(true);
    //      ChatPanel?.SetPanelSize(ChatPanelSize.Value);
    //    }
    //  }

    //  [HarmonyPostfix]
    //  [HarmonyPatch(nameof(Menu.Hide))]
    //  static void HidePostfix() {
    //    if (IsModEnabled.Value) {
    //      ChatPanel?.ToggleGrabber(false);
    //      ChatPanel?.SetPanelSize(ChatPanelSize.Value);
    //      ChatPanel?.SetVerticalScrollPosition(0f);
    //    }
    //  }
  }
}
