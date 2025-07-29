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

public static class ComponentExtensions {
  public static T GetOrAddComponent<T>(this Component component) where T : Component {
    return component.gameObject.TryGetComponent(out T existingComponent)
        ? existingComponent
        : component.gameObject.AddComponent<T>();
  }
}

public static class GameObjectExtensions {
  public static T GetOrAddComponent<T>(this GameObject gameObject) where T : Component {
    return gameObject.TryGetComponent(out T existingComponent) ? existingComponent : gameObject.AddComponent<T>();
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

  public static T SetName<T>(this T unityObject, string name) where T : UnityEngine.Object {
    unityObject.name = name;
    return unityObject;
  }
}
