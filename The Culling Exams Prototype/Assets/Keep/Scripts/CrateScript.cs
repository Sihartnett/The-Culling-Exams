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
        
        // Deselect everything
        if (tileComponent.Tile.crateState == ObjectState.highlighted)
            playManager.SetCrate(tileComponent.Row, tileComponent.Column, tileComponent.Tile.crateType, ObjectState.none);

        if (tileComponent.Tile.crateState == ObjectState.ghostHighlighted)
            playManager.SetCrate(tileComponent.Row, tileComponent.Column, tileComponent.Tile.crateType, ObjectState.ghost);

        if (tileComponent.Tile.tileState == ObjectState.highlighted)
            playManager.SetTile(tileComponent.Row, tileComponent.Column, tileComponent.Tile.tileType, ObjectState.none);

        if (tileComponent.Tile.tileState == ObjectState.ghostHighlighted)
            playManager.SetTile(tileComponent.Row, tileComponent.Column, tileComponent.Tile.tileType, ObjectState.ghost);

        // Do nothing Ever if im standing on the tile
        if (tileComponent.Row == playManager.currentTile.x && playManager.currentTile.y == tileComponent.Column)
            return returnValue;

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
                        if (tileComponent.Tile.tileType == TileType.moveableTile || tileComponent.Tile.tileType == TileType.moveableBarrierTile)
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

    public void Select ()
    {
        TileComponent tileComponent = this.transform.parent.GetComponent<TileComponent>();
        TilePlayManager playManager = this.transform.parent.transform.parent.GetComponent<TilePlayManager>();
        SceneManagerSystem SMS = FindObjectOfType<SceneManagerSystem>();

        // Deselect everything nothing is currently highlighted but we have a selection
        if (currentHighlight == null && currentSelection != null)
        {
            SMS.DeSelectCrate();
            
            TileComponent sourceCrateTileComponent = currentSelection.transform.parent.GetComponent<TileComponent>();

            playManager.SetCrate(sourceCrateTileComponent.Row, sourceCrateTileComponent.Column, sourceCrateTileComponent.Tile.crateType, ObjectState.none);
            playManager.SetTile(sourceCrateTileComponent.Row, sourceCrateTileComponent.Column, sourceCrateTileComponent.Tile.tileType, ObjectState.none);
            
            ResetGhosts(playManager, sourceCrateTileComponent);
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
                    playManager.SetCrate(tileComponent.Row, tileComponent.Column, tileComponent.Tile.crateType, ObjectState.selected);
                else if (myType == SelectionType.tile)
                    playManager.SetTile(tileComponent.Row, tileComponent.Column, tileComponent.Tile.tileType, ObjectState.selected);

                CreateGhosts( playManager, tileComponent );
            }

            // We have a valid selection and a valid highlight so do the move
            else
            {
                TileComponent sourceCrateTileComponent = currentSelection.transform.parent.GetComponent<TileComponent>();

                if (myType == SelectionType.crate || currentSelectionType == SelectionType.crate)
                {
                    SMS.DeSelectCrate();

                    // Logic for highlighted after placement
                    // Place the Crate and decide if we should highlight it
                    if (( playManager.currentTile.x - 1 <= tileComponent.Row && tileComponent.Row <= playManager.currentTile.x + 1 )
                        &&
                        ( playManager.currentTile.y - 1 <= tileComponent.Column && tileComponent.Column <= playManager.currentTile.y + 1 ))
                    {
                        currentHighlight = tileComponent.gameObject;
                        playManager.SetCrate(tileComponent.Row, tileComponent.Column, sourceCrateTileComponent.Tile.crateType, ObjectState.highlighted);
                    }
                    else
                    {
                        currentHighlight = null;
                        playManager.SetCrate(tileComponent.Row, tileComponent.Column, sourceCrateTileComponent.Tile.crateType, ObjectState.none);
                    }

                    // remove the previous crate
                    playManager.SetCrate(sourceCrateTileComponent.Row, sourceCrateTileComponent.Column, CrateType.none, ObjectState.none);

                    // This is for when a blue crate lands on a blue tile or the door close
                    if (tileComponent.Tile.crateType == CrateType.blueCrate)
                        if (tileComponent.Tile.tileType == TileType.blueTile)
                            OpenDoor(playManager, WallType.blueDoor, WallType.blueDoorOpen);
                        else
                            OpenDoor(playManager, WallType.blueDoorOpen, WallType.blueDoor);

                    // This is for when a red crate lands on a red tile or the door close
                    if (tileComponent.Tile.crateType == CrateType.redCrate)
                        if (tileComponent.Tile.tileType == TileType.redTile)
                            OpenDoor(playManager, WallType.redDoor, WallType.redDoorOpen);
                        else
                            OpenDoor(playManager, WallType.redDoorOpen, WallType.redDoor);

                    // This is for when a brown crate lands on a brown tile or the door close
                    if (tileComponent.Tile.crateType == CrateType.brownCrate)
                        if (tileComponent.Tile.tileType == TileType.brownTile)
                            OpenDoor(playManager, WallType.brownDoor, WallType.brownDoorOpen);
                        else
                            OpenDoor(playManager, WallType.brownDoorOpen, WallType.brownDoor);

                    // This is for when a purple crate lands on a purple tile or the door close
                    if (tileComponent.Tile.crateType == CrateType.purpleCrate)
                        if (tileComponent.Tile.tileType == TileType.purpleTile)
                            OpenDoor(playManager, WallType.purpleDoor, WallType.purpleDoorOpen);
                        else
                            OpenDoor(playManager, WallType.purpleDoorOpen, WallType.purpleDoor);

                    // This is for when a orange crate lands on a orange tile or the door close
                    if (tileComponent.Tile.crateType == CrateType.orangeCrate)
                        if (tileComponent.Tile.tileType == TileType.orangeTile)
                            OpenDoor(playManager, WallType.orangeDoor, WallType.orangeDoorOpen);
                        else
                            OpenDoor(playManager, WallType.orangeDoorOpen, WallType.orangeDoor);

                    // This is for when a lightBlue crate lands on a lightBlue tile or the door close
                    if (tileComponent.Tile.crateType == CrateType.lightBlueCrate)
                        if (tileComponent.Tile.tileType == TileType.lightBlueTile)
                            OpenDoor(playManager, WallType.lightBlueDoor, WallType.lightBlueDoorOpen);
                        else
                            OpenDoor(playManager, WallType.lightBlueDoorOpen, WallType.lightBlueDoor);
                }

                else if (myType == SelectionType.tile)
                {
                    playManager.SetTile(tileComponent.Row, tileComponent.Column, sourceCrateTileComponent.Tile.tileType, ObjectState.none);
                    playManager.SetTile(sourceCrateTileComponent.Row, sourceCrateTileComponent.Column, TileType.fallTile, ObjectState.none);
                }

                ResetGhosts(playManager, sourceCrateTileComponent);
            }
        }
    }

    private static void OpenDoor ( TilePlayManager playManager, WallType source, WallType destination)
    {
        // Open all blue Doors
        for (int row = 0; row < playManager.rows; row++)
        {
            for (int column = 0; column < playManager.columns; column++)
            {
                TileComponent tile = playManager.allTiles[row, column].GetComponent<TileComponent>();
                if (tile.Tile.eastWallType == source)
                    playManager.SetEastWall(tile.Row, tile.Column, destination);
                if (tile.Tile.northWallType == source)
                    playManager.SetNorthWall(tile.Row, tile.Column, destination);
                if (tile.Tile.southWallType == source)
                    playManager.SetSouthWall(tile.Row, tile.Column, destination);
                if (tile.Tile.westWallType == source)
                    playManager.SetWestWall(tile.Row, tile.Column, destination);
            }
        }
    }

    static public void Deselect()
    {
        selectedTile = null;
        currentSelection = null;
        currentHighlight = null;
        currentSelectionType = SelectionType.none;
    }

    private void CreateGhosts ( TilePlayManager playManager, TileComponent tileComponent )
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

    private void FlipGhost ( TilePlayManager playManager, TileComponent tileComponent )
    {
        // if im standing on the tile do nothing
        if (tileComponent.Row != (int)playManager.currentTile.x || tileComponent.Column != (int)playManager.currentTile.y)
        {
            if (myType == SelectionType.crate)
            {
                // make sure there is no crate and is a valid place to put it
                if (tileComponent.Tile.crateType != CrateType.none)
                    return;

                if (tileComponent.Tile.tileType == TileType.barrierTile
                    || tileComponent.Tile.tileType == TileType.moveableBarrierTile
                    || tileComponent.Tile.tileType == TileType.fallTile)
                    return;

                TileComponent currentTile = playManager.allTiles[(int)playManager.currentTile.x, (int)playManager.currentTile.y].GetComponent<TileComponent>();

                // now check for all the impassible wall tiles in yourself and in your target 
                if (currentTile.Row < tileComponent.Row
                    &&
                    ( currentTile.Tile.northWallType == WallType.blueDoor
                    || currentTile.Tile.northWallType == WallType.brownDoor
                    || currentTile.Tile.northWallType == WallType.purpleDoor
                    || currentTile.Tile.northWallType == WallType.redDoor
                    || currentTile.Tile.northWallType == WallType.orangeDoor
                    || currentTile.Tile.northWallType == WallType.lightBlueDoor
                    || currentTile.Tile.northWallType == WallType.wall
                    || currentTile.Tile.northWallType == WallType.window
                    || tileComponent.Tile.southWallType == WallType.blueDoor
                    || tileComponent.Tile.southWallType == WallType.brownDoor
                    || tileComponent.Tile.southWallType == WallType.purpleDoor
                    || tileComponent.Tile.southWallType == WallType.redDoor
                    || tileComponent.Tile.southWallType == WallType.wall
                    || tileComponent.Tile.southWallType == WallType.window
                    || tileComponent.Tile.southWallType == WallType.orangeDoor
                    || tileComponent.Tile.southWallType == WallType.lightBlueDoor
                    ))
                    return;

                if (currentTile.Row > tileComponent.Row
                    &&
                    ( currentTile.Tile.southWallType == WallType.blueDoor
                    || currentTile.Tile.southWallType == WallType.brownDoor
                    || currentTile.Tile.southWallType == WallType.purpleDoor
                    || currentTile.Tile.southWallType == WallType.redDoor
                    || currentTile.Tile.southWallType == WallType.wall
                    || currentTile.Tile.southWallType == WallType.window
                    || currentTile.Tile.southWallType == WallType.orangeDoor
                    || currentTile.Tile.southWallType == WallType.lightBlueDoor
                    || tileComponent.Tile.northWallType == WallType.blueDoor
                    || tileComponent.Tile.northWallType == WallType.brownDoor
                    || tileComponent.Tile.northWallType == WallType.purpleDoor
                    || tileComponent.Tile.northWallType == WallType.redDoor
                    || tileComponent.Tile.northWallType == WallType.wall
                    || tileComponent.Tile.northWallType == WallType.window
                    || tileComponent.Tile.northWallType == WallType.orangeDoor
                    || tileComponent.Tile.northWallType == WallType.lightBlueDoor ))
                    return;

                if (currentTile.Column < tileComponent.Column
                    &&
                    ( currentTile.Tile.westWallType == WallType.blueDoor
                    || currentTile.Tile.westWallType == WallType.brownDoor
                    || currentTile.Tile.westWallType == WallType.purpleDoor
                    || currentTile.Tile.westWallType == WallType.redDoor
                    || currentTile.Tile.westWallType == WallType.wall
                    || currentTile.Tile.westWallType == WallType.window
                    || currentTile.Tile.westWallType == WallType.orangeDoor
                    || currentTile.Tile.westWallType == WallType.lightBlueDoor
                    || tileComponent.Tile.eastWallType == WallType.blueDoor
                    || tileComponent.Tile.eastWallType == WallType.brownDoor
                    || tileComponent.Tile.eastWallType == WallType.purpleDoor
                    || tileComponent.Tile.eastWallType == WallType.redDoor
                    || tileComponent.Tile.eastWallType == WallType.wall
                    || tileComponent.Tile.eastWallType == WallType.window
                    || tileComponent.Tile.eastWallType == WallType.orangeDoor
                    || tileComponent.Tile.eastWallType == WallType.lightBlueDoor
                    ))
                    return;

                if (currentTile.Column > tileComponent.Column
                    &&
                    ( currentTile.Tile.eastWallType == WallType.blueDoor
                    || currentTile.Tile.eastWallType == WallType.brownDoor
                    || currentTile.Tile.eastWallType == WallType.purpleDoor
                    || currentTile.Tile.eastWallType == WallType.redDoor
                    || currentTile.Tile.eastWallType == WallType.wall
                    || currentTile.Tile.eastWallType == WallType.window
                    || currentTile.Tile.eastWallType == WallType.orangeDoor
                    || currentTile.Tile.eastWallType == WallType.lightBlueDoor
                    || tileComponent.Tile.westWallType == WallType.blueDoor
                    || tileComponent.Tile.westWallType == WallType.brownDoor
                    || tileComponent.Tile.westWallType == WallType.purpleDoor
                    || tileComponent.Tile.westWallType == WallType.redDoor
                    || tileComponent.Tile.westWallType == WallType.wall
                    || tileComponent.Tile.westWallType == WallType.window
                    || tileComponent.Tile.westWallType == WallType.orangeDoor
                    || tileComponent.Tile.westWallType == WallType.lightBlueDoor
                    ))
                    return;

                playManager.SetCrate(tileComponent.Row, tileComponent.Column, CrateType.crate, ObjectState.ghost);

            }
            else if (myType == SelectionType.tile && tileComponent.Tile.tileType == TileType.fallTile)
                playManager.SetTile(tileComponent.Row, tileComponent.Column, tileComponent.Tile.tileType, ObjectState.ghost);
        }
    }

    static public void ResetGhosts ( TilePlayManager playManager, TileComponent tileComponent )
    {
        selectedTile = null;
        currentSelection = null;
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

    // Removing Funtionality
    //public void ShootRay ( TilePlayManager playManager, GameObject player )
    //{
    //    TileComponent currentTile = playManager.allTiles[(int)playManager.currentTile.x, (int)playManager.currentTile.y].GetComponent<TileComponent>();
        
    //    // Find the Forward direction of the player
    //    // Loop through the forward direction until we hit something
    //    float playerRotationDegree = player.transform.eulerAngles.y;

    //    // Find our forward Direction
    //    Vector2 forwardDirection = Vector2.zero;
    //    if (45 <= playerRotationDegree && playerRotationDegree <= 135)
    //        forwardDirection.x = 1;
    //    else if (135 <= playerRotationDegree && playerRotationDegree <= 225)
    //        forwardDirection.y = -1;
    //    else if (225 <= playerRotationDegree && playerRotationDegree <= 315)
    //        forwardDirection.x = -1;
    //    else if (315 <= playerRotationDegree && playerRotationDegree <= 360 || 0 <= playerRotationDegree && playerRotationDegree <= 45)
    //        forwardDirection.y = 1;
        
    //}
}