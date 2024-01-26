using System.Collections;
using System.Collections.Generic;

using UnityEngine;

namespace Transporter {
  public static class TeleportManager {
    public sealed class TeleportRequest {
      public long PlayerId { get; }
      public Vector3 Destination { get; }

      public TeleportRequest(long playerId, Vector3 destination) {
        PlayerId = playerId;
        Destination = destination;
      }
    }

    public static readonly Dictionary<long, TeleportRequest> PendingTeleports = new();

    public static void TeleportPlayer(long playerId, Vector3 destination) {
      Transporter.LogInfo($"Pending teleport requested for Player (PID: {playerId}) to {destination:F0}.");
      PendingTeleports[playerId] = new(playerId, destination);
    }

    public static bool CancelTeleportPlayer(long playerId) {
      if (PendingTeleports.Remove(playerId)) {
        Transporter.LogInfo($"Cancelling pending teleport for Player (PID: {playerId}).");
        return true;
      }

      return false;
    }

    static readonly List<TeleportRequest> _requests = new();

    public static void ProcessPendingTeleports() {
      int count = PendingTeleports.Count;

      if (count <= 0) {
        return;
      }

      _requests.Clear();
      _requests.AddRange(PendingTeleports.Values);

      int processed = 0;

      foreach (TeleportRequest request in _requests) {
        if (ProcessTeleport(request)) {
          PendingTeleports.Remove(request.PlayerId);
          processed++;
        }
      }

      if (processed > 0) {
        Transporter.LogInfo($"Processed {processed}/{count} pending teleports.");
      }

      _requests.Clear();
    }

    public static bool ProcessTeleport(TeleportRequest request) {
      if (PlayerUtils.TryGetPlayerZDO(request.PlayerId, out ZDO playerZDO) && playerZDO.HasOwner()) {
        ZNet.m_instance.StartCoroutine(DelayedTeleport(request.PlayerId, playerZDO, request.Destination));
        return true;
      }

      return false;
    }

    public static void Teleport(long playerId, ZDO playerZDO, Vector3 destination) {
      Transporter.LogInfo(
          $"Teleporting Player ({playerId}) ({playerZDO.m_uid}) from {playerZDO.m_position:F0} to {destination:F0}");

      ZRoutedRpc.s_instance.InvokeRoutedRPC(
          playerZDO.GetOwner(), playerZDO.m_uid, "RPC_TeleportTo", destination, Quaternion.identity, true);
    }

    static IEnumerator DelayedTeleport(long playerId, ZDO playerZDO, Vector3 destination) {
      ZDOID playerZDOID = playerZDO.m_uid;
      float endTime = Time.time + 2f;

      while (Time.time < endTime) {
        yield return null;
      }

      Teleport(playerId, playerZDO, destination);

      endTime = Time.time + 8f;
      
      while (Time.time < endTime) {
        yield return null;

        if (!playerZDO.IsValid() || !playerZDO.HasOwner()) {
          Transporter.LogError($"Player ({playerId}) ZDO ({playerZDOID}) became invalid during teleport!");
          yield break;
        }

        if (Vector3.Distance(playerZDO.m_position, destination) < 1f) {
          Transporter.LogInfo($"Player ({playerId}) sucessfully teleported to {destination:F0}!");
          yield break;
        }
      }

      Transporter.LogError(
          $"Player ({playerId}) ZDO ({playerZDOID}) ... "
              + $"position {playerZDO.m_position:F0} not at destination {destination:F0} within 8s! Retrying.");

      TeleportPlayer(playerId, destination);
    }
  }
}
