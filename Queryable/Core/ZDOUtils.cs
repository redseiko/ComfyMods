namespace Queryable;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Database;

using UnityEngine;

public static class ZDOUtils {
  public static Container CreateContainer(ZDO zdo) {
    Vector3 position = zdo.m_position;

    return new Container() {
      PrefabHash = zdo.m_prefab,
      PositionX = Mathf.RoundToInt(position.x),
      PositionY = Mathf.RoundToInt(position.y),
      PositionZ = Mathf.RoundToInt(position.z),
      CreatorId = zdo.GetLong(ZDOVars.s_creator, 0),
    };
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
    items.Capacity = count;

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

  public static bool TryReadItemDropItem(ZDO zdo, out ItemDropItem item) {
    ZDOID zid = zdo.m_uid;
    Vector3 position = zdo.m_position;

    item = new ItemDropItem() {
      PrefabHash = zdo.m_prefab,
      PositionX = Mathf.RoundToInt(position.x),
      PositionY = Mathf.RoundToInt(position.y),
      PositionZ = Mathf.RoundToInt(position.z),
    };

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
