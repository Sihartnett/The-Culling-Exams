using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TilePlayManager : MonoBehaviour
{
    public int rows;
    public int columns;

    public GameObject tilePrefab;
    public GameObject cratePrefab;

    private GameObject[,] allTiles;
    private List<GameObject> allCrates;
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
        if (tileMap != null && ( tileMap.rowCount != rows || tileMap.columnCount != columns ))
        {
            Debug.Log("ScirptableObjectDoes not match tiles");
            return;
        }

        // Create all GameObjects
        // Tiles
        allTiles = new GameObject[rows, columns];
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                allTiles[row, column] = Instantiate(tilePrefab, new Vector3(row, 0, column), this.transform.rotation, this.transform);
                allTiles[row, column].GetComponent<Tile>().Row = row;
                allTiles[row, column].GetComponent<Tile>().Column = column;
            }
        }

        // Objects
        allCrates = new List<GameObject>();
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                TileMap.Row.Tile tile = tileMap.rows[row].column[column];

                if (tile.Crate || tile.Mirror)
                {
                    GameObject addMe = Instantiate(
                            cratePrefab, new Vector3(row, 0.5f, column),
                            Quaternion.Euler(tile.rotation),
                            this.transform);

                    MeshRenderer[] meshes = addMe.GetComponentsInChildren<MeshRenderer>();

                    if (tile.Crate)
                        meshes[0].material = crateMaterial;
                    else if (tile.Mirror)
                        meshes[0].material = mirrorMaterial;
                    else
                    {
                        meshes[0].material = defaultMaterial;
                    }
                    
                    allCrates.Add(addMe);
                }
            }
        }
    }

    private bool editLoop = true;
    // Update is called once per frame
    void Update ()
    {
        if (editLoop)
        {
            editLoop = false;
            SetMapValues();
        }

        SetDynamicValues();
    }

    private void SetDynamicValues ()
    {
    }

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
        }
    }
}
