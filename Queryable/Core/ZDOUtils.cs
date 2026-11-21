namespace Queryable;

using System.Collections.Generic;
using System.IO;
using System.Text;

using Database;

using UnityEngine;

public static class ZDOUtils {
  public static readonly int EpochTimeCreatedHash = "epochTimeCreated".GetStableHashCode();
  public static readonly int OriginalUidUserHash = "originalUid_u".GetStableHashCode();
  public static readonly int OriginalUidIdHash = "originalUid_i".GetStableHashCode();

  public static DataObject CreateDataObject(ZDO zdo) {
    Vector3 position = zdo.m_position;

    DataObject dataObject =
        new DataObject() {
          PrefabHash = zdo.m_prefab,
          PositionX = Mathf.RoundToInt(position.x),
          PositionY = Mathf.RoundToInt(position.y),
          PositionZ = Mathf.RoundToInt(position.z),
        };

    ZDOID zid = zdo.m_uid;

    if (ZDOExtraData.GetLong(zid, ZDOVars.s_creator, out long creatorId)) {
      dataObject.CreatorId = creatorId;
    }

    if (ZDOExtraData.GetInt(zid, ZDOVars.s_cheated, out int cheated)) {
      dataObject.Cheated = cheated;
    }

    if (ZDOExtraData.GetLong(zid, EpochTimeCreatedHash, out long epochTimeCreated)) {
      dataObject.EpochTimeCreated = epochTimeCreated;
    }

    if (ZDOExtraData.GetLong(zid, OriginalUidUserHash, out long originalUidUser)
        && ZDOExtraData.GetLong(zid, OriginalUidIdHash, out long originalUidId)) {
      dataObject.OriginalUid = $"{originalUidUser}:{originalUidId}";
    }

    return dataObject;
  }

  public static DataObject CreateDataObject(ChunkZDO zdo) {
    Vector3 position = zdo.Position;

    DataObject dataObject =
        new DataObject() {
          PrefabHash = zdo.PrefabHash,
          PositionX = Mathf.RoundToInt(position.x),
          PositionY = Mathf.RoundToInt(position.y),
          PositionZ = Mathf.RoundToInt(position.z),
        };

    if (zdo.Longs.TryGetValue(ZDOVars.s_creator, out long creatorId)) {
      dataObject.CreatorId = creatorId;
    }

    if (zdo.Ints.TryGetValue(ZDOVars.s_cheated, out int cheated)) {
      dataObject.Cheated = cheated;
    }

    if (zdo.Longs.TryGetValue(EpochTimeCreatedHash, out long epochTimeCreated)) {
      dataObject.EpochTimeCreated = epochTimeCreated;
    }

    if (zdo.Longs.TryGetValue(OriginalUidUserHash, out long originalUidUser)
        && zdo.Longs.TryGetValue(OriginalUidIdHash, out long originalUidId)) {
      dataObject.OriginalUid = $"{originalUidUser}:{originalUidId}";
    }

    return dataObject;
  }

  public static bool TryReadContainerItems(ZDO zdo, List<ContainerItem> items) {
    if (!ZDOExtraData.GetByteArray(zdo.m_uid, ZDOVars.s_items, out byte[] itemsData)) {
      return false;
    }

    return TryReadContainerItems(itemsData, items);
  }

  public static bool TryReadContainerItems(ChunkZDO zdo, List<ContainerItem> items) {
    if (!zdo.ByteArrays.TryGetValue(ZDOVars.s_items, out byte[] itemsData)) {
      return false;
    }

    return TryReadContainerItems(itemsData, items);
  }

  public static bool TryReadContainerItems(byte[] itemsData, List<ContainerItem> items) {
    if (itemsData == null || itemsData.Length <= 0) {
      return false;
    }

    using MemoryStream stream = new(itemsData, writable: false);
    using BinaryReader reader = new(stream, Encoding.UTF8);

    int version = reader.ReadInt32();

    if (version < (int) Version.Item.Smaller) {
      return false;
    }

    int count = reader.ReadUInt16();

    if (count <= 0) {
      return false;
    }

    items.Clear();

    for (int i = 0; i < count; i++) {
      items.Add(ReadContainerItem(reader, version));
    }

    return true;
  }

  static int ReadNumItems(BinaryReader reader) {
    int count = reader.ReadByte();

    if ((count & 128) != 0) {
      count = ((count & 127) << 8) | reader.ReadByte();
    }

    return count;
  }

  static void ReadItemFields(
      BinaryReader reader,
      int version,
      out int prefabHash,
      out float durability,
      out int gridPositionX,
      out int gridPositionY,
      out int worldLevel,
      out bool isPickedUp,
      out bool isEquipped,
      out int quality,
      out int stack,
      out int variant,
      out long crafterId,
      out string crafterName,
      out string customDataJson,
      out bool isCheated) {
    durability = reader.ReadInt32() * 0.01f;
    gridPositionX = reader.ReadByte();
    gridPositionY = reader.ReadByte();
    worldLevel = reader.ReadByte();

    byte flags = reader.ReadByte();
    isPickedUp = (flags & 1) != 0;
    isEquipped = (flags & 2) != 0;
    quality = (flags & 4) == 0 ? 1 : reader.ReadUInt16();
    stack = (flags & 8) == 0 ? 1 : reader.ReadUInt16();
    variant = (flags & 16) == 0 ? 0 : reader.ReadInt32();
    crafterId = (flags & 32) == 0 ? 0L : reader.ReadInt64();
    crafterName = (flags & 32) == 0 ? string.Empty : reader.ReadString();
    prefabHash = (flags & 64) == 0 ? 0 : reader.ReadInt32();

    int customDataCount = (flags & 128) == 0 ? 0 : ReadNumItems(reader);
    customDataJson = customDataCount > 0 ? ReadItemCustomDataToJson(reader, customDataCount) : string.Empty;

    isCheated = false;

    if (version >= (int) Version.Item.ChunksNCheats || version == (int) Version.Item.AbandonedDN) {
      byte flags2 = reader.ReadByte();

      if (version >= (int) Version.Item.NewCheats) {
        isCheated = (flags2 & 1) != 0;
      }
    }
  }

  static ContainerItem ReadContainerItem(BinaryReader reader, int version) {
    ReadItemFields(
        reader,
        version,
        out int prefabHash,
        out float durability,
        out int gridPositionX,
        out int gridPositionY,
        out int worldLevel,
        out bool isPickedUp,
        out bool isEquipped,
        out int quality,
        out int stack,
        out int variant,
        out long crafterId,
        out string crafterName,
        out string customDataJson,
        out bool isCheated);

    string name =
        prefabHash == 0
            ? string.Empty
            : (PrefabUtils.TryGetPrefabName(prefabHash, out string prefabName) ? prefabName : prefabHash.ToString());

    return new() {
      Name = name,
      Stack = stack,
      Durability = durability,
      GridPositionX = gridPositionX,
      GridPositionY = gridPositionY,
      IsEquipped = isEquipped,
      Quality = quality,
      Variant = variant,
      CrafterId = crafterId,
      CrafterName = crafterName,
      CustomDataJson = customDataJson,
      WorldLevel = worldLevel,
      IsPickedUp = isPickedUp,
      IsCheated = isCheated,
    };
  }

  static ItemDropItem ReadItemDropItem(BinaryReader reader, int version) {
    ReadItemFields(
        reader,
        version,
        out int prefabHash,
        out float durability,
        out int _,
        out int _,
        out int worldLevel,
        out bool isPickedUp,
        out bool _,
        out int quality,
        out int stack,
        out int variant,
        out long crafterId,
        out string crafterName,
        out string customDataJson,
        out bool isCheated);

    string? name =
        prefabHash == 0
            ? null
            : (PrefabUtils.TryGetPrefabName(prefabHash, out string prefabName) ? prefabName : prefabHash.ToString());

    return new() {
      Name = name,
      Durability = durability,
      Stack = stack,
      Quality = quality,
      Variant = variant,
      CrafterId = crafterId,
      CrafterName = crafterName,
      CustomDataJson = customDataJson,
      WorldLevel = worldLevel,
      IsPickedUp = isPickedUp,
      IsCheated = isCheated,
    };
  }

  static readonly StringBuilder _jsonBuilder = new(capacity: 128);

  public static string ReadItemCustomDataToJson(BinaryReader reader, int count) {
    if (count <= 0) {
      return string.Empty;
    }

    _jsonBuilder
        .Clear()
        .Append("{")
        .Append($"\"{reader.ReadString()}\":\"{reader.ReadString()}\"");

    for (int i = 1; i < count; i++) {
      _jsonBuilder.Append($",\"{reader.ReadString()}\": \"{reader.ReadString()}\"");
    }

    _jsonBuilder.Append("}");

    string jsonResult = _jsonBuilder.ToString();

    _jsonBuilder.Clear();

    return jsonResult;
  }

  static bool TryReadItemDropItem(ZDOID zid, int itemDataHash, out ItemDropItem item) {
    if (!ZDOExtraData.GetByteArray(zid, itemDataHash, out byte[] itemDataBytes)) {
      item = default;
      return false;
    }

    return TryReadItemDropItem(itemDataBytes, out item);
  }

  static bool TryReadItemDropItem(ChunkZDO zdo, int itemDataHash, out ItemDropItem item) {
    if (!zdo.ByteArrays.TryGetValue(itemDataHash, out byte[] itemDataBytes)) {
      item = default;
      return false;
    }

    return TryReadItemDropItem(itemDataBytes, out item);
  }

  static bool TryReadItemDropItem(byte[] itemDataBytes, out ItemDropItem item) {
    if (itemDataBytes == null || itemDataBytes.Length <= 2) {
      item = default;
      return false;
    }

    using MemoryStream stream = new(itemDataBytes, writable: false);
    using BinaryReader reader = new(stream, Encoding.UTF8);

    int version = reader.ReadByte();

    if (version >= (int) Version.Item.Smaller) {
      item = ReadItemDropItem(reader, version);
      return true;
    }

    item = default;
    return false;
  }

  static bool TryGetAttachedItemName(ZDOID zid, int itemKeyHash, out string itemName) {
    if (ZDOExtraData.GetInt(zid, itemKeyHash, out int itemHash) && itemHash != 0) {
      itemName = PrefabUtils.TryGetPrefabName(itemHash, out string prefabName) ? prefabName : itemHash.ToString();
      return true;
    }

    if (ZDOExtraData.GetString(zid, itemKeyHash, out itemName) && !string.IsNullOrEmpty(itemName)) {
      return true;
    }

    itemName = default;
    return false;
  }

  static bool TryGetAttachedItemName(ChunkZDO zdo, int itemKeyHash, out string itemName) {
    if (zdo.Ints.TryGetValue(itemKeyHash, out int itemHash) && itemHash != 0) {
      itemName = PrefabUtils.TryGetPrefabName(itemHash, out string prefabName) ? prefabName : itemHash.ToString();
      return true;
    }

    if (zdo.Strings.TryGetValue(itemKeyHash, out itemName) && !string.IsNullOrEmpty(itemName)) {
      return true;
    }

    itemName = default;
    return false;
  }

  public static bool TryReadArmorStandItems(ZDO zdo, int slotCount, List<ItemDropItem> armorStandItems) {
    if (slotCount <= 0) {
      return false;
    }

    PrefabUtils.CacheArmorStandSlots(slotCount);
    armorStandItems.Clear();
    ZDOID zid = zdo.m_uid;

    for (int slotIndex = 0; slotIndex < slotCount; slotIndex++) {
      ItemSlot itemSlot = PrefabUtils.ArmorStandItemSlots[slotIndex];

      if (!TryGetAttachedItemName(zid, itemSlot.ItemHash, out string itemName)) {
        continue;
      }

      if (!TryReadItemDropItem(zid, itemSlot.ItemDataHash, out ItemDropItem item)) {
        item = new ItemDropItem();
      }

      item.Name = itemName;
      armorStandItems.Add(item);
    }

    return armorStandItems.Count > 0;
  }

  public static bool TryReadArmorStandItems(ChunkZDO zdo, int slotCount, List<ItemDropItem> armorStandItems) {
    if (slotCount <= 0) {
      return false;
    }

    PrefabUtils.CacheArmorStandSlots(slotCount);
    armorStandItems.Clear();

    for (int slotIndex = 0; slotIndex < slotCount; slotIndex++) {
      ItemSlot itemSlot = PrefabUtils.ArmorStandItemSlots[slotIndex];

      if (!TryGetAttachedItemName(zdo, itemSlot.ItemHash, out string itemName)) {
        continue;
      }

      if (!TryReadItemDropItem(zdo, itemSlot.ItemDataHash, out ItemDropItem item)) {
        item = new ItemDropItem();
      }

      item.Name = itemName;
      armorStandItems.Add(item);
    }

    return armorStandItems.Count > 0;
  }

  public static bool TryReadItemStandItem(ZDO zdo, out ItemDropItem item) {
    if (!TryGetAttachedItemName(zdo.m_uid, ZDOVars.s_item, out string itemName)) {
      item = default;
      return false;
    }

    if (!TryReadItemDropItem(zdo, out item)) {
      item = new ItemDropItem();
    }

    item.Name = itemName;
    return true;
  }

  public static bool TryReadItemStandItem(ChunkZDO zdo, out ItemDropItem item) {
    if (!TryGetAttachedItemName(zdo, ZDOVars.s_item, out string itemName)) {
      item = default;
      return false;
    }

    if (!TryReadItemDropItem(zdo, ZDOVars.s_itemData, out item)) {
      item = new ItemDropItem();
    }

    item.Name = itemName;
    return true;
  }

  public static bool TryReadItemDropItem(ZDO zdo, out ItemDropItem item) {
    if (!TryReadItemDropItem(zdo.m_uid, ZDOVars.s_itemData, out item)) {
      return false;
    }

    if (string.IsNullOrEmpty(item.Name) && zdo.m_prefab != 0) {
      item.Name =
          PrefabUtils.TryGetPrefabName(zdo.m_prefab, out string prefabName) ? prefabName : zdo.m_prefab.ToString();
    }

    return true;
  }

  public static bool TryReadItemDropItem(ChunkZDO zdo, out ItemDropItem item) {
    if (!TryReadItemDropItem(zdo, ZDOVars.s_itemData, out item)) {
      return false;
    }

    if (string.IsNullOrEmpty(item.Name) && zdo.PrefabHash != 0) {
      item.Name =
          PrefabUtils.TryGetPrefabName(zdo.PrefabHash, out string prefabName) ? prefabName : zdo.PrefabHash.ToString();
    }

    return true;
  }
}
