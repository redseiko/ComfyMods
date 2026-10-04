namespace Perpetuity;

using System.Collections.Generic;
using System.IO;
using System.Linq;

using ComfyLib;

public static class DataWriterUtils {
  public static void WritePersistentEventData(
      IReadOnlyCollection<PersistentEventSystem.PersistentEvent> persistentEvents, string filename) {
    Perpetuity.LogInfo($"Writing PersistentEvent data to: {filename}");

    using StreamWriter writer = File.CreateText(filename);
    writer.WriteLine(PersistentEventCsvHeader);

    foreach (PersistentEventSystem.PersistentEvent persistentEvent in persistentEvents) {
      writer.WriteLine(persistentEvent.ToCsvString());
      writer.Flush();
    }

    Perpetuity.LogInfo($"Finished writing {persistentEvents.Count} PersistentEvent rows to {filename}.");
  }

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

  public static string ToCsvString(this PersistentEventSystem.PersistentEvent persistentEvent) {
    return string.Join(
        ",",
        persistentEvent.internalName,
        persistentEvent.mapTokenString,
        persistentEvent.minRadius,
        persistentEvent.maxRadius,
        persistentEvent.maxConcurrent,
        persistentEvent.locationToSpawn,
        persistentEvent.hasDuration,
        persistentEvent.minDurationInSeconds,
        persistentEvent.maxDurationInSeconds,
        persistentEvent.objectsToSpawn?.Count ?? 0,
        persistentEvent.biomes.GetBiomes().ToCsvString(),
        persistentEvent.minDistanceFromCenter,
        persistentEvent.maxDistanceFromCenter,
        persistentEvent.minHeight,
        persistentEvent.maxHeight,
        persistentEvent.maxSlope,
        persistentEvent.minDistanceFromSimilar,
        persistentEvent.environmentOverride,
        persistentEvent.perBiomeEnvironments,
        persistentEvent.graphicalEffects.GetFlags().ToCsvString());
  }

  public static void WriteObjectSpawnSettingsData(
      IReadOnlyCollection<PersistentEventSystem.PersistentEvent> persistentEvents, string filename) {
    Perpetuity.LogInfo($"Writing ObjectSpawnSettings data to: {filename}");

    using StreamWriter writer = File.CreateText(filename);
    writer.WriteLine(ObjectSpawnSettingsCsvHeader);

    int rowsWritten = 0;

    foreach (PersistentEventSystem.PersistentEvent persistentEvent in persistentEvents) {
      if (persistentEvent.objectsToSpawn == null) {
        continue;
      }

      foreach (PersistentEventSystem.ObjectSpawnSettings setting in persistentEvent.objectsToSpawn) {
        writer.WriteLine(setting.ToCsvString(persistentEvent.internalName));
        writer.Flush();
        rowsWritten++;
      }
    }

    Perpetuity.LogInfo($"Finished writing {rowsWritten} ObjectSpawnSettings rows to {filename}.");
  }

  public static readonly string ObjectSpawnSettingsCsvHeader =
      "persistentEvent,"
          + "name,"
          + "prefabs,"
          + "important,"
          + "randomRotation,"
          + "minCount,maxCount,"
          + "minDistanceFromOtherEventObjects,"
          + "maxDistanceFromEventCenter,"
          + "maxSlope,"
          + "waterSpawning";

  public static string ToCsvString(
      this PersistentEventSystem.ObjectSpawnSettings setting, string persistentEventName) {
    string prefabsString =
        setting.prefabs == default
            ? "\"\""
            : setting.prefabs.Select(prefab => prefab ? prefab.name : string.Empty).ToCsvString();

    return string.Join(
        ",",
        persistentEventName,
        setting.name,
        prefabsString,
        setting.important,
        setting.randomRotation,
        setting.minCount,
        setting.maxCount,
        setting.minDistanceFromOtherEventObjects,
        setting.maxDistanceFromEventCenter,
        setting.maxSlope,
        setting.waterSpawning.ToString());
  }
}
