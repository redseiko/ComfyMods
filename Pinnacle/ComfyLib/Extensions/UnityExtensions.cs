namespace ComfyLib;

using System;
using System.Collections.Generic;
using System.Linq;

using UnityEngine;
using UnityEngine.UI;

public static class ChatExtensions {
  public static void AddMessage(this Chat chat, object obj) {
    if (chat) {
      chat.AddString(obj.ToString());
      chat.m_hideTimer = 0f;
    }
  }
}

public static class GameObjectExtensions {
  public static GameObject SetParent(
      this GameObject gameObject, Transform transform, bool worldPositionStays = false) {
    gameObject.transform.SetParent(transform, worldPositionStays);
    return gameObject;
  }

  public static IEnumerable<GameObject> Children(this GameObject gameObject) {
    return gameObject
        ? gameObject.transform.Cast<Transform>().Select(t => t.gameObject)
        : [];
  }

  public static Button Button(this GameObject gameObject) {
    return gameObject ? gameObject.GetComponent<Button>() : null;
  }

  public static Image Image(this GameObject gameObject) {
    return gameObject ? gameObject.GetComponent<Image>() : null;
  }

  public static LayoutElement LayoutElement(this GameObject gameObject) {
    return gameObject ? gameObject.GetComponent<LayoutElement>() : null;
  }

  public static RectTransform RectTransform(this GameObject gameObject) {
    return gameObject ? gameObject.GetComponent<RectTransform>() : null;
  }

  public static Text Text(this GameObject gameObject) {
    return gameObject ? gameObject.GetComponent<Text>() : null;
  }
}

public static class ObjectExtensions {
  public static T FirstByName<T>(this T[] unityObjects, string name) where T : UnityEngine.Object {
    foreach (T unityObject in unityObjects) {
      if (unityObject.name == name) {
        return unityObject;
      }
    }

    return default;
  }

  public static T FirstByNameOrThrow<T>(this T[] unityObjects, string name) where T : UnityEngine.Object {
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
