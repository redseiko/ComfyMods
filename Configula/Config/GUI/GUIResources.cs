using System;

using UnityEngine;

namespace Configula {
  public static class GUIResources {
    public static readonly Lazy<GUIStyle> WordWrapTextField =
        new(() => new(GUI.skin.textField) { wordWrap = true });
  }
}
