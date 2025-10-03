namespace Keysential;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

using static PluginConfig;

public sealed class VendorKeyManager : MonoBehaviour {
  static readonly float _vendorNearbyDistance = 8f;
  static readonly string[] _vendorNearbyGlobalKeys = ["defeated_goblinking"];

  void Awake() {
    if (ZNet.m_isServer && VendorKeyManagerPosition.Value != Vector3.zero) {
      GlobalKeysManager.StartKeyManager(
          "haldor0",
          VendorKeyManagerPosition.Value,
          _vendorNearbyDistance,
          VendorPlayerProximityCoroutine(
              "haldor0", VendorKeyManagerPosition.Value, _vendorNearbyDistance, _vendorNearbyGlobalKeys));
    }
  }

  public static IEnumerator VendorPlayerProximityCoroutine(
      string managerId, Vector3 vendorPosition, float vendorDistance, IEnumerable<string> vendorKeys) {
    Keysential.LogInfo(
        $"Starting VendorPlayerProximityCoroutine coroutine... "
            + $"position: {vendorPosition}, distance: {vendorDistance}, keys: {string.Join(",", vendorKeys)}");

    List<string> originalKeys = [];
    List<string> nearbyKeys = [];

    HashSet<long> nearbyPeers = GlobalKeysManager.NearbyPeerIdsCache[managerId];
    WaitForSeconds waitInterval = new(seconds: 3f);

    while (ZNet.m_instance) {
      originalKeys.Clear();
      originalKeys.AddRange(ZoneSystem.m_instance.m_globalKeys);

      nearbyKeys.Clear();
      nearbyKeys.AddRange(originalKeys);
      nearbyKeys.AddRange(vendorKeys);

      foreach (ZNetPeer netPeer in ZNet.m_instance.m_peers) {
        bool isNearby = Vector3.Distance(netPeer.m_refPos, vendorPosition) <= vendorDistance;

        if (isNearby) {
          if (nearbyPeers.Contains(netPeer.m_uid)) {
            // Do nothing.
          } else {
            Keysential.LogInfo($"Sending nearby global keys to peer: {netPeer.m_uid}");
            ZRoutedRpc.s_instance.InvokeRoutedRPC(netPeer.m_uid, "GlobalKeys", nearbyKeys);
            nearbyPeers.Add(netPeer.m_uid);

            GlobalKeysManager.SendChatMessage(
                netPeer.m_uid,
                vendorPosition,
                "<color=green>Haldor</color>",
                EggNearbyChatMessages[Random.Range(0, EggNearbyChatMessages.Length)]);
          }
        } else {
          if (nearbyPeers.Contains(netPeer.m_uid)) {
            Keysential.LogInfo($"Sending original global keys to peer: {netPeer.m_uid}");
            ZRoutedRpc.s_instance.InvokeRoutedRPC(netPeer.m_uid, "GlobalKeys", originalKeys);
            nearbyPeers.Remove(netPeer.m_uid);
          } else {
            // Do nothing.
          }
        }
      }

      yield return waitInterval;
    }
  }

  public static readonly string[] EggNearbyChatMessages = [
    "I'm egg-traordinary!",
    "I'm feeling egg-cellent today!",
    "I'm egg-ceptional!",
    "I'm egg-cited to see you!",
    "You're so eggs-tra!",
    "My wares are in-eggs-pensive!",
    "I'm egg-static!",
    "Egg-cuse me?",
    "I have egg-axctly the thing you're after.",
    "Buy my eggs-clusive wares!",
  ];
}
