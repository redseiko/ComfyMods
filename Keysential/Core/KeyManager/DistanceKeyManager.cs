namespace Keysential;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class DistanceKeyManager {
  public static IEnumerator DistanceXZProximityCoroutine(
      string managerId, Vector3 position, float distance, IEnumerable<string> keysToAdd) {
    Keysential.LogInfo(
        $"Starting DistanceXZProximityCoroutine coroutine... "
            + $"position: {position}, distance: {distance}, keys: {string.Join(",", keysToAdd)}");

    List<string> originalKeys = [];
    List<string> nearbyKeys = [];

    HashSet<long> nearbyPeers = GlobalKeysManager.NearbyPeerIdsCache[managerId];
    WaitForSeconds waitInterval = new(seconds: 3f);

    while (ZNet.m_instance) {
      originalKeys.Clear();
      originalKeys.AddRange(ZoneSystem.m_instance.m_globalKeys);

      nearbyKeys.Clear();
      nearbyKeys.AddRange(originalKeys);
      nearbyKeys.AddRange(keysToAdd);

      foreach (ZNetPeer netPeer in ZNet.m_instance.m_peers) {
        bool isNearby = Utils.DistanceXZ(netPeer.m_refPos, position) <= distance;

        if (isNearby) {
          if (nearbyPeers.Contains(netPeer.m_uid)) {
            // Do nothing.
          } else {
            Keysential.LogInfo($"Sending nearby global keys to peer: {netPeer.m_uid}");
            ZRoutedRpc.s_instance.InvokeRoutedRPC(netPeer.m_uid, "GlobalKeys", nearbyKeys);
            nearbyPeers.Add(netPeer.m_uid);

            GlobalKeysManager.SendChatMessage(
                netPeer.m_uid, position, "<color=green>Keysential</color>", $"Now entering: {managerId}");
          }
        } else {
          if (nearbyPeers.Contains(netPeer.m_uid)) {
            Keysential.LogInfo($"Sending original global keys to peer: {netPeer.m_uid}");
            ZRoutedRpc.s_instance.InvokeRoutedRPC(netPeer.m_uid, "GlobalKeys", originalKeys);
            nearbyPeers.Remove(netPeer.m_uid);

            GlobalKeysManager.SendChatMessage(
                netPeer.m_uid, position, "<color=green>Keysential</color>", $"Now exiting: {managerId}");
          } else {
            // Do nothing.
          }
        }
      }

      yield return waitInterval;
    }
  }
}
