using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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

        [Serializable]
        public class Tile
        {
            public bool fallTile = false;
            public bool barrierTile = false;
            public bool startTile = false;
            public bool finishTile = false;
            public bool moveableTile = false;

            public bool Crate = false;
            public bool Mirror = false;
            public Vector3 rotation;
        }
    }
}

