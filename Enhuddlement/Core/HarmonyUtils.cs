namespace Enhuddlement;

using System;
using System.Collections.Generic;
using System.Reflection;

using HarmonyLib;

public static class HarmonyUtils {
  public static readonly HashSet<string> TargetHarmonyIds = ["MK_BetterUI"];

  public static void UnpatchType(Type type) {
    foreach (MethodInfo method in AccessTools.GetDeclaredMethods(type)) {
      UnpatchMethod(method);
    }
  }

  static void UnpatchMethod(MethodInfo method) {
    Patches patches = Harmony.GetPatchInfo(method);

    if (patches == null) {
      return;
    }

    foreach (string harmonyId in patches.Owners) {
      if (TargetHarmonyIds.Contains(harmonyId)) {
        Enhuddlement.HarmonyInstance.Unpatch(method, HarmonyPatchType.All, harmonyId);
      }
    }
  }
}
