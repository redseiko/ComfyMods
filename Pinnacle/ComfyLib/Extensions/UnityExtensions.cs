namespace ComfyLib;

public static class ChatExtensions {
  public static void AddMessage(this Chat chat, object obj) {
    if (chat) {
      chat.AddString($"{obj}");
      chat.m_hideTimer = 0f;
    }
  }
}

public static class ObjectExtensions {
  public static T Ref<T>(this T unityObject) where T : UnityEngine.Object {
    return unityObject ? unityObject : null;
  }
}
