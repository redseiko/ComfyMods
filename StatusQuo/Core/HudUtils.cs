namespace StatusQuo;

using System.Collections.Generic;

using ComfyLib;

using TMPro;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

public static class HudUtils {
  public static void ToggleHudSetup(bool toggleOn) {
    if (!Hud.m_instance) {
      return;
    }

    SetupStatusEffectListRoot(Hud.m_instance, toggleOn);

    foreach (RectTransform rectTransform in Hud.m_instance.m_statusEffects) {
      Object.Destroy(rectTransform.gameObject);
    }

    Hud.m_instance.m_statusEffects.Clear();
  }

  public static void SetupStatusEffectListRoot(Hud hud, bool toggleOn) {
    hud.m_statusEffectListRoot
        .SetAnchorMin(Vector2.one)
        .SetAnchorMax(Vector2.one)
        .SetPivot(Vector2.one);

    if (toggleOn) {
      hud.m_statusEffectListRoot
          .SetSizeDelta(Vector2.zero)
          .SetPosition(StatusEffectListRectPosition.Value);
    } else {
      hud.m_statusEffectListRoot
          .SetSizeDelta(new(431.47f, 55f))
          .SetPosition(new(-271f, -45f));
    }
  }

  public static void SetupStatusEffects(Hud hud) {
    Vector2 sizeDelta = StatusEffectRectSizeDelta.Value;
    float height = sizeDelta.y;

    int maxRows = StatusEffectMaxRows.Value > 0 ? StatusEffectMaxRows.Value : int.MaxValue;
    int effectRow = 0;
    int effectColumn = 0;

    List<RectTransform> effectRectTransforms = hud.m_statusEffects;

    for (int index = 0, count = effectRectTransforms.Count; index < count; index++) {
      RectTransform effectRectTransform = effectRectTransforms[index];

      SetupEffect(effectRectTransform, sizeDelta, effectRow, effectColumn);
      SetupEffectName(effectRectTransform.Find("Name"), height);
      SetupEffectCooldown(effectRectTransform.Find("Cooldown"), height);
      SetupEffectIcon(effectRectTransform.Find("Icon"), height);
      SetupEffectTime(effectRectTransform.Find("TimeText"));

      effectRow++;

      if (effectRow >= maxRows) {
        effectRow = 0;
        effectColumn++;
      }
    }
  }

  static void SetupEffect(RectTransform effectRectTransform, Vector2 sizeDelta, int row, int column) {
    effectRectTransform
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.up)
        .SetPivot(Vector2.one)
        .SetSizeDelta(sizeDelta)
        .SetPosition(new(column * (-sizeDelta.x - 5f), row * (-sizeDelta.y - 2.5f)));

    if (!effectRectTransform.gameObject.TryGetComponent(out Image image)) {
      image = effectRectTransform.gameObject.AddComponent<Image>();
    }

    image
        .SetSprite(UIResources.GetSprite("UISprite"))
        .SetType(Image.Type.Sliced)
        .SetColor(StatusEffectBackgroundColor.Value);
  }

  static void SetupEffectName(Transform nameTransform, float height) {
    nameTransform.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0f, 0.5f))
        .SetSizeDelta(new(-height - 15f - 50f, 0f))
        .SetPosition(new(height + 15f, 0f));

    nameTransform.GetComponent<TextMeshProUGUI>()
        .SetAlignment(TextAlignmentOptions.Left)
        .SetColor(StatusEffectNameFontColor.Value)
        .SetEnableAutoSizing(StatusEffectNameFontAutoSizing.Value)
        .SetOverflowMode(StatusEffectNameTextOverflowMode.Value)
        .SetTextWrappingMode(StatusEffectNameTextWrappingMode.Value)
        .SetFontSizeMin(8f)
        .SetFontSizeMax(StatusEffectNameFontSize.Value)
        .SetFontSize(StatusEffectNameFontSize.Value);
  }

  static void SetupEffectCooldown(Transform cooldownTransform, float height) {
    cooldownTransform.GetComponent<RectTransform>()
        .SetAnchorMin(new(0f, 0.5f))
        .SetAnchorMax(new(0f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetSizeDelta(new(height, height))
        .SetPosition(new((height / 2f) + 5f, 0f));

    cooldownTransform.GetComponent<Image>()
        .SetPreserveAspect(true);
  }

  static void SetupEffectIcon(Transform iconTransform, float height) {
    iconTransform.GetComponent<RectTransform>()
        .SetAnchorMin(new(0f, 0.5f))
        .SetAnchorMax(new(0f, 0.5f))
        .SetPivot(new(0.5f, 0.5f))
        .SetSizeDelta(new(height, height))
        .SetPosition(new((height / 2f) + 5f, 0f));

    iconTransform.GetComponent<Image>()
        .SetPreserveAspect(true);
  }

  static void SetupEffectTime(Transform timeTransform) {
    timeTransform.GetComponent<RectTransform>()
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.one)
        .SetPivot(new(0f, 0.5f))
        .SetSizeDelta(new(-10f, 0f))
        .SetPosition(Vector2.zero);

    timeTransform.GetComponent<TextMeshProUGUI>()
        .SetAlignment(TextAlignmentOptions.Right)
        .SetColor(StatusEffectTimeFontColor.Value)
        .SetEnableAutoSizing(StatusEffectTimeFontAutoSizing.Value)
        .SetOverflowMode(StatusEffectTimeTextOverflowMode.Value)
        .SetTextWrappingMode(StatusEffectTimeTextWrappingMode.Value)
        .SetFontSizeMin(8f)
        .SetFontSizeMax(StatusEffectTimeFontSize.Value)
        .SetFontSize(StatusEffectTimeFontSize.Value);
  }
}
