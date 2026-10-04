namespace Perpetuity;

using static PersistentEventSystem;

public static class PersistentEventManager {
  public static readonly PersistentEventOverride[] ComfyPersistentEventOverrides = [
    new PersistentEventOverride() {
      InternalName = "jotun_invasion",
      Biomes = Heightmap.Biome.DeepNorth,
    },
  ];

  public static void ApplyOverrides(PersistentEventSystem system) {
    foreach (PersistentEventOverride persistentEventOverride in ComfyPersistentEventOverrides) {
      if (system.TryGetPersistentEvent(persistentEventOverride.InternalName, out PersistentEvent persistentEvent)) {
        Perpetuity.LogInfo($"Overriding PersistentEvent {persistentEvent.internalName}...");
        persistentEventOverride.Apply(persistentEvent);
      }
    }
  }

  public static bool TryGetPersistentEvent(
      this PersistentEventSystem persistentEventSystem,
      string persistentEventName,
      out PersistentEvent persistentEvent) {
    foreach (PersistentEvent possibleEvent in persistentEventSystem.m_possibleEvents) {
      if (possibleEvent.internalName == persistentEventName) {
        persistentEvent = possibleEvent;
        return true;
      }
    }

    persistentEvent = default;
    return false;
  }
}
