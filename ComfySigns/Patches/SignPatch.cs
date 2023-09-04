using System;
using System.Reflection;

using ComfyLib;

using HarmonyLib;

using TMPro;

using static ComfySigns.PluginConfig;

namespace ComfySigns {
  [HarmonyPatch(typeof(Sign))]
  static class SignPatch {
    [HarmonyPostfix]
    [HarmonyPatch(nameof(Sign.Awake))]
    static void AwakePostfix(ref Sign __instance) {
      if (IsModEnabled.Value) {
        //__instance.m_textWidget
        //    .SetFont(UIFonts.GetFontAsset(SignDefaultTextFont.Value))
        //    .SetColor(SignDefaultTextColor.Value);
        TMP_FontAsset valheimNorseFont =  UIFonts.GetFontAsset(UIFonts.ValheimNorse);
        ZLog.Log($"ValheimNorseFont-material: {valheimNorseFont.material.name}");

        ZLog.Log($"TextWidth.fontSharedMaterial: {__instance.m_textWidget.fontSharedMaterial}");
        __instance.m_textWidget.fontSharedMaterial = valheimNorseFont.material;

        __instance.m_textWidget.color = SignDefaultTextColor.Value;

        __instance.m_characterLimit = 999;
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
