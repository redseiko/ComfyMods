namespace Eventuality;

using ComfyLib;

public static class StopRandomEventCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "stop-random-event",
        "(Eventuality) stop-random-event",
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

    RandEventSystem randEventSystem = RandEventSystem.m_instance;

    if (randEventSystem.m_randomEvent == null) {
      Eventuality.LogInfo("No current RandomEvent to stop.");
    }

    RandEventManager.ResetRandomEvent(randEventSystem);
    return true;
  }
}
