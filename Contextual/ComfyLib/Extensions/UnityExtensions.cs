namespace ComfyLib;

using System;
using System.Collections.Generic;

using UnityEngine;

public static class ObjectExtensions {
  public static T FirstByNameOrThrow<T>(this IEnumerable<T> unityObjects, string name) where T : UnityEngine.Object {
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

public static class TransformExtensions {
  public static T SetLocalPosition<T>(this T transform, Vector3 localPosition) where T : Transform {
    transform.localPosition = localPosition;
    return transform;
  }
}
