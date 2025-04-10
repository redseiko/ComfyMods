﻿namespace ComfyLib;

using System;
using System.Reflection.Emit;

using HarmonyLib;

public static class CodeMatcherExtensions {
  public static CodeMatcher CreateLabelOffset(this CodeMatcher matcher, int offset, out Label label) {
    return matcher.CreateLabelAt(matcher.Pos + offset, out label);
  }

  public static CodeMatcher DeclareLocal(
      this CodeMatcher matcher, ILGenerator generator, Type localType, out LocalBuilder localBuilder) {
    localBuilder = generator.DeclareLocal(localType);
    return matcher;
  }
}
