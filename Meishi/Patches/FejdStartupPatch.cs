namespace Meishi;

using HarmonyLib;

using UnityEngine;

[HarmonyPatch(typeof(FejdStartup))]
static class FejdStartupPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.Start))]
  static void StartPostfix(FejdStartup __instance) {
    Transform menuTransform = __instance.m_characterSelectScreen.transform;

    MeishiController.RegisterParent(menuTransform);
    MeishiController.CreateTestButton(menuTransform);
  }
}
