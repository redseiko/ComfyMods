namespace ComfyLib;

using System;

public static class ComponentExtensions {
  public static bool TryGetComponentInChildren<T>(
      this UnityEngine.Component rootComponent, out T component) where T : UnityEngine.Component {
    component = rootComponent.GetComponentInChildren<T>(includeInactive: true);
    return component;
  }
}

public static class ObjectExtensions {
  public static T FirstByNameOrThrow<T>(this T[] unityObjects, string name) where T : UnityEngine.Object {
    foreach (T unityObject in unityObjects) {
      if (unityObject.name == name) {
        return unityObject;
      }
    }

    throw new InvalidOperationException($"Could not find Unity object of type {typeof(T)} with name: {name}");
  }

  public static T Ref<T>(this T unityObject) where T : UnityEngine.Object {
    return unityObject ? unityObject : null;
  }
}
