namespace ComfyLib;

using System;
using System.Collections.Generic;
using System.Reflection;

using BepInEx.Configuration;

using HarmonyLib;

[AttributeUsage(AttributeTargets.Method, AllowMultiple = false, Inherited = false)]
public sealed class ComfyConfigAttribute : Attribute {
  internal readonly MethodBase TargetMethod;

  public ComfyConfigAttribute() {
    TargetMethod = default;
  }

  public ComfyConfigAttribute(Type lateBindType, string lateBindMethodName) {
    TargetMethod = AccessTools.Method(lateBindType, lateBindMethodName);
  }
}

[HarmonyPatch]
public static class ComfyConfigUtils {
  static IEnumerable<MethodBase> TargetMethods() {
    return _configBinders.Keys;
  }

  static void Postfix(MethodBase __originalMethod) {
    if (_configBinders.Count > 0 && _configBinders.TryGetValue(__originalMethod, out List<ConfigBinder> binders)) {
      foreach (ConfigBinder binder in binders) {
        binder.BindConfigMethod.Invoke(null, new object[] { binder.ConfigFile });
      }

      binders.Clear();
      _configBinders.Remove(__originalMethod);
    }
  }

  public static void BindConfig(ConfigFile config) {
    List<MethodInfo> methods = GetBindConfigMethods(Assembly.GetExecutingAssembly());

    foreach (MethodInfo method in methods) {
      ComfyConfigAttribute attribute = method.GetCustomAttribute<ComfyConfigAttribute>(inherit: false);

      if (attribute.TargetMethod == default) {
        method.Invoke(null, new object[] { config });
      } else {
        AddConfigBinder(config, attribute.TargetMethod, method);
      }
    }
  }

  static readonly Dictionary<MethodBase, List<ConfigBinder>> _configBinders = new();

  static void AddConfigBinder(ConfigFile configFile, MethodBase targetMethod, MethodBase bindConfigMethod) {
    if (!_configBinders.TryGetValue(targetMethod, out List<ConfigBinder> binders)) {
      binders = new();
      _configBinders[targetMethod] = binders;
    }

    binders.Add(new(configFile, targetMethod, bindConfigMethod));
  }

  sealed class ConfigBinder {
    public readonly ConfigFile ConfigFile;
    public readonly MethodBase TargetMethod;
    public readonly MethodBase BindConfigMethod;

    public ConfigBinder(ConfigFile configFile, MethodBase targetMethod, MethodBase bindConfigMethod) {
      ConfigFile = configFile;
      TargetMethod = targetMethod;
      BindConfigMethod = bindConfigMethod;
    }
  }

  static List<MethodInfo> GetBindConfigMethods(Assembly assembly) {
    List<MethodInfo> methods = new();

    foreach (Type type in assembly.GetTypes()) {
      foreach (
          MethodInfo method in type.GetMethods(BindingFlags.Static | BindingFlags.Public | BindingFlags.NonPublic)) {
        ComfyConfigAttribute attribute = method.GetCustomAttribute<ComfyConfigAttribute>(inherit: false);

        if (attribute == null) {
          continue;
        }

        ParameterInfo[] parameters = method.GetParameters();

        if (parameters.Length == 1 && parameters[0].ParameterType == typeof(ConfigFile)) {
          methods.Add(method);
        }
      }
    }

    return methods;
  }
}
