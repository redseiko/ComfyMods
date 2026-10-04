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
    
    PersistentEventUtils.WritePersistentEventData(
        PersistentEventSystem.instance.m_possibleEvents,
        $"persistent-event-data-{DateTimeOffset.UtcNow.ToUnixTimeSeconds()}.txt");

    return true;
  }
}
