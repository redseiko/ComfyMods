namespace ComfyAutoRepair;

using System.Collections.Generic;

public static class RecipeManager {
  static readonly Dictionary<string, Recipe> _recipeByItemNameCache = [];

  public static void Reset() {
    _recipeByItemNameCache.Clear();
  }

  public static bool TryGetRecipe(ItemDrop.ItemData item, out Recipe recipe) {
    string itemName = item.m_shared.m_name;

    if (!_recipeByItemNameCache.TryGetValue(itemName, out recipe)) {
      recipe = GetRecipeByItemName(itemName);
      _recipeByItemNameCache[itemName] = recipe;
    }

    return recipe;
  }

  public static Recipe GetRecipeByItemName(string itemName) {
    foreach (Recipe recipe in ObjectDB.m_instance.m_recipes) {
      if (recipe.m_item && recipe.m_item.m_itemData.m_shared.m_name == itemName) {
        return recipe;
      }
    }

    return default;
  }
}
