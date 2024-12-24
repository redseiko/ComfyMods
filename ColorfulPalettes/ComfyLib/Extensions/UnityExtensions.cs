namespace ComfyLib;

using System;

public static class ChatExtensions {
  public static void AddMessage(this Chat chat, object obj) {
    if (chat) {
      chat.AddString($"{obj}");
      chat.m_hideTimer = 0f;
    }
  }
}

public static class ComponentExtensions {
  public static bool TryGetComponentInChildren<T>(
      this UnityEngine.GameObject gameObject, out T component) where T : UnityEngine.Component {
    component = gameObject.GetComponentInChildren<T>();
    return component;
  }

  public static bool TryGetComponentInParent<T>(
      this UnityEngine.GameObject gameObject, out T component) where T : UnityEngine.Component {
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
