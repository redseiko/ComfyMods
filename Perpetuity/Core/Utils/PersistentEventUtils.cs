namespace Perpetuity;

using System.Collections.Generic;
using System.IO;

using ComfyLib;

public static class PersistentEventUtils {
  public static readonly string PersistentEventCsvHeader =
      "internalName,"
          + "mapTokenString,"
          + "minRadius,maxRadius,"
          + "maxConcurrent,"
          + "locationToSpawn,"
          + "hasDuration,minDurationInSeconds,maxDurationInSeconds,"
          + "objectsToSpawnCount,"
          + "biomes,"
          + "minDistanceFromCenter,maxDistanceFromCenter,"
          + "minHeight,maxHeight,maxSlope,"
          + "minDistanceFromSimilar,"
          + "environmentOverride,"
          + "perBiomeEnvironments,"
          + "graphicalEffects";

  public static void WritePersistentEventData(
      IReadOnlyCollection<PersistentEventSystem.PersistentEvent> persistentEvents, string filename) {
    Perpetuity.LogInfo($"Writing PersistentEvent data to: {filename}");

    using StreamWriter writer = File.CreateText(filename);
    writer.WriteLine(PersistentEventCsvHeader);

    foreach (PersistentEventSystem.PersistentEvent @event in persistentEvents) {
      writer.WriteLine(@event.ToCsvString());
      writer.Flush();
    }

    Perpetuity.LogInfo($"Finished writing {persistentEvents.Count} PersistentEvent rows to {filename}.");
  }

  public static string ToCsvString(this PersistentEventSystem.PersistentEvent ev) {
    return string.Join(
        ",",
        ev.internalName,
        ev.mapTokenString,
        ev.minRadius,
        ev.maxRadius,
        ev.maxConcurrent,
        ev.locationToSpawn,
        ev.hasDuration,
        ev.minDurationInSeconds,
        ev.maxDurationInSeconds,
        ev.objectsToSpawn?.Count ?? 0,
        ev.biomes.GetBiomes().ToCsvString(),
        ev.minDistanceFromCenter,
        ev.maxDistanceFromCenter,
        ev.minHeight,
        ev.maxHeight,
        ev.maxSlope,
        ev.minDistanceFromSimilar,
        ev.environmentOverride,
        ev.perBiomeEnvironments,
        ev.graphicalEffects.GetFlags().ToCsvString());
  }
}
