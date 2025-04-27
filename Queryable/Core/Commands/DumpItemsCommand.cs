namespace Queryable;

using System.Collections;
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
    List<ZDO> zdos = [.. ZDOMan.s_instance.m_objectsByID.Values];

    string databaseName = $"{ZNet.m_world.m_fileName}-items-{System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.db";
    ComfyLogger.LogInfo($"Parsing {zdos.Count} ZDOs and dumping items to: {databaseName}", context);

    using SQLiteConnection database = DatabaseUtils.CreateDatabaseAndTables(databaseName);
    DumpReport report = new();

    Task task = Task.Run(() => RunThread(database, zdos, report));

    while (!task.IsCompleted) {
      yield return null;
    }

    ComfyLogger.LogInfo(
        $"Parsed {zdos.Count} ZDOs and found...\n"
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

  public static void RunThread(SQLiteConnection database, List<ZDO> zdos, DumpReport report) {
    DatabaseUtils.InsertPrefabHashes(database, ZNetScene.s_instance.m_namedPrefabs);

    List<ContainerItem> items = [];
    int zdoCount = zdos.Count;
    int tenPercentMod = Mathf.CeilToInt(zdoCount / 10);

    for (int i = 0; i < zdoCount; i++) {
      if (i % tenPercentMod == 0) {
        ComfyLogger.LogInfo($"Parsed {i}/{zdoCount} ZDOs.");
      }

      ZDO zdo = zdos[i];

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
    }
  }
}
