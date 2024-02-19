namespace Transporter;

using System.Collections;
using System.Collections.Generic;

using UnityEngine;

public static class TeleportManager {
  public static void RequestTeleport(ZRpc rpc, long playerId, Vector3 destination) {
    string steamId = rpc.m_socket.GetHostName();
    bool hasAccess = AccessUtils.HasAccess(steamId);

    AccessUtils.LogAccess($"RequestTeleportPlayerId,{hasAccess},{steamId},{playerId},\"{destination:F0}\"");

    if (hasAccess) {
      Transporter.LogInfo(
          $"RPC RequestTeleport from ({steamId}) for PlayerId ({playerId}) to {destination:F0} allowed.");

      RequestManager.Instance.AddRequest(playerId, destination);
    } else {
      Transporter.LogError(
          $"RPC RequestTeleport from ({steamId}) for PlayerId ({playerId}) to {destination:F0} denied.");
    }
  }

  public static void RequestTeleportByZDOID(ZRpc rpc, ZDOID playerZDOID, Vector3 destination) {
    string steamId = rpc.m_socket.GetHostName();
    bool hasAccess = AccessUtils.HasAccess(steamId);

    AccessUtils.LogAccess($"RequestTeleportByZDOID,{hasAccess},{steamId},{playerZDOID},\"{destination:F0}\"");

    if (hasAccess) {
      if (PlayerUtils.TryGetPlayerId(playerZDOID, out long playerId)) {
        Transporter.LogInfo(
            $"RPC RequestTeleport from ({steamId}) for PlayerZDOID ({playerZDOID}) PlayerId ({playerId}) to "
                + $"{destination:F0} allowed.");

        RequestManager.Instance.AddRequest(playerId, destination);
      } else {
        Transporter.LogError(
            $"RPC RequestTeleport from ({steamId}) for PlayerZDOID ({playerZDOID}) to {destination:F0} failed to "
                + $"find matching PlayerId.");
      }
    } else {
      Transporter.LogError(
          $"RPC RequestTeleport from ({steamId}) for PlayerZDOID ({playerZDOID}) to {destination:F0} denied.");
    }
  }

  public static void TeleportPlayer(long playerId, Vector3 destination) {
    AccessUtils.LogAccess($"TeleportPlayer,{playerId},\"{destination:F0}\"");
    Transporter.LogInfo($"Pending teleport requested for PlayerId ({playerId}) to {destination:F0}.");

    RequestManager.Instance.AddRequest(playerId, destination);
  }

  public static bool CancelTeleportPlayer(long playerId) {
    AccessUtils.LogAccess($"CancelTeleportPlayer,{playerId}");

    if (RequestManager.Instance.RemoveRequest(playerId)) {
      Transporter.LogInfo($"Cancelling pending teleport for PlayerId ({playerId}).");
      return true;
    }

    return false;
  }

  static readonly List<TeleportRequest> _requests = new();

  public static void ProcessPendingTeleports() {
    int count = RequestManager.Instance.PendingRequests.Count;

    if (count <= 0) {
      return;
    }

    _requests.Clear();
    _requests.AddRange(RequestManager.Instance.PendingRequests.Values);

    int processed = 0;

    foreach (TeleportRequest request in _requests) {
      if (ProcessTeleport(request)) {
        RequestManager.Instance.RemoveRequest(request.PlayerId);
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
