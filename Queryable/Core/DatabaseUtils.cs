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

    database.CreateTable<PrefabHash>();
    database.CreateTable<DataObject>();
    database.CreateTable<ContainerItem>();
    database.CreateTable<ItemDropItem>();

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
}
