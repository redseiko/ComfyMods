using System;
using System.Collections;
using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using UnityEngine;
using UnityEngine.UI;

using static SearsCatalog.PluginConfig;

namespace SearsCatalog {
  [HarmonyPatch(typeof(Hud))]
  static class HudPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Hud.Awake))]
    static void AwakePostfix(ref Hud __instance) {
      if (IsModEnabled.Value) {
        SetupPieceSelectionWindow(__instance);
        __instance.StartCoroutine(SetupBuildHudPanelDelayed());
      }
    }

    static readonly WaitForSeconds _waitOneSecond = new(seconds: 1f);

    static IEnumerator SetupBuildHudPanelDelayed() {
      yield return _waitOneSecond;
      SearsCatalog.SetupBuildHudPanel();
    }

    static void SetupPieceSelectionWindow(Hud hud) {
      Transform parentTransform = hud.m_pieceListRoot.parent;

      DefaultControls.Resources resources = new() {
        standard = UIResources.GetSprite("UISprite")
      };

      Scrollbar scrollbar = DefaultControls.CreateScrollbar(resources).GetComponent<Scrollbar>();
      scrollbar.transform.SetParent(parentTransform, worldPositionStays: false);

      scrollbar.GetComponent<RectTransform>()
          .SetAnchorMin(Vector2.right)
          .SetAnchorMax(Vector2.one)
          .SetPivot(Vector2.one)
          .SetPosition(Vector2.zero)
          .SetSizeDelta(new(10f, 0f));

      scrollbar.direction = Scrollbar.Direction.BottomToTop;
      scrollbar.GetComponent<Image>().SetColor(new(0f, 0f, 0f, 0.6f));
      scrollbar.handleRect.GetComponent<Image>().SetColor(new(1f, 1f, 1f, 0.9f));

      parentTransform.GetComponent<RectTransform>()
          .OffsetSizeDelta(new(10f, 0f));

      ScrollRect scrollRect = parentTransform.gameObject.AddComponent<ScrollRect>();
      scrollRect.content = hud.m_pieceListRoot;
      scrollRect.viewport = parentTransform.GetComponent<RectTransform>();
      scrollRect.verticalScrollbar = scrollbar;
      scrollRect.movementType = ScrollRect.MovementType.Clamped;
      scrollRect.inertia = false;
      scrollRect.scrollSensitivity = hud.m_pieceIconSpacing;
      scrollRect.verticalScrollbarVisibility = ScrollRect.ScrollbarVisibility.Permanent;

      GameObject panel = hud.m_pieceSelectionWindow.transform.parent.gameObject;

      RectTransform panelTransform = panel.RectTransform();
      panelTransform.SetPosition(BuildHudPanelPosition.Value);

      PanelDragger panelDragger = panel.AddComponent<PanelDragger>();
      panelDragger.TargetRectTransform = panelTransform;
      panelDragger.OnPanelEndDrag += (_, position) => BuildHudPanelPosition.Value = position;

      PanelResizer panelResizer = UIBuilder.CreateResizer(panelTransform).AddComponent<PanelResizer>();
      panelResizer.TargetRectTransform = panelTransform;
      panelResizer.OnPanelEndResize += (_, sizeDelta) => SearsCatalog.ResizeBuildHudPanel(sizeDelta);

      SearsCatalog.BuildHudPanelTransform = panelTransform;
      SearsCatalog.BuildHudScrollbar = scrollbar;
      SearsCatalog.BuildHudScrollRect = scrollRect;
      SearsCatalog.BuildHudNeedRefresh = true;
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Hud.TogglePieceSelection))]
    static void TogglePieceSelectionPostfix(ref Hud __instance) {
      if (IsModEnabled.Value && __instance.m_pieceSelectionWindow.activeSelf) {
        SearsCatalog.CenterOnSelectedIndex();
      }
    }

    [HarmonyPrefix]
    [HarmonyPatch(nameof(Hud.UpdatePieceList))]
    static void UpdatePieceListPrefix(ref Hud __instance, ref Piece.PieceCategory category, ref bool __state) {
      if (IsModEnabled.Value) {
        __state = __instance.m_lastPieceCategory == category;
      }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Hud.UpdatePieceList))]
    static void UpdatePieceListPostfix(ref Hud __instance, ref bool __state) {
      if (IsModEnabled.Value && SearsCatalog.BuildHudNeedIconLayoutRefresh) {
        SearsCatalog.BuildHudNeedIconLayoutRefresh = false;

        foreach (Hud.PieceIconData pieceIconData in __instance.m_pieceIcons) {
          pieceIconData.m_go.RectTransform()
              .SetAnchorMin(Vector2.up)
              .SetAnchorMax(Vector2.up)
              .SetPivot(Vector2.up);
        }
      }

      if (IsModEnabled.Value && (!__state || SearsCatalog.BuildHudNeedIconRecenter)) {
        SearsCatalog.BuildHudNeedIconRecenter = false;
        SearsCatalog.CenterOnSelectedIndex();
      }
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Hud.UpdatePieceList))]
    static IEnumerable<CodeInstruction> UpdatePieceListTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Callvirt, AccessTools.Method(typeof(Player), nameof(Player.GetBuildPieces))),
              new CodeMatch(OpCodes.Stloc_0),
              new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(15)),
              new CodeMatch(OpCodes.Stloc_1))
          .Advance(offset: 1)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<List<Piece>, List<Piece>>>(GetBuildPiecesDelegate))
          .Advance(offset: 2)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(GridColumnsDelegate))
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Ldc_I4_6),
              new CodeMatch(OpCodes.Stloc_2))
          .Advance(offset: 1)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(GridRowsDelegate))
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Ldloc_1),
              new CodeMatch(OpCodes.Ldloc_2),
              new CodeMatch(OpCodes.Mul))
          .Advance(offset: 3)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(PieceIconsCountMultiplyDelegate))
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Ldfld, AccessTools.Field(typeof(Hud), nameof(Hud.m_pieceIcons))),
              new CodeMatch(
                  OpCodes.Callvirt,
                  AccessTools.Method(typeof(List<Hud.PieceIconData>), nameof(List<Hud.PieceIconData>.Clear))))
          .Advance(offset: 2)
          .InsertAndAdvance(
              new CodeInstruction(OpCodes.Ldarg_0),
              Transpilers.EmitDelegate<Action<Hud>>(PieceIconsClearPostDelegate))
          .InstructionEnumeration();
    }

    static List<Piece> GetBuildPiecesDelegate(List<Piece> buildPieces) {
      if (IsModEnabled.Value) {
        SearsCatalog.BuildHudRows =
            (buildPieces.Count / SearsCatalog.BuildHudColumns)
                + (buildPieces.Count % SearsCatalog.BuildHudColumns == 0 ? 0 : 1);
      }

      return buildPieces;
    }

    static int PieceIconsCountMultiplyDelegate(int value) {
      if (IsModEnabled.Value && SearsCatalog.BuildHudNeedRefresh) {
        return -1;
      }

      return value;
    }

    static void PieceIconsClearPostDelegate(Hud hud) {
      if (IsModEnabled.Value) {
        hud.m_pieceListRoot.sizeDelta =
            new(
                hud.m_pieceIconSpacing * SearsCatalog.BuildHudColumns,
                hud.m_pieceIconSpacing * SearsCatalog.BuildHudRows);

        SearsCatalog.BuildHudNeedRefresh = false;
        SearsCatalog.BuildHudNeedIconLayoutRefresh = true;
        SearsCatalog.BuildHudNeedIconRecenter = true;
      }
    }

    [HarmonyTranspiler]
    [HarmonyPatch(nameof(Hud.GetSelectedGrid))]
    static IEnumerable<CodeInstruction> GetSelectedGridTranspiler(IEnumerable<CodeInstruction> instructions) {
      return new CodeMatcher(instructions)
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Ldc_I4_S, Convert.ToSByte(15)),
              new CodeMatch(OpCodes.Stloc_0))
          .Advance(offset: 1)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(GridColumnsDelegate))
          .MatchForward(
              useEnd: false,
              new CodeMatch(OpCodes.Ldc_I4_6),
              new CodeMatch(OpCodes.Stloc_1))
          .Advance(offset: 1)
          .InsertAndAdvance(Transpilers.EmitDelegate<Func<int, int>>(GridRowsDelegate))
          .InstructionEnumeration();
    }

    static int GridColumnsDelegate(int columns) {
      return IsModEnabled.Value ? SearsCatalog.BuildHudColumns : columns;
    }

    static int GridRowsDelegate(int rows) {
      return IsModEnabled.Value ? SearsCatalog.BuildHudRows : rows;
    }
  }
}
