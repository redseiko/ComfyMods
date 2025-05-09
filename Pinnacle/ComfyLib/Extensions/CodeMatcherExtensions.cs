namespace ComfyLib;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

public static class CodeMatcherExtensions {
  public static CodeMatcher CreateLabelOffset(this CodeMatcher matcher, int offset, out Label label) {
    return matcher.CreateLabelAt(matcher.Pos + offset, out label);
  }

  public static CodeMatcher ExtractLabels(this CodeMatcher matcher, out List<Label> labels) {
    labels = [.. matcher.Labels];
    matcher.Labels.Clear();

    return matcher;
  }

  public static CodeMatcher SaveOperand(this CodeMatcher matcher, out object operand) {
    operand = matcher.Operand;
    return matcher;
  }
}
