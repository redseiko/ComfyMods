namespace ComfyLib {
  public static class UnityExtensons {
    public static T Ref<T>(this T gameObject) where T : UnityEngine.Object {
      return gameObject ? gameObject : null;
    }
  }
}
