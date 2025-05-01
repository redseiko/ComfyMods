namespace Queryable;

using System;
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

    if (ZDOExtraData.GetLong(zid, EpochTimeCreatedHash, out long epochTimeCreated)) {
      dataObject.EpochTimeCreated = epochTimeCreated;
    }

    if (ZDOExtraData.GetLong(zid, OriginalUidUserHash, out long originalUidUser)
        && ZDOExtraData.GetLong(zid, OriginalUidIdHash, out long originalUidId)) {
      dataObject.OriginalUid = $"{originalUidUser}:{originalUidId}";
    }

    return dataObject;
  }

  public static bool TryReadContainerItems(ZDO zdo, List<ContainerItem> items) {
    if (!zdo.GetString(ZDOVars.s_items, out string itemsInBase64) || string.IsNullOrEmpty(itemsInBase64)) {
      return false;
    }

    byte[] itemsData = Convert.FromBase64String(itemsInBase64);
    MemoryStream stream = new(itemsData, writable: false);
    BinaryReader reader = new(stream, Encoding.UTF8);

    int version = reader.ReadInt32();
    int count = reader.ReadInt32();

    if (count <= 0) {
      return false;
    }

    items.Clear();

    if (version == 106) {
      for (int i = 0; i < count; i++) {
        ContainerItem item = ReadContainerItem(reader);
        items.Add(item);
      }
    } else {
      for (int i = 0; i < count; i++) {
        ContainerItem item = ReadContainerItemOld(reader, version);
        items.Add(item);
      }
    }

    return true;
  }

  public static ContainerItem ReadContainerItem(BinaryReader reader) {
    ContainerItem containerItem = new() {
      Name = reader.ReadString(),
      Stack = reader.ReadInt32(),
      Durability = reader.ReadSingle(),
      GridPositionX = reader.ReadInt32(),
      GridPositionY = reader.ReadInt32(),
      IsEquipped = reader.ReadBoolean(),
      Quality = reader.ReadInt32(),
      Variant = reader.ReadInt32(),
      CrafterId = reader.ReadInt64(),
      CrafterName = reader.ReadString(),
      CustomDataJson = ReadItemCustomDataToJson(reader),
      WorldLevel = reader.ReadInt32(),
      IsPickedUp = reader.ReadBoolean(),
    };

    return containerItem;
  }

  public static ContainerItem ReadContainerItemOld(BinaryReader reader, int version) {
    ContainerItem containerItem = new() {
      Name = reader.ReadString(),
      Stack = reader.ReadInt32(),
      Durability = reader.ReadSingle(),
      GridPositionX = reader.ReadInt32(),
      GridPositionY = reader.ReadInt32(),
      IsEquipped = reader.ReadBoolean(),
      Quality = version >= 101 ? reader.ReadInt32() : 1,
      Variant = version >= 102 ? reader.ReadInt32() : 0,
      CrafterId = version >= 103 ? reader.ReadInt64() : 0,
      CrafterName = version >= 103 ? reader.ReadString() : string.Empty,
      CustomDataJson = version >= 104 ? ReadItemCustomDataToJson(reader) : string.Empty,
      WorldLevel = version >= 105 ? reader.ReadInt32() : 0,
      IsPickedUp = version >= 106 && reader.ReadBoolean(),
    };

    return containerItem;
  }

  static readonly StringBuilder _jsonBuilder = new(capacity: 128);

  public static string ReadItemCustomDataToJson(BinaryReader reader) {
    int count = reader.ReadInt32();

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

  public static bool TryReadArmorStandItems(ZDO zdo, int slotCount, List<ItemDropItem> armorStandItems) {
    if (slotCount <= 0) {
      return false;
    }

    PrefabUtils.CacheArmorStandSlots(slotCount);
    armorStandItems.Clear();
    ZDOID zid = zdo.m_uid;

    for (int slotIndex = 0; slotIndex < slotCount; slotIndex++) {
      ItemSlot itemSlot = PrefabUtils.ArmorStandItemSlots[slotIndex];
      ItemDropItem item = new();

      if (ZDOExtraData.GetString(zid, itemSlot.ItemHash, out string itemName)) {
        item.Name = itemName;
      }

      if (ZDOExtraData.GetFloat(zid, itemSlot.DurabiltyHash, out float durability)) {
        item.Durability = durability;
      }

      if (ZDOExtraData.GetInt(zid, itemSlot.StackHash, out int stack)) {
        item.Stack = stack;
      }

      if (ZDOExtraData.GetInt(zid, itemSlot.QualityHash, out int quality)) {
        item.Quality = quality;
      }

      if (ZDOExtraData.GetInt(zid, itemSlot.VariantHash, out int variant)) {
        item.Variant = variant;
      }

      if (ZDOExtraData.GetLong(zid, itemSlot.CrafterIdHash, out long crafterId)) {
        item.CrafterId = crafterId;
      }

      if (ZDOExtraData.GetString(zid, itemSlot.CrafterNameHash, out string crafterName)) {
        item.CrafterName = crafterName;
      }

      if (ZDOExtraData.GetInt(zid, itemSlot.DataCountHash, out int dataCount) && dataCount > 0) {
        item.CustomDataJson = ReadItemCustomDataToJson(zid, slotIndex, dataCount);
      }

      if (ZDOExtraData.GetInt(zid, itemSlot.WorldLevelHash, out int worldLevel)) {
        item.WorldLevel = worldLevel;
      }

      if (ZDOExtraData.GetBool(zid, itemSlot.PickedUpHash, out bool isPickedUp)) {
        item.IsPickedUp = isPickedUp;
      }

      if (!item.IsNull()) {
        armorStandItems.Add(item);
      }
    }

    return true;
  }

  public static string ReadItemCustomDataToJson(ZDOID zid, int slotIndex, int dataCount) {
    if (dataCount <= 0) {
      return string.Empty;
    }

    string slotIndexStr = slotIndex.ToString();

    _jsonBuilder
        .Clear()
        .Append("{")
        .Append("\"")
        .Append(ZDOExtraData.GetString(zid, $"{slotIndexStr}_data_0".GetStableHashCode(), string.Empty))
        .Append("\":\"")
        .Append(ZDOExtraData.GetString(zid, $"{slotIndexStr}_data__0".GetStableHashCode(), string.Empty))
        .Append("\"");


    for (int i = 1; i < dataCount; i++) {
      _jsonBuilder.Append($",\"")
          .Append(ZDOExtraData.GetString(zid, $"{slotIndexStr}_data_{i}".GetStableHashCode(), string.Empty))
          .Append("\": \"")
          .Append(ZDOExtraData.GetString(zid, $"{slotIndexStr}_data__{i}".GetStableHashCode(), string.Empty))
          .Append("\"");
    }

    _jsonBuilder.Append("}");

    string jsonResult = _jsonBuilder.ToString();

    _jsonBuilder.Clear();

    return jsonResult;
  }

  public static bool TryReadItemStandItem(ZDO zdo, out ItemDropItem item) {
    if (!TryReadItemDropItem(zdo, out item)) {
      return false;
    }

    if (ZDOExtraData.GetString(zdo.m_uid, ZDOVars.s_item, out string itemName)) {
      item.Name = itemName;
    }

    return true;
  }

  public static bool TryReadItemDropItem(ZDO zdo, out ItemDropItem item) {
    ZDOID zid = zdo.m_uid;
    item = new ItemDropItem();

    if (ZDOExtraData.GetFloat(zid, ZDOVars.s_durability, out float durability)) {
      item.Durability = durability;
    }

    if (ZDOExtraData.GetInt(zid, ZDOVars.s_stack, out int stack)) {
      item.Stack = stack;
    }

    if (ZDOExtraData.GetInt(zid, ZDOVars.s_quality, out int quality)) {
      item.Quality = quality;
    }

    if (ZDOExtraData.GetInt(zid, ZDOVars.s_variant, out int variant)) {
      item.Variant = variant;
    }

    if (ZDOExtraData.GetLong(zid, ZDOVars.s_crafterID, out long crafterId)) {
      item.CrafterId = crafterId;
    }

    if (ZDOExtraData.GetString(zid, ZDOVars.s_crafterName, out string crafterName)) {
      item.CrafterName = crafterName;
    }

    if (ZDOExtraData.GetInt(zid, ZDOVars.s_dataCount, out int dataCount) && dataCount > 0) {
      item.CustomDataJson = ReadItemCustomDataToJson(zid, dataCount);
    }

    if (ZDOExtraData.GetInt(zid, ZDOVars.s_worldLevel, out int worldLevel)) {
      item.WorldLevel = worldLevel;
    }

    if (ZDOExtraData.GetBool(zid, ZDOVars.s_pickedUp, out bool isPickedUp)) {
      item.IsPickedUp = isPickedUp;
    }

    return !item.IsNull();
  }

  public static readonly int CustomDataKey0HashCode = "data_0".GetStableHashCode();
  public static readonly int CustomDataValue0HashCode = "data__0".GetStableHashCode();

  public static string ReadItemCustomDataToJson(ZDOID zid, int dataCount) {
    if (dataCount <= 0) {
      return string.Empty;
    }

    _jsonBuilder
        .Clear()
        .Append("{")
        .Append("\"")
        .Append(ZDOExtraData.GetString(zid, CustomDataKey0HashCode, string.Empty))
        .Append("\":\"")
        .Append(ZDOExtraData.GetString(zid, CustomDataValue0HashCode, string.Empty))
        .Append("\"");


    for (int i = 1; i < dataCount; i++) {
      _jsonBuilder.Append($",\"")
          .Append(ZDOExtraData.GetString(zid, $"data_{i}".GetStableHashCode(), string.Empty))
          .Append("\": \"")
          .Append(ZDOExtraData.GetString(zid, $"data__{i}".GetStableHashCode(), string.Empty))
          .Append("\"");
    }

    _jsonBuilder.Append("}");

    string jsonResult = _jsonBuilder.ToString();

    _jsonBuilder.Clear();

    return jsonResult;
  }
}
