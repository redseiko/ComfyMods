namespace Queryable;

using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

using ComfyLib;

using Database;

using SQLite;

using UnityEngine;

public static class DumpWorldZDOsCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "dump-world-zdos",
        "(Queryable) dump-world-zdos <world-name>",
        Run,
        optionsFetcher: WorldUtils.GetWorldFilenames,
        alwaysRefreshTabOptions: true);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    try {
      ComfyLogger.PushContext(args.Context);

      if (args.Length <= 1) {
        ComfyLogger.LogError("Missing world filename arg.");
        return false;
      }

      string worldFilename = args[1];

      if (string.IsNullOrEmpty(worldFilename)) {
        ComfyLogger.LogError("Missing or invalid world filename arg.");
        return false;
      }

      if (!WorldUtils.TryGetWorldChunks(worldFilename, out int totalZDOCount, out List<string> chunkFilePaths)) {
        return false;
      }

      if (!PrefabUtils.TryGetPrefabCache(out Dictionary<int, GameObject> prefabCache)) {
        ComfyLogger.LogError("Could not find a valid prefab-cache!");
        return false;
      }

      PrefabUtils.ProcessPrefabCache(prefabCache);

      args.Context.StartCoroutine(RunCoroutine(args.Context, worldFilename, totalZDOCount, chunkFilePaths));
      return true;
    } finally {
      ComfyLogger.PopContext();
    }
  }

  public static IEnumerator RunCoroutine(
      Terminal context, string worldFilename, int zdoCount, List<string> chunkFilePaths) {
    long startTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    string databaseName = $"{worldFilename}-zdos-{startTimeSeconds}.db";

    using (SQLiteConnection populateDatabase = DatabaseUtils.CreateDatabaseAndTables(databaseName)) {
      Task populateTask =
          Task.Run(() => DatabaseUtils.InsertPrefabHashes(populateDatabase, PrefabUtils.PrefabNames));

      while (!populateTask.IsCompleted) {
        yield return null;
      }
    }

    ComfyLogger.LogInfo(
        $"Parsing {zdoCount} ZDOs across {chunkFilePaths.Count} chunks and dumping ZDOs to: {databaseName}",
        context);

    using SQLiteConnection database = DatabaseUtils.CreateDatabase(databaseName);

    int parsedZDOCount = 0;
    int tenPercentMod = Mathf.Max(1, Mathf.CeilToInt(zdoCount / 10f));
    int nextCount = tenPercentMod;

    Task task = Task.Run(() => RunThread(database, chunkFilePaths, ref parsedZDOCount));

    while (!task.IsCompleted) {
      yield return null;

      int currentCount = Volatile.Read(ref parsedZDOCount);

      if (currentCount >= nextCount && zdoCount > 0) {
        nextCount = currentCount + tenPercentMod;
        ComfyLogger.LogInfo($"Parsed {currentCount}/{zdoCount} ({currentCount * 1f / zdoCount:P2}) ZDOs.");
      }
    }

    TimeSpan elapsedTime = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - startTimeSeconds);

    ComfyLogger.LogInfo($"Finished dumping ZDOs, elapsed time: {elapsedTime}", context);
  }

  public static void RunThread(SQLiteConnection database, List<string> chunkFilePaths, ref int parsedZDOCount) {
    ChunkZDO zdo = new();

    database.BeginTransaction();

    foreach (string chunkFilePath in chunkFilePaths) {
      using MemoryStream stream = new(File.ReadAllBytes(chunkFilePath), writable: false);
      using BinaryReader reader = new(stream, Encoding.UTF8);

      int worldVersion = reader.ReadInt16();
      int chunkZDOCount = reader.ReadInt32();

      for (int zdoIndex = 0; zdoIndex < chunkZDOCount; zdoIndex++) {
        zdo.Read(reader, worldVersion, includeItemData: false);
        Interlocked.Increment(ref parsedZDOCount);

        InsertDataObject(database, zdo);
      }
    }

    database.Commit();
  }

  static DataObject InsertDataObject(SQLiteConnection database, ChunkZDO zdo) {
    DataObject dataObject = ZDOUtils.CreateDataObject(zdo);
    database.Insert(dataObject, typeof(DataObject));

    return dataObject;
  }
}

