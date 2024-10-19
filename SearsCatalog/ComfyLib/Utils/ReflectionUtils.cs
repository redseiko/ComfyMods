namespace ComfyLib;

using System;
using System.Reflection;

using HarmonyLib;

public static class ReflectionUtils {
  public static MethodInfo GetFirstMatchingMethod(Type type, string methodName, params Type[][] parameterTypesList) {
    foreach (Type[] parameterTypes in parameterTypesList) {
      MethodInfo method = type.GetMethod(methodName, AccessTools.allDeclared, default, parameterTypes, default);

      if (method != default) {
        return method;
      }
    }

    return default;
  }
}
