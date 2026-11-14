namespace BetterZeeLog;

using System.Reflection;

using HarmonyLib;

using Mono.Cecil.Cil;

using MonoMod.Cil;
using MonoMod.RuntimeDetour;

using UnityEngine;
using UnityEngine.Rendering;

static class TerminalDetour {
  public static void Hook() {
    if (SystemInfo.graphicsDeviceType != GraphicsDeviceType.Null) {
      return;
    }

    MethodInfo targetMethod = AccessTools.Method(typeof(Terminal), nameof(Terminal.AddString), [typeof(string)]);

    if (targetMethod != null) {
      new ILHook(targetMethod, AddStringManipulator);
    }
  }

  static void AddStringManipulator(ILContext ilContext) {
    ILCursor cursor = new ILCursor(ilContext);

    cursor.GotoNext(
        MoveType.AfterLabel,
        instruction => instruction.MatchLdstr("Console: "),
        instruction => instruction.MatchLdarg(1),
        instruction => instruction.MatchCall(
            AccessTools.Method(typeof(string), nameof(string.Concat), [typeof(string), typeof(string)])),
        instruction => instruction.MatchCall(AccessTools.Method(typeof(ZLog), nameof(ZLog.Log))));

    // Target [4] (br.s IL_0058), skipping [0..3] (`ZLog.Log("Console: " + text)`).
    ILLabel loopConditionLabel = ilContext.DefineLabel(cursor.Instrs[cursor.Index + 4]);
    cursor.Emit(OpCodes.Br, loopConditionLabel);
  }
}
