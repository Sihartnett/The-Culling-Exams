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

    void Start()
    {
    }

    public void Highlight(Collider collider)
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

    public bool SetHighLight(bool highlight)
    {
        bool returnValue = false;

        TileComponent tileComponent = this.transform.parent.GetComponent<TileComponent>();
        TilePlayManager playManager = this.transform.parent.transform.parent.GetComponent<TilePlayManager>();

        // Do nothing Ever if im standing on the tile
        if (tileComponent.Row == playManager.currentTile.x && playManager.currentTile.y == tileComponent.Column)
            return returnValue;

        if (tileComponent.Tile.crateType != CrateType.none)
        {
            // Deselect everything
            if (tileComponent.Tile.crateState == CrateState.highlighted)
                tileComponent.Tile.crateState = CrateState.none;

            if (tileComponent.Tile.crateState == CrateState.ghostHighlighted)
                tileComponent.Tile.crateState = CrateState.ghost;

            // Tile Highlights
            //if (tileComponent.Tile.tileType == TileType.moveableHighlighted)
            //    tileComponent.Tile.tileType = TileType.moveable;
            //if (tileComponent.Tile.tileType == TileType.fallHighlighted)
            //    tileComponent.Tile.tileType = TileType.fallGhost;
        }

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
                        tileComponent.Tile.crateState = CrateState.highlighted;
                        returnValue = true;
                    }

                    // Tile Movement
                    //if (this.myType == SelectionType.tile)
                    //{
                    //    tileComponent.Tile.tileType = TileType.moveableHighlighted;
                    //    currentSelectionType = SelectionType.tile;
                    //    returnValue = true;
                    //}
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
                        if (tileComponent.Tile.crateState == CrateState.ghost)
                        {
                            tileComponent.Tile.crateState = CrateState.ghostHighlighted;
                            returnValue = true;
                        }
                    }

                    // Tile stuff
                    //if (this.myType == SelectionType.tile)
                    //{
                    //    if (tileComponent.Tile.tileType == TileType.fallGhost)
                    //    {
                    //        tileComponent.Tile.tileType = TileType.fallHighlighted;
                    //        currentSelectionType = SelectionType.tile;
                    //        returnValue = true;
                    //    }
                    //}
                }
            }
        }

        return returnValue;
    }

    public void Select(Collider collider)
    {
        TileComponent tileComponent = this.transform.parent.GetComponent<TileComponent>();
        TilePlayManager playManager = this.transform.parent.transform.parent.GetComponent<TilePlayManager>();
        SceneManagerSystem SMS = FindObjectOfType<SceneManagerSystem>();

        // Deselect everything nothing is currently highlighted so deselect
        if (currentHighlight == null && currentSelection != null)
        {
            if (myType == SelectionType.crate)
            {
                SMS.DeSelectCrate();

                TileComponent sourceCrateTileComponent = currentSelection.transform.parent.GetComponent<TileComponent>();
                

                sourceCrateTileComponent.Tile.crateState = CrateState.none;
                ResetGhosts(sourceCrateTileComponent, playManager);
            }
        }

        // We have a valid highlighted object
        else if (currentHighlight != null)
        {
            // We do not have a current selection so lets make the highlighted object the current selection
            if (currentSelection == null)
            {
                currentSelection = currentHighlight;
                currentHighlight = null;
                selectedTile = new Vector2(tileComponent.Row, tileComponent.Column);
                currentSelectionType = myType;

                // am i a crate selection or a tile
                if (myType == SelectionType.crate)
                {
                    SMS.SelectCrate();

                    // Flip myself to selected
                    tileComponent.Tile.crateState = CrateState.selected;
                    
                    // Get the tile component of 
                    
                    CreateGhosts(tileComponent, playManager);
                }
            }
            
            else
            {
                TileComponent sourceCrateTileComponent = currentSelection.transform.parent.GetComponent<TileComponent>();

                if (myType == SelectionType.crate)
                {
                    SMS.DeSelectCrate();

                    tileComponent.Tile.crateType = sourceCrateTileComponent.Tile.crateType;
                    tileComponent.Tile.crateState = CrateState.none;

                    // This is for when a blue crate lands on a blue tile
                    if (tileComponent.Tile.tileType == TileType.blueTile &&
                        tileComponent.Tile.crateType == CrateType.bluecrate)
                    {
                        // Open all blue Doors
                        for (int row = 0; row < playManager.rows; row++)
                        {
                            for (int column = 0; column < playManager.columns; column++)
                            {
                                TileComponent tile = playManager.allTiles[row, column].GetComponent<TileComponent>();
                                if (tile.Tile.eastWallType == WallType.blueDoor)
                                    tile.Tile.eastWallType = WallType.blueDoorOpen;
                                if (tile.Tile.northWallType == WallType.blueDoor)
                                    tile.Tile.northWallType = WallType.blueDoorOpen;
                                if (tile.Tile.southWallType == WallType.blueDoor)
                                    tile.Tile.southWallType = WallType.blueDoorOpen;
                                if (tile.Tile.westWallType == WallType.blueDoor)
                                    tile.Tile.westWallType = WallType.blueDoorOpen;
                            }
                        }
                    }

                    // Red crate lands on red tile
                    if (tileComponent.Tile.tileType == TileType.redTile &&
                        tileComponent.Tile.crateType == CrateType.redcrate)
                    {
                        // Open all red Doors
                        for (int row = 0; row < playManager.rows; row++)
                        {
                            for (int column = 0; column < playManager.columns; column++)
                            {
                                TileComponent tile = playManager.allTiles[row, column].GetComponent<TileComponent>();
                                if (tile.Tile.eastWallType == WallType.redDoor)
                                    tile.Tile.eastWallType = WallType.redDoorOpen;
                                if (tile.Tile.northWallType == WallType.redDoor)
                                    tile.Tile.northWallType = WallType.redDoorOpen;
                                if (tile.Tile.southWallType == WallType.redDoor)
                                    tile.Tile.southWallType = WallType.redDoorOpen;
                                if (tile.Tile.westWallType == WallType.redDoor)
                                    tile.Tile.westWallType = WallType.redDoorOpen;
                            }
                        }
                    }

                    // TODO: Red tile has Red crate moved off of it
                    // TODO: Blue tile has Blue crate moved off of it
                    
                    sourceCrateTileComponent.Tile.crateType = CrateType.none;
                    sourceCrateTileComponent.Tile.crateState = CrateState.none;
                }

                ResetGhosts(sourceCrateTileComponent, playManager);
            }
        }
    }

    private void CreateGhosts ( TileComponent tileComponent, TilePlayManager playManager)
    {
        if (myType == SelectionType.crate)
        {
            TileComponent tile = playManager.allTiles[tileComponent.Row - 1, tileComponent.Column].GetComponent<TileComponent>();
            FlipGhost(playManager, tile);

            tile = playManager.allTiles[tileComponent.Row + 1, tileComponent.Column].GetComponent<TileComponent>();
            FlipGhost(playManager, tile);

            tile = playManager.allTiles[tileComponent.Row, tileComponent.Column + 1].GetComponent<TileComponent>();
            FlipGhost(playManager, tile);

            tile = playManager.allTiles[tileComponent.Row, tileComponent.Column - 1].GetComponent<TileComponent>();
            FlipGhost(playManager, tile);
        }
    }

    private static void FlipGhost ( TilePlayManager playManager, TileComponent tile )
    {
        if (!( tile.Row == (int)playManager.currentTile.x && tile.Column == (int)playManager.currentTile.y ) &&
            tile.Tile.crateType == CrateType.none &&
            (tile.Tile.tileType == TileType.basic || tile.Tile.tileType == TileType.redTile || tile.Tile.tileType == TileType.blueTile ))
        {
            tile.Tile.crateType = CrateType.crate;
            tile.Tile.crateState = CrateState.ghost;
        }
    }

    private void ResetGhosts(TileComponent tileComponent, TilePlayManager playManager)
    {
        selectedTile = null;
        currentSelection = null;
        currentSelectionType = SelectionType.none;
        
        if (myType == SelectionType.crate)
        {
            TileComponent tile = playManager.allTiles[tileComponent.Row - 1, tileComponent.Column].GetComponent<TileComponent>();
            if (tile.Tile.crateState == CrateState.ghost)
            {
                if (tile.Tile.crateType == CrateType.crate)
                {
                    tile.Tile.crateType = CrateType.none;
                    tile.Tile.crateState = CrateState.none;
                }
            }

            tile = playManager.allTiles[tileComponent.Row + 1, tileComponent.Column].GetComponent<TileComponent>();
            if (tile.Tile.crateState == CrateState.ghost)
            {
                if (tile.Tile.crateType == CrateType.crate)
                {
                    tile.Tile.crateType = CrateType.none;
                    tile.Tile.crateState = CrateState.none;
                }
            }

            tile = playManager.allTiles[tileComponent.Row, tileComponent.Column + 1].GetComponent<TileComponent>();
            if (tile.Tile.crateState == CrateState.ghost)
            {
                if (tile.Tile.crateType == CrateType.crate)
                {
                    tile.Tile.crateType = CrateType.none;
                    tile.Tile.crateState = CrateState.none;
                }
            }

            tile = playManager.allTiles[tileComponent.Row, tileComponent.Column - 1].GetComponent<TileComponent>();
            if (tile.Tile.crateState == CrateState.ghost)
            {
                if (tile.Tile.crateType == CrateType.crate)
                {
                    tile.Tile.crateType = CrateType.none;
                    tile.Tile.crateState = CrateState.none;
                }
            }
        }
    }
}