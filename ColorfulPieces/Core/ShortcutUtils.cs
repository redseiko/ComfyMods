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
  }

  public static void CheckPickerShortcuts() {
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
    if (ColorPickerController.HasVisibleInstance()) {
      ColorPickerController.Instance.HideColorPicker();
    } else if (Game.instance) {
      ColorPickerController.Instance.ShowColorPicker(
          currentColor: TargetPieceColor.Value,
          selectColorCallback: SetTargetPieceColor,
          selectColorOnClose: SelectColorOnClose.Value,
          useColorAlpha: false,
          showEmissionColorFactor: true,
          currentEmissionColorFactor: TargetPieceEmissionColorFactor.Value,
          selectEmissionColorFactorCallback: SetTargetPieceEmissionColorFactor,
          paletteColors: TargetPieceColor.GetPaletteColors(),
          changePaletteColorsCallback: TargetPieceColor.SetPaletteColors);
    }
  }

  public static void SetTargetPieceColor(Color color) {
    TargetPieceColor.SetValue(color);
  }

  public static void SetTargetPieceEmissionColorFactor(float factor) {
    TargetPieceEmissionColorFactor.Value = factor;
  }
}
