namespace ComfyAutoRepair;

using HarmonyLib;

[HarmonyPatch(typeof(ObjectDB))]
static class ObjectDBPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ObjectDB.CopyOtherDB))]
  static void CopyOtherDBPostfix() {
    RecipeManager.Reset();
  }
}
