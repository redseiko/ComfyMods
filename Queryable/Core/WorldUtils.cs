namespace Queryable;

using System;
using System.IO;

using ComfyLib;

public static class WorldUtils {
  public static string WorldsDir {
    get => Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), "worlds_local");
  }

  public static bool QueryWorldFile(string worldFilename) {
    string worldPath = Path.Combine(WorldsDir, worldFilename);

    if (!File.Exists(worldPath)) {
      ComfyLogger.LogError($"Could not find world file at: {worldPath}");
      return false;
    }

    ComfyLogger.LogInfo($"Parsing world file: {worldPath}");
    WorldData worldData = new(worldPath);

    try {
      FileReader fileReader = new(worldPath, FileHelpers.FileSource.Local, FileHelpers.FileHelperType.Binary);

      ParseWorldData(worldData, fileReader.m_binary);
      ComfyLogger.LogInfo(worldData);
    } catch (Exception exception) {
      ComfyLogger.LogError($"Exception while parsing world file: {worldPath}\n{exception}");
      return false;
    }

    return true;
  }

  public static void ParseWorldData(WorldData worldData, BinaryReader reader) {
    worldData.WorldVersion = reader.ReadInt32();

    if (worldData.WorldVersion < global::Version.c_WorldVersionNewSaveFormat) {
      throw new InvalidDataException($"World version {worldData.WorldVersion} is too old to be parsed.");
    }

    worldData.NetTime = reader.ReadDouble();
    reader.ReadInt64();
    worldData.NextUid = reader.ReadUInt32();
    worldData.ZDOCount = reader.ReadInt32();
  }
}

public sealed class WorldData {
  public string WorldPath { get; }

  public int WorldVersion { get; set; }
  public double NetTime { get; set; }
  public uint NextUid { get; set; }
  public int ZDOCount { get; set; }

  public WorldData(string worldPath) {
    this.WorldPath = worldPath;
  }

  public override string ToString() {
    return $"WorldData {{\n"
        + $"  WorldPath: {WorldPath}\n"
        + $"  WorldVersion: {WorldVersion}\n"
        + $"  NetTime: {NetTime:F2}\n"
        + $"  NextUid: {NextUid}\n"
        + $"  ZDOCount: {ZDOCount}\n"
        + $"}}";
  }
}
