namespace Parrot;

using Splatform;

using Steamworks;

using static PluginConfig;

public static class ZNetManager {
  public static string ServerName { get; private set; } = "Server";
  public static PlatformUserID ServerPlatformUserId { get; private set; } = PlatformUserID.None;

  public static void SetupServerPlayer(ZNet net) {
    ServerName = "Server";
    ServerPlatformUserId = new(net.m_steamPlatform, SteamGameServer.GetSteamID().ToString());

    Parrot.LogInfo($"Parrot current server PlatformUserID: {ServerPlatformUserId}");
  }

  public static void AddServerPlayers(ZNet net, ZPackage package) {
    if (!AddServerToPlayerList.Value) {
      return;
    }

    package.SetPos(0);
    int playersCount = package.ReadInt();

    package.SetPos(0);
    package.Write(playersCount + 1);

    package.Write(ServerName);                      // m_name
    package.Write(ZDOMan.s_instance.m_sessionID);   // m_characterID.UserKey
    package.Write(uint.MaxValue);                   // m_characterID.ID
    package.Write(ServerPlatformUserId.ToString()); // m_userInfo.m_id
    package.Write(ServerName);                      // m_userInfo.m_displayName
    package.Write(ServerName);                      // m_serverAssignedDisplayName
    package.Write(false);                           // m_publicPosition
  }
}
