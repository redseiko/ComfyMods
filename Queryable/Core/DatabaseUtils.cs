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
}
