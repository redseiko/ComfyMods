namespace EmDee;

using HarmonyLib;

using Markdig;

[HarmonyPatch(typeof(FejdStartup))]
static class FejdStartupPatch {
  [HarmonyPostfix]
  [HarmonyPatch(nameof(FejdStartup.Start))]
  static void StartPostfix(FejdStartup __instance) {
    string html = Markdown.ToHtml($"# Hello world!\n\nThis is a test.\n\n*Of markdown?\n*Of markdown.");
    EmDee.LogInfo(html);
  }
}
