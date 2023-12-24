using System;
using System.Reflection;

using ComfyLib;

using HarmonyLib;

using UnityEngine;

using static ComfySigns.PluginConfig;

namespace ComfySigns {
  [HarmonyPatch(typeof(Sign))]
  static class SignPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Sign.Awake))]
    static void AwakePostfix(ref Sign __instance) {
      if (IsModEnabled.Value) {
        __instance.m_characterLimit = 999;

        if (__instance.m_textWidget.TryGetComponentInParent(out Canvas canvas)) {
          canvas.transform.localPosition = new(0f, 0f, 0.10f);
        }
      }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Sign.SetText))]
    static void SetTextPostfix(ref Sign __instance) {
      if (IsModEnabled.Value) {
        ComfySigns.ProcessSignText(__instance);
        ComfySigns.ProcessSignEffect(__instance);
      }
    }

    [HarmonyPostfix]
    [HarmonyPatch(nameof(Sign.UpdateText))]
    static void UpdateTextPostfix(ref Sign __instance) {
      if (IsModEnabled.Value) {
        ComfySigns.ProcessSignEffect(__instance);
      }
    }

    [HarmonyPatch]
    static class SignUpdateTextPatch {
      static FieldInfo _sign;

      [HarmonyTargetMethod]
      static MethodBase FindUpdateTextMethod() {
        Type type = AccessTools.Inner(typeof(Sign), "<>c__DisplayClass4_0");
        _sign = AccessTools.Field(type, "<>4__this");

        return AccessTools.Method(type, "<UpdateText>b__0");
      }

      [HarmonyPostfix]
      static void UpdateTextPostfix(object __instance) {
        if (IsModEnabled.Value) {
          Sign sign = (Sign) _sign.GetValue(__instance);
          ComfySigns.ProcessSignText(sign);
        }
      }
    }
  }
}
