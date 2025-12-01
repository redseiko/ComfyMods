namespace AlaCarte;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

public static class MenuUtils {
  public static RectTransform MenuDialogVanilla { get; private set; }
  public static RectTransform MenuDialogOld { get; private set; }

  static UIGroupHandler _menuUIGroupDefaultHandler;
  static GameObject _handlerDefaultElementVanilla;
  static GameObject _handlerDefaultElementOld;

  public static void SetupMenuDialogs(Menu menu) {
    SetupMenuDialogVanilla(menu.m_menuDialog.GetComponent<RectTransform>());
    SetupMenuDialogOld(menu.m_root.Find("OLD_menu").GetComponent<RectTransform>());

    _menuUIGroupDefaultHandler = menu.m_root.GetComponentInChildren<UIGroupHandler>();
    _handlerDefaultElementVanilla = _menuUIGroupDefaultHandler.m_defaultElement;
    _handlerDefaultElementOld = MenuDialogOld.Find("Button_close").gameObject;
  }

  public static void SetupMenuDialogVanilla(RectTransform menuTransform) {
    MenuDialogVanilla = menuTransform;

    Image darkenImage = menuTransform.Find("darken").GetComponent<Image>();
    darkenImage.raycastTarget = false;

    Transform ornamentTransform = menuTransform.Find("ornament");

    if (!ornamentTransform.TryGetComponent(out MenuDialogDragger dragger)) {
      dragger = ornamentTransform.gameObject.AddComponent<MenuDialogDragger>();
      dragger.MenuRectTransform = menuTransform;
      dragger.OnMenuDragEnd += OnMenuDialogEndDrag;
    }
  }

  public static void SetupMenuDialogOld(RectTransform menuTransform) {
    MenuDialogOld = menuTransform;

    if (!menuTransform.TryGetComponent(out MenuDialogDragger dragger)) {
      dragger = menuTransform.gameObject.AddComponent<MenuDialogDragger>();
      dragger.MenuRectTransform = menuTransform;
      dragger.OnMenuDragEnd += OnMenuDialogEndDrag;
    }
  }

  static void OnMenuDialogEndDrag(object sender, Vector3 position) {
    MenuDialogPosition.Value = position;
  }

  public static void ShowMenuDialog(Menu menu) {
    RectTransform menuDialog = GetMenuDialogByType(MenuDialogType.Value);

    if (menu.m_menuDialog != menuDialog) {
      menu.m_menuDialog.gameObject.SetActive(false);
      menu.m_menuDialog = menuDialog;
    }

    menuDialog.anchoredPosition = MenuDialogPosition.Value;
    _menuUIGroupDefaultHandler.m_defaultElement = GetHandlerDefaultElementByType(MenuDialogType.Value);
  }

  public static RectTransform GetMenuDialogByType(DialogType dialogType) {
    switch (dialogType) {
      case DialogType.Vanilla:
        return MenuDialogVanilla;

      case DialogType.Old:
        return MenuDialogOld;

      default:
        throw new System.NotImplementedException($"DialogType {dialogType:F} not supported.");
    }
  }

  static GameObject GetHandlerDefaultElementByType(DialogType dialogType) {
    switch (dialogType) {
      case DialogType.Vanilla:
        return _handlerDefaultElementVanilla;

      case DialogType.Old:
        return _handlerDefaultElementOld;

      default:
        throw new System.NotImplementedException($"DialogType {dialogType:F} not supported.");
    }
  }
}
