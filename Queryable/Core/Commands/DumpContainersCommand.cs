namespace Queryable;

using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;

using ComfyLib;

using Database;

using SQLite;

using UnityEngine;

public static class DumpContainersCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "dump-containers",
        "(Queryable) dump-containers",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    args.Context.StartCoroutine(RunCoroutine(args.Context));
    return true;
  }

  public static IEnumerator RunCoroutine(Terminal context) {
    ComfyLogger.PushContext(context);

    List<ZDO> zdos = [.. ZDOMan.s_instance.m_objectsByID.Values];

    string databaseName = $"{ZNet.m_world.m_fileName}-containers-{System.DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.db";
    ComfyLogger.LogInfo($"Parsing {zdos.Count} ZDOs and dumping containers to: {databaseName}");

    using SQLiteConnection database = DatabaseUtils.CreateDatabaseAndTables(databaseName);

    int containerCount = 0;
    int itemCount = 0;

    Task task = Task.Run(() => RunThread(database, zdos, out containerCount, out itemCount));

    while (!task.IsCompleted) {
      yield return null;
    }

    ComfyLogger.LogInfo(
        $"Parsed {zdos.Count} ZDOs and found...\n"
            + $"  * {containerCount} containers\n"
            + $"  * {itemCount} items in containers");

    ComfyLogger.PopContext();
  }

  public static void RunThread(SQLiteConnection database, List<ZDO> zdos, out int containerCount, out int itemCount) {
    DatabaseUtils.InsertPrefabHashes(database, ZNetScene.s_instance.m_namedPrefabs);

    List<ContainerItem> items = [];
    containerCount = 0;
    itemCount = 0;

    for (int i = 0, count = zdos.Count; i < count; i++) {
      ZDO zdo = zdos[i];

      if (ZDOUtils.TryReadContainerItems(zdo, items)) {
        containerCount++;
        itemCount += items.Count;

        InsertContainerAndItems(database, zdo, items);
        items.Clear();
      }
    }
  }

  public static void InsertContainerAndItems(SQLiteConnection database, ZDO zdo, List<ContainerItem> items) {
    Container container = CreateContainer(zdo);
    database.Insert(container);

    for (int i = 0, count = items.Count; i < count; i++) {
      items[0].ContainerId = container.ContainerId;
    }

    database.InsertAll(items, runInTransaction: false);
  }

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
}
