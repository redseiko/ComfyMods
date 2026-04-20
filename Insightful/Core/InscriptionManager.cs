namespace Insightful;

using ComfyLib;

using UnityEngine;

public static class InscriptionManager {
  public const int InscriptionTopicHash = 725742509;  // InscriptionTopic
  public const int InscriptionTextHash = 706967417;   // InscriptionText
  public const int InscriptionStyleHash = 334462699;  // InscriptionStyle

  static GameObject _cachedHovering = default;
  static bool _cachedHasInscription = false;

  public static bool HasInscription(GameObject hovering) {
    if (hovering == _cachedHovering) {
      return _cachedHasInscription;
    }

    _cachedHovering = hovering;
    _cachedHasInscription = false;

    if (hovering
        && hovering.TryGetComponentInParent(out ZNetView netView)
        && netView.IsValid()
        && ZDOExtraData.s_strings.TryGetValue(netView.m_zdo.m_uid, out BinarySearchDictionary<int, string> values)
        && values.ContainsKey(InscriptionTopicHash)
        && values.ContainsKey(InscriptionTextHash)) {
      _cachedHasInscription = true;
    }

    return _cachedHasInscription;
  }

  public static void ReadInscription(GameObject hovering) {
    if (!hovering || !hovering.TryGetComponentInParent(out ZNetView netView) || !netView.IsValid()) {
      return;
    }

    ZDO zdo = netView.m_zdo;

    if (!zdo.TryGetString(InscriptionTopicHash, out string inscriptionTopic)
        || !zdo.TryGetString(InscriptionTextHash, out string inscriptionText)) {
      Insightful.LogInfo("RuneStone does not have custom Inscription Topic or Text.");
      return;
    }

    Insightful.LogInfo($"Found hidden Inscription on RuneStone: {zdo.m_uid}");

    if (!zdo.TryGetEnum(InscriptionStyleHash, out TextViewer.Style style)) {
      style = TextViewer.Style.Rune;
    }

    TextViewer.instance.ShowText(style, inscriptionTopic, inscriptionText, autoHide: true);
  }
}
