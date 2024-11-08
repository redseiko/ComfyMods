namespace ComfyAutoRepair;

using UnityEngine;

using static PluginConfig;

public static class RepairManager {
  public static bool RepairAllItems(Player player, CraftingStation craftingStation, bool checkUsable = true) {
    if (!player) {
      return false;
    }

    bool noPlacementCost = player.NoCostCheat();

    if ((!noPlacementCost && !craftingStation)
        || (checkUsable && !craftingStation.CheckUsable(player, showMessage: false))) {
      return false;
    }

    string craftingStationName = craftingStation.m_name;
    int craftingStationLevel = Mathf.Min(craftingStation.GetLevel(checkExtensions: true), 4);

    int repairCount = 0;
    bool showRepairMessage = ShowVanillaRepairMessage.Value;

    foreach (ItemDrop.ItemData item in player.GetInventory().GetAllItems()) {
      if (!item.m_shared.m_useDurability || !item.m_shared.m_canBeReparied) {
        continue;
      }

      float maxDurablity = item.GetMaxDurability(item.m_quality);

      if (item.m_durability >= maxDurablity) {
        continue;
      }

      if (noPlacementCost || CanRepairItem(item, craftingStationName, craftingStationLevel)) {
        player.RaiseSkill(Skills.SkillType.Crafting, 1f - (item.m_durability / maxDurablity));

        item.m_durability = maxDurablity;

        if (showRepairMessage) {
          player.Message(
              MessageHud.MessageType.Center,
              Localization.instance.Localize("$msg_repaired", item.m_shared.m_name));
        }

        repairCount++;
      }
    }

    if (repairCount > 0) {
      ComfyAutoRepair.LogInfo($"Repaired {repairCount} items.");

      if (craftingStation) {
        craftingStation.m_repairItemDoneEffects?.Create(craftingStation.transform.position, Quaternion.identity);
      }
    }

    return true;
  }

  public static bool CanRepairItem(ItemDrop.ItemData item, string craftingStationName, int craftingStationLevel) {
    return
        RecipeManager.TryGetRecipe(item, out Recipe recipe)
        && (recipe.m_craftingStation || recipe.m_repairStation)
        && ((recipe.m_repairStation && recipe.m_repairStation.m_name == craftingStationName)
            || (recipe.m_craftingStation && recipe.m_craftingStation.m_name == craftingStationName)
            || (item.m_worldLevel < Game.m_worldLevel))
        && (craftingStationLevel >= recipe.m_minStationLevel);
  }
}
