namespace Queryable;

using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Threading.Tasks;

using ComfyLib;

using Database;

using SQLite;

using UnityEngine;

public static class DumpItemsCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "dump-items",
        "(Queryable) dump-items",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    if (!PrefabUtils.TryGetPrefabCache(out Dictionary<int, GameObject> prefabCache)) {
      ComfyLogger.LogError("Could not find a valid prefab-cache!", args.Context);
      return false;
    }

    PrefabUtils.ProcessPrefabCache(prefabCache);

    args.Context.StartCoroutine(RunCoroutine(args.Context));
    return true;
  }

  public static IEnumerator RunCoroutine(Terminal context) {
    ConcurrentQueue<ZDO> zdos = new(ZDOMan.s_instance.m_objectsByID.Values);
    long startTimeSeconds = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    string databaseName = $"{ZNet.m_world.m_fileName}-items-{startTimeSeconds}.db";

    using (SQLiteConnection populateDatabase = DatabaseUtils.CreateDatabaseAndTables(databaseName)) {
      Task populateTask =
          Task.Run(() => DatabaseUtils.InsertPrefabHashes(populateDatabase, ZNetScene.s_instance.m_namedPrefabs));

      while (!populateTask.IsCompleted) {
        yield return null;
      }
    }


    ComfyLogger.LogInfo($"Parsing {zdos.Count} ZDOs and dumping items to: {databaseName}", context);

    using SQLiteConnection database1 = DatabaseUtils.CreateDatabase(databaseName);
    using SQLiteConnection database2 = DatabaseUtils.CreateDatabase(databaseName);

    DumpReport report = new();

    int zdoCount = zdos.Count;
    int tenPercentMod = Mathf.CeilToInt(zdoCount / 10);
    int nextCount = zdoCount - tenPercentMod;

    Task task1 = Task.Run(() => RunThread(database1, zdos, report));
    Task task2 = Task.Run(() => RunThread(database2, zdos, report));

    while (!task1.IsCompleted && !task2.IsCompleted) {
      yield return null;

      if (zdos.Count < nextCount) {
        int parsedCount = zdoCount - zdos.Count;
        nextCount = zdos.Count - tenPercentMod;
        ComfyLogger.LogInfo($"Parsed {parsedCount}/{zdoCount} ({parsedCount * 1f / zdoCount:P2}) ZDOs.");
      }
    }

    TimeSpan elapsedTime = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - startTimeSeconds);

    ComfyLogger.LogInfo(
            $"Finisheh dumping items, elapsed time: {elapsedTime}\n"
            + $"Parsed {zdoCount} ZDOs and found...\n"
            + $"  * {report.ContainerCount} containers\n"
            + $"  * {report.ItemCount} items in containers\n"
            + $"  * {report.ItemDropCount} item drops",
        context);
  }

  public sealed class DumpReport {
    public int ContainerCount = 0;
    public int ItemCount = 0;
    public int ItemDropCount = 0;
  }

  public static void RunThread(SQLiteConnection database, ConcurrentQueue<ZDO> zdos, DumpReport report) {
    List<ContainerItem> items = [];

    while (zdos.TryDequeue(out ZDO zdo)) {
      database.BeginTransaction();

      if (ZDOUtils.TryReadContainerItems(zdo, items)) {
        report.ContainerCount++;
        report.ItemCount += items.Count;

        DatabaseUtils.InsertContainerAndItems(database, zdo, items);
        items.Clear();
      }

      if (PrefabUtils.HasItemDropComponent(zdo) && ZDOUtils.TryReadItemDropItem(zdo, out ItemDropItem item)) {
        report.ItemDropCount++;
        database.Insert(item);
      }

      database.Commit();
    }
  }
}
