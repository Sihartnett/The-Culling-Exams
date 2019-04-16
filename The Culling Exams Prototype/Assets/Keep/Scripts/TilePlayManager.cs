﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TilePlayManager : TileManagerBase
{
    public GameObject cratePrefab;

    private List<GameObject> allCrates;

    // Start is called before the first frame update
    void Start()
    {
        if (tileMap != null && (tileMap.rowCount != rows || tileMap.columnCount != columns))
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
            }
        }

        // Objects
        allCrates = new List<GameObject>();
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                TileMap.Row.Tile tile = tileMap.rows[row].column[column];

                if (tile.crateType != TileMap.CrateType.none)
                {

                    GameObject addMe = Instantiate(
                            cratePrefab, tile.CenterPoint,
                            Quaternion.Euler(tile.rotation),
                            this.transform);

                    MeshRenderer[] meshes = addMe.GetComponentsInChildren<MeshRenderer>();

                    if (tile.crateType == TileMap.CrateType.crate)
                        meshes[0].material = crateMaterial;
                    else if (tile.crateType == TileMap.CrateType.mirror)
                        meshes[0].material = mirrorMaterial;
                    else
                        meshes[0].material = defaultMaterial;

                    allCrates.Add(addMe);
                }
            }
        }

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                TileMap.Row.Tile dataTile = tileMap.rows[row].column[column];
                if (dataTile.tileType == TileMap.TileType.start)
                    this.transform.GetChild(0).transform.SetPositionAndRotation(dataTile.CenterPoint, Quaternion.identity);

            }
        }

        SetMapValues();
    }

    // Update is called once per frame
    void Update()
    {
        SetDynamicValues();

        Vector3 playerPosition = this.transform.GetChild(0).transform.position;
        foreach (GameObject tile in allTiles)
        {
            // If the player is on this tile

            Tile scriptedTile = tile.GetComponent<Tile>();
            if (scriptedTile.Row + 0.5f > playerPosition.x && playerPosition.x > scriptedTile.Row - 0.5f
                && scriptedTile.Column + 0.5f > playerPosition.z && playerPosition.z > scriptedTile.Column - 0.5f)
            {
                // This will need to be turned into a switch

                TileMap.Row.Tile dataTile = tileMap.rows[scriptedTile.Row].column[scriptedTile.Column];
                if (dataTile.tileType == TileMap.TileType.finish)
                {
                    for (int row = 0; row < rows; row++)
                    {
                        for (int column = 0; column < columns; column++)
                        {
                            TileMap.Row.Tile innerDataTile = tileMap.rows[row].column[column];

                            if(innerDataTile.tileType == TileMap.TileType.start )
                                this.transform.GetChild(0).transform.SetPositionAndRotation(innerDataTile.CenterPoint, Quaternion.identity);
                        }
                    }
                }
            }
        }
    }
}
