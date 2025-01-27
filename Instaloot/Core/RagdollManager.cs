namespace Instaloot;

public static class RagdollManager {
  // Values last generated for v0.219.16 using GetStableHashCode().
  public const int HasFieldsHash = -310439593;        // HasFields
  public const int HasFieldsRagdollHash = 1878724298; // HasFieldsRagdoll
  public const int RagdollTtlHash = 556035891;        // Ragdoll.m_ttl

  public static bool HasCustomFieldTtl(ZNetView netView) {
    if (!netView || !netView.IsValid()) {
      return false;
    }

    ZDO zdo = netView.m_zdo;

    if (zdo.GetBool(HasFieldsHash)
        && zdo.GetBool(HasFieldsRagdollHash)
        && zdo.GetFloat(RagdollTtlHash, out float _)) {
      return true;
    }

    return false;
  }
}
