using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateScript : MonoBehaviour
{
    public SelectionType myType;

    public static GameObject CurrentSelection = null;
    public static GameObject CurrentHighlight = null;

    static SelectionType currentSelectionType = SelectionType.none;
    static Vector2? selectedTile = null;

    void Start ()
    {
    }

    public void Highlight ( Collider collider )
    {
        // If there is a current highlight remove it
        if (CurrentHighlight != collider.gameObject)
        {
            if (CurrentHighlight != null)
            {
                CrateScript currentCrateScript = CurrentHighlight.GetComponent<CrateScript>();
                if (currentCrateScript != null)
                {
                    currentCrateScript.SetHighLight(false);
                    CurrentHighlight = null;
                }
            }

            if (SetHighLight(true))
            {
                CurrentHighlight = collider.gameObject;
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
            playManager.SetCrate(tileComponent.Row, tileComponent.Column, tileComponent.Tile.crateType, ObjectState.none);

        if (tileComponent.Tile.crateState == ObjectState.ghostHighlighted)
            playManager.SetCrate(tileComponent.Row, tileComponent.Column, tileComponent.Tile.crateType, ObjectState.ghost);

        if (tileComponent.Tile.tileState == ObjectState.highlighted)
            playManager.SetTile(tileComponent.Row, tileComponent.Column, tileComponent.Tile.tileType, ObjectState.none);

        if (tileComponent.Tile.tileState == ObjectState.ghostHighlighted)
            playManager.SetTile(tileComponent.Row, tileComponent.Column, tileComponent.Tile.tileType, ObjectState.ghost);

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
                        playManager.SetCrate(tileComponent.Row, tileComponent.Column, tileComponent.Tile.crateType, ObjectState.highlighted);
                        returnValue = true;
                    }

                    if (this.myType == SelectionType.tile)
                    {
                        if (tileComponent.Tile.tileType == TileType.moveableTile 
                            || tileComponent.Tile.tileType == TileType.moveableBarrierTile 
                            || tileComponent.Tile.tileType == TileType.moveableCrateTile)
                        {
                            playManager.SetTile(tileComponent.Row, tileComponent.Column, tileComponent.Tile.tileType, ObjectState.highlighted);
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
                            playManager.SetCrate(tileComponent.Row, tileComponent.Column, tileComponent.Tile.crateType, ObjectState.ghostHighlighted);
                            returnValue = true;
                        }
                    }

                    if (this.myType == SelectionType.tile)
                    {
                        if (tileComponent.Tile.tileState == ObjectState.ghost)
                        {
                            playManager.SetTile(tileComponent.Row, tileComponent.Column, tileComponent.Tile.tileType, ObjectState.ghostHighlighted);
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
        if (CurrentHighlight == null && CurrentSelection != null)
        {
            SMS.DeSelectCrate();

            TileComponent sourceCrateTileComponent = CurrentSelection.transform.parent.GetComponent<TileComponent>();

            playManager.SetCrate(tileComponent.Row, tileComponent.Column, tileComponent.Tile.crateType, ObjectState.none);
            playManager.SetTile(tileComponent.Row, tileComponent.Column, tileComponent.Tile.tileType, ObjectState.none);

            ResetGhosts(sourceCrateTileComponent, playManager);
        }

        // We have a valid highlight
        else if (CurrentHighlight != null)
        {
            // We do not have a current selection so lets make the highlighted object the current selection
            if (CurrentSelection == null)
            {
                CurrentSelection = CurrentHighlight;
                CurrentHighlight = null;
                selectedTile = new Vector2(tileComponent.Row, tileComponent.Column);
                currentSelectionType = myType;

                SMS.SelectCrate();

                if (myType == SelectionType.crate)
                    playManager.SetCrate(tileComponent.Row, tileComponent.Column, tileComponent.Tile.crateType, ObjectState.selected);
                else if (myType == SelectionType.tile)
                    playManager.SetTile(tileComponent.Row, tileComponent.Column, tileComponent.Tile.tileType, ObjectState.selected);

                CreateGhosts(tileComponent, playManager);
            }

            // We have a valid selection and a valid highlight so do the move
            else
            {
                TileComponent sourceCrateTileComponent = CurrentSelection.transform.parent.GetComponent<TileComponent>();

                if (myType == SelectionType.crate)
                {
                    SMS.DeSelectCrate();

                    playManager.SetCrate(tileComponent.Row, tileComponent.Column, sourceCrateTileComponent.Tile.crateType, ObjectState.highlighted);
                    playManager.SetCrate(sourceCrateTileComponent.Row, sourceCrateTileComponent.Column, CrateType.none, ObjectState.none);

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
                                    playManager.SetEastWall(tile.Row, tile.Column, WallType.blueDoorOpen);
                                if (tile.Tile.northWallType == WallType.blueDoor)
                                    playManager.SetNorthWall(tile.Row, tile.Column, WallType.blueDoorOpen);
                                if (tile.Tile.southWallType == WallType.blueDoor)
                                    playManager.SetSouthWall(tile.Row, tile.Column, WallType.blueDoorOpen);
                                if (tile.Tile.westWallType == WallType.blueDoor)
                                    playManager.SetWestWall(tile.Row, tile.Column, WallType.blueDoorOpen);
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
                                    playManager.SetEastWall(tile.Row, tile.Column, WallType.redDoorOpen);
                                if (tile.Tile.northWallType == WallType.redDoor)
                                    playManager.SetNorthWall(tile.Row, tile.Column, WallType.redDoorOpen);
                                if (tile.Tile.southWallType == WallType.redDoor)
                                    playManager.SetSouthWall(tile.Row, tile.Column, WallType.redDoorOpen);
                                if (tile.Tile.westWallType == WallType.redDoor)
                                    playManager.SetWestWall(tile.Row, tile.Column, WallType.redDoorOpen);
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
                                    playManager.SetEastWall(tile.Row, tile.Column, WallType.purpleDoorOpen);
                                if (tile.Tile.northWallType == WallType.purpleDoor)
                                    playManager.SetNorthWall(tile.Row, tile.Column, WallType.purpleDoorOpen);
                                if (tile.Tile.southWallType == WallType.purpleDoor)
                                    playManager.SetSouthWall(tile.Row, tile.Column, WallType.purpleDoorOpen);
                                if (tile.Tile.westWallType == WallType.purpleDoor)
                                    playManager.SetWestWall(tile.Row, tile.Column, WallType.purpleDoorOpen);
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
                                    playManager.SetEastWall(tile.Row, tile.Column, WallType.brownDoorOpen);
                                if (tile.Tile.northWallType == WallType.brownDoor)
                                    playManager.SetNorthWall(tile.Row, tile.Column, WallType.brownDoorOpen);
                                if (tile.Tile.southWallType == WallType.brownDoor)
                                    playManager.SetSouthWall(tile.Row, tile.Column, WallType.brownDoorOpen);
                                if (tile.Tile.westWallType == WallType.brownDoor)
                                    playManager.SetWestWall(tile.Row, tile.Column, WallType.brownDoorOpen);
                            }
                        }
                    }
                }

                else if (myType == SelectionType.tile)
                {
                    playManager.SetTile(tileComponent.Row, tileComponent.Column, sourceCrateTileComponent.Tile.tileType, ObjectState.none);
                    playManager.SetCrate(tileComponent.Row, tileComponent.Column, sourceCrateTileComponent.Tile.crateType, ObjectState.none);

                    playManager.SetTile(sourceCrateTileComponent.Row, sourceCrateTileComponent.Column, TileType.fallTile, ObjectState.none);
                    playManager.SetCrate(sourceCrateTileComponent.Row, sourceCrateTileComponent.Column, CrateType.none, ObjectState.none);
                }

                ResetGhosts(sourceCrateTileComponent, playManager);
            }
        }
    }

    public void CreateGhosts ( TileComponent tileComponent, TilePlayManager playManager )
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

    private void FlipGhost ( TilePlayManager playManager, TileComponent tile )
    {
        // if im standing on the tile do nothing
        if (tile.Row != (int)playManager.currentTile.x || tile.Column != (int)playManager.currentTile.y)
        {
            if (myType == SelectionType.crate)
            {
                // make sure there is no crate and is a valid place to put it
                if (tile.Tile.crateType == CrateType.none
                   &&
                   ( tile.Tile.tileType == TileType.basicTile
                    || tile.Tile.tileType == TileType.redTile
                    || tile.Tile.tileType == TileType.blueTile
                    || tile.Tile.tileType == TileType.brownTile
                    || tile.Tile.tileType == TileType.purpleTile
                    || tile.Tile.tileType == TileType.moveableCrateTile ))
                {
                    playManager.SetCrate(tile.Row, tile.Column, CrateType.crate, ObjectState.ghost);
                }
            }
            else if (myType == SelectionType.tile && tile.Tile.tileType == TileType.fallTile)
                playManager.SetTile(tile.Row, tile.Column, tile.Tile.tileType, ObjectState.ghost);
        }
    }

    private void ResetGhosts ( TileComponent tileComponent, TilePlayManager playManager )
    {
        selectedTile = null;
        CurrentSelection = null;
        currentSelectionType = SelectionType.none;

        TileComponent tile = playManager.allTiles[tileComponent.Row - 1, tileComponent.Column].GetComponent<TileComponent>();
        if (tile.Tile.crateState == ObjectState.ghost && tile.Tile.crateType == CrateType.crate)
            playManager.SetCrate(tile.Row, tile.Column, CrateType.none, ObjectState.none);

        if (tile.Tile.tileState == ObjectState.ghost && tile.Tile.tileType == TileType.fallTile)
            playManager.SetTile(tile.Row, tile.Column, tile.Tile.tileType, ObjectState.none);

        tile = playManager.allTiles[tileComponent.Row + 1, tileComponent.Column].GetComponent<TileComponent>();
        if (tile.Tile.crateState == ObjectState.ghost && tile.Tile.crateType == CrateType.crate)
            playManager.SetCrate(tile.Row, tile.Column, CrateType.none, ObjectState.none);

        if (tile.Tile.tileState == ObjectState.ghost && tile.Tile.tileType == TileType.fallTile)
            playManager.SetTile(tile.Row, tile.Column, tile.Tile.tileType, ObjectState.none);

        tile = playManager.allTiles[tileComponent.Row, tileComponent.Column + 1].GetComponent<TileComponent>();
        if (tile.Tile.crateState == ObjectState.ghost && tile.Tile.crateType == CrateType.crate)
            playManager.SetCrate(tile.Row, tile.Column, CrateType.none, ObjectState.none);

        if (tile.Tile.tileState == ObjectState.ghost && tile.Tile.tileType == TileType.fallTile)
            playManager.SetTile(tile.Row, tile.Column, tile.Tile.tileType, ObjectState.none);

        tile = playManager.allTiles[tileComponent.Row, tileComponent.Column - 1].GetComponent<TileComponent>();
        if (tile.Tile.crateState == ObjectState.ghost && tile.Tile.crateType == CrateType.crate)
            playManager.SetCrate(tile.Row, tile.Column, CrateType.none, ObjectState.none);

        if (tile.Tile.tileState == ObjectState.ghost && tile.Tile.tileType == TileType.fallTile)
            playManager.SetTile(tile.Row, tile.Column, tile.Tile.tileType, ObjectState.none);
    }
}