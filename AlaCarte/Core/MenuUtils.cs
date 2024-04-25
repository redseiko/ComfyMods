namespace AlaCarte;

using ComfyLib;

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

    Image image = menuTransform.Find("darken").GetComponent<Image>();
    image.raycastTarget = false;

    PanelDragger dragger = menuTransform.Find("ornament").gameObject.AddComponent<PanelDragger>();
    dragger.TargetRectTransform = menuTransform;
    dragger.OnPanelEndDrag += OnMenuDialogEndDrag;
  }

  public static void SetupMenuDialogOld(RectTransform menuTransform) {
    MenuDialogOld = menuTransform;

    PanelDragger dragger = menuTransform.gameObject.AddComponent<PanelDragger>();
    dragger.TargetRectTransform = menuTransform;
    dragger.OnPanelEndDrag += OnMenuDialogEndDrag;
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
    return dialogType switch {
      DialogType.Vanilla => MenuDialogVanilla,
      DialogType.Old => MenuDialogOld,
      _ => throw new System.NotImplementedException(),
    };
  }

  static GameObject GetHandlerDefaultElementByType(DialogType dialogType) {
    return dialogType switch {
      DialogType.Vanilla => _handlerDefaultElementVanilla,
      DialogType.Old => _handlerDefaultElementOld,
      _ => default,
    };
  }
}
