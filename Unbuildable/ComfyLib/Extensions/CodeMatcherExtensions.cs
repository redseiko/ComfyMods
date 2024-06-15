namespace ComfyLib;

using HarmonyLib;

public static class CodeMatcherExtensions {
  public static CodeMatcher SaveInstruction(this CodeMatcher matcher, int offset, out CodeInstruction instruction) {
    instruction = matcher.InstructionAt(offset);
    return matcher;
  }
}
