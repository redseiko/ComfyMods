namespace Transporter;

using System.Collections.Generic;
using System.IO;

using UnityEngine;

using static PluginConfig;

public sealed class RequestManager {
  static RequestManager _instance;

  public static RequestManager Instance {
    get => _instance ??= new();
  }

  public Dictionary<long, TeleportRequest> PendingRequests { get; }
  public SyncedList RequestList { get; }

  RequestManager() {
    PendingRequests = new();

    RequestList =
        new SyncedList(
            Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), RequestListFilename.Value),
            "Transporter teleport request list. Request per line: <playerId>,<destination>,<epochTimestamp>");
  }

  public void AddRequest(long playerId, Vector3 destination) {
    TeleportRequest request = new(playerId, destination);
    PendingRequests[playerId] = request;

    SavePendingRequests();
  }

  public bool RemoveRequest(long playerId) {
    if (PendingRequests.Remove(playerId)) {
      SavePendingRequests();
      return true;
    }

    return false;
  }

  public void LoadPendingRequests() {
    RequestList.Load();

    foreach (string text in RequestList.m_list) {
      if (TeleportRequest.TryParse(text, out TeleportRequest request)) {
        PendingRequests[request.PlayerId] = request;
      }
    }
  }

  public void SavePendingRequests() {
    RequestList.m_list.Clear();

    foreach (TeleportRequest request in PendingRequests.Values) {
      RequestList.m_list.Add(request.ToString());
    }

    RequestList.Save();
  }
}
