namespace ColorfulPieces;

using ComfyLib;

using UnityEngine;

using static PluginConfig;

public static class ShortcutUtils {
  public static void CheckShortcuts(GameObject hovering) {
    if (hovering) {
      if (ChangePieceColorShortcut.Value.IsDown()) {
        OnChangePieceColorShortcut(hovering);
      }

      if (ClearPieceColorShortcut.Value.IsDown()) {
        OnClearPieceColorShortcut(hovering);
      }

      if (CopyPieceColorShortcut.Value.IsDown()) {
        OnCopyPieceColorShortcut(hovering);
      }
    }

    if (ToggleColorPickerShortcut.Value.IsDown()) {
      OnToggleColorPickerShortcut();
    }
  }

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

  public static void OnToggleColorPickerShortcut() {
    if (!ColorPickerController.HasVisibleInstance() && Game.instance) {
      ColorPickerController.Instance.ShowColorPicker(
          currentColor: TargetPieceColor.Value,
          onColorSelectedCallback: SetTargetPieceColorConfigValue);
    }
  }

  public static void SetTargetPieceColorConfigValue(Color color) {
    ColorfulPieces.LogInfo($"Setting Color.targetPieceColor config value to: {color}");
    TargetPieceColor.SetValue(color);
  }
}
