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
    string databaseName = $"{WorldUtils.GetWorldName(ZNet.m_world)}-items-{startTimeSeconds}.db";

    using (SQLiteConnection populateDatabase = DatabaseUtils.CreateDatabaseAndTables(databaseName)) {
      Task populateTask =
          Task.Run(() => DatabaseUtils.InsertPrefabHashes(populateDatabase, PrefabUtils.PrefabNames));

      while (!populateTask.IsCompleted) {
        yield return null;
      }
    }

    ComfyLogger.LogInfo($"Parsing {zdos.Count} ZDOs and dumping items to: {databaseName}", context);

    using SQLiteConnection database = DatabaseUtils.CreateDatabase(databaseName);

    DumpReport report = new();

    int zdoCount = zdos.Count;
    int tenPercentMod = Mathf.CeilToInt(zdoCount / 10);
    int nextCount = zdoCount - tenPercentMod;

    Task task = Task.Run(() => RunThread(database, zdos, report));

    while (!task.IsCompleted) {
      yield return null;

      if (zdos.Count < nextCount) {
        int parsedCount = zdoCount - zdos.Count;
        nextCount = zdos.Count - tenPercentMod;
        ComfyLogger.LogInfo($"Parsed {parsedCount}/{zdoCount} ({parsedCount * 1f / zdoCount:P2}) ZDOs.");
      }
    }

    TimeSpan elapsedTime = TimeSpan.FromSeconds(DateTimeOffset.UtcNow.ToUnixTimeSeconds() - startTimeSeconds);

    ComfyLogger.LogInfo(
        $"Finished dumping items, elapsed time: {elapsedTime}\n"
            + $"Parsed {zdoCount} ZDOs and found...\n"
            + $"  * {report.ContainerCount} Containers\n"
            + $"  * {report.ContainerItemCount} items in Containers ({report.CheatedContainerItemCount} cheated)\n"
            + $"  * {report.ItemDropCount} ItemDrop items ({report.CheatedItemDropCount} cheated)\n"
            + $"  * {report.ItemStandItemCount} ItemStand items ({report.CheatedItemStandItemCount} cheated)\n"
            + $"  * {report.ArmorStandCount} ArmorStands\n"
            + $"  * {report.ArmorStandItemCount} items on ArmorStands ({report.CheatedArmorStandItemCount} cheated)",
        context);
  }

  public sealed class DumpReport {
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

  public static void RunThread(SQLiteConnection database, ConcurrentQueue<ZDO> zdos, DumpReport report) {
    List<ContainerItem> containerItems = [];
    List<ItemDropItem> armorStandItems = [];

    database.BeginTransaction();

    while (zdos.TryDequeue(out ZDO zdo)) {
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

      if (PrefabUtils.HasItemDropComponent(zdo) && ZDOUtils.TryReadItemDropItem(zdo, out ItemDropItem itemDropItem)) {
        report.ItemDropCount++;

        if (itemDropItem.IsCheated == true) {
          report.CheatedItemDropCount++;
        }

        dataObject ??= InsertDataObject(database, zdo);
        itemDropItem.ObjectId = dataObject.ObjectId;
        itemDropItem.ItemDropId = dataObject.ObjectId;
        database.Insert(itemDropItem, typeof(ItemDropItem));
      }

      if (PrefabUtils.HasItemStandComponent(zdo)
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

      if (PrefabUtils.HasArmorStandComponent(zdo, out int slotCount)
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

    database.Commit();
  }

  static DataObject InsertDataObject(SQLiteConnection database, ZDO zdo) {
    DataObject dataObject = ZDOUtils.CreateDataObject(zdo);
    database.Insert(dataObject, typeof(DataObject));

    return dataObject;
  }
}
