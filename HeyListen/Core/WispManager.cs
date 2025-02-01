namespace HeyListen;

using ComfyLib;

using UnityEngine;
using UnityEngine.Animations;

using static PluginConfig;

public static class WispManager {
  public static readonly int ColorShaderId = Shader.PropertyToID("_Color");
  public static readonly int EmissionColorShaderId = Shader.PropertyToID("_EmissionColor");

  public static readonly int DemisterBallBodyScaleHashCode = "DemisterBallBodyScale".GetStableHashCode();
  public static readonly int DemisterBallBodyColorHashCode = "DemisterBallBodyColor".GetStableHashCode();
  public static readonly int DemisterBallBodyBrightnessHashCode = "DemisterBallBodyBrightness".GetStableHashCode();
  public static readonly int DemisterBallPointLightColorHashCode = "DemisterBallPointLightColor".GetStableHashCode();

  public static readonly int FlameEffectsEnabledHashCode = "FlameEffectsEnabled".GetStableHashCode();
  public static readonly int FlameEffectsColorHashCode = "FlameEffectsColor".GetStableHashCode();

  public static readonly int FlameEffectsEmbersColorHashCode = "FlameEffectsEmbersColor".GetStableHashCode();
  public static readonly int FlameEffectsEmbersBrightnessHashCode =
      "FlameEffectsEmbersBrightness".GetStableHashCode();
  public static readonly int FlameEffectsSparcsColorHashCode = "FlameEffectsSparcsColor".GetStableHashCode();
  public static readonly int FlameEffectsSparcsBrightnessHashCode =
      "FlameEffectsSparcsBrightness".GetStableHashCode();

  public static SE_Demister LocalPlayerDemister { get; set; }

  public static DemisterBallControl LocalPlayerDemisterBall { get; set; }
  public static ZNetView LocalPlayerDemisterBallNetView { get; set; }

  public static void SetLocalPlayerDemisterBallControl(DemisterBallControl demisterBallControl) {
    LocalPlayerDemisterBall = demisterBallControl;
    LocalPlayerDemisterBallNetView = demisterBallControl.NetView;

    UpdateDemisterBallControlZdo(LocalPlayerDemisterBallNetView.m_zdo);
    LocalPlayerDemisterBall.UpdateDemisterBall(forceUpdate: true);
  }

  public static void UpdatePlayerDemisterBall() {
    if (LocalPlayerDemisterBall && LocalPlayerDemisterBallNetView) {
      UpdateDemisterBallControlZdo(LocalPlayerDemisterBallNetView.m_zdo);
      LocalPlayerDemisterBall.UpdateDemisterBall(forceUpdate: true);
    }
  }

  public static void UpdatePlayerDemisterBallFlameEffects() {
    if (LocalPlayerDemisterBall && LocalPlayerDemisterBallNetView) {
      UpdateDemisterBallControlZdo(LocalPlayerDemisterBallNetView.m_zdo);
      LocalPlayerDemisterBall.UpdateFlameEffects();
    }
  }

  public static void UpdateUseCustomSettings() {
    bool useCustomSettings = IsModEnabled.Value && DemisterBallUseCustomSettings.Value;

    foreach (Demister demister in Demister.m_instances) {
      GameObject prefab = demister.transform.root.gameObject;

      if (!prefab.name.StartsWith("demister_ball", System.StringComparison.Ordinal)) {
        continue;
      }

      if (prefab.TryGetComponent(out DemisterBallControl demisterBallControl)) {
        if (!useCustomSettings) {
          UnityEngine.Object.Destroy(demisterBallControl);
        }
      } else if (useCustomSettings) {
        prefab.AddComponent<DemisterBallControl>();
      }
    }

    if (useCustomSettings) {
      LocalPlayerDemisterBall = default;
      LocalPlayerDemisterBallNetView = default;
    }
  }

  static void UpdateDemisterBallControlZdo(ZDO zdo) {
    zdo.Set(DemisterBallBodyScaleHashCode, DemisterBallBodyScale.Value);
    zdo.Set(DemisterBallBodyColorHashCode, DemisterBallBodyColor.Value);
    zdo.Set(DemisterBallBodyBrightnessHashCode, DemisterBallBodyBrightness.Value);
    zdo.Set(DemisterBallPointLightColorHashCode, DemisterBallPointLightColor.Value);

    zdo.Set(FlameEffectsEnabledHashCode, (int) DemisterBallFlameEffectsEnabled.Value);
    zdo.Set(FlameEffectsColorHashCode, DemisterBallFlameEffectsColor.Value);
    zdo.Set(FlameEffectsEmbersColorHashCode, FlameEffectsEmbersColor.Value);
    zdo.Set(FlameEffectsEmbersBrightnessHashCode, FlameEffectsEmbersBrightness.Value);
    zdo.Set(FlameEffectsSparcsColorHashCode, FlameEffectsSparcsColor.Value);
    zdo.Set(FlameEffectsSparcsBrightnessHashCode, FlameEffectsSparcsBrightness.Value);
  }

  public static void SetupParentConstraint() {
    if (LocalPlayerDemister
        && LocalPlayerDemister.m_ballInstance
        && LocalPlayerDemister.m_character == Player.m_localPlayer) {
      if (!LocalPlayerDemister.m_ballInstance.TryGetComponent(out ParentConstraint parentConstraint)) {
        parentConstraint = LocalPlayerDemister.m_ballInstance.AddComponent<ParentConstraint>();
      }

      SetupParentConstraint(parentConstraint, LocalPlayerDemister.m_character);
    }
  }

  public static void SetupParentConstraint(ParentConstraint parentConstraint, Character character) {
    parentConstraint.constraintActive = DemisterBallLockPosition.Value;

    if (parentConstraint.sourceCount < 1) {
      parentConstraint.AddSource(new ConstraintSource());
    }

    ConstraintSource constraintSource =
        new() {
          sourceTransform =
            DemisterBallLockTarget.Value switch {
              LockTarget.Head => character.m_head,
              LockTarget.Eye => character.m_eye,
              _ => character.transform
            },
          weight = 1f
        };

    parentConstraint.SetSource(0, constraintSource);
    parentConstraint.SetTranslationOffset(0, DemisterBallLockOffset.Value);
  }
}
