﻿using System;
using UnityEngine;

public enum WallType { none, wall, window, redDoor, blueDoor, purpleDoor, brownDoor, redDoorOpen, blueDoorOpen, purpleDoorOpen, brownDoorOpen, orangeDoor, orangeDoorOpen, lightBlueDoor, lightBlueDoorOpen };

public enum TileType { basicTile, fallTile, barrierTile, startTile, finishTile, moveableTile, redTile, blueTile, purpleTile, brownTile, moveableBarrierTile, moveableCrateTile, orangeTile, lightBlueTile };
public enum CrateType { none, crate, mirror, redCrate, blueCrate, purpleCrate, brownCrate, orangeCrate, lightBlueCrate };

public enum PickupType { none, fatiguePickup, crateMovePickup, timePickup }

public enum ObjectState { none, selected, highlighted, ghost, ghostHighlighted };

public enum SelectionType { none, crate, tile }

[Serializable]
public class Tile
{
    [HideInInspector]
    public Vector3 CenterPoint;

    public TileType tileType;
    public WallType northWallType;
    public WallType southWallType;
    public WallType eastWallType;
    public WallType westWallType;
    public CrateType crateType;
    
    public PickupType pickupType;
    public float pickupCount;
    
    [NonSerialized]
    public ObjectState crateState = ObjectState.none;
    [NonSerialized]
    public ObjectState tileState = ObjectState.none;

    public Vector3 rotation;

    public Tile () { }
    public Tile ( Tile tile )
    {
        this.CenterPoint = tile.CenterPoint;
        this.tileType = tile.tileType;
        this.northWallType = tile.northWallType;
        this.southWallType = tile.southWallType;
        this.westWallType = tile.westWallType;
        this.eastWallType = tile.eastWallType;
        this.crateType = tile.crateType;
        this.rotation = tile.rotation;
        this.pickupType = tile.pickupType;
    }
}

[CreateAssetMenu(fileName = "Level1", menuName = "TileMap/Levels", order = 1)]
public class TileMap : ScriptableObject
{
    public Row[] rows;

    public int rowCount;
    public int columnCount;

    [Serializable]
    public class Row
    {
        public Tile[] column;
    }
}



