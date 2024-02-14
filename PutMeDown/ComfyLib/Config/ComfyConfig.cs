namespace ComfyLib;

using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BepInEx.Configuration;

using HarmonyLib;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class ComfyConfigAttribute : Attribute {
  public bool LateBind { get; set; } = false;
}

public static class ComfyConfigUtils {
  public static void BindConfig(ConfigFile config) {
    BindConfigs(config, Assembly.GetExecutingAssembly());
  }

  static void BindConfigs(ConfigFile config, Assembly assembly) {
    foreach ((MethodInfo method, ComfyConfigAttribute comfyConfig) in GetBindConfigMethods(assembly)) {
      ComfyConfigBinder.Bind(config, method, comfyConfig);
    }
  }

  static IEnumerable<(MethodInfo, ComfyConfigAttribute)> GetBindConfigMethods(Assembly assembly) {
    return assembly.GetTypes()
        .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
        .SelectMany(method => GetBindConfigMethod(method));
  }

  static IEnumerable<(MethodInfo, ComfyConfigAttribute)> GetBindConfigMethod(MethodInfo method) {
    ComfyConfigAttribute attribute = method.GetCustomAttribute< ComfyConfigAttribute>(inherit: false);

    if (attribute != null) {
      ParameterInfo[] parameters = method.GetParameters();

      if (parameters.Length == 1 && parameters[0].ParameterType == typeof(ConfigFile)) {
        yield return (method, attribute);
      }
    }
  }

  [HarmonyPatch]
  static class ComfyConfigBinder {
    static readonly Queue<Action> _lateBindQueue = new();
    static bool _startupPatched = false;

    public static void Bind(ConfigFile config, MethodInfo method, ComfyConfigAttribute attribute) {
      if (!attribute.LateBind || _startupPatched) {
        method.Invoke(null, new object[] { config });
      } else {
        _lateBindQueue.Enqueue(() => method.Invoke(null, new object[] { config }));
      }
    }

    [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.Start))]
    static void Postfix() {
      while (_lateBindQueue.Count > 0) {
        _lateBindQueue.Dequeue()?.Invoke();
      }

      _startupPatched = true;
    }
  }
}
