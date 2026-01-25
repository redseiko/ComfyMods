namespace PostalCode;

using HarmonyLib;
using UnityEngine;

[HarmonyPatch(typeof(Minimap))]
static class MinimapPatch {

  // ====================================================================
  // PREFIX: Awake
  // Purpose: Resize minimap texture and scope to cover the extended world.
  // ====================================================================

  [HarmonyPrefix]
  [HarmonyPatch(nameof(Minimap.Awake))]
  static void AwakePrefix(Minimap __instance) {
    // Vanilla: Texture=2048, PixelSize=12 => 2048 * 12 = 24,576 meters coverage (Diameter)
    // World Radius = 10,500 => Diameter = 21,000. Fits nicely.
    
    // New World Radius = 20,000 => Diameter = 40,000.
    // Option A: Double Texture Size (2048 -> 4096).
    // 4096 * 12 = 49,152 coverage. Fits 40,000. 
    // Cost: 4x Memory usage. 
    
    // Option B: Double Pixel Size (12 -> 24).
    // 2048 * 24 = 49,152 coverage. Fits 40,000.
    // Cost: Half resolution.
    
    // Let's go with Option A (Texture Size) for quality, user can optimize if needed.
    // Actually, texture size is usually locked to powers of 2. 4096 is standard for "4k".
    
    __instance.m_textureSize = 4096;
    
    // We can leave pixel size as is (12) or adjust slightly if we want exact bounds.
    // 4096 * 10 = 40960. (Perfect fit for 20k radius).
    // Let's set it to 10 to get slightly better detail than 12, since we have the pixels.
    __instance.m_pixelSize = 10f; 
  }
}
