using System.Collections.Generic;

using ComfyLib;

namespace ColorfulPieces {
  public static class ColorPickerCommand {
    [ComfyCommand]
    public static IEnumerable<Terminal.ConsoleCommand> Register() {
      yield return new Terminal.ConsoleCommand(
          "color-picker",
          "(ColorfulPieces) Toggle the color picker.",
          args => Run(new ComfyArgs(args)));
    }

    static ColorPicker _colorPicker;

    public static bool Run(ComfyArgs args) {
      if (_colorPicker?.Panel) {
        _colorPicker.Panel.gameObject.SetActive(!_colorPicker.Panel.gameObject.activeSelf);
      } else {
        _colorPicker = new(Hud.instance.m_rootObject.transform);
      }

      return true;
    }
  }
}
