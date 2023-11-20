using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

using BepInEx.Configuration;

using HarmonyLib;

namespace ComfyLib {
  [AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
  public class ComfyConfigAttribute : Attribute {
    public bool LateBind { get; set; }
  }

  public static class ComfyConfigUtils {
    static readonly Queue<Action> _lateBindConfigQueue = new();

    [HarmonyPatch(typeof(FejdStartup), nameof(FejdStartup.Start))]
    static class LateBindConfigPatch {
      static void Postfix() {
        while (_lateBindConfigQueue.Count > 0) {
          _lateBindConfigQueue.Dequeue()?.Invoke();
        }
      }
    }

    public static void BindConfig(ConfigFile config) {
      BindConfigs(config, Assembly.GetExecutingAssembly());
    }

    static void BindConfigs(ConfigFile config, Assembly assembly) {
      foreach ((MethodInfo method, ComfyConfigAttribute comfyConfig) in GetBindConfigMethods(assembly)) {
        if (comfyConfig.LateBind) {
          _lateBindConfigQueue.Enqueue(() => method?.Invoke(null, new object[] { config }));
        } else {
          method?.Invoke(null, new object[] { config });
        }
      }
    }

    static IEnumerable<(MethodInfo, ComfyConfigAttribute)> GetBindConfigMethods(Assembly assembly) {
      return assembly.GetTypes()
          .SelectMany(type => type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic))
          .SelectMany(method => GetBindConfigMethod(method));
    }

    static IEnumerable<(MethodInfo, ComfyConfigAttribute)> GetBindConfigMethod(MethodInfo method) {
      ComfyConfigAttribute[] attributes =
          (ComfyConfigAttribute[]) method.GetCustomAttributes(typeof(ComfyConfigAttribute), inherit: false);

      if (attributes.Length > 0) {
        ParameterInfo[] parameters = method.GetParameters();

        if (parameters.Length == 1) {
          yield return (method, attributes[0]);
        }
      }
    }
  }
}
