namespace ColorfulPieces;

using UnityEngine;

public static class MaterialManExtensions {
  public static MaterialMan.PropertyContainer GetPropertyContainer(
      this MaterialMan materialManager, GameObject gameObject) {
    int instanceId = gameObject.GetInstanceID();

    if (!materialManager.m_blocks.TryGetValue(instanceId, out MaterialMan.PropertyContainer propertyContainer)) {
      gameObject.AddComponent<MaterialManNotifier>();
      propertyContainer = new(gameObject, materialManager.m_propertyBlock);
      propertyContainer.MarkDirty += materialManager.QueuePropertyUpdate;
      materialManager.m_blocks.Add(instanceId, propertyContainer);
    }

    return propertyContainer;
  }

  public static MaterialMan.PropertyContainer SetPropertyValue<T>(
      this MaterialMan.PropertyContainer propertyContainer, int propertyId, T propertyValue) {
    propertyContainer.SetValue(propertyId, propertyValue);
    return propertyContainer;
  }
}
