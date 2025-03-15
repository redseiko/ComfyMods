namespace ComfyLib;

using UnityEngine;

public static class ColorExtensions {
  public static string GetColorHtmlString(this Color color) {
    return color.a == 1f
        ? ColorUtility.ToHtmlStringRGB(color)
        : ColorUtility.ToHtmlStringRGBA(color);
  }

  public static Color SetAlpha(this Color color, float alpha) {
    color.a = alpha;
    return color;
  }
}

public static class GameObjectExtensions {
  public static bool TryGetComponentInChildren<T>(
      this GameObject gameObject, out T component) where T : Component {
    component = gameObject.GetComponentInChildren<T>();
    return component;
  }

  public static bool TryGetComponentInParent<T>(
      this GameObject gameObject, out T component) where T : Component {
    component = gameObject.GetComponentInParent<T>();
    return component;
  }
}

public static class ParticleSystemExtensions {
  public static void Restart(this ParticleSystem particleSystem) {
    particleSystem.Clear();
    particleSystem.Simulate(0f);
    particleSystem.Play();
  }
}

public static class UnityExtensions {
  public static T Ref<T>(this T unityObject) where T : Object {
    return unityObject ? unityObject : default;
  }
}
