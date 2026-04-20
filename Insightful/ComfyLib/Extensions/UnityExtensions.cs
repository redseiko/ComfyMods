namespace ComfyLib;

using System;

using TMPro;

public static class ChatExtensions {
  public static void AddMessage(this Chat chat, object obj) {
    if (chat) {
      chat.AddString(obj.ToString());
      chat.m_hideTimer = 0f;
    }
  }
}

public static class ComponentExtensions {
  public static bool TryGetComponentInChildren<T>(
      this UnityEngine.Component parentComponent, out T component) where T : UnityEngine.Component {
    component = parentComponent.GetComponentInChildren<T>();
    return component;
  }

  public static bool TryGetComponentInChildren<T>(
      this UnityEngine.GameObject gameObject, out T component) where T : UnityEngine.Component {
    component = gameObject.GetComponentInChildren<T>();
    return component;
  }

  public static bool TryGetComponentInParent<T>(
    this UnityEngine.Component childComponent, out T component) where T : UnityEngine.Component {
    component = childComponent.GetComponentInParent<T>();
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

public static class TextMeshProExtensiosn {
  public static TextMeshProUGUI Append(this TextMeshProUGUI tmpText, string value) {
    tmpText.text = tmpText.text.Length == 0 ? value : $"{tmpText.text}\n{value}";
    return tmpText;
  }
}

public static class ZDOExtensions {
  public static bool TryGetString(this ZDO zdo, int keyHashCode, out string result) {
    if (ZDOExtraData.s_strings.TryGetValue(zdo.m_uid, out BinarySearchDictionary<int, string> values)
        && values.TryGetValue(keyHashCode, out result)) {
      return true;
    }

    result = default;
    return false;
  }

  public static bool TryGetEnum<T>(this ZDO zdo, int keyHashCode, out T result) {
    if (ZDOExtraData.s_ints.TryGetValue(zdo.m_uid, out BinarySearchDictionary<int, int> values)
        && values.TryGetValue(keyHashCode, out int value)) {
      result = (T) Enum.ToObject(typeof(T), value);
      return true;
    }

    result = default;
    return false;
  }
}
