using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileEditorManager : TileManagerBase
{
    // Start is called before the first frame update
    void Start ()
    {
        // If the Scriptable Object for this level does not have the same rows columns lets go ahead and reset it
        if (tileMap != null && ( tileMap.rowCount != rows || tileMap.columnCount != columns ))
        {
            tileMap.rowCount = rows;
            tileMap.columnCount = columns;

            tileMap.rows = new TileMap.Row[rows];

            for (int row = 0; row < rows; row++)
            {
                tileMap.rows[row] = new TileMap.Row();
                tileMap.rows[row].column = new Tile[columns];
            }
#if UNITY_EDITOR
            EditorUtility.SetDirty(tileMap);
#endif
        }

        allTiles = new GameObject[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Tile tile = tileMap.rows[row].column[column];

                tile.CenterPoint = new Vector3(row, 0.5f, column);

                allTiles[row, column] = Instantiate(tilePrefab, new Vector3(row, 0, column), this.transform.rotation, this.transform);
                allTiles[row, column].GetComponent<TileComponent>().Row = row;
                allTiles[row, column].GetComponent<TileComponent>().Column = column;
            }
        }
    }
    void Update ()
    {
        SetMapValues();
    }
}