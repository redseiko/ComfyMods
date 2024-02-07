namespace ColorfulPieces;

using ComfyLib;

using UnityEngine;

public static class ShortcutUtils {
  public static bool OnChangePieceColorShortcut(GameObject hovering) {
    if (hovering.TryGetComponentInParent(out WearNTear changeTarget)) {
      ColorfulPieces.ChangePieceColorAction(changeTarget);
      return true;
    }

    return false;
  }

  public static bool OnClearPieceColorShortcut(GameObject hovering) {
    if (hovering.TryGetComponentInParent(out WearNTear clearTarget)) {
      ColorfulPieces.ClearPieceColorAction(clearTarget);
      return true;
    }

    return false;
  }

  public static bool OnCopyPieceColorShortcut(GameObject hovering) {
    if (hovering.TryGetComponentInParent(out WearNTear copyTarget)) {
      ColorfulPieces.CopyPieceColorAction(copyTarget.m_nview);
      return true;
    }

    return false;
  }
}
