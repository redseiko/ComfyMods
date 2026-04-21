namespace ConstructionDerby;

using ComfyLib;

public static class StartDerbyGameCommand {
  [ComfyCommand]
  public static Terminal.ConsoleCommand Register() {
    return new Terminal.ConsoleCommand(
        "start-derby-game",
        "(ConstructionDerby) start-derby-game",
        Run);
  }

  public static object Run(Terminal.ConsoleEventArgs args) {
    if (!ZNet.instance) {
      return true;
    }

    if (DerbyManager.HasCurrentGame()) {
      ConstructionDerby.LogInfo($"Current Derby game in-progress.");
      return true;
    }

    ZNet.instance.StartCoroutine(DerbyManager.StartGameCoroutine());

    return true;
  }
}
