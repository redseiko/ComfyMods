namespace ComfyLib;

using System;
using System.Collections.Generic;
using System.Linq;

public static class ChatExtensions {
  public static void AddMessage(this Chat chat, object obj) {
    if (chat) {
      chat.AddString(obj.ToString());
      chat.m_hideTimer = 0f;
    }
  }
}

public static class EnumExtensions {
  public static IEnumerable<Enum> GetFlags(this Enum input) {
    foreach (Enum value in Enum.GetValues(input.GetType())) {
      if (input.HasFlag(value)) {
        yield return value;
      }
    }
  }

  public static readonly Heightmap.Biome[] HeightmapBiomes =
      ((Heightmap.Biome[]) typeof(Heightmap.Biome).GetEnumValues())
          .Where(biome => biome != Heightmap.Biome.None)
          .ToArray();

  public static IEnumerable<Heightmap.Biome> GetBiomes(this Heightmap.Biome input) {
    if (input == Heightmap.Biome.None) {
      yield return input;
    } else {
      foreach (Heightmap.Biome biome in HeightmapBiomes) {
        if (input.HasFlag(biome)) {
          yield return biome;
        }
      }
    }
  }
}

public static class ObjectExtensions {
  public static T FirstByNameOrThrow<T>(this T[] unityObjects, string name) where T : UnityEngine.Object {
    foreach (T unityObject in unityObjects) {
      if (unityObject.name == name) {
        return unityObject;
      }
    }

    throw new InvalidOperationException($"Could not find Unity object of type {typeof(T)} with name: {name}");
  }

  public static T Ref<T>(this T unityObject) where T : UnityEngine.Object {
    return unityObject ? unityObject : null;
  }
}
