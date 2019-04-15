using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TileEditorManager : MonoBehaviour
{
    public int rows;
    public int columns;

    public GameObject tilePrefab;

    private GameObject[,] allTiles;
    public TileMap tileMap;

    // Map Editor Materials
    public Material defaultMaterial;
    public Material startMaterial;
    public Material finishMaterial;
    public Material barrierMaterial;
    public Material fallMaterial;
    public Material moveableMaterial;

    public Material crateMaterial;
    public Material mirrorMaterial;

    // Dynamic Materials
    public Material normalMaterial;
    public Material highlightMaterial;
    public Material currentMaterial;

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
                tileMap.rows[row].column = new TileMap.Row.Tile[columns];
            }
            EditorUtility.SetDirty(tileMap);
        }
        
        allTiles = new GameObject[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                TileMap.Row.Tile tile = tileMap.rows[row].column[column];
                
                tile.CenterPoint = new Vector3(row, 0.5f, column);

                allTiles[row, column] = Instantiate(tilePrefab, new Vector3(row,0,column), this.transform.rotation, this.transform);
                allTiles[row, column].GetComponent<Tile>().Row = row;
                allTiles[row, column].GetComponent<Tile>().Column = column;
            }
        }
        

    }
    
    // Update is called once per frame
    void Update()
    {
        SetMapValues();
    }

    // For the editor this has to happen every frame since your editing the scriptable object itself
    private void SetMapValues ()
    {
        foreach (GameObject tile in allTiles)
        {
            Tile scriptedTile = tile.GetComponent<Tile>();
            MeshRenderer[] meshes = tile.GetComponentsInChildren<MeshRenderer>();

            int row = scriptedTile.Row;
            int column = scriptedTile.Column;

            TileMap.Row.Tile tileMapTile = tileMap.rows[row].column[column];

            if (tileMapTile.startTile && startMaterial != null)
                meshes[0].material = startMaterial;

            else if (tileMapTile.finishTile && finishMaterial != null)
                meshes[0].material = finishMaterial;

            else if (tileMapTile.barrierTile && barrierMaterial != null)
                meshes[0].material = barrierMaterial;

            else if (tileMapTile.fallTile && fallMaterial != null)
                meshes[0].material = fallMaterial;

            else
                meshes[0].material = defaultMaterial;
            
            transform.rotation = Quaternion.Euler(tileMapTile.rotation);
            if (tileMapTile.Crate)
            {
                meshes[1].enabled = true;
                meshes[1].material = crateMaterial;
            }
            else if (tileMapTile.Mirror)
            {
                meshes[1].enabled = true;
                meshes[1].material = mirrorMaterial;
            }
            else
                meshes[1].enabled = false;
        }
    }
}
