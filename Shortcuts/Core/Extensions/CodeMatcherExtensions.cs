namespace Shortcuts;

using System;
using System.Reflection.Emit;

using HarmonyLib;

using UnityEngine;

public static class CodeMatcherExtensions {
  public static readonly CodeMatch ZInputGetButtonDown =
      new(OpCodes.Call, AccessTools.Method(typeof(ZInput), nameof(ZInput.GetButtonDown), new[] { typeof(string) }));

  public static readonly CodeMatch ZInputGetKeyDownMatch =
      new(
          OpCodes.Call,
          AccessTools.Method(typeof(ZInput), nameof(ZInput.GetKeyDown), new[] { typeof(KeyCode), typeof(bool) }));

  public static readonly CodeMatch ZInputGetKeyMatch =
      new(
          OpCodes.Call,
          AccessTools.Method(typeof(ZInput), nameof(ZInput.GetKey), new[] { typeof(KeyCode), typeof(bool) }));

  public static CodeMatcher MatchGetButtonDown(this CodeMatcher matcher, string keyName) {
    return matcher
        .MatchForward(
            useEnd: false,
            new CodeMatch(OpCodes.Ldstr, keyName),
            ZInputGetButtonDown)
        .ThrowIfInvalid($"Could not patch ZInput.GetButtonDown() for Key: {keyName}")
        .Advance(offset: 1);
  }

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
