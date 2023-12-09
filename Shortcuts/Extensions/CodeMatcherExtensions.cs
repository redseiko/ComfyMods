using System;
using System.Reflection.Emit;

using HarmonyLib;

using UnityEngine;

namespace Shortcuts {
  public static class CodeMatcherExtensions {
    public static readonly CodeMatch ZInputGetKeyDownMatch =
        new(
            OpCodes.Call,
            AccessTools.Method(
                typeof(ZInput), nameof(ZInput.GetKeyDown), new Type[] { typeof(KeyCode), typeof(bool) }));

    public static readonly CodeMatch ZInputGetKeyMatch =
        new(
            OpCodes.Call,
            AccessTools.Method(typeof(ZInput), nameof(ZInput.GetKey), new Type[] { typeof(KeyCode), typeof(bool) }));

    public static CodeMatcher MatchGetKeyDown(this CodeMatcher matcher, int key) {
      return matcher
          .MatchForward(
              useEnd: false,
              key > 127 ? new CodeMatch(OpCodes.Ldc_I4, key) : new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(key)),
              new CodeMatch(OpCodes.Ldc_I4_1),
              ZInputGetKeyDownMatch)
          .ThrowIfInvalid($"Could not patch ZInput.GetKeyDown() for Key: {key}!")
          .Advance(offset: 2);
    }

    public static CodeMatcher MatchGetKey(this CodeMatcher matcher, int key) {
      return matcher
          .MatchForward(
              useEnd: false,
              key > 127 ? new CodeMatch(OpCodes.Ldc_I4, key) : new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(key)),
              new CodeMatch(OpCodes.Ldc_I4_1),
              ZInputGetKeyMatch)
          .ThrowIfInvalid($"Could not patch ZInput.GetKey() for Key: {key}!")
          .Advance(offset: 2);
    }
  }
}
