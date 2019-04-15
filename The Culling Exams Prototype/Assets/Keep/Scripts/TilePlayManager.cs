﻿using System;
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
                TileMap.Row.Tile dataTile = tileMap.rows[row].column[column];

                allTiles[row, column] = Instantiate(tilePrefab, new Vector3(row, 0, column), this.transform.rotation, this.transform);
                
                Tile tileComponent = allTiles[row, column].GetComponent<Tile>();
                tileComponent.Row = row;
                tileComponent.Column = column;

                // Turn on the invisible Collision on the tile prefabs
                allTiles[row, column].transform.GetChild(1).gameObject.SetActive(dataTile.barrierTile);

                MeshRenderer[] meshes = allTiles[row, column].GetComponentsInChildren<MeshRenderer>();
               

                if (dataTile.startTile && startMaterial != null)
                    meshes[0].material = startMaterial;

                else if (dataTile.finishTile && finishMaterial != null)
                    meshes[0].material = finishMaterial;

                else if (dataTile.barrierTile && barrierMaterial != null)
                    meshes[0].material = barrierMaterial;

                else if (dataTile.fallTile && fallMaterial != null)
                    meshes[0].material = fallMaterial;

                else
                    meshes[0].material = defaultMaterial;
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
                            cratePrefab, tile.CenterPoint,
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


        // Set players start position for scene
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                TileMap.Row.Tile dataTile = tileMap.rows[row].column[column];
                if (dataTile.startTile)
                    this.transform.GetChild(0).transform.SetPositionAndRotation(dataTile.CenterPoint, Quaternion.identity);

            }
        }
    }
    
    // Update is called once per frame
    void Update ()
    {
        SetDynamicValues();
    }

    private void SetDynamicValues ()
    {



    }

}
