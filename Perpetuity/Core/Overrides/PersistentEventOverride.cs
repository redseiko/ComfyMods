namespace Perpetuity;

public sealed record PersistentEventOverride {
  public string InternalName { get; init; } = string.Empty;

  public string? MapTokenString { get; init; }

  public float? MinRadius { get; init; }
  public float? MaxRadius { get; init; }

  public int? MaxConcurrent { get; init; }
  public string? LocationToSpawn { get; init; }

  public bool? HasDuration { get; init; }
  public float? MinDurationInSeconds { get; init; }
  public float? MaxDurationInSeconds { get; init; }

  public Heightmap.Biome? Biomes { get; init; }

  public float? MinDistanceFromCenter { get; init; }
  public float? MaxDistanceFromCenter { get; init; }
  public float? MinHeight { get; init; }
  public float? MaxHeight { get; init; }
  public float? MaxSlope { get; init; }
  public float? MinDistanceFromSimilar { get; init; }

  public string? EnvironmentOverride { get; init; }
  public bool? PerBiomeEnvironments { get; init; }

  public PersistentEventSystem.EventGraphicalEffects? GraphicalEffects { get; init; }

  public PersistentEventSystem.PersistentEvent Apply(PersistentEventSystem.PersistentEvent persistentEvent) {
    persistentEvent.mapTokenString = MapTokenString ?? persistentEvent.mapTokenString;
    persistentEvent.minRadius = MinRadius ?? persistentEvent.minRadius;
    persistentEvent.maxRadius = MaxRadius ?? persistentEvent.maxRadius;
    persistentEvent.maxConcurrent = MaxConcurrent ?? persistentEvent.maxConcurrent;
    persistentEvent.locationToSpawn = LocationToSpawn ?? persistentEvent.locationToSpawn;
    persistentEvent.hasDuration = HasDuration ?? persistentEvent.hasDuration;
    persistentEvent.minDurationInSeconds = MinDurationInSeconds ?? persistentEvent.minDurationInSeconds;
    persistentEvent.maxDurationInSeconds = MaxDurationInSeconds ?? persistentEvent.maxDurationInSeconds;
    persistentEvent.biomes = Biomes ?? persistentEvent.biomes;
    persistentEvent.minDistanceFromCenter = MinDistanceFromCenter ?? persistentEvent.minDistanceFromCenter;
    persistentEvent.maxDistanceFromCenter = MaxDistanceFromCenter ?? persistentEvent.maxDistanceFromCenter;
    persistentEvent.minHeight = MinHeight ?? persistentEvent.minHeight;
    persistentEvent.maxHeight = MaxHeight ?? persistentEvent.maxHeight;
    persistentEvent.maxSlope = MaxSlope ?? persistentEvent.maxSlope;
    persistentEvent.minDistanceFromSimilar = MinDistanceFromSimilar ?? persistentEvent.minDistanceFromSimilar;
    persistentEvent.environmentOverride = EnvironmentOverride ?? persistentEvent.environmentOverride;
    persistentEvent.perBiomeEnvironments = PerBiomeEnvironments ?? persistentEvent.perBiomeEnvironments;
    persistentEvent.graphicalEffects = GraphicalEffects ?? persistentEvent.graphicalEffects;

    return persistentEvent;
  }
}
