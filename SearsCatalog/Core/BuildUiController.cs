namespace SearsCatalog;

using System.Collections.Generic;

using ComfyLib;

using UnityEngine;
using UnityEngine.EventSystems;

using static PluginConfig;

public static class BuildUiController {
  public static RectTransform SelectionWindow { get; private set; }
  public static PanelDragger WindowDragger { get; private set; }
  public static PanelResizer WindowResizer { get; private set; }

  public static PooledGridView PieceGridView { get; private set; }
  public static List<Piece> CurrentPieces { get; } = [];
  public static List<Piece> FilteredPieces { get; } = [];

  static readonly Dictionary<string, List<string>> _cachedSearchTerms = [];

  public static void SetupBuildUi(BuildUi buildUi) {
    RectTransform selectionWindow = (RectTransform) buildUi.transform.Find("bar/SelectionWindow");

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

    buildUi.m_tagListContainer
        .SetAnchorMin(Vector2.zero)
        .SetAnchorMax(Vector2.up)
        .SetPivot(Vector2.zero)
        .SetPosition(Vector2.zero);

    ((RectTransform) buildUi.m_tagListContainer.Find("LayoutGroup"))
        .SetAnchorMin(Vector2.up)
        .SetAnchorMax(Vector2.up)
        .SetPivot(Vector2.up)
        .SetPosition(Vector2.zero);

    SelectionWindow = selectionWindow;
    WindowDragger = panelDragger;
    WindowResizer = panelResizer;

    PieceGridView = new PooledGridView(buildUi.m_pieceScrollRect);
    PieceGridView.Setup(
        parent => {
          GameObject buttonObject = Object.Instantiate(buildUi.m_pieceButtonPrefab, parent);
          return buttonObject.GetComponent<RectTransform>();
        },
        (index, item) => {
          if (index >= 0 && index < FilteredPieces.Count) {
            BuildUiPieceButton button = item.GetComponent<BuildUiPieceButton>();
            button.Setup(FilteredPieces[index], buildUi);
          }
        });
  }

  static void HandlePanelDragEnd(Vector2 position) {
    BuildUiPanelPosition.Value = position;
  }

  static void HandlePanelResizeEnd(Vector2 sizeDelta) {
    BuildUiPanelSizeDelta.Value = sizeDelta;

    PieceGridView?.UpdatePoolSize();
    PieceGridView?.Refresh();

    Hud.m_instance.m_buildUi.ConfigureButtonNavigation();
  }

  public static void DestroyBuildUi(BuildUi buildUi) {
    SelectionWindow = default;
    WindowDragger = default;
    WindowResizer = default;
    PieceGridView = default;
    CurrentPieces.Clear();
    FilteredPieces.Clear();
    _cachedSearchTerms.Clear();
  }

  public static void ShowBuildUi(BuildUi buildUi) {
    if (!SelectionWindow) {
      return;
    }

    if (IsModEnabled.Value) {
      SelectionWindow
          .SetSizeDelta(BuildUiPanelSizeDelta.Value)
          .SetPosition(BuildUiPanelPosition.Value);

      WindowDragger.enabled = BuildUiCanMovePanel.Value;
      WindowResizer.enabled = BuildUiCanResizePanel.Value;

      PieceGridView?.UpdatePoolSize();
      PieceGridView?.Refresh();
    } else {
      SelectionWindow
          .SetSizeDelta(Vector2.zero)
          .SetPosition(Vector2.zero);

      WindowDragger.enabled = false;
      WindowResizer.enabled = false;
    }
  }

  public static void UpdatePieceButtons(BuildUi buildUi) {
    buildUi.m_availablePieceCount = buildUi.m_currentBuildTool.m_availablePieces.Count;
    bool hasSpecial = false;
    IPieceList pieceList = buildUi.m_pieceLists[buildUi.m_currentPieceList];

    if (buildUi.m_specialPieceButton != null) {
      Object.Destroy(buildUi.m_specialPieceButton.gameObject);
      buildUi.m_specialPieceButton = null;
    }

    buildUi.m_tempPieces.Clear();
    pieceList.GetAvailablePiecesWithTag(buildUi.m_currentTagId, buildUi.m_currentBuildTool, buildUi.m_tempPieces);

    CurrentPieces.Clear();

    for (int i = 0; i < buildUi.m_tempPieces.Count; i++) {
      Piece piece = buildUi.m_tempPieces[i];

      if (piece.m_repairPiece || piece.m_removePiece) {
        if (!pieceList.ShowTags) {
          continue;
        }

        hasSpecial = true;
        BuildUiPieceButton specialButton =
            Object.Instantiate(buildUi.m_pieceButtonPrefab, buildUi.m_specialButtonsContainer)
                .GetComponent<BuildUiPieceButton>();
        specialButton.transform.SetAsFirstSibling();
        buildUi.m_specialPieceButton = specialButton;
        specialButton.gameObject.SetActive(true);
        specialButton.Setup(piece, buildUi);
      } else {
        CurrentPieces.Add(piece);
      }
    }

    buildUi.m_tempPieces.Clear();

    string searchTerm =
        buildUi.m_searchField != null && buildUi.m_searchField.isActiveAndEnabled
            ? buildUi.m_searchField.text
            : string.Empty;

    ApplySearch(searchTerm);

    buildUi.m_specialButtonsContainer.gameObject.SetActive(hasSpecial);
    buildUi.ConfigureButtonNavigation();
  }

  public static void UpdateSearch(BuildUi buildUi, string term) {
    term = term.Replace(" ", "").ToLowerInvariant();

    if (!string.IsNullOrEmpty(term) && buildUi.m_currentTagId != -1) {
      buildUi.SetCurrentTag(-1, false);
      buildUi.UpdatePieceButtons();
      return;
    }

    ApplySearch(term);
    buildUi.ConfigureButtonNavigation();
  }

  static void ApplySearch(string term) {
    term = term.Replace(" ", "").ToLowerInvariant();

    FilteredPieces.Clear();

    if (string.IsNullOrEmpty(term)) {
      FilteredPieces.AddRange(CurrentPieces);
    } else {
      for (int i = 0; i < CurrentPieces.Count; i++) {
        Piece piece = CurrentPieces[i];
        if (MatchesSearch(piece, term)) {
          FilteredPieces.Add(piece);
        }
      }
    }

    PieceGridView?.SetTotalItems(FilteredPieces.Count);
  }

  static bool MatchesSearch(Piece piece, string term) {
    if (!_cachedSearchTerms.TryGetValue(piece.gameObject.name, out List<string> terms)) {
      terms = [Localization.instance.Localize(piece.m_name).ToLowerInvariant().Replace(" ", "")];

      if (piece.m_resources != null) {
        for (int i = 0; i < piece.m_resources.Length; i++) {
          Piece.Requirement req = piece.m_resources[i];
          if (req != null && req.m_resItem != null && req.m_resItem.m_itemData != null) {
            terms.Add(Localization.instance.Localize(req.m_resItem.m_itemData.m_shared.m_name).ToLowerInvariant().Replace(" ", ""));
          }
        }
      }

      _cachedSearchTerms[piece.gameObject.name] = terms;
    }

    for (int i = 0; i < terms.Count; i++) {
      if (terms[i].Contains(term)) {
        return true;
      }
    }

    return false;
  }

  public static void UpdatePieceSelection(BuildUi buildUi, Piece selectedPiece) {
    bool found = false;

    if (selectedPiece != null) {
      int index = FilteredPieces.FindIndex(p => p.gameObject.name == selectedPiece.gameObject.name);
      if (index >= 0) {
        PieceGridView?.ScrollToItem(index);
        GameObject buttonObject = PieceGridView?.GetItemGameObject(index);
        if (buttonObject != null) {
          EventSystem.current.SetSelectedGameObject(buttonObject);
          found = true;
        }
      }
    }

    if (!found && buildUi.m_pieceButtonsContainer.GetActiveChildCount() > 0) {
      EventSystem.current.SetSelectedGameObject(buildUi.m_pieceButtonsContainer.GetActiveChild(0).gameObject);
      found = true;
    }

    if (!found && buildUi.m_tagListContainer.gameObject.activeSelf) {
      EventSystem.current.SetSelectedGameObject(buildUi.m_showAllTagsButton.gameObject);
    }
  }
}
