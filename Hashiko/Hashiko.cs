namespace Hashiko;

using System.Collections.Generic;

using BepInEx.Logging;

using Mono.Cecil;

public static class Hashiko {
  public const string PluginGuid = "redseiko.valheim.hashiko";
  public const string PluginName = "Hashiko";
  public const string PluginVersion = "1.0.0";

  public static IEnumerable<string> TargetDLLs { get; } = ["assembly_utils.dll"];

  static ManualLogSource _logger;

  public static void Initialize() {
    _logger = Logger.CreateLogSource(PluginName);
  }

  public static void Patch(AssemblyDefinition assembly) {
    ModuleDefinition module = assembly.MainModule;

    if (!HashikoManager.TryGetStringExtensionMethodsType(module, out TypeDefinition stringExtensionMethodsType)) {
      _logger.LogError($"Could not find class `StringExtensionMethods`.");
      return;
    }

    if (HashikoManager.HasGetStableHashCodeMethod(stringExtensionMethodsType)) {
      _logger.LogInfo($"Found method `GetStableHashCode(this string str)`.");
      return;
    }

    _logger.LogInfo("Method `GetStableHashCode(this string str)` not found. Adding...");

    MethodDefinition clonedMethod = HashikoManager.CloneGetStableHashCodeMethod(module);
    stringExtensionMethodsType.Methods.Add(clonedMethod);

    _logger.LogInfo("Successfully added method `GetStableHashCode(this string str)`.");
  }
}
