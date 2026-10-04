namespace ComfyLib;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

public static class CodeMatcherExtensions {
  public static CodeMatcher CopyOperand<T>(this CodeMatcher matcher, out T target) {
    target = (T) matcher.Operand;
    return matcher;
  }

  public static CodeMatcher ExtractLabels(this CodeMatcher matcher, out List<Label> labels) {
    labels = [.. matcher.Labels];
    matcher.Labels.Clear();

    return matcher;
  }
}
