namespace Queryable;

using System.Collections.Generic;

using Database;

using SQLite;

using UnityEngine;

public static class DatabaseUtils {
  public static SQLiteConnection CreateDatabaseAndTables(string databaseName) {
    SQLiteConnection database = new SQLiteConnection(databaseName);

    database.CreateTable<Container>();
    database.CreateTable<ContainerItem>();
    database.CreateTable<ItemDropItem>();
    database.CreateTable<PrefabHash>();

    return database;
  }

  public static void InsertPrefabHashes(SQLiteConnection database, Dictionary<int, GameObject> prefabCache) {   
    foreach (KeyValuePair<int, GameObject> pair in prefabCache) {
      database.InsertOrReplace(
          new PrefabHash() {
            Hash = pair.Key,
            PrefabName = pair.Value.name,
          });
    }
  }

  public static void InsertContainerAndItems(SQLiteConnection database, ZDO zdo, List<ContainerItem> items) {
    Container container = ZDOUtils.CreateContainer(zdo);
    database.Insert(container);

    for (int i = 0, count = items.Count; i < count; i++) {
      items[0].ContainerId = container.ContainerId;
    }

    database.InsertAll(items, runInTransaction: false);
  }
}
