namespace ComfyLib;

using System;

using UnityEngine;

public static class ChatExtensions {
  public static void AddMessage(this Chat chat, object obj) {
    if (chat) {
      chat.AddString(obj.ToString());
      chat.m_hideTimer = 0f;
    }
  }
}

public static class ColliderExtensions {
  public static Collider SetEnabled(this Collider collider, bool enabled) {
    collider.enabled = enabled;
    return collider;
  }

  public static Collider SetIsTrigger(this Collider collider, bool isTrigger) {
    collider.isTrigger = isTrigger;
    return collider;
  }

  public static SphereCollider SetRadius(this SphereCollider collider, float radius) {
    collider.radius = radius;
    return collider;
  }
}

public static class ComponentExtensions {
  public static T GetOrAddComponent<T>(this Component component) where T : Component {
    return component.TryGetComponent(out T componentOut)
        ? componentOut
        : component.gameObject.AddComponent<T>();
  }

  public static bool TryGetComponentInChildren<T>(this Component component, out T componentOut) where T : Component {
    componentOut = component.GetComponentInChildren<T>();
    return componentOut;
  }

  public static bool TryGetComponentInParent<T>(this Component component, out T componentOut) where T : Component {
    componentOut = component.GetComponentInParent<T>();
    return componentOut;
  }
}

public static class GameObjectExtensions {
  public static bool TryGetComponentInChildren<T>(this GameObject gameObject, out T component) where T : Component {
    component = gameObject.GetComponentInChildren<T>();
    return component;
  }

  public static bool TryGetComponentInParent<T>(this GameObject gameObject, out T component) where T : Component {
    component = gameObject.GetComponentInParent<T>();
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
