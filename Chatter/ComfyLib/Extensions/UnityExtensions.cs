namespace ComfyLib;

using System;
using System.Collections.Generic;

using UnityEngine;

public static class BehaviourExtensions {
  public static T SetEnabled<T>(this T behaviour, bool enabled) where T : Behaviour {
    behaviour.enabled = enabled;
    return behaviour;
  }
}

public static class GameObjectExtensions {
  public static GameObject SetParent(
      this GameObject gameObject, Transform transform, bool worldPositionStays = false) {
    gameObject.transform.SetParent(transform, worldPositionStays);
    return gameObject;
  }
}

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
    return unityObject ? unityObject : default;
  }

  public static T SetName<T>(this T unityObject, string name) where T : UnityEngine.Object {
    unityObject.name = name;
    return unityObject;
  }
}
