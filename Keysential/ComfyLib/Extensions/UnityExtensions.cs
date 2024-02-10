namespace ComfyLib;

public static class ObjectExtensions {
  public static T Ref<T>(this T unityObject) where T : UnityEngine.Object {
    return unityObject ? unityObject : null;
  }
}
