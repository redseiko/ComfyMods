namespace ComfyAutoRepair;

using HarmonyLib;

using static PluginConfig;

[HarmonyPatch(typeof(ObjectDB))]
static class ObjectDBPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(ObjectDB.CopyOtherDB))]
  static void CopyOtherDBPostfix() {
    RecipeManager.Reset();
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(ObjectDB.GetRecipe))]
  static bool GetRecipePrefix(InventoryGui __instance, ItemDrop.ItemData item, ref Recipe __result) {
    if (IsModEnabled.Value) {
      RecipeManager.TryGetRecipe(item, out __result);
      return false;
    }

    return true;
  }
}
