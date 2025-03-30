namespace LetMePlay;

using System;

using ComfyLib;

using UnityEngine;

public static class ItemDropUtils {
  public static readonly ResourceCache<Sprite> SpriteCache = new();

  public static void ValidateIcons(ItemDrop.ItemData itemData, int variant, int iconCount) {
    if (variant >= 0 && variant < iconCount) {
      return;
    }

    if (iconCount > 0) {
      itemData.m_variant = 0;
      return;
    }

    ItemDrop.ItemData.SharedData sharedData = itemData.m_shared;

    Array.Resize(ref sharedData.m_icons, variant + 1);
    sharedData.m_icons[variant] = SpriteCache.GetResource("hammer_icon_small");

    sharedData.m_name = itemData.m_dropPrefab.name;
    sharedData.m_description = $"Non-player item: {sharedData.m_name}";
    sharedData.m_itemType = ItemDrop.ItemData.ItemType.Misc;

    if (itemData.m_crafterID == default) {
      itemData.m_crafterID = 12345678L;
    }

    if (string.IsNullOrEmpty(itemData.m_crafterName)) {
      itemData.m_crafterName = "redseiko.valheim.letmeplay";
    }
  }
}
