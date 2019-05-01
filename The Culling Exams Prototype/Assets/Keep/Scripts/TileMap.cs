﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum TileType { basic, fall, barrier, start, finish, moveable, redTile, blueTile, fallHighlighted, fallGhost, moveableHighlighted };
public enum WallType { none, wall, window, redDoor, blueDoor, redDoorOpen, blueDoorOpen };
public enum CrateType { none, crate, mirror, redcrate, bluecrate };
public enum CrateState { none, selected, highlighted, ghost, ghostHighlighted };

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

    [NonSerialized]
    public CrateState crateState = CrateState.none;

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



