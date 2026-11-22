namespace Eventuality;

using System;
using System.IO;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

public static class RandEventManager {
  public static FileLogWriter RandomEventLogWriter { get; private set; } =
      new FileLogWriter(
          Path.Combine(Utils.GetSaveDataPath(FileHelpers.FileSource.Local), RandomEventLogFilename.Value),
          "timestamp,status,eventName,time,posX,posY,posZ");

  public static void LogEvent(string status, RandomEvent randomEvent) {
    long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();
    Vector3 position = randomEvent.m_pos;

    RandomEventLogWriter.WriteLine(
        $"{timestamp},{status},{randomEvent.m_name},{randomEvent.m_time:F0},"
            + $"{position.x:F0},{position.y:F0},{position.z:F0}");

    Eventuality.LogInfo(
        $"[RandomEvent] "
            + $"status: {status}, event: {randomEvent.m_name}, time: {randomEvent.m_time:F0}, position: {position:F0}");
  }

  public static void LogSetEventRequest(
      long senderId, string hostName, string status, string eventName, Vector3 position) {
    AccessManager.LogAccess(senderId, hostName, status, eventName, position);

    Eventuality.LogInfo(
        $"[SetRandomEventRequest] "
            + $"status: {status}, sender: {hostName} ({senderId}), event: {eventName}, position: {position:F0}");
  }

  public static bool ProcessSetEventRequest(
      RandEventSystem randEventSystem, long senderPeerId, string eventName, float time, Vector3 position) {
    string hostName = ZRoutedRpc.s_instance.GetPeer(senderPeerId)?.m_socket.GetHostName();

    if (!AccessManager.HasAccess(hostName)) {
      LogSetEventRequest(senderPeerId, hostName, "unauthorized", eventName, position);
      return false;
    }

    if (string.Equals(eventName, "none", StringComparison.OrdinalIgnoreCase)) {
      LogSetEventRequest(senderPeerId, hostName, "event_stop", eventName, position);
      ResetRandomEvent(randEventSystem);

      return true;
    }

    RandomEvent targetEvent = randEventSystem.GetEvent(eventName);

    if (targetEvent == null) {
      LogSetEventRequest(senderPeerId, hostName, "event_not_found", eventName, position);
      return false;
    }

    LogSetEventRequest(senderPeerId, hostName, "authorized", eventName, position);
    SetRandomEvent(randEventSystem, targetEvent, position);

    return true;
  }

  public static void ResetRandomEvent(RandEventSystem randEventSystem) {
    StopCurrentRandomEvent(randEventSystem);

    randEventSystem.SendCurrentRandomEvent();
  }

  public static void SetRandomEvent(RandEventSystem randEventSystem, RandomEvent randomEvent, Vector3 position) {
    StopCurrentRandomEvent(randEventSystem);
    StartRandomEvent(randEventSystem, randomEvent, position);

    randEventSystem.SendCurrentRandomEvent();
  }

  static void StartRandomEvent(RandEventSystem randEventSystem, RandomEvent randomEvent, Vector3 position) {
    if (randomEvent == null) {
      return;
    }

    RandomEvent nextEvent = randomEvent.Clone();
    nextEvent.m_pos = position;
    SetPauseIfNoPlayerInArea(nextEvent);

    nextEvent.OnStart();
    randEventSystem.m_randomEvent = nextEvent;

    LogEvent("event_start", nextEvent);
  }

  static void StopCurrentRandomEvent(RandEventSystem randEventSystem) {
    RandomEvent currentEvent = randEventSystem.m_randomEvent;

    if (currentEvent == null) {
      return;
    }

    LogEvent("event_end", currentEvent);

    if (currentEvent == randEventSystem.m_activeEvent) {
      randEventSystem.SetActiveEvent(ev: null, end: true);
    }

    currentEvent.OnStop();
    randEventSystem.m_randomEvent = null;
  }

  static void SetPauseIfNoPlayerInArea(RandomEvent randomEvent) {
    switch (PauseIfNoPlayerRequirement.Value) {
      case PauseEventRequirement.PauseIfNoPlayer:
        randomEvent.m_pauseIfNoPlayerInArea = true;
        break;

      case PauseEventRequirement.NeverPause:
        randomEvent.m_pauseIfNoPlayerInArea = false;
        break;
    }
  }

  public static bool IsInValidBiome(RandomEvent randomEvent, Vector3 point) {
    Heightmap.Biome biome = WorldGenerator.m_instance.GetBiome(point);

    if ((biome & RandomEventBiomesToExclude.Value) != Heightmap.Biome.None) {
      return false;
    }

    return randomEvent.m_biome == Heightmap.Biome.None || (biome & randomEvent.m_biome) != Heightmap.Biome.None;
  }

  public static bool CheckBase(RandomEvent randomEvent, RandEventSystem.PlayerEventData player) {
    switch (CheckPlayerBaseRequirement.Value) {
      case PlayerBaseRequirement.NearBase:
        return player.baseValue >= MinimumPlayerBaseValue.Value;

      case PlayerBaseRequirement.AwayFromBase:
        return player.baseValue < MinimumPlayerBaseValue.Value;

      case PlayerBaseRequirement.Anywhere:
        return true;

      default:
        return !randomEvent.m_nearBaseOnly || player.baseValue >= MinimumPlayerBaseValue.Value;
    }
  }
}
