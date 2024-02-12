namespace Intermission;

using System.Collections;

using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

public static class ImageScaleUtils {
  static Coroutine _scaleLerpCoroutine;

  public static IEnumerator ScaleLerp(
      Transform transform, Vector3 startScale, Vector3 endScale, float lerpDuration) {
    transform.localScale = startScale;
    float timeElapsed = 0f;

    while (timeElapsed < lerpDuration) {
      float t = timeElapsed / lerpDuration;
      t = t * t * (3f - (2f * t));

      transform.localScale = Vector3.Lerp(startScale, endScale, t);
      timeElapsed += Time.deltaTime;

      yield return null;
    }

    transform.localScale = endScale;
  }

  public static void ScaleLerpLoadingImage(Image loadingImage) {
    if (_scaleLerpCoroutine != null) {
      Hud.m_instance.Ref()?.StopCoroutine(_scaleLerpCoroutine);
      _scaleLerpCoroutine = null;
    }

    if (LoadingImageUseScaleLerp.Value && loadingImage) {
      _scaleLerpCoroutine =
          Hud.m_instance.Ref()?.StartCoroutine(
              ScaleLerp(
                  loadingImage.transform,
                  Vector3.one,
                  Vector3.one * LoadingImageScaleLerpEndScale.Value,
                  LoadingImageScaleLerpDuration.Value));
    }
  }
}
