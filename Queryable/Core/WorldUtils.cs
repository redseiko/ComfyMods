namespace Queryable;

using System.IO;

public static class WorldUtils {
  public static string WorldsDir {
    get => Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), "worlds_local");
  }
}
