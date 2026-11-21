namespace Queryable;

using System.Collections.Generic;
using System.IO;

using UnityEngine;

public sealed class ChunkZDO {
  public int PrefabHash;
  public Vector3 Position;

  public readonly Dictionary<int, int> Ints = [];
  public readonly Dictionary<int, long> Longs = [];
  public readonly Dictionary<int, string> Strings = [];
  public readonly Dictionary<int, byte[]> ByteArrays = [];

  public void Clear() {
    PrefabHash = 0;
    Position = Vector3.zero;
    Ints.Clear();
    Longs.Clear();
    Strings.Clear();
    ByteArrays.Clear();
  }

  public void Read(BinaryReader reader, int worldVersion, bool includeItemData) {
    Clear();

    bool isChunkedSave = worldVersion >= (int) Version.World.ChunkedSave;
    ushort extraDataFlags = reader.ReadUInt16();

    if (!isChunkedSave) {
      reader.BaseStream.Seek(4L, SeekOrigin.Current);
    }

    if ((extraDataFlags & 8192) != 0) {
      Position = new Vector3(reader.ReadInt16(), 0f, reader.ReadInt16());
    } else {
      Position = new Vector3(reader.ReadSingle(), reader.ReadSingle(), reader.ReadSingle());
    }

    PrefabHash = reader.ReadInt32();

    if ((extraDataFlags & 4096) != 0) {
      if (isChunkedSave) {
        ushort rotation = reader.ReadUInt16();

        if ((rotation & 32768) == 0) {
          reader.ReadUInt16();
        }
      } else {
        reader.BaseStream.Seek(12L, SeekOrigin.Current);
      }
    }

    if ((extraDataFlags & 255) == 0) {
      return;
    }

    if ((extraDataFlags & 1) != 0) {
      reader.BaseStream.Seek(5L, SeekOrigin.Current);
    }

    if ((extraDataFlags & 2) != 0) {
      int count = ReadNumItems(reader, worldVersion);
      reader.BaseStream.Seek(count * 8L, SeekOrigin.Current);
    }

    if ((extraDataFlags & 4) != 0) {
      int count = ReadNumItems(reader, worldVersion);
      reader.BaseStream.Seek(count * 16L, SeekOrigin.Current);
    }

    if ((extraDataFlags & 8) != 0) {
      int count = ReadNumItems(reader, worldVersion);
      reader.BaseStream.Seek(count * 20L, SeekOrigin.Current);
    }

    if ((extraDataFlags & 16) != 0) {
      int count = ReadNumItems(reader, worldVersion);

      for (int i = 0; i < count; i++) {
        int key = reader.ReadInt32();
        int value = reader.ReadInt32();
        Ints[key] = value;
      }
    }

    if ((extraDataFlags & 32) != 0) {
      int count = ReadNumItems(reader, worldVersion);

      for (int i = 0; i < count; i++) {
        int key = reader.ReadInt32();
        long value = reader.ReadInt64();
        Longs[key] = value;
      }
    }

    if ((extraDataFlags & 64) != 0) {
      int count = ReadNumItems(reader, worldVersion);

      if (includeItemData) {
        for (int i = 0; i < count; i++) {
          int key = reader.ReadInt32();
          string value = reader.ReadString();
          Strings[key] = value;
        }
      } else {
        for (int i = 0; i < count; i++) {
          reader.ReadInt32();
          reader.ReadString();
        }
      }
    }

    if ((extraDataFlags & 128) != 0) {
      int count = ReadNumItems(reader, worldVersion);

      if (includeItemData) {
        for (int i = 0; i < count; i++) {
          int key = reader.ReadInt32();
          int length = reader.ReadInt32();
          ByteArrays[key] = reader.ReadBytes(length);
        }
      } else {
        for (int i = 0; i < count; i++) {
          reader.ReadInt32();
          int length = reader.ReadInt32();
          reader.BaseStream.Seek(length, SeekOrigin.Current);
        }
      }
    }
  }

  static int ReadNumItems(BinaryReader reader, int worldVersion) {
    if (worldVersion < (int) Version.World.NumItems) {
      return reader.ReadByte();
    }

    int count = reader.ReadByte();

    if ((count & 128) != 0) {
      count = ((count & 127) << 8) | reader.ReadByte();
    }

    return count;
  }
}
