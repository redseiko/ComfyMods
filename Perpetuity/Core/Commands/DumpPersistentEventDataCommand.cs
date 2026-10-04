namespace Perpetuity;

using ComfyLib;

using System;

public static class DumpPersistentEventDataCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "dump-persistent-event-data",
        "(Perpetuity) dump-persistent-event-data",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    return Run(new ComfyArgs(args));
  }

  public static bool Run(ComfyArgs args) {
    if (!PersistentEventSystem.instance) {
      Perpetuity.LogError("PersistentEventSystem.instance is null.");
      return false;
    }
    
    long timestamp = DateTimeOffset.UtcNow.ToUnixTimeSeconds();

    DataWriterUtils.WritePersistentEventData(
        PersistentEventSystem.instance.m_possibleEvents,
        $"persistent-event-data-{timestamp}.txt");

    DataWriterUtils.WriteObjectSpawnSettingsData(
        PersistentEventSystem.instance.m_possibleEvents,
        $"object-spawn-settings-data-{timestamp}.txt");

    return true;
  }
}
