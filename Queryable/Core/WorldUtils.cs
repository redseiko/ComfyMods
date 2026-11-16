namespace Queryable;

using System;
using System.Collections.Generic;
using System.IO;
using System.Text.RegularExpressions;

using ComfyLib;

public static class WorldUtils {
  public static string WorldsDir {
    get => Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), "worlds_local");
  }

  public static List<string> GetWorldFilenames() {
    if (!Directory.Exists(WorldsDir)) {
      return [];
    }

    List<string> worldFilenames = [];

    foreach (string directory in Directory.GetDirectories(WorldsDir)) {
      if (Directory.GetFiles(directory, "_main.*.db2").Length > 0) {
        worldFilenames.Add(Path.GetFileName(directory));
      }
    }

    return worldFilenames;
  }

  public static string GetWorldName(World world) {
    if (world == null) {
      return "unknown";
    }

    return string.IsNullOrEmpty(world.m_worldName) ? world.m_name : world.m_worldName;
  }

  static readonly Regex _mainDb2Regex = new(@"^_main\.(\d+)\.db2$", RegexOptions.Compiled);

  public static bool TryGetWorldChunks(
      string worldFilename, out int totalZDOCount, out List<string> chunkFilePaths) {
    totalZDOCount = 0;
    chunkFilePaths = [];

    string worldPath = Path.Combine(WorldsDir, worldFilename);

    if (!Directory.Exists(worldPath)) {
      ComfyLogger.LogError($"Could not find world directory at: {worldPath}");
      return false;
    }

    if (!TryGetLatestSaveNumber(worldPath, out uint saveNumber, out string _)) {
      ComfyLogger.LogError($"Could not find any _main.<N>.db2 file in: {worldPath}");
      return false;
    }

    string chunksPath = Path.Combine(worldPath, $"_main.{saveNumber}.chunks");

    if (!File.Exists(chunksPath)) {
      ComfyLogger.LogError($"Could not find chunks file at: {chunksPath}");
      return false;
    }

    try {
      using MemoryStream stream = new(File.ReadAllBytes(chunksPath), writable: false);
      using BinaryReader reader = new(stream);

      reader.ReadUInt16();
      totalZDOCount = reader.ReadInt32();
      int chunkCount = reader.ReadInt32();

      chunkFilePaths = new(capacity: chunkCount);

      for (int i = 0; i < chunkCount; i++) {
        ushort chunkIndex = reader.ReadUInt16();
        byte chunkSize = reader.ReadByte();
        uint chunkVersion = reader.ReadUInt32();
        reader.ReadInt32();

        string chunkFilename = $"{(chunkIndex >> 8):x2}_{chunkIndex & 0xFF:x2}__{chunkSize}_{chunkVersion}.chunk";
        string chunkFilePath = Path.Combine(worldPath, chunkFilename);

        if (File.Exists(chunkFilePath)) {
          chunkFilePaths.Add(chunkFilePath);
        } else {
          ComfyLogger.LogError($"Missing chunk file: {chunkFilePath}");
        }
      }

      return chunkFilePaths.Count > 0;
    } catch (Exception exception) {
      ComfyLogger.LogError($"Exception while reading chunks file: {chunksPath}\n{exception}");
      return false;
    }
  }

  public static bool QueryWorldFile(string worldFilename) {
    string worldPath = Path.Combine(WorldsDir, worldFilename);

    if (!Directory.Exists(worldPath)) {
      ComfyLogger.LogError($"Could not find world directory at: {worldPath}");
      return false;
    }

    if (!TryGetLatestSaveNumber(worldPath, out uint saveNumber, out string db2Path)) {
      ComfyLogger.LogError($"Could not find any _main.<N>.db2 file in: {worldPath}");
      return false;
    }

    ComfyLogger.LogInfo($"Parsing world directory: {worldPath}");

    WorldData worldData = new(worldPath) {
      SaveNumber = saveNumber,
    };

    FileReader db2Reader = default;
    FileReader chunksReader = default;

    try {
      db2Reader = new(db2Path, FileHelpers.FileSource.Local, FileHelpers.FileHelperType.Binary);
      ParseWorldDb2(worldData, db2Reader.m_binary);

      string chunksPath = Path.Combine(worldPath, $"_main.{saveNumber}.chunks");

      if (File.Exists(chunksPath)) {
        chunksReader = new(chunksPath, FileHelpers.FileSource.Local, FileHelpers.FileHelperType.Binary);
        ParseWorldChunks(worldData, chunksReader.m_binary);
      }

      ComfyLogger.LogInfo(worldData);
    } catch (Exception exception) {
      ComfyLogger.LogError($"Exception while parsing world: {worldPath}\n{exception}");
      return false;
    } finally {
      chunksReader?.Dispose();
      db2Reader?.Dispose();
    }

    return true;
  }

  static bool TryGetLatestSaveNumber(string worldPath, out uint saveNumber, out string db2Path) {
    saveNumber = 0;
    db2Path = string.Empty;
    bool found = false;

    foreach (string filePath in Directory.GetFiles(worldPath, "_main.*.db2")) {
      Match match = _mainDb2Regex.Match(Path.GetFileName(filePath));

      if (match.Success && uint.TryParse(match.Groups[1].Value, out uint parsedNumber)) {
        if (!found || parsedNumber > saveNumber) {
          saveNumber = parsedNumber;
          db2Path = filePath;
          found = true;
        }
      }
    }

    return found;
  }

  public static void ParseWorldDb2(WorldData worldData, BinaryReader reader) {
    worldData.WorldVersion = reader.ReadInt32();

    if (worldData.WorldVersion < (int)global::Version.World.ChunkedSave) {
      throw new InvalidDataException($"World version {worldData.WorldVersion} is unexpected.");
    }

    worldData.NetTime = reader.ReadDouble();
  }

  public static void ParseWorldChunks(WorldData worldData, BinaryReader reader) {
    reader.ReadUInt16();
    worldData.ZDOCount = reader.ReadInt32();
    worldData.ChunkCount = reader.ReadInt32();
  }
}

public sealed class WorldData {
  public string WorldPath { get; }

  public uint SaveNumber { get; set; }
  public int WorldVersion { get; set; }
  public double NetTime { get; set; }
  public int ChunkCount { get; set; }
  public int ZDOCount { get; set; }

  public WorldData(string worldPath) {
    this.WorldPath = worldPath;
  }

  public override string ToString() {
    return $"WorldData {{\n"
        + $"  WorldPath: {WorldPath}\n"
        + $"  SaveNumber: {SaveNumber}\n"
        + $"  WorldVersion: {WorldVersion}\n"
        + $"  NetTime: {NetTime:F2}\n"
        + $"  ChunkCount: {ChunkCount}\n"
        + $"  ZDOCount: {ZDOCount}\n"
        + $"}}";
  }
}
