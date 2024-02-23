namespace Configula;

using System;

using UnityEngine;

public static class GUIResources {
  public static readonly Lazy<GUIStyle> WordWrapTextField =
      new(() => new(GUI.skin.textField) { wordWrap = true });
}
