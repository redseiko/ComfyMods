namespace Queryable;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

using Database;

public static class ZDOUtils {
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
}
