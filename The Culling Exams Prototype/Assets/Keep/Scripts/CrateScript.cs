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
        if (tileComponent.Tile.crateState == ObjectState.highlighted)
            tileComponent.Tile.crateState = ObjectState.none;

        if (tileComponent.Tile.crateState == ObjectState.ghostHighlighted)
            tileComponent.Tile.crateState = ObjectState.ghost;

        if (tileComponent.Tile.tileState == ObjectState.highlighted)
            tileComponent.Tile.tileState = ObjectState.none;

        if (tileComponent.Tile.tileState == ObjectState.ghostHighlighted)
            tileComponent.Tile.tileState = ObjectState.ghost;

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
                        tileComponent.Tile.crateState = ObjectState.highlighted;
                        returnValue = true;
                    }

                    if (this.myType == SelectionType.tile)
                    {
                        if (tileComponent.Tile.tileType == TileType.moveable || tileComponent.Tile.tileType == TileType.moveableBarrier)
                        {
                            tileComponent.Tile.tileState = ObjectState.highlighted;
                            returnValue = true;
                        }
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
                        if (tileComponent.Tile.crateState == ObjectState.ghost)
                        {
                            tileComponent.Tile.crateState = ObjectState.ghostHighlighted;
                            returnValue = true;
                        }
                    }

                    if (this.myType == SelectionType.tile)
                    {
                        if (tileComponent.Tile.tileState == ObjectState.ghost)
                        {
                            tileComponent.Tile.tileState = ObjectState.ghostHighlighted;
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
        TileComponent tileComponent = this.transform.parent.GetComponent<TileComponent>();
        TilePlayManager playManager = this.transform.parent.transform.parent.GetComponent<TilePlayManager>();
        SceneManagerSystem SMS = FindObjectOfType<SceneManagerSystem>();

        // Deselect everything nothing is currently highlighted but we have a selection
        if (currentHighlight == null && currentSelection != null)
        {
            if (myType == SelectionType.crate)
            {
                SMS.DeSelectCrate();

                TileComponent sourceCrateTileComponent = currentSelection.transform.parent.GetComponent<TileComponent>();

                sourceCrateTileComponent.Tile.crateState = ObjectState.none;
                sourceCrateTileComponent.Tile.tileState = ObjectState.none;

                ResetGhosts(sourceCrateTileComponent, playManager);
            }
        }

        // We have a valid highlight
        else if (currentHighlight != null)
        {
            // We do not have a current selection so lets make the highlighted object the current selection
            if (currentSelection == null)
            {
                currentSelection = currentHighlight;
                currentHighlight = null;
                selectedTile = new Vector2(tileComponent.Row, tileComponent.Column);
                currentSelectionType = myType;

                SMS.SelectCrate();

                if (myType == SelectionType.crate)
                    tileComponent.Tile.crateState = ObjectState.selected;
                else if (myType == SelectionType.tile)
                    tileComponent.Tile.tileState = ObjectState.selected;

                CreateGhosts(tileComponent, playManager);
            }

            // We have a valid selection and a valid highlight so do the move
            else
            {
                TileComponent sourceCrateTileComponent = currentSelection.transform.parent.GetComponent<TileComponent>();

                if (myType == SelectionType.crate)
                {
                    SMS.DeSelectCrate();

                    tileComponent.Tile.crateType = sourceCrateTileComponent.Tile.crateType;
                    tileComponent.Tile.crateState = ObjectState.none;

                    // This is for when a blue crate lands on a blue tile
                    if (tileComponent.Tile.tileType == TileType.blueTile &&
                        tileComponent.Tile.crateType == CrateType.blueCrate)
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
                        tileComponent.Tile.crateType == CrateType.redCrate)
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

                    // purple crate lands on purple tile
                    if (tileComponent.Tile.tileType == TileType.purpleTile &&
                        tileComponent.Tile.crateType == CrateType.purpleCrate)
                    {
                        // Open all purple Doors
                        for (int row = 0; row < playManager.rows; row++)
                        {
                            for (int column = 0; column < playManager.columns; column++)
                            {
                                TileComponent tile = playManager.allTiles[row, column].GetComponent<TileComponent>();
                                if (tile.Tile.eastWallType == WallType.purpleDoor)
                                    tile.Tile.eastWallType = WallType.purpleDoorOpen;
                                if (tile.Tile.northWallType == WallType.purpleDoor)
                                    tile.Tile.northWallType = WallType.purpleDoorOpen;
                                if (tile.Tile.southWallType == WallType.purpleDoor)
                                    tile.Tile.southWallType = WallType.purpleDoorOpen;
                                if (tile.Tile.westWallType == WallType.purpleDoor)
                                    tile.Tile.westWallType = WallType.purpleDoorOpen;
                            }
                        }
                    }

                    // brown crate lands on brown tile
                    if (tileComponent.Tile.tileType == TileType.brownTile &&
                        tileComponent.Tile.crateType == CrateType.brownCrate)
                    {
                        // Open all brown Doors
                        for (int row = 0; row < playManager.rows; row++)
                        {
                            for (int column = 0; column < playManager.columns; column++)
                            {
                                TileComponent tile = playManager.allTiles[row, column].GetComponent<TileComponent>();
                                if (tile.Tile.eastWallType == WallType.brownDoor)
                                    tile.Tile.eastWallType = WallType.brownDoorOpen;
                                if (tile.Tile.northWallType == WallType.brownDoor)
                                    tile.Tile.northWallType = WallType.brownDoorOpen;
                                if (tile.Tile.southWallType == WallType.brownDoor)
                                    tile.Tile.southWallType = WallType.brownDoorOpen;
                                if (tile.Tile.westWallType == WallType.brownDoor)
                                    tile.Tile.westWallType = WallType.brownDoorOpen;
                            }
                        }
                    }

                    sourceCrateTileComponent.Tile.crateType = CrateType.none;
                    sourceCrateTileComponent.Tile.crateState = ObjectState.none;
                }

                else if (myType == SelectionType.tile)
                {
                    tileComponent.Tile.tileType = sourceCrateTileComponent.Tile.tileType;
                    tileComponent.Tile.tileState = ObjectState.none;

                    sourceCrateTileComponent.Tile.tileType = TileType.fall;
                    sourceCrateTileComponent.Tile.tileState = ObjectState.none;
                }

                ResetGhosts(sourceCrateTileComponent, playManager);
            }
        }
    }
    
    private void CreateGhosts ( TileComponent tileComponent, TilePlayManager playManager)
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

    private void FlipGhost ( TilePlayManager playManager, TileComponent tile)
    {
        // if im standing on the tile do nothing
        if (tile.Row != (int)playManager.currentTile.x || tile.Column != (int)playManager.currentTile.y)
        {
            if (myType == SelectionType.crate)
            {
                // make sure there is no crate and is a valid place to put it
                if (tile.Tile.crateType == CrateType.none
                   &&
                   ( tile.Tile.tileType == TileType.basic
                    || tile.Tile.tileType == TileType.redTile
                    || tile.Tile.tileType == TileType.blueTile
                    || tile.Tile.tileType == TileType.brownTile
                    || tile.Tile.tileType == TileType.purpleTile ))
                {
                    tile.Tile.crateType = CrateType.crate;
                    tile.Tile.crateState = ObjectState.ghost;
                }
            }
            else if (myType == SelectionType.tile && tile.Tile.tileType == TileType.fall)
                tile.Tile.tileState = ObjectState.ghost;
        }
    }

    private void ResetGhosts ( TileComponent tileComponent, TilePlayManager playManager)
    {
        selectedTile = null;
        currentSelection = null;
        currentSelectionType = SelectionType.none;
        
            TileComponent tile = playManager.allTiles[tileComponent.Row - 1, tileComponent.Column].GetComponent<TileComponent>();
            if (tile.Tile.crateState == ObjectState.ghost && tile.Tile.crateType == CrateType.crate)
            {
                tile.Tile.crateType = CrateType.none;
                tile.Tile.crateState = ObjectState.none;
            }

            if (tile.Tile.tileState == ObjectState.ghost && tile.Tile.tileType == TileType.fall)
                tile.Tile.tileState = ObjectState.none;

            tile = playManager.allTiles[tileComponent.Row + 1, tileComponent.Column].GetComponent<TileComponent>();
            if (tile.Tile.crateState == ObjectState.ghost && tile.Tile.crateType == CrateType.crate)
            {
                tile.Tile.crateType = CrateType.none;
                tile.Tile.crateState = ObjectState.none;
            }

            if (tile.Tile.tileState == ObjectState.ghost && tile.Tile.tileType == TileType.fall)
                tile.Tile.tileState = ObjectState.none;

            tile = playManager.allTiles[tileComponent.Row, tileComponent.Column + 1].GetComponent<TileComponent>();
            if (tile.Tile.crateState == ObjectState.ghost && tile.Tile.crateType == CrateType.crate)
            {
                tile.Tile.crateType = CrateType.none;
                tile.Tile.crateState = ObjectState.none;
            }

            if (tile.Tile.tileState == ObjectState.ghost && tile.Tile.tileType == TileType.fall)
                tile.Tile.tileState = ObjectState.none;

            tile = playManager.allTiles[tileComponent.Row, tileComponent.Column - 1].GetComponent<TileComponent>();
            if (tile.Tile.crateState == ObjectState.ghost && tile.Tile.crateType == CrateType.crate)
            {
                tile.Tile.crateType = CrateType.none;
                tile.Tile.crateState = ObjectState.none;
            }

            if (tile.Tile.tileState == ObjectState.ghost && tile.Tile.tileType == TileType.fall)
                tile.Tile.tileState = ObjectState.none;
    }
}