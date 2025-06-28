namespace Entitlement.Database;

using System;
using System.IO;

using SQLite;

public static class DatabaseManager {
  public static string GetDatabasePath() {
    return Path.Combine(Utils.m_saveDataOverride ?? Utils.persistantDataPath, "entitlement.sqlite.db");
  }

  public static SQLiteConnection OpenDatabase() {
    return new SQLiteConnection(GetDatabasePath());
  }

  public static void Initialize() {
    Entitlement.LogInfo($"Initializing Entitlement database...");

    using (SQLiteConnection database = OpenDatabase()) {
      database.CreateTable<PlayerTitle>();
      database.Insert(new PlayerTitle() { TitleName = $"Title {DateTimeOffset.Now.ToUnixTimeSeconds()}" });
    }
  }
}
