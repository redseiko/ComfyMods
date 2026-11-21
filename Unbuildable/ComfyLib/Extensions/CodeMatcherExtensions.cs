namespace ComfyLib;

using HarmonyLib;

public static class CodeMatcherExtensions {
  public static CodeMatcher CopyInstruction(this CodeMatcher matcher, out CodeInstruction instruction) {
    instruction = new CodeInstruction(matcher.Opcode, matcher.Operand);
    return matcher;
  }
}
