using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateScript : MonoBehaviour
{
    public SelectionType myType;

    static GameObject currentSelection = null;
    static GameObject currentHighlight = null;

    static SelectionType currentSelectionType = SelectionType.none;
    static Vector2? selectedTile = null;

    void Start ()
    {
    }

    public void Highlight ( Collider collider )
    {
        // If there is a current highlight remove it
        if (currentHighlight != collider.gameObject)
        {
            if (currentHighlight != null)
            {
                CrateScript currentCrateScript = currentHighlight.GetComponent<CrateScript>();
                if (currentCrateScript != null)
                {
                    currentCrateScript.SetHighLight(false);
                    currentHighlight = null;
                }
            }

            if (SetHighLight(true))
            {
                currentHighlight = collider.gameObject;
            }
        }
    }

    public bool SetHighLight ( bool highlight )
    {
        bool returnValue = false;

        TileComponent tileComponent = this.transform.parent.GetComponent<TileComponent>();
        TilePlayManager playManager = this.transform.parent.transform.parent.GetComponent<TilePlayManager>();

        // Do nothing Ever if im standing on the tile
        if (tileComponent.Row == playManager.currentTile.x && playManager.currentTile.y == tileComponent.Column)
            return returnValue;

        // Deselect everything
        if (tileComponent.Tile.crateType == CrateType.crateHighlighted)
            tileComponent.Tile.crateType = CrateType.crate;
        if (tileComponent.Tile.crateType == CrateType.crateGhostHighlighted)
            tileComponent.Tile.crateType = CrateType.crateGhost;

        if (tileComponent.Tile.crateType == CrateType.mirrorHighlighted)
            tileComponent.Tile.crateType = CrateType.mirror;
        if (tileComponent.Tile.crateType == CrateType.mirrorGhostHighlighted)
            tileComponent.Tile.crateType = CrateType.mirrorGhost;

        if (tileComponent.Tile.tileType == TileType.moveableHighlighted)
            tileComponent.Tile.tileType = TileType.moveable;
        if (tileComponent.Tile.tileType == TileType.fallHighlighted)
            tileComponent.Tile.tileType = TileType.fallGhost;

        if (highlight)
        {
            // If I have a selection then I can only select on thoes types
            if (selectedTile != null && currentSelectionType != this.myType)
                return returnValue;

            // Highlight Ranges Nothing Selected
            if (selectedTile == null)
            {
                if (tileComponent.Row >= playManager.currentTile.x - 1 && playManager.currentTile.x + 1 >= tileComponent.Row
                    && tileComponent.Column >= playManager.currentTile.y - 1 && playManager.currentTile.y + 1 >= tileComponent.Column)
                {
                    if (this.myType == SelectionType.crate)
                    {
                        tileComponent.Tile.crateType = CrateType.crateHighlighted;
                        currentSelectionType = SelectionType.crate;
                        returnValue = true;
                    }

                    if (this.myType == SelectionType.tile)
                    {
                        tileComponent.Tile.tileType = TileType.moveableHighlighted;
                        currentSelectionType = SelectionType.tile;
                        returnValue = true;
                    }
                }
            }
            else if (selectedTile != null)
            {
                // You cannot highlight the currently selected tile
                if (tileComponent.Row == selectedTile.Value.x && selectedTile.Value.y == tileComponent.Column)
                    return returnValue;

                if (tileComponent.Row >= selectedTile.Value.x - 1 && selectedTile.Value.x + 1 >= tileComponent.Row
                    && tileComponent.Column >= selectedTile.Value.y - 1 && selectedTile.Value.y + 1 >= tileComponent.Column)
                {
                    if (this.myType == SelectionType.crate)
                    {
                        if (tileComponent.Tile.crateType == CrateType.crateGhost)
                        {
                            tileComponent.Tile.crateType = CrateType.crateGhostHighlighted;
                            currentSelectionType = SelectionType.crate;
                            returnValue = true;
                        }
                    }
                    else if (this.myType == SelectionType.tile)
                    {
                        if (tileComponent.Tile.tileType == TileType.fallGhost)
                        {
                            tileComponent.Tile.tileType = TileType.fallHighlighted;
                            currentSelectionType = SelectionType.tile;
                            returnValue = true;
                        }
                    }
                }
            }
        }

        return returnValue;
    }

    public void Select ( Collider collider )
    {
        SetSelect(collider);
    }

    public void SetSelect ( Collider collider )
    {
        TileComponent tileComponent = this.transform.parent.GetComponent<TileComponent>();
        TilePlayManager playManager = this.transform.parent.transform.parent.GetComponent<TilePlayManager>();

        // Deselect everything
        if (currentHighlight == null && currentSelection != null)
        {
            tileComponent.Tile.crateType = CrateType.crate;
            tileComponent.Tile.tileType = TileType.basic;

            ResetSelection(tileComponent, playManager);
        }
        else if (currentHighlight != null)
        {
            if (currentSelection == null)
            {
                currentSelection = collider.gameObject;
                selectedTile = new Vector2(tileComponent.Row, tileComponent.Column);

                currentSelectionType = myType;

                if (myType == SelectionType.crate)
                {
                    // Flip myself to selected
                    tileComponent.Tile.crateType = CrateType.crateSelected;

                    // check all four directions for empty slots
                    // Flip all surrounding tiles to ghosts
                    TileComponent tile = playManager.allTiles[tileComponent.Row - 1, tileComponent.Column].GetComponent<TileComponent>();
                    if (tile.Row != playManager.currentTile.x && tile.Column != playManager.currentTile.y &&
                        tile.Tile.crateType == CrateType.none && tile.Tile.tileType == TileType.basic)
                        tile.Tile.crateType = CrateType.crateGhost;

                    tile = playManager.allTiles[tileComponent.Row + 1, tileComponent.Column].GetComponent<TileComponent>();
                    if (tile.Row != playManager.currentTile.x && tile.Column != playManager.currentTile.y &&
                        tile.Tile.crateType == CrateType.none && tile.Tile.tileType == TileType.basic)
                        tile.Tile.crateType = CrateType.crateGhost;

                    tile = playManager.allTiles[tileComponent.Row, tileComponent.Column + 1].GetComponent<TileComponent>();
                    if (tile.Row != playManager.currentTile.x && tile.Column != playManager.currentTile.y &&
                        tile.Tile.crateType == CrateType.none && tile.Tile.tileType == TileType.basic)
                        tile.Tile.crateType = CrateType.crateGhost;

                    tile = playManager.allTiles[tileComponent.Row, tileComponent.Column - 1].GetComponent<TileComponent>();
                    if (tile.Row != playManager.currentTile.x && tile.Column != playManager.currentTile.y &&
                        tile.Tile.crateType == CrateType.none && tile.Tile.tileType == TileType.basic)
                        tile.Tile.crateType = CrateType.crateGhost;
                }
                else if (myType == SelectionType.tile)
                {
                    // TODO: flip all empty tiles to ghosts 
                }
            }
            else
            {
                TileComponent sourceCrateTileComponent = currentSelection.transform.parent.GetComponent<TileComponent>();
                
                if (myType == SelectionType.crate)
                {
                    tileComponent.Tile.crateType = CrateType.crate;
                    sourceCrateTileComponent.Tile.crateType = CrateType.none;
                }
                else if (myType == SelectionType.tile)
                {

                }

                ResetSelection(tileComponent, playManager);
            }
        }
    }

    private static void ResetSelection ( TileComponent tileComponent, TilePlayManager playManager )
    {
        selectedTile = null;
        currentSelection = null;
        currentSelectionType = SelectionType.none;

        TileComponent tile = playManager.allTiles[tileComponent.Row - 1, tileComponent.Column].GetComponent<TileComponent>();
        if (tile.Tile.crateType == CrateType.crateGhost)
            tile.Tile.crateType = CrateType.none;
        if (tile.Tile.tileType == TileType.fallGhost)
            tile.Tile.tileType = TileType.fall;

        tile = playManager.allTiles[tileComponent.Row + 1, tileComponent.Column].GetComponent<TileComponent>();
        if (tile.Tile.crateType == CrateType.crateGhost)
            tile.Tile.crateType = CrateType.none;
        if (tile.Tile.tileType == TileType.fallGhost)
            tile.Tile.tileType = TileType.fall;

        tile = playManager.allTiles[tileComponent.Row, tileComponent.Column + 1].GetComponent<TileComponent>();
        if (tile.Tile.crateType == CrateType.crateGhost)
            tile.Tile.crateType = CrateType.none;
        if (tile.Tile.tileType == TileType.fallGhost)
            tile.Tile.tileType = TileType.fall;

        tile = playManager.allTiles[tileComponent.Row, tileComponent.Column - 1].GetComponent<TileComponent>();
        if (tile.Tile.crateType == CrateType.crateGhost)
            tile.Tile.crateType = CrateType.none;
        if (tile.Tile.tileType == TileType.fallGhost)
            tile.Tile.tileType = TileType.fall;
    }
}

