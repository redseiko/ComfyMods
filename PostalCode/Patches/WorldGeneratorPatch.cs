namespace PostalCode;

using System.Collections.Generic;
using System.Reflection.Emit;

using HarmonyLib;

using UnityEngine;

[HarmonyPatch(typeof(WorldGenerator))]
static class WorldGeneratorPatch {
  // ====================================================================
  // CONFIGURATION
  // ====================================================================

  // This should theoretically be configurable
  const float NewWorldSize = 20000f;

  // The original hardcoded limit we want to replace
  const float VanillaLimit = 10500f;

  // The gap where we force the "Moat" (Abyss) to separate old world from new
  const float MoatStart = 10500f;
  const float MoatEnd = 11000f;

  [HarmonyTranspiler]
  [HarmonyPatch(nameof(WorldGenerator.GetBiomeHeight))]
  static IEnumerable<CodeInstruction> GetBiomeHeightTranspiler(
      IEnumerable<CodeInstruction> instructions, ILGenerator generator) {
    return new CodeMatcher(instructions, generator)
        .Start()
        .MatchStartForward(
            new CodeMatch(OpCodes.Ldarg_2),
            new CodeMatch(OpCodes.Ldarg_3),
            new CodeMatch(
                OpCodes.Call,
                AccessTools.Method(typeof(DUtils), nameof(DUtils.Length), [typeof(float), typeof(float)])),
            new CodeMatch(OpCodes.Ldc_R4, 10500f))
        .ThrowIfInvalid($"Could not patch WorldGenerator.GetBiomeHeight()! (world-edge-height)")
        .Advance(offset: 3)
        .InsertAndAdvance(
            new CodeInstruction(
                OpCodes.Call, AccessTools.Method(typeof(WorldGeneratorPatch), nameof(WorldEdgeHeightDelegate))))
        .InstructionEnumeration();
  }

  static float WorldEdgeHeightDelegate(float length) {
    if (length < 11000f) {
      return length;
    }

    return 0f;
  }

  [HarmonyPrefix]
  [HarmonyPatch(nameof(WorldGenerator.GetBiome), [typeof(float), typeof(float), typeof(float), typeof(bool)])]
  static bool GetBiomePrefix(WorldGenerator __instance, float wx, float wy, ref Heightmap.Biome __result) {
    float distance = DUtils.Length(wx, wy);

    if (distance <= MoatStart) {
      return true;
    }

    // Might just combine this with above.
    if (distance < MoatEnd) {
      __result = Heightmap.Biome.Ocean;
      return false;
    }

    if (wy > 10000f + __instance.m_offset0) {
      __result = Heightmap.Biome.DeepNorth;
      return false;
    }

    if (wy < -10000f - __instance.m_offset0) {
      __result = Heightmap.Biome.AshLands;
      return false;
    }

    float baseHeight = __instance.GetBaseHeight(wx, wy, false);

    // Simple logic for the Outer Ring test:
    // Just generate some noise based biomes to prove it works.

    // "Outer Mistlands"
    // Replicating the functionality roughly:
    // if (DUtils.PerlinNoise(...) > minDarklandNoise && baseHeight > 0.1f) ...

    // For the SCAFFOLD, we will just use a placeholder Ocean/Plains mix to test checking boundaries.
    if (baseHeight > 0.4f) {
      __result = Heightmap.Biome.Mountain;
    } else if (baseHeight > 0.1f) {
      // Random mix based on noise
      float noise = Mathf.PerlinNoise((wx + __instance.m_offset0) * 0.001f, (wy + __instance.m_offset0) * 0.001f);
      if (noise > 0.6f) {
        __result = Heightmap.Biome.Mistlands;
      } else if (noise > 0.4f) {
        __result = Heightmap.Biome.Plains;
      } else {
        __result = Heightmap.Biome.BlackForest;
      }
    } else {
      __result = Heightmap.Biome.Ocean;
    }

    return false; // Skip original
  }

  // ====================================================================
  // PREFIX: GetBaseHeight
  // Purpose: Ensure base height generation continues beyond 10500m.
  // ====================================================================

  [HarmonyPrefix]
  [HarmonyPatch(nameof(WorldGenerator.GetBaseHeight))]
  static bool GetBaseHeightPrefix(WorldGenerator __instance, float wx, float wy, bool menuTerrain, ref float __result) {
    if (menuTerrain) {
      return true;
    }

    float distance = DUtils.Length(wx, wy);

    if (distance < 11000f) {
      return true;
    }

    // 2. THE MOAT (10,500m - 11,000m)
    // Force the Abyss/Ocean gap to cleanly separate the old world from new.
    //if (distance < MoatEnd) {
    //  __result = -2f;
    //  return false; // Skip original execution
    //}

    // 3. THE OUTER LANDS (> 11,000m)
    // Replicate vanilla logic but without the > 10000f clamp

    float wxOffset = wx + 100000f + __instance.m_offset0;
    float wyOffset = wy + 100000f + __instance.m_offset1;

    // Base Perlin generation (Copied from WorldGenerator.GetBaseHeight)
    float baseHeight = 0f;

    baseHeight +=
      Mathf.PerlinNoise(wxOffset * 0.002f * 0.5f, wyOffset * 0.002f * 0.5f)
          * Mathf.PerlinNoise(wxOffset * 0.003f * 0.5f, wyOffset * 0.003f * 0.5f)
          * 1f;

    baseHeight +=
      Mathf.PerlinNoise(wxOffset * 0.002f * 1f, wyOffset * 0.002f * 1f)
          * Mathf.PerlinNoise(wxOffset * 0.003f * 1f, wyOffset * 0.003f * 1f)
          * baseHeight * 0.9f;

    baseHeight +=
      Mathf.PerlinNoise(wxOffset * 0.005f * 1f, wyOffset * 0.005f * 1f)
          * Mathf.PerlinNoise(wxOffset * 0.01f * 1f, wyOffset * 0.01f * 1f)
          * 0.5f
          * baseHeight;

    baseHeight -= 0.07f;

    float mountainBase = Mathf.PerlinNoise(wxOffset * 0.002f * 0.25f + 0.123f, wyOffset * 0.002f * 0.25f + 0.15123f);
    float mountainMask = Mathf.PerlinNoise(wxOffset * 0.002f * 0.25f + 0.321f, wyOffset * 0.002f * 0.25f + 0.231f);
    float mountainDistance = Mathf.Abs(mountainBase - mountainMask);
    float mountainWeight = 1f - Utils.LerpStep(0.02f, 0.12f, mountainDistance);

    // Vanilla uses SmoothStep(744, 1000, distance) here to fade in mountains? 
    // We are far out, so we can assume fully faded in (= 1). 
    // Or we might want to check against our own "Outer Edge" if we wanted another fade, but for now full intensity.
    mountainWeight *= 1f;

    baseHeight *= (1f - mountainWeight);

    // The vanilla code would check if (distance > 10000f) here and clamp. We SKIP that.

    // Mountain Logic (Copied)
    // Note: m_minMountainDistance is usually 3000. distance > 11000 is definitely > 3000.
    if (baseHeight > 0.28f) {
      // Deep North / Ashlands mountain logic skipped for brevity, standard mountains:
      float steepness = Utils.Clamp01((baseHeight - 0.28f) / 0.1f);
      // Lerp(Lerp(0.28, 0.38, ...), baseHeight, ...)
      // Simplified:
      float minMountain = Mathf.Lerp(0.28f, 0.38f, steepness);
      baseHeight = Mathf.Lerp(minMountain, baseHeight, 1f); // 1f because we are far past minMountainDistance
    }

    __result = baseHeight;
    return false;
  }

  // ====================================================================
  // PREFIX: IsAshlands
  // Purpose: Prevent EnvMan from switching to Ashlands environment (hiding water)
  //          in the NEW extended area (-19500 < y < -10500).
  //          We MUST preserve vanilla Ashlands (-10500 < y < ?).
  // ====================================================================
  [HarmonyPrefix]
  [HarmonyPatch(nameof(WorldGenerator.IsAshlands))]
  static bool IsAshlandsPrefix(float x, float y, ref bool __result) {
    // If we are strictly beyond the new pole, it is Ashlands.
    if (y < -19500f) {
      __result = true;
      return false;
    }

    // If we are in the vanilla world (including vanilla Ashlands), run original logic.
    // Vanilla limit is roughly 10500. Let's start the override a bit further out to be safe.
    if (y > -11000f) {
      return true; // Run original (which returns true if y < -10500)
    }

    // In the gap (-19500 to -11000), force FALSE to allow other biomes/water to render.
    __result = false;
    return false;
  }

  // ====================================================================
  // PREFIX: IsDeepNorth
  // Purpose: Prevent EnvMan from switching to Deep North environment
  //          in the NEW extended area.
  // ====================================================================
  [HarmonyPrefix]
  [HarmonyPatch(nameof(WorldGenerator.IsDeepnorth))]
  static bool IsDeepNorthPrefix(float x, float y, ref bool __result) {
    if (y > 19500f) {
      __result = true;
      return false;
    }

    if (y < 11000f) {
      return true; // Run original
    }

    __result = false;
    return false;
  }
}
