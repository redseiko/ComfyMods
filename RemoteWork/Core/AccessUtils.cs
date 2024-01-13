using System.IO;

using static RemoteWork.PluginConfig;

namespace RemoteWork {
  public static class AccessUtils {
    static SyncedList _accessList;

    public static SyncedList AccessList {
      get {
        _accessList ??=
            new SyncedList(
                Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), AccessListFilename.Value),
                "RemoteWork access list.");

        return _accessList;
      }
    }

    public static bool HasAccess(string steamId) {
      return AccessList.Contains(steamId) || ZNet.m_instance.m_adminList.Contains(steamId);
    }
  }
}
