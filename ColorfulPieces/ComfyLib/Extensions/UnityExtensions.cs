namespace ComfyLib;

public static class ComponentExtensions {
  public static bool TryGetComponentInParent<T>(
      this UnityEngine.GameObject gameObject, out T component) where T : UnityEngine.Component {
    component = gameObject.GetComponentInParent<T>();
    return component;
  }
}

public static class ObjectExtensions {
  public static T Ref<T>(this T unityObject) where T : UnityEngine.Object {
    return unityObject ? unityObject : null;
  }
}
