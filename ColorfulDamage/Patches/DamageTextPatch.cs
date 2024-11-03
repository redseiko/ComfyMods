namespace ColorfulDamage;

using System.Collections.Generic;
using System.Reflection.Emit;

using ComfyLib;

using HarmonyLib;

using TMPro;

using UnityEngine;

using static PluginConfig;

[HarmonyPatch(typeof(DamageText))]
static class DamageTextPatch {
  static string _localizedMsgTooHard = string.Empty;
  static string _localizedMsgBlocked = string.Empty;

  [HarmonyPostfix]
  [HarmonyPatch(nameof(DamageText.Awake))]
  static void AwakePostfix() {
    _localizedMsgTooHard = Localization.m_instance.Localize("$msg_toohard");
    _localizedMsgBlocked = Localization.m_instance.Localize("$msg_blocked: ");
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(DamageText.AddInworldText))]
  static bool AddInworldText(
      ref DamageText __instance, DamageText.TextType type, Vector3 pos, float distance, string text, bool mySelf) {
    if (!IsModEnabled.Value) {
      return true;
    }

    if (text == "0" && __instance.m_worldTexts.Count > 200) {
      return false;
    }

    DamageText.WorldTextInstance worldText = new();
    __instance.m_worldTexts.Add(worldText);

    worldText.m_worldPos = pos + (UnityEngine.Random.insideUnitSphere * 0.5f);
    worldText.m_gui = UnityEngine.Object.Instantiate(__instance.m_worldTextBase, __instance.transform);

    worldText.m_textField = worldText.m_gui.GetComponent<TMP_Text>();
    worldText.m_textField.text = GetWorldTextText(type, text);
    worldText.m_textField.color = GetWorldTextColor(type, text, mySelf);

    worldText.m_textField.font = FontCache.GetFontAssetByName(DamageTextMessageFont.Value);
    worldText.m_textField.fontSize =
        distance > DamageTextSmallPopupDistance.Value
            ? DamageTextSmallFontSize.Value
            : DamageTextLargeFontSize.Value;

    worldText.m_timer = 0f;

    return false;
  }

  static Color GetWorldTextColor(DamageText.TextType damageTextType, string text, bool isPlayerDamage) {
    if (isPlayerDamage && damageTextType <= DamageText.TextType.Immune) {
      return text == "0f" ? DamageTextPlayerNoDamageColor.Value : DamageTextPlayerDamageColor.Value;
    }

    return damageTextType switch {
      DamageText.TextType.Normal => DamageTextNormalColor.Value,
      DamageText.TextType.Resistant => DamageTextResistantColor.Value,
      DamageText.TextType.Weak => DamageTextWeakColor.Value,
      DamageText.TextType.Immune => DamageTextImmuneColor.Value,
      DamageText.TextType.Heal => DamageTextHealColor.Value,
      DamageText.TextType.TooHard => DamageTextTooHardColor.Value,
      DamageText.TextType.Blocked => DamageTextBlockedColor.Value,
      DamageText.TextType.Bonus => DamageTextBonusColor.Value,
      _ => Color.white,
    };
  }

  static string GetWorldTextText(DamageText.TextType damageTextType, string text) {
    return damageTextType switch {
      DamageText.TextType.Heal => "+" + text,
      DamageText.TextType.TooHard => _localizedMsgTooHard,
      DamageText.TextType.Blocked => _localizedMsgBlocked + text,
      _ => Localization.m_instance.Localize(text),
    };
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(DamageText.UpdateWorldTexts))]
  static bool UpdateWorldTextsPrefix(ref DamageText __instance, float dt) {
    if (!IsModEnabled.Value) {
      return true;
    }

    Camera camera = Utils.GetMainCamera();

    float width = Screen.width;
    float height = Screen.height;
    float widthScaling = (width / camera.pixelWidth);
    float heightScaling = (height / camera.pixelHeight);
    
    for (int i = __instance.m_worldTexts.Count - 1; i >= 0; i--) {
      DamageText.WorldTextInstance worldText = __instance.m_worldTexts[i];
      worldText.m_timer += dt;

      if (worldText.m_timer > DamageTextPopupDuration.Value) {
        UnityEngine.Object.Destroy(worldText.m_gui);

        // Source: https://www.vertexfragment.com/ramblings/list-removal-performance/
        __instance.m_worldTexts[i] = __instance.m_worldTexts[__instance.m_worldTexts.Count - 1];
        __instance.m_worldTexts.RemoveAt(__instance.m_worldTexts.Count - 1);

        continue;
      }

      float t = worldText.m_timer / DamageTextPopupDuration.Value;

      Vector3 position =
          Vector3.Lerp(worldText.m_worldPos, worldText.m_worldPos + DamageTextPopupLerpPosition.Value, t);

      Vector3 point = camera.WorldToScreenPoint(position);
      point.x *= widthScaling;
      point.y *= heightScaling;

      if (point.x < 0f || point.x > width || point.y < 0f || point.y > height || point.z < 0f) {
        worldText.m_gui.SetActive(false);
        continue;
      }
      
      Color color = worldText.m_textField.color;
      color.a = 1f - (t * t * t);
      worldText.m_textField.color = color;

      worldText.m_gui.SetActive(true);
      worldText.m_gui.transform.position = point;
    }

    return false;
  }

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(DamageText.RPC_DamageText))]
  static IEnumerable<CodeInstruction> RpcDamageTextTranspiler(IEnumerable<CodeInstruction> instructions) {
    return new CodeMatcher(instructions)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_0),
            new CodeMatch(
                OpCodes.Ldfld, AccessTools.Field(typeof(DamageText), nameof(DamageText.m_maxTextDistance))))
        .ThrowIfInvalid($"Could not patch DamageText.RPC_DamageText()! (max-text-distance")
        .Advance(offset: 2)
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call,
                AccessTools.Method(typeof(DamageTextPatch), nameof(MaxTextDistanceDelegate))))
        .InstructionEnumeration();
  }

  static float MaxTextDistanceDelegate(float maxTextDistance) {
    return IsModEnabled.Value ? DamageTextMaxPopupDistance.Value : maxTextDistance;
  }
}
