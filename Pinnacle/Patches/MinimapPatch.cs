namespace Pinnacle;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(Minimap))]
static class MinimapPatch {
  [HarmonyWrapSafe]
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Minimap.Start))]
  static void StartPostfix(Minimap __instance) {
    if (IsModEnabled.Value) {
      PinMarkerUtils.SetupPinNamePrefab(__instance);

      Pinnacle.TogglePinEditPanel(pinToEdit: null);
      Pinnacle.TogglePinListPanel(toggleOn: false);
      Pinnacle.TogglePinFilterPanel(toggleOn: true);

      Pinnacle.ToggleVanillaIconPanels(toggleOn: false);

      Pinnacle.PinFilterPanel?.UpdatePinIconFilters();
    }
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Minimap.OnMapDblClick))]
  static IEnumerable<CodeInstruction> OnMapDblClickTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Minimap), nameof(Minimap.m_selectedType))),
            new CodeMatch(OpCodes.Ldc_I4_4))
        .ThrowIfInvalid($"Could not patch Minimap.OnMapDblClick()! (m_selectedType)")
        .Advance(offset: 2)
        .InsertAndAdvance(Transpilers.EmitDelegate(OnMapDblClickSelectedTypeDelegate))
        .InstructionEnumeration();
  }

  static int OnMapDblClickSelectedTypeDelegate(int pinTypeDeath) {
    if (IsModEnabled.Value) {
      return -1;
    }

    return pinTypeDeath;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Minimap.OnMapLeftClick))]
  static bool OnMapLeftClickPrefix(ref Minimap __instance) {
    if (IsModEnabled.Value
        && global::Console.m_instance.IsCheatsEnabled()
        && Player.m_localPlayer
        && ZInput.GetKey(KeyCode.LeftShift)) {
      Vector3 targetPosition = __instance.ScreenToWorldPoint(Input.mousePosition);

      __instance.SetMapMode(Minimap.MapMode.Small);
      __instance.m_smallRoot.SetActive(true);

      PinnacleUtils.TeleportTo(targetPosition);

      return false;
    }

    return true;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Minimap.OnMapLeftClick))]
  static IEnumerable<CodeInstruction> OnMapLeftClickTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Minimap), nameof(Minimap.GetClosestPin))),
            new CodeMatch(OpCodes.Stloc_1))
        .ThrowIfInvalid($"Could not patch Minimap.OnMapLeftClick()! (GetClosestPin)")
        .Advance(offset: 1)
        .InsertAndAdvance(Transpilers.EmitDelegate(GetClosestPinDelegate))
        .InstructionEnumeration();
  }

  static Minimap.PinData GetClosestPinDelegate(Minimap.PinData closestPin) {
    if (IsModEnabled.Value) {
      Pinnacle.TogglePinEditPanel(closestPin);
      return null;
    }

    return closestPin;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Minimap.Update))]
  static IEnumerable<CodeInstruction> UpdateTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Minimap), nameof(Minimap.InTextInput))))
        .ThrowIfInvalid("Could not patch Minimap.Update()! (InTextInput)")
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            Transpilers.EmitDelegate(InTextInputPreDelegate))
        .InstructionEnumeration();
  }

  static void InTextInputPreDelegate(Minimap minimap) {
    if (IsModEnabled.Value && minimap.m_mode == Minimap.MapMode.Large) {
      if (PinListPanelToggleShortcut.Value.IsDown()) {
        Pinnacle.TogglePinListPanel();
      }

      if (AddPinAtMouseShortcut.Value.IsDown()) {
        minimap.OnMapDblClick();
      }
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Minimap.InTextInput))]
  static void InTextInputPostfix(ref bool __result) {
    if (IsModEnabled.Value && !__result) {
      if (Pinnacle.PinEditPanel?.Panel
          && Pinnacle.PinEditPanel.Panel.activeSelf
          && Pinnacle.PinEditPanel.HasFocus()) {
        __result = true;
      } else if (Pinnacle.PinListPanel?.Panel && Pinnacle.PinListPanel.HasFocus()) {
        __result = true;
      }
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Minimap.UpdateMap))]
  static void UpdateMapPrefix(ref bool takeInput) {
    if (IsModEnabled.Value && Pinnacle.PinListPanel?.Panel && Pinnacle.PinListPanel.HasFocus()) {
      takeInput = false;
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Minimap.RemovePin), typeof(Minimap.PinData))]
  static void RemovePinPrefix(ref Minimap.PinData pin) {
    if (IsModEnabled.Value && Pinnacle.PinEditPanel?.TargetPin == pin) {
      Pinnacle.TogglePinEditPanel(null);
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Minimap.SetMapMode))]
  static void SetMapModePrefix(ref Minimap __instance, ref Minimap.MapMode mode, ref Minimap.MapMode __state) {
    if (IsModEnabled.Value
        && Pinnacle.PinListPanel?.Panel
        && Pinnacle.PinListPanel.Panel.activeSelf) {
      __state = __instance.m_mode;
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Minimap.SetMapMode))]
  static void SetMapModePostfix(ref Minimap.MapMode mode, ref Minimap.MapMode __state) {
    if (IsModEnabled.Value && mode != Minimap.MapMode.Large) {
      if (Pinnacle.PinEditPanel?.Panel) {
        Pinnacle.TogglePinEditPanel(null);
      }

      if (Pinnacle.PinListPanel?.Panel) {
        Pinnacle.PinListPanel.PinNameFilter.InputField.DeactivateInputField();
      }
    }

    if (IsModEnabled.Value && mode == Minimap.MapMode.Large && __state != mode) {
      Pinnacle.PinListPanel.SetTargetPins();
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Minimap.ShowPinNameInput))]
  static bool ShowPinNameInputPrefix(ref Minimap __instance, Vector3 pos) {
    if (IsModEnabled.Value) {
      __instance.m_namePin = null;

      Pinnacle.TogglePinEditPanel(__instance.AddPin(pos, __instance.m_selectedType, string.Empty, true, false, 0L));
      Pinnacle.PinEditPanel?.PinName?.Value?.InputField.Ref()?.ActivateInputField();

      return false;
    }

    return true;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Minimap.SelectIcon))]
  static void SelectIconPostfix() {
    if (IsModEnabled.Value) {
      Pinnacle.PinFilterPanel?.UpdatePinIconFilters();
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Minimap.ToggleIconFilter))]
  static void ToggleIconFilterPostfix() {
    if (IsModEnabled.Value) {
      Pinnacle.PinFilterPanel?.UpdatePinIconFilters();
    }
  }
}
