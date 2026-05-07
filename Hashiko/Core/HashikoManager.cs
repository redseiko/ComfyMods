namespace Hashiko;

using Mono.Cecil;
using Mono.Cecil.Cil;

public static class HashikoManager {
  public static bool TryGetStringExtensionMethodsType(
      ModuleDefinition module, out TypeDefinition stringExtensionMethodsType) {
    stringExtensionMethodsType = module.GetType("StringExtensionMethods");
    return stringExtensionMethodsType != default;
  }

  public static bool HasGetStableHashCodeMethod(TypeDefinition stringExtensionMethodsType) {
    foreach (MethodDefinition method in stringExtensionMethodsType.Methods) {
      if (method.Name == "GetStableHashCode"
          && method.IsStatic
          && method.Parameters.Count == 1) {
        return true;
      }
    }

    return false;
  }

  public static MethodDefinition CloneGetStableHashCodeMethod(ModuleDefinition targetModule) {
    using AssemblyDefinition assembly = AssemblyDefinition.ReadAssembly(typeof(Hashiko).Assembly.Location);
    MethodDefinition sourceMethod = GetSourceGetStableHashCodeMethod(assembly);

    return CloneMethod(sourceMethod, targetModule);
  }

  static MethodDefinition GetSourceGetStableHashCodeMethod(AssemblyDefinition assembly) {
    foreach (MethodDefinition method in assembly.MainModule.GetType(typeof(HashikoManager).FullName).Methods) {
      if (method.Name == "GetStableHashCode") {
        return method;
      }
    }

    return default;
  }

  public static int GetStableHashCode(this string str) {
    int num = 5381;
    int num2 = num;
    int num3 = 0;

    while (num3 < str.Length && str[num3] != '\0') {
      num = ((num << 5) + num) ^ str[num3];
      if (num3 == str.Length - 1 || str[num3 + 1] == '\0') {
        break;
      }
      num2 = ((num2 << 5) + num2) ^ str[num3 + 1];
      num3 += 2;
    }

    return num + num2 * 1566083941;
  }

  public static MethodDefinition CloneMethod(MethodDefinition sourceMethod, ModuleDefinition targetModule) {
    MethodDefinition targetMethod =
        new(sourceMethod.Name, sourceMethod.Attributes, targetModule.ImportReference(sourceMethod.ReturnType));

    foreach (CustomAttribute attrib in sourceMethod.CustomAttributes) {
      targetMethod.CustomAttributes.Add(new(targetModule.ImportReference(attrib.Constructor)));
    }

    foreach (ParameterDefinition parameter in sourceMethod.Parameters) {
      targetMethod.Parameters.Add(
          new(parameter.Name, parameter.Attributes, targetModule.ImportReference(parameter.ParameterType)));
    }

    foreach (VariableDefinition variable in sourceMethod.Body.Variables) {
      targetMethod.Body.Variables.Add(new(targetModule.ImportReference(variable.VariableType)));
    }

    ILProcessor ilProcessor = targetMethod.Body.GetILProcessor();

    foreach (Instruction instruction in sourceMethod.Body.Instructions) {
      object operand = instruction.Operand;

      if (operand == null) {
        ilProcessor.Emit(instruction.OpCode);
      } else if (operand is Instruction label) {
        ilProcessor.Emit(instruction.OpCode, label);
      } else if (operand is Instruction[] labels) {
        ilProcessor.Emit(instruction.OpCode, labels);
      } else if (operand is VariableDefinition variableDefinition) {
        ilProcessor.Emit(instruction.OpCode, variableDefinition);
      } else if (operand is ParameterDefinition parameterDefinition) {
        ilProcessor.Emit(instruction.OpCode, parameterDefinition);
      } else if (operand is TypeReference typeReference) {
        ilProcessor.Emit(instruction.OpCode, targetModule.ImportReference(typeReference));
      } else if (operand is MethodReference methodReference) {
        ilProcessor.Emit(instruction.OpCode, targetModule.ImportReference(methodReference));
      } else if (operand is FieldReference fieldReference) {
        ilProcessor.Emit(instruction.OpCode, targetModule.ImportReference(fieldReference));
      } else if (operand is string stringValue) {
        ilProcessor.Emit(instruction.OpCode, stringValue);
      } else if (operand is int intValue) {
        ilProcessor.Emit(instruction.OpCode, intValue);
      } else if (operand is long longValue) {
        ilProcessor.Emit(instruction.OpCode, longValue);
      } else if (operand is float floatValue) {
        ilProcessor.Emit(instruction.OpCode, floatValue);
      } else if (operand is double doubleValue) {
        ilProcessor.Emit(instruction.OpCode, doubleValue);
      } else if (operand is byte byteValue) {
        ilProcessor.Emit(instruction.OpCode, byteValue);
      } else if (operand is sbyte sbyteValue) {
        ilProcessor.Emit(instruction.OpCode, sbyteValue);
      } else {
        throw new System.NotSupportedException($"Unsupported operand type: {operand.GetType().FullName}");
      }
    }

    return targetMethod;
  }

}
