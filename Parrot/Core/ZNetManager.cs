namespace Parrot;

using Splatform;

using Steamworks;

using static PluginConfig;

public static class ZNetManager {
  public static string ServerName { get; private set; } = "Server";
  public static long ServerPeerId { get; private set; } = 0L;
  public static ZDOID ServerCharacterID { get; private set; } = ZDOID.None;
  public static PlatformUserID ServerPlatformUserId { get; private set; } = PlatformUserID.None;

  public static void SetupServerPlayer(ZNet net) {
    ServerName = "Server";
    ServerPeerId = ZDOMan.s_instance.m_sessionID;
    ServerCharacterID = new(ServerPeerId, uint.MaxValue);
    ServerPlatformUserId = new(net.m_steamPlatform, SteamGameServer.GetSteamID().ToString());

    Parrot.LogInfo($"Parrot current server PlatformUserID: {ServerPlatformUserId}");
    Parrot.LogInfo($"Parrot current CharacterID: {ServerCharacterID}");
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
    package.Write(ServerCharacterID);               // m_characterID
    package.Write(ServerPlatformUserId.ToString()); // m_userInfo.m_id
    package.Write(ServerName);                      // m_userInfo.m_displayName
    package.Write(ServerName);                      // m_serverAssignedDisplayName
    package.Write(false);                           // m_publicPosition
  }
}
