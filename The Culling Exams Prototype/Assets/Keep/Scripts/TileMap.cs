using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "Level1", menuName = "TileMap/Levels", order = 1)]
public class TileMap : ScriptableObject
{
    public enum TileType { basic, fall, barrier, start, finish, moveable };
    public enum WallType { none, wall, window, door };
    public enum CrateType { none, crate, mirror };

    public Row[] rows;

    public int rowCount;
    public int columnCount;

    [Serializable]
    public class Row
    {
        public Tile[] column;

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
            
            public Vector3 rotation;
        }
    }
}

