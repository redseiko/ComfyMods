using BepInEx;

using HarmonyLib;

using System.Collections.Generic;
using System;
using System.Reflection;
using System.Reflection.Emit;

using UnityEngine;

using static Shortcuts.PluginConfig;

namespace Shortcuts {
  [BepInPlugin(PluginGUID, PluginName, PluginVersion)]
  public class Shortcuts : BaseUnityPlugin {
    public const string PluginGUID = "redseiko.valheim.shortcuts";
    public const string PluginName = "Shortcuts";
    public const string PluginVersion = "1.0.0";

    Harmony _harmony;

    public void Awake() {
      CreateConfig(Config);

      if (_isModEnabled.Value) {
        _harmony = Harmony.CreateAndPatchAll(Assembly.GetExecutingAssembly(), harmonyInstanceId: PluginGUID);
      }
    }

    public void OnDestroy() {
      _harmony?.UnpatchSelf();
    }

    static readonly CodeMatch _inputGetKeyDownMatch =
        new(
            OpCodes.Call,
            AccessTools.Method(typeof(Input), nameof(Input.GetKeyDown), new Type[] { typeof(KeyCode) }));

    static readonly CodeMatch _inputGetKeyMatch =
        new(OpCodes.Call, AccessTools.Method(typeof(Input), nameof(Input.GetKey), new Type[] { typeof(KeyCode) }));

    [HarmonyPatch(typeof(Hud))]
    class HudPatch {
      [HarmonyTranspiler]
      [HarmonyPatch(nameof(Hud.Update))]
      static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
        return new CodeMatcher(instructions)
            .MatchForward(useEnd: false, new CodeMatch(OpCodes.Ldc_I4, 0x11C), _inputGetKeyDownMatch)
            .Advance(offset: 1)
            .SetAndAdvance(
                OpCodes.Call,
                Transpilers.EmitDelegate<Func<KeyCode, bool>>(keyCode => _toggleHudShortcut.Value.IsDown()).operand)
            .MatchForward(useEnd: false, new CodeMatch(OpCodes.Ldc_I4, 0x132), _inputGetKeyMatch)
            .Advance(offset: 1)
            .SetAndAdvance(OpCodes.Call, Transpilers.EmitDelegate<Func<KeyCode, bool>>(keyCode => true).operand)
            .InstructionEnumeration();
      }
    }

    [HarmonyPatch(typeof(Player))]
    class PlayerPatch {
      [HarmonyTranspiler]
      [HarmonyPatch(nameof(Player.Update))]
      static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
        return new CodeMatcher(instructions)
            .MatchForward(useEnd: false, new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(122)), _inputGetKeyDownMatch)
            .Advance(offset: 1)
            .SetAndAdvance(
                OpCodes.Call,
                Transpilers.EmitDelegate<Func<KeyCode, bool>>(
                    keyCode => _toggleDebugFlyShortcut.Value.IsDown()).operand)
            .MatchForward(useEnd: false, new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(98)), _inputGetKeyDownMatch)
            .Advance(offset: 1)
            .SetAndAdvance(
                OpCodes.Call,
                Transpilers.EmitDelegate<Func<KeyCode, bool>>(
                    keyCode => _toggleDebugNoCostShortcut.Value.IsDown()).operand)
            .MatchForward(useEnd: false, new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(107)), _inputGetKeyDownMatch)
            .Advance(offset: 1)
            .SetAndAdvance(
                OpCodes.Call,
                Transpilers.EmitDelegate<Func<KeyCode, bool>>(
                    keyCode => _debugKillAllShortcut.Value.IsDown()).operand)
            .MatchForward(useEnd: false, new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(49)), _inputGetKeyDownMatch)
            .Advance(offset: 1)
            .SetAndAdvance(
                OpCodes.Call,
                Transpilers.EmitDelegate<Func<KeyCode, bool>>(keyCode => _hotbarItem1Shortcut.Value.IsDown()).operand)
            .MatchForward(useEnd: false, new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(50)), _inputGetKeyDownMatch)
            .Advance(offset: 1)
            .SetAndAdvance(
                OpCodes.Call,
                Transpilers.EmitDelegate<Func<KeyCode, bool>>(keyCode => _hotbarItem2Shortcut.Value.IsDown()).operand)
            .MatchForward(useEnd: false, new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(51)), _inputGetKeyDownMatch)
            .Advance(offset: 1)
            .SetAndAdvance(
                OpCodes.Call,
                Transpilers.EmitDelegate<Func<KeyCode, bool>>(keyCode => _hotbarItem3Shortcut.Value.IsDown()).operand)
            .MatchForward(useEnd: false, new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(52)), _inputGetKeyDownMatch)
            .Advance(offset: 1)
            .SetAndAdvance(
                OpCodes.Call,
                Transpilers.EmitDelegate<Func<KeyCode, bool>>(keyCode => _hotbarItem4Shortcut.Value.IsDown()).operand)
            .MatchForward(useEnd: false, new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(53)), _inputGetKeyDownMatch)
            .Advance(offset: 1)
            .SetAndAdvance(
                OpCodes.Call,
                Transpilers.EmitDelegate<Func<KeyCode, bool>>(keyCode => _hotbarItem5Shortcut.Value.IsDown()).operand)
            .MatchForward(useEnd: false, new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(54)), _inputGetKeyDownMatch)
            .Advance(offset: 1)
            .SetAndAdvance(
                OpCodes.Call,
                Transpilers.EmitDelegate<Func<KeyCode, bool>>(keyCode => _hotbarItem6Shortcut.Value.IsDown()).operand)
            .MatchForward(useEnd: false, new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(55)), _inputGetKeyDownMatch)
            .Advance(offset: 1)
            .SetAndAdvance(
                OpCodes.Call,
                Transpilers.EmitDelegate<Func<KeyCode, bool>>(keyCode => _hotbarItem7Shortcut.Value.IsDown()).operand)
            .MatchForward(useEnd: false, new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(56)), _inputGetKeyDownMatch)
            .Advance(offset: 1)
            .SetAndAdvance(
                OpCodes.Call,
                Transpilers.EmitDelegate<Func<KeyCode, bool>>(keyCode => _hotbarItem8Shortcut.Value.IsDown()).operand)
            .InstructionEnumeration();
      }
    }

    [HarmonyPatch(typeof(Console))]
    class ConsolePatch {
      [HarmonyTranspiler]
      [HarmonyPatch(nameof(Console.Update))]
      static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
        return new CodeMatcher(instructions)
            .MatchForward(useEnd: false, new CodeMatch(OpCodes.Ldc_I4, 0x11E), _inputGetKeyDownMatch)
            .Advance(offset: 1)
            .SetAndAdvance(
                OpCodes.Call,
                Transpilers.EmitDelegate<Func<KeyCode, bool>>(keyCode => _toggleConsoleShortcut.Value.IsDown()).operand)
            .InstructionEnumeration();
      }
    }

    [HarmonyPatch(typeof(GameCamera))]
    class GameCameraPatch {
      [HarmonyTranspiler]
      [HarmonyPatch(nameof(GameCamera.LateUpdate))]
      static IEnumerable<CodeInstruction> LateUpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
        return new CodeMatcher(instructions)
            .MatchForward(useEnd: false, new CodeMatch(OpCodes.Ldc_I4, 0x124), _inputGetKeyDownMatch)
            .Advance(offset: 1)
            .SetAndAdvance(
                OpCodes.Call,
                Transpilers.EmitDelegate<Func<KeyCode, bool>>(
                    keyCode => _takeScreenshotShortcut.Value.IsDown()).operand)
            .InstructionEnumeration();
      }

      [HarmonyTranspiler]
      [HarmonyPatch(nameof(GameCamera.UpdateMouseCapture))]
      static IEnumerable<CodeInstruction> UpdateMouseCaptureTranspiler(IEnumerable<CodeInstruction> instructions) {
        return new CodeMatcher(instructions)
            .MatchForward(useEnd: false, new CodeMatch(OpCodes.Ldc_I4, 0x132), _inputGetKeyMatch)
            .Advance(offset: 1)
            .SetAndAdvance(
                OpCodes.Call,
                Transpilers.EmitDelegate<Func<KeyCode, bool>>(
                    keyCode => _toggleMouseCaptureShortcut.Value.IsDown()).operand)
            .MatchForward(useEnd: false, new CodeMatch(OpCodes.Ldc_I4, 0x11A), _inputGetKeyDownMatch)
            .Advance(offset: 1)
            .SetAndAdvance(OpCodes.Call, Transpilers.EmitDelegate<Func<KeyCode, bool>>(keyCode => true).operand)
            .InstructionEnumeration();
      }
    }
  }
}