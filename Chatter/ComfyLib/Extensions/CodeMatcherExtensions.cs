namespace ComfyLib;

using HarmonyLib;

public static class CodeMatcherExtensions {
  public static CodeMatcher CopyOperand<T>(this CodeMatcher matcher, out T target) {
    target = (T) matcher.Operand;
    return matcher;
  }
}
