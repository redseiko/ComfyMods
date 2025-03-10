namespace Keysential;

using System.Collections;
using System.Collections.Generic;

using Splatform;

using Steamworks;

using UnityEngine;

public static class GlobalKeysManager {
  public class KeyManager {
    public string ManagerId { get; }
    public Vector3 Position { get; }
    public float Distance { get; }
    public Coroutine Coroutine { get; }

    public KeyManager(string managerId, Vector3 position, float distance, Coroutine coroutine) {
      ManagerId = managerId;
      Position = position;
      Distance = distance;
      Coroutine = coroutine;
    }
  }

  public static Dictionary<string, KeyManager> CurrentKeyManagers { get; private set; } = [];
  public static Dictionary<string, HashSet<long>> NearbyPeerIdsCache { get; private set; } = [];

  public static bool StartKeyManager(string managerId, Vector3 position, float distance, IEnumerator managerMethod) {
    if (CurrentKeyManagers.ContainsKey(managerId)) {
      Keysential.LogError($"Cannot start due to existing KeyManager coroutine found with id: {managerId}");
      return false;
    }

    Keysential.LogInfo($"Starting KeyManager coroutine with id: {managerId}");

    NearbyPeerIdsCache[managerId] = new(capacity: 256);
    Coroutine managerCoroutine = ZNet.m_instance.StartCoroutine(managerMethod);
    CurrentKeyManagers[managerId] = new(managerId, position, distance, managerCoroutine);

    return true;
  }

  public static bool StopKeyManager(string managerId) {
    if (!CurrentKeyManagers.TryGetValue(managerId, out KeyManager keyManager)) {
      Keysential.LogError($"Could not find KeyManager coroutine with id: {managerId}");
      return false;
    }

    if (keyManager.Coroutine != null) {
      Keysential.LogInfo($"Stopping KeyManager coroutine with id: {managerId}");
      ZNet.instance.StopCoroutine(keyManager.Coroutine);
    }

    CurrentKeyManagers.Remove(managerId);

    ResetNearbyPeers(keyManager, NearbyPeerIdsCache[managerId]);
    NearbyPeerIdsCache.Remove(managerId);

    return true;
  }

  static void ResetNearbyPeers(KeyManager keyManager, HashSet<long> nearbyPeerIds) {
    List<string> originalKeys = [.. ZoneSystem.m_instance.m_globalKeys];

    foreach (long nearbyPeerId in nearbyPeerIds) {
      Keysential.LogInfo($"Sending original global keys to peer: {nearbyPeerId}");
      ZRoutedRpc.s_instance.InvokeRoutedRPC(nearbyPeerId, "GlobalKeys", originalKeys);

      SendChatMessage(
          nearbyPeerId,
          keyManager.Position,
          "<color=green>Keysential</color>",
          $"Now exiting: {keyManager.ManagerId}");
    }
  }

  public static void SendChatMessage(long netPeerId, Vector3 position, string name, string message) {
    string steamId = SteamGameServer.GetSteamID().ToString();

    UserInfo userInfo = new UserInfo() {
      Name = name,
      UserId = new PlatformUserID(ZNet.m_instance.m_steamPlatform, steamId),
    };

    ZRoutedRpc.s_instance.InvokeRoutedRPC(
        netPeerId, "ChatMessage", position, (int) Talker.Type.Normal, userInfo, message);
  }
}
