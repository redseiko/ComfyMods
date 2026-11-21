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

public static class DumpWorldItemsCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "dump-world-items",
        "(Queryable) dump-world-items <world-name>",
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
    string databaseName = $"{worldFilename}-items-{startTimeSeconds}.db";

    using (SQLiteConnection populateDatabase = DatabaseUtils.CreateDatabaseAndTables(databaseName)) {
      Task populateTask =
          Task.Run(() => DatabaseUtils.InsertPrefabHashes(populateDatabase, PrefabUtils.PrefabNames));

      while (!populateTask.IsCompleted) {
        yield return null;
      }
    }

    ComfyLogger.LogInfo(
        $"Parsing {zdoCount} ZDOs across {chunkFilePaths.Count} chunks and dumping items to: {databaseName}",
        context);

    using SQLiteConnection database = DatabaseUtils.CreateDatabase(databaseName);

    DumpReport report = new();

    int tenPercentMod = Mathf.Max(1, Mathf.CeilToInt(zdoCount / 10f));
    int nextCount = tenPercentMod;

    Task task = Task.Run(() => RunThread(database, chunkFilePaths, report));

    while (!task.IsCompleted) {
      yield return null;

      int parsedCount = Volatile.Read(ref report.ParsedZDOCount);

      if (parsedCount >= nextCount && zdoCount > 0) {
        nextCount = parsedCount + tenPercentMod;
        ComfyLogger.LogInfo($"Parsed {parsedCount}/{zdoCount} ({parsedCount * 1f / zdoCount:P2}) ZDOs.");
      }
    }

    TimeSpan elapsedTime = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - startTimeSeconds);

    ComfyLogger.LogInfo(
        $"Finished dumping items, elapsed time: {elapsedTime}\n"
            + $"Parsed {report.ParsedZDOCount} ZDOs and found...\n"
            + $"  * {report.ContainerCount} Containers\n"
            + $"  * {report.ContainerItemCount} items in Containers ({report.CheatedContainerItemCount} cheated)\n"
            + $"  * {report.ItemDropCount} ItemDrop items ({report.CheatedItemDropCount} cheated)\n"
            + $"  * {report.ItemStandItemCount} ItemStand items ({report.CheatedItemStandItemCount} cheated)\n"
            + $"  * {report.ArmorStandCount} ArmorStands\n"
            + $"  * {report.ArmorStandItemCount} items on ArmorStands ({report.CheatedArmorStandItemCount} cheated)",
        context);
  }

  public sealed class DumpReport {
    public int ParsedZDOCount = 0;
    public int ContainerCount = 0;
    public int ContainerItemCount = 0;
    public int CheatedContainerItemCount = 0;
    public int ItemDropCount = 0;
    public int CheatedItemDropCount = 0;
    public int ItemStandItemCount = 0;
    public int CheatedItemStandItemCount = 0;
    public int ArmorStandCount = 0;
    public int ArmorStandItemCount = 0;
    public int CheatedArmorStandItemCount = 0;
  }

  public static void RunThread(SQLiteConnection database, List<string> chunkFilePaths, DumpReport report) {
    ChunkZDO zdo = new();
    List<ContainerItem> containerItems = [];
    List<ItemDropItem> armorStandItems = [];

    database.BeginTransaction();

    foreach (string chunkFilePath in chunkFilePaths) {
      using MemoryStream stream = new(File.ReadAllBytes(chunkFilePath), writable: false);
      using BinaryReader reader = new(stream, Encoding.UTF8);

      int worldVersion = reader.ReadInt16();
      int chunkZDOCount = reader.ReadInt32();

      for (int zdoIndex = 0; zdoIndex < chunkZDOCount; zdoIndex++) {
        zdo.Read(reader, worldVersion, includeItemData: true);
        Interlocked.Increment(ref report.ParsedZDOCount);

        DataObject dataObject = default;

        if (ZDOUtils.TryReadContainerItems(zdo, containerItems)) {
          report.ContainerCount++;
          report.ContainerItemCount += containerItems.Count;

          dataObject ??= InsertDataObject(database, zdo);
          int containerId = dataObject.ObjectId;

          for (int i = 0, count = containerItems.Count; i < count; i++) {
            containerItems[i].ObjectId = containerId;

            if (containerItems[i].IsCheated) {
              report.CheatedContainerItemCount++;
            }
          }

          database.InsertAll(containerItems, typeof(ContainerItem), runInTransaction: false);
          containerItems.Clear();
        }

        if (PrefabUtils.HasItemDropComponent(zdo.PrefabHash)
            && ZDOUtils.TryReadItemDropItem(zdo, out ItemDropItem itemDropItem)) {
          report.ItemDropCount++;

          if (itemDropItem.IsCheated == true) {
            report.CheatedItemDropCount++;
          }

          dataObject ??= InsertDataObject(database, zdo);
          itemDropItem.ObjectId = dataObject.ObjectId;
          itemDropItem.ItemDropId = dataObject.ObjectId;
          database.Insert(itemDropItem, typeof(ItemDropItem));
        }

        if (PrefabUtils.HasItemStandComponent(zdo.PrefabHash)
            && ZDOUtils.TryReadItemStandItem(zdo, out ItemDropItem itemStandItem)) {
          report.ItemStandItemCount++;

          if (itemStandItem.IsCheated == true) {
            report.CheatedItemStandItemCount++;
          }

          dataObject ??= InsertDataObject(database, zdo);
          itemStandItem.ObjectId = dataObject.ObjectId;
          itemStandItem.ItemStandId = dataObject.ObjectId;
          database.Insert(itemStandItem, typeof(ItemDropItem));
        }

        if (PrefabUtils.HasArmorStandComponent(zdo.PrefabHash, out int slotCount)
            && ZDOUtils.TryReadArmorStandItems(zdo, slotCount, armorStandItems)) {
          report.ArmorStandCount++;
          report.ArmorStandItemCount += armorStandItems.Count;

          dataObject ??= InsertDataObject(database, zdo);
          int armorStandId = dataObject.ObjectId;

          for (int i = 0, count = armorStandItems.Count; i < count; i++) {
            armorStandItems[i].ObjectId = armorStandId;
            armorStandItems[i].ArmorStandId = armorStandId;

            if (armorStandItems[i].IsCheated == true) {
              report.CheatedArmorStandItemCount++;
            }
          }

          database.InsertAll(armorStandItems, typeof(ItemDropItem), runInTransaction: false);
          armorStandItems.Clear();
        }
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

