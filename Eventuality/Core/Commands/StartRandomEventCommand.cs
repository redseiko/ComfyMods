namespace Eventuality;

using System.Collections.Generic;

using ComfyLib;

using UnityEngine;

public static class StartRandomEventCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "start-random-event",
        "(Eventuality) start-random-event --event=<name|random> [--position=<x,y,z>]",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    return Run(new ComfyArgs(args));
  }

  public static bool Run(ComfyArgs args) {
    if (!ZNet.m_instance || !ZNet.m_isServer || !RandEventSystem.m_instance) {
      Eventuality.LogError("Command can only be run on an active server instance.");
      return false;
    }

    if (!args.TryGetValue("event", out string eventName) || string.IsNullOrEmpty(eventName)) {
      Eventuality.LogError("Missing or invalid value for --event.");
      return false;
    }

    Vector3? targetPosition = default;

    if (args.TryGetValue("position", "pos", out string positionArg)) {
      if (!positionArg.TryParseVector3(out Vector3 position) || position.y > 3000f) {
        Eventuality.LogError($"Invalid Vector3 value for --position: {positionArg}");
        return false;
      }

      targetPosition = position;
    }

    RandEventSystem randEventSystem = RandEventSystem.m_instance;
    List<RandomEvent> eligibleEvents = [];

    if (eventName == "random") {
      eligibleEvents.AddRange(randEventSystem.m_events);
    } else {
      RandomEvent targetEvent = randEventSystem.GetEvent(eventName);

      if (targetEvent == null) {
        Eventuality.LogError($"Could not find matching RandomEvent for {eventName}.");
        return false;
      }

      eligibleEvents.Add(targetEvent);
    }

    RandEventSystem.RefreshPlayerEventData();

    List<KeyValuePair<RandomEvent, Vector3>> candidateEvents =
        targetPosition.HasValue
            ? GenerateCandidateEvents(randEventSystem, eligibleEvents, targetPosition.Value)
            : GenerateCandidateEvents(randEventSystem, eligibleEvents);

    if (candidateEvents.Count <= 0) {
      Eventuality.LogWarning($"Could not generate a RandomEvent for {eventName}.");
      return false;
    }

    KeyValuePair<RandomEvent, Vector3> selected = candidateEvents[Random.Range(0, candidateEvents.Count)];
    RandEventManager.SetRandomEvent(randEventSystem, selected.Key, selected.Value);

    return true;
  }

  static List<KeyValuePair<RandomEvent, Vector3>> GenerateCandidateEvents(
      RandEventSystem randEventSystem, List<RandomEvent> eligibleEvents) {
    List<KeyValuePair<RandomEvent, Vector3>> candidateEvents = [];

    foreach (RandomEvent randomEvent in eligibleEvents) {
      if (!randomEvent.m_enabled
          || !randomEvent.m_random
          || !randEventSystem.HaveGlobalKeys(randomEvent, RandEventSystem.s_playerEventDatas)) {
        continue;
      }

      List<Vector3> validPoints = randEventSystem.GetValidEventPoints(randomEvent, RandEventSystem.s_playerEventDatas);

      if (validPoints.Count > 0) {
        candidateEvents.Add(new(randomEvent, validPoints[Random.Range(0, validPoints.Count)]));
      }
    }

    return candidateEvents;
  }

  static List<KeyValuePair<RandomEvent, Vector3>> GenerateCandidateEvents(
      RandEventSystem randEventSystem, List<RandomEvent> eligibleEvents, Vector3 targetPosition) {
    List<KeyValuePair<RandomEvent, Vector3>> candidateEvents = [];

    foreach (RandomEvent randomEvent in eligibleEvents) {
      if (!randomEvent.m_enabled
          || !randomEvent.m_random
          || !randEventSystem.HaveGlobalKeys(randomEvent, RandEventSystem.s_playerEventDatas)) {
        continue;
      }

      if (RandEventManager.IsInValidBiome(randomEvent, targetPosition)) {
        candidateEvents.Add(new(randomEvent, targetPosition));
      }
    }

    return candidateEvents;
  }
}
