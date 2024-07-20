namespace ComfyLib;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

public static class CodeMatcherExtensions {
  public static CodeMatcher ExtractLabels(this CodeMatcher matcher, out List<Label> labels) {
    labels = new(matcher.Labels);
    matcher.Labels.Clear();
    return matcher;
  }
}
