namespace Queryable;

using System.Collections.Generic;

using Database;

using SQLite;

using UnityEngine;

public static class DatabaseUtils {
  public static SQLiteConnection CreateDatabase(string databaseName) {
    SQLiteConnection database = new SQLiteConnection(databaseName);
    return database;
  }

  public static SQLiteConnection CreateDatabaseAndTables(string databaseName) {
    SQLiteConnection database = CreateDatabase(databaseName);

    database.EnableWriteAheadLogging();

    database.CreateTable<Container>();
    database.CreateTable<ContainerItem>();
    database.CreateTable<DataObject>();
    database.CreateTable<ItemDropItem>();
    database.CreateTable<PrefabHash>();

    return database;
  }

  public static void InsertPrefabHashes(SQLiteConnection database, Dictionary<int, GameObject> prefabCache) {
    database.BeginTransaction();

    foreach (KeyValuePair<int, GameObject> pair in prefabCache) {
      database.InsertOrReplace(
          new PrefabHash() {
            Hash = pair.Key,
            PrefabName = pair.Value.name,
          },
          typeof(PrefabHash));
    }

    database.Commit();
  }

  public static void InsertContainerAndItems(SQLiteConnection database, ZDO zdo, List<ContainerItem> items) {
    Container container = ZDOUtils.CreateContainer(zdo);
    database.Insert(container, typeof(Container));

    int containerId = container.ContainerId;

    for (int i = 0, count = items.Count; i < count; i++) {
      items[i].ContainerId = containerId;
    }

    database.InsertAll(items, typeof(ContainerItem), runInTransaction: true);
  }
}
