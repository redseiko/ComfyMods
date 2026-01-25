namespace Pinnacle;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

[HarmonyPatch(typeof(Minimap))]
static class MinimapPatch {
  [HarmonyWrapSafe]
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Minimap.Start))]
  static void StartPostfix(Minimap __instance) {
    if (IsModEnabled.Value) {
      PinMarkerUtils.SetupPinNamePrefab(__instance);
      PinEditPanelController.TogglePanel(pinToEdit: null);
      PinListPanelController.TogglePanel(toggleOn: false);
      PinFilterPanelController.TogglePanel(toggleOn: true);

      PinnacleUtils.ToggleVanillaIconPanels(toggleOn: false);
      PinFilterPanelController.UpdateIconFilters();
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
        .ThrowIfInvalid($"Could not patch Minimap.OnMapDblClick()! (m-selected-type)")
        .Advance(offset: 2)
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(MinimapPatch), nameof(OnMapDblClickSelectedTypeDelegate))))
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
  static bool OnMapLeftClickPrefix(Minimap __instance) {
    if (IsModEnabled.Value
        && Console.m_instance.IsCheatsEnabled()
        && Player.m_localPlayer
        && ZInput.GetKey(KeyCode.LeftShift)) {
      ProcessMapLeftClickTeleport(__instance);
      return false;
    }

    return true;
  }

  static void ProcessMapLeftClickTeleport(Minimap minimap) {
    Vector3 targetPosition = minimap.ScreenToWorldPoint(ZInput.mousePosition);

    Minimap.PinData closestPin =
        minimap.GetClosestPin(targetPosition, minimap.m_removeRadius * minimap.m_largeZoom * 2f, mustBeVisible: true);

    minimap.SetMapMode(Minimap.MapMode.Small);
    minimap.m_smallRoot.SetActive(true);

    if (closestPin == default) {
      PinnacleUtils.TeleportTo(targetPosition);
    } else {
      PinnacleUtils.TeleportTo(closestPin);
    }
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Minimap.OnMapLeftClick))]
  static IEnumerable<CodeInstruction> OnMapLeftClickTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Call, AccessTools.Method(typeof(Minimap), nameof(Minimap.GetClosestPin))),
            new CodeMatch(OpCodes.Stloc_1))
        .ThrowIfInvalid($"Could not patch Minimap.OnMapLeftClick()! (get-closest-pin)")
        .Advance(offset: 1)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MinimapPatch), nameof(GetClosestPinDelegate))))
        .InstructionEnumeration();
  }

  static Minimap.PinData GetClosestPinDelegate(Minimap.PinData closestPin) {
    if (IsModEnabled.Value) {
      PinEditPanelController.TogglePanel(closestPin);
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
        .ThrowIfInvalid("Could not patch Minimap.Update()! (in-text-input)")
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldarg_0),
            new CodeInstruction(OpCodes.Call, AccessTools.Method(typeof(MinimapPatch), nameof(InTextInputPreDelegate))))
        .InstructionEnumeration();
  }

  static void InTextInputPreDelegate(Minimap minimap) {
    if (IsModEnabled.Value && minimap.m_mode == Minimap.MapMode.Large) {
      if (PinListPanelToggleShortcut.Value.IsDown()) {
        PinListPanelController.TogglePanel();
      }

      if (AddPinAtMouseShortcut.Value.IsDown()) {
        minimap.OnMapDblClick();
      }

      if (QuickMapPinShortcut.Value.IsDown() && Player.m_localPlayer) {
        PinnacleUtils.AddQuickMapPin(minimap, Player.m_localPlayer.transform.position);
      }
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Minimap.InTextInput))]
  static void InTextInputPostfix(ref bool __result) {
    if (IsModEnabled.Value && !__result) {
      if (PinEditPanelController.HasFocus() || PinListPanelController.HasFocus()) {
        __result = true;
      }
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Minimap.UpdateMap))]
  static void UpdateMapPrefix(ref bool takeInput) {
    if (IsModEnabled.Value && PinListPanelController.HasFocus()) {
      takeInput = false;
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Minimap.RemovePin), typeof(Minimap.PinData))]
  static void RemovePinPrefix(ref Minimap.PinData pin) {
    if (IsModEnabled.Value && PinEditPanelController.PinEditPanel?.TargetPin == pin) {
      PinEditPanelController.TogglePanel(null);
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Minimap.SetMapMode))]
  static void SetMapModePrefix(ref Minimap __instance, ref Minimap.MapMode mode, ref Minimap.MapMode __state) {
    if (IsModEnabled.Value
        && PinListPanelController.PinListPanel?.Panel
        && PinListPanelController.PinListPanel.Panel.activeSelf) {
      __state = __instance.m_mode;
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Minimap.SetMapMode))]
  static void SetMapModePostfix(ref Minimap.MapMode mode, ref Minimap.MapMode __state) {
    if (IsModEnabled.Value && mode != Minimap.MapMode.Large) {
      if (PinEditPanelController.PinEditPanel?.Panel) {
        PinEditPanelController.TogglePanel(null);
      }

      if (PinListPanelController.PinListPanel?.Panel) {
        PinListPanelController.PinListPanel.PinNameFilter.InputField.DeactivateInputField();
      }
    }

    if (IsModEnabled.Value && mode == Minimap.MapMode.Large && __state != mode) {
      PinListPanelController.PinListPanel.SetTargetPins();
    }
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Minimap.ShowPinNameInput))]
  static bool ShowPinNameInputPrefix(Minimap __instance, Vector3 pos) {
    if (IsModEnabled.Value) {
      __instance.m_namePin = null;

      PinEditPanelController.TogglePanel(PinnacleUtils.AddNewPin(__instance, pos));
      PinEditPanelController.PinEditPanel?.ActivatePinNameInputField();

      return false;
    }

    return true;
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Minimap.SelectIcon))]
  static void SelectIconPostfix() {
    if (IsModEnabled.Value) {
      PinFilterPanelController.UpdateIconFilters();
    }
  }

  [HarmonyPostfix]
  [HarmonyPatch(nameof(Minimap.ToggleIconFilter))]
  static void ToggleIconFilterPostfix() {
    if (IsModEnabled.Value) {
      PinFilterPanelController.UpdateIconFilters();
    }
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(Minimap.UpdatePins))]
  static IEnumerable<CodeInstruction> UpdatePinsTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Minimap.PinData), nameof(Minimap.PinData.m_icon))),
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertySetter(typeof(Image), nameof(Image.sprite))))
        .ThrowIfInvalid("Could not patch Minimap.UpdatePins()! (pin-icon-element-set-sprite)")
        .SaveOperand(out object pinDataLocal)
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(
                OpCodes.Callvirt, AccessTools.PropertyGetter(typeof(GameObject), nameof(GameObject.transform))),
            new CodeMatch(OpCodes.Ldstr, "Checked"))
        .ThrowIfInvalid($"Could not patch Minimap.UpdatePins()! (pin-icon-find-checked)")
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_S, pinDataLocal),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(MinimapPatch), nameof(SetIconElementSpriteDelegate))))
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(
                OpCodes.Ldfld, AccessTools.Field(typeof(Minimap.PinData), nameof(Minimap.PinData.m_iconElement))),
            new CodeMatch(OpCodes.Ldloc_S),
            new CodeMatch(OpCodes.Callvirt, AccessTools.PropertySetter(typeof(Graphic), nameof(Graphic.color))))
        .ThrowIfInvalid($"Could not ptach Minimap.UpdatePins()! (set-icon-element-color)")
        .CreateLabelOffset(offset: 4, out Label skipSetColorLabel)
        .InsertAndAdvance(
            new CodeInstruction(OpCodes.Ldloc_S, pinDataLocal),
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(PinIconManager), nameof(PinIconManager.HasIconTagFlag))),
            new CodeInstruction(OpCodes.Brtrue, skipSetColorLabel))
        .InstructionEnumeration();
  }

  static void SetIconElementSpriteDelegate(Minimap.PinData pinData) {
    if (IsModEnabled.Value && PinIconManager.IsValidIconTagName(pinData.m_name)) {
      PinIconManager.ProcessIconTagsCreated(pinData);
    }
  }
}

[HarmonyPatch(typeof(Minimap.PinNameData))]
static class PinNameDataPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(Minimap.PinNameData.SetTextAndGameObject))]
  static void SetTextAndGameObjectPostfix(Minimap.PinNameData __instance) {
    if (IsModEnabled.Value) {
      PinIconManager.RemoveIconTagText(__instance);
    }
  }
}
