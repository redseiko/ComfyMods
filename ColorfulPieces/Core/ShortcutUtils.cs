namespace ColorfulPieces;

using ComfyLib;

using UnityEngine;

public static class ShortcutUtils {
  public static bool OnChangePieceColorShortcut(GameObject hovering) {
    if (hovering.TryGetComponentInParent(out WearNTear changeTarget) && changeTarget) {
      ColorfulUtils.ChangePieceColorAction(changeTarget);
      return true;
    }

    return false;
  }

  public static bool OnClearPieceColorShortcut(GameObject hovering) {
    if (hovering.TryGetComponentInParent(out WearNTear clearTarget) && clearTarget) {
      ColorfulUtils.ClearPieceColorAction(clearTarget);
      return true;
    }

    return false;
  }

  public static bool OnCopyPieceColorShortcut(GameObject hovering) {
    if (hovering.TryGetComponentInParent(out WearNTear copyTarget) && copyTarget) {
      ColorfulUtils.CopyPieceColorAction(copyTarget.m_nview);
      return true;
    }

    return false;
  }
}
