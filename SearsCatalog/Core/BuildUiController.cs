namespace SearsCatalog;

using System.Collections.Generic;

using ComfyLib;

using UnityEngine;
using UnityEngine.UI;

using static PluginConfig;

public static class BuildUiController {
  public static RectTransform SelectionWindow { get; private set; }
  public static PanelDragger WindowDragger { get; private set; }
  public static PanelResizer WindowResizer { get; private set; }

  public static PooledGridView PieceGridView { get; private set; }
  public static List<Piece> CurrentPieces => PieceSearchFilter.CurrentPieces;
  public static List<Piece> FilteredPieces => PieceSearchFilter.FilteredPieces;

  public static void SetupBuildUi(BuildUi buildUi) {
    RectTransform selectionWindow = (RectTransform) buildUi.transform.Find("bar/SelectionWindow");
    SelectionWindow = selectionWindow;

    SetupWindowInteractions(selectionWindow);
    SetupTagListLayout(buildUi.m_tagListContainer);
    SetupPieceGridView(buildUi.m_pieceScrollRect);

    PieceSearchFilter.Initialize();
  }

  static void SetupWindowInteractions(RectTransform selectionWindow) {
    if (!selectionWindow.TryGetComponent(out PanelDragger panelDragger)) {
      panelDragger = selectionWindow.gameObject.AddComponent<PanelDragger>();
      panelDragger.OnPanelDragEnd.AddListener(HandlePanelDragEnd);
      panelDragger.enabled = false;
    }

    if (!selectionWindow.TryGetComponentInChildren(out PanelResizer panelResizer)) {
      GameObject resizer = UIBuilder.CreateResizer(selectionWindow);

      panelResizer = resizer.AddComponent<PanelResizer>();
      panelResizer.SetTargetRectTransform(selectionWindow);
      panelResizer.OnPanelResizeEnd.AddListener(HandlePanelResizeEnd);
      panelResizer.enabled = false;
    }

    WindowDragger = panelDragger;
    WindowResizer = panelResizer;
  }

  static void SetupTagListLayout(RectTransform tagListContainer) {
    tagListContainer
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.up)
        .SetPivot(Vector2.zero)
        .SetPosition(Vector2.zero);

    ((RectTransform) tagListContainer.Find("LayoutGroup"))
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.up)
        .SetPivot(Vector2.up)
        .SetPosition(Vector2.zero);
  }

  static void SetupPieceGridView(ScrollRect scrollRect) {
    PieceGridView = new PooledGridView(scrollRect);
    PieceGridView.Setup(CreatePieceButton, RenderPieceButton);
    PieceGridView.ScrollPadding = PieceGridView.StepSize.y;
  }

  static RectTransform CreatePieceButton(Transform parent) {
    GameObject buttonObject = Object.Instantiate(Hud.m_instance.m_buildUi.m_pieceButtonPrefab, parent);
    return buttonObject.GetComponent<RectTransform>();
  }

  static void RenderPieceButton(int index, GameObject item) {
    if (index >= 0 && index < FilteredPieces.Count) {
      BuildUiPieceButton button = item.GetComponent<BuildUiPieceButton>();
      button.Setup(FilteredPieces[index], Hud.m_instance.m_buildUi);
    }
  }

  static void HandlePanelDragEnd(Vector2 position) {
    BuildUiPanelPosition.Value = position;
  }

  static void HandlePanelResizeEnd(Vector2 sizeDelta) {
    BuildUiPanelSizeDelta.Value = sizeDelta;

    Canvas.ForceUpdateCanvases();
    PieceGridView.UpdatePoolSize();
    Hud.m_instance.m_buildUi.ConfigureButtonNavigation();

    if (Player.m_localPlayer) {
      Player.m_localPlayer.GetBuildSelection(out Piece piece, out _, out _, out _, out _);
      UpdatePieceSelection(Hud.m_instance.m_buildUi, piece);
    }
  }

  public static void DestroyBuildUi(BuildUi buildUi) {
    PieceSearchFilter.Destroy();

    SelectionWindow = default;
    WindowDragger = default;
    WindowResizer = default;
    PieceGridView = default;
  }

  public static void ShowBuildUi(BuildUi buildUi) {
    if (!SelectionWindow) {
      return;
    }

    if (!IsModEnabled.Value) {
      SelectionWindow
          .SetSizeDelta(Vector2.zero)
          .SetPosition(Vector2.zero);

      WindowDragger.enabled = false;
      WindowResizer.enabled = false;

      return;
    }

    SelectionWindow
        .SetSizeDelta(BuildUiPanelSizeDelta.Value)
        .SetPosition(BuildUiPanelPosition.Value);

    WindowDragger.enabled = BuildUiCanMovePanel.Value;
    WindowResizer.enabled = BuildUiCanResizePanel.Value;
  }

  public static void UpdatePieceButtons(BuildUi buildUi) {
    buildUi.m_availablePieceCount = buildUi.m_currentBuildTool.m_availablePieces.Count;
    IPieceList pieceList = buildUi.m_pieceLists[buildUi.m_currentPieceList];

    bool hasSpecial = LoadPieces(buildUi, pieceList);
    buildUi.m_specialButtonsContainer.gameObject.SetActive(hasSpecial);

    PieceSearchFilter.ResetSearchTerm();
    ApplySearch(GetSearchTerm(buildUi));

    buildUi.ConfigureButtonNavigation();
  }

  static bool LoadPieces(BuildUi buildUi, IPieceList pieceList) {
    buildUi.m_tempPieces.Clear();
    pieceList.GetAvailablePiecesWithTag(buildUi.m_currentTagId, buildUi.m_currentBuildTool, buildUi.m_tempPieces);

    CurrentPieces.Clear();
    bool hasSpecial = false;

    for (int i = 0, count = buildUi.m_tempPieces.Count; i < count; i++) {
      Piece piece = buildUi.m_tempPieces[i];

      if (piece.m_repairPiece || piece.m_removePiece) {
        if (pieceList.ShowTags) {
          hasSpecial = true;
          SetupSpecialButton(buildUi, piece);
        }
      } else {
        CurrentPieces.Add(piece);
      }
    }

    buildUi.m_tempPieces.Clear();

    if (!hasSpecial && buildUi.m_specialPieceButton) {
      buildUi.m_specialPieceButton.gameObject.SetActive(false);
    }

    return hasSpecial;
  }

  static void SetupSpecialButton(BuildUi buildUi, Piece piece) {
    BuildUiPieceButton specialButton = buildUi.m_specialPieceButton;

    if (!specialButton) {
      specialButton =
          Object.Instantiate(buildUi.m_pieceButtonPrefab, buildUi.m_specialButtonsContainer)
              .GetComponent<BuildUiPieceButton>();

      specialButton.transform.SetAsFirstSibling();
      buildUi.m_specialPieceButton = specialButton;
    }

    specialButton.gameObject.SetActive(true);
    specialButton.Setup(piece, buildUi);
  }

  static string GetSearchTerm(BuildUi buildUi) {
    return buildUi.m_searchField && buildUi.m_searchField.isActiveAndEnabled
        ? buildUi.m_searchField.text
        : string.Empty;
  }

  public static void UpdateSearch(BuildUi buildUi, string term) {
    if (!string.IsNullOrWhiteSpace(term) && buildUi.m_currentTagId != -1) {
      buildUi.SetCurrentTag(-1, false);
      buildUi.FocusTag(-1);
      buildUi.UpdatePieceButtons();
      return;
    }

    ApplySearch(term);
    buildUi.ConfigureButtonNavigation();
  }

  static void ApplySearch(string term) {
    PieceSearchFilter.ApplySearch(term);
    PieceGridView.SetTotalItems(FilteredPieces.Count);
  }

  public static void UpdatePieceSelection(BuildUi buildUi, Piece selectedPiece) {
    if (PieceSearchFilter.TryFindFilteredPieceIndex(selectedPiece, out int index)) {
      PieceGridView.ScrollToItem(index);

      if (PieceGridView.TryGetGridItem(index, out GameObject item)
          && item.TryGetComponent(out BuildUiPieceButton pieceButton)) {
        buildUi.SelectPiece(pieceButton);
        return;
      }
    }

    if (FilteredPieces.Count > 0
        && PieceGridView.TryGetFirstVisibleItem(out GameObject firstItem)
        && firstItem.TryGetComponent(out BuildUiPieceButton firstButton)) {
      buildUi.SelectPiece(firstButton);
      return;
    }

    if (buildUi.m_tagListContainer.gameObject.activeSelf) {
      buildUi.SelectTag(buildUi.m_showAllTagsButton.gameObject);
    }
  }

  public static void FocusPiece(BuildUi buildUi, BuildUiPieceButton button) {
    buildUi.OnHoverPiece(button);

    if (button && PieceSearchFilter.TryFindFilteredPieceIndex(button.Piece, out int index)) {
      PieceGridView.ScrollToItem(index);
    }
  }

  public static void RefreshFavoritePiece(string pieceName) {
    if (PieceSearchFilter.TryFindFilteredPieceIndex(pieceName, out int index)
        && PieceGridView.TryGetGridItem(index, out GameObject item)
        && item.TryGetComponent(out BuildUiPieceButton pieceButton)) {
      pieceButton.RefreshFavorite();
    }
  }

  public static void ConfigureButtonNavigation(BuildUi buildUi) {
    ConfigureGridNavigation(buildUi);

    GridNavigationUtility.ConfigureExplicitNavigation(
        buildUi.m_tagListScrollContent, buildUi.GetTagListEdgeNavigation(), shouldWrapHorizontally: false);

    if (buildUi.m_specialPieceButton && buildUi.m_specialPieceButton.TryGetComponent(out Button specialButton)) {
      specialButton.navigation = buildUi.GetSpecialButtonNavigation();
    }

    ConfigureSearchFieldNavigation(buildUi);
  }

  static void ConfigureGridNavigation(BuildUi buildUi) {
    Selectable upTarget =
        buildUi.m_specialPieceButton && buildUi.m_specialPieceButton.gameObject.activeInHierarchy
            ? buildUi.m_specialPieceButton.GetComponent<Selectable>()
            : buildUi.m_searchField;

    Selectable leftTarget = GetActiveTagSelectable(buildUi);

    PieceGridView.EdgeNavigation =
        new Navigation {
          selectOnLeft = leftTarget,
          selectOnUp = upTarget,
        };

    PieceGridView.UpdateNavigation();
  }

  static void ConfigureSearchFieldNavigation(BuildUi buildUi) {
    Selectable downTarget = GetActiveTagSelectable(buildUi);

    if (!downTarget
        && PieceGridView.TryGetFirstVisibleItem(out GameObject firstItem)
        && firstItem.TryGetComponent(out Selectable firstSelectable)) {
      downTarget = firstSelectable;
    }

    Selectable rightTarget =
        buildUi.m_specialPieceButton && buildUi.m_specialPieceButton.gameObject.activeInHierarchy
            ? buildUi.m_specialPieceButton.GetComponent<Selectable>()
            : default;

    buildUi.m_searchField.navigation =
        new Navigation {
          mode = Navigation.Mode.Explicit,
          selectOnDown = downTarget,
          selectOnRight = rightTarget,
        };
  }

  static Selectable GetActiveTagSelectable(BuildUi buildUi) {
    if (!buildUi.m_tagListContainer.gameObject.activeSelf) {
      return default;
    }

    return buildUi.TryGetTagButtonById(buildUi.m_currentTagId, out BuildUiTagButton tagButton) && tagButton
        ? tagButton.GetComponent<Selectable>()
        : buildUi.m_showAllTagsButton.GetComponent<Selectable>();
  }

  public static GameObject GetFirstPieceButton() {
    if (PieceGridView.TryGetFirstVisibleItem(out GameObject item)) {
      return item;
    }

    return default;
  }

  public static void SelectFirstPiece(BuildUi buildUi) {
    if (PieceGridView.TryGetFirstVisibleItem(out GameObject item)
        && item.TryGetComponent(out BuildUiPieceButton pieceButton)) {
      buildUi.SelectPiece(pieceButton);
    }
  }

  public static void SelectFirstAvailableButton(BuildUi buildUi) {
    if (buildUi.m_tagListContainer.gameObject.activeSelf) {
      buildUi.SelectTag(buildUi.m_showAllTagsButton.gameObject);
      return;
    }

    SelectFirstPiece(buildUi);
  }
}
