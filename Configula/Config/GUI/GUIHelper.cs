using System.Collections.Generic;

using UnityEngine;

namespace Configula {
  public static class GUIHelper {
    static readonly Stack<Color> _colorStack = new();

    public static void BeginColor(Color color) {
      _colorStack.Push(GUI.color);
      GUI.color = color;
    }

    public static void EndColor() {
      GUI.color = _colorStack.Pop();
    }

    public static bool IsEnterPressed() {
      return
          Event.current.isKey
          && (Event.current.keyCode == KeyCode.Return || Event.current.keyCode == KeyCode.KeypadEnter);
    }
  }
}
