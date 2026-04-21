namespace ConstructionDerby;

using ComfyLib;

public static class StopDerbyGameCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "stop-derby-game",
        "(ConstructionDerby) stop-derby-game",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    if (!ZNet.instance) {
      return true;
    }

    if (!DerbyManager.HasCurrentGame()) {
      ConstructionDerby.LogInfo($"No current Derby game to stop.");
      return true;
    }

    ZNet.instance.StartCoroutine(DerbyManager.StopGameCoroutine());

    return true;
  }
}
