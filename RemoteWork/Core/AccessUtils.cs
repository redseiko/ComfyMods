namespace RemoteWork;

using System.IO;

using ComfyLib;

using static PluginConfig;

public static class AccessUtils {
  static SyncedAuthList _accessList;

  public static SyncedAuthList AccessList {
    get {
      _accessList ??=
          new SyncedAuthList(
              Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), AccessListFilename.Value),
              "RemoteWork access list.");

      return _accessList;
    }
  }

  public static bool HasAccess(string steamId) {
    return AccessList.Contains(steamId) || ZNet.m_instance.m_adminList.Contains(steamId);
  }
}
