using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TilePlayManager : TileManagerBase
{
    public GameObject rayCastSphere;
    
    Transform cameraTransform;
    Transform centerTransform;

    //Scene Manager
    private SceneManagerSystem SMS;

    public Vector2 currentTile = new Vector2();

    // Current Fatigue Counter Set at the begining of every level
    public int fatigue = 0;
    
    // needed for ray cast
    private int layerMask;

    // Start is called before the first frame update
    void Start ()
    {
        SMS = FindObjectOfType<SceneManagerSystem>();

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
                Tile dataTile = tileMap.rows[row].column[column];

                allTiles[row, column] = Instantiate(tilePrefab, new Vector3(row, 0, column), this.transform.rotation, this.transform);

                TileComponent tileComponent = allTiles[row, column].GetComponent<TileComponent>();
                tileComponent.Row = row;
                tileComponent.Column = column;
                tileComponent.Tile = new Tile(dataTile);
            }
        }

        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                Tile dataTile = tileMap.rows[row].column[column];
                if (dataTile.tileType == TileType.start)
                    this.transform.GetChild(0).transform.SetPositionAndRotation(dataTile.CenterPoint, Quaternion.identity);
            }
        }

        SetMapValues();

        cameraTransform = Camera.main.transform;
        centerTransform = GameObject.Find("Cylinder").transform;

        layerMask = LayerMask.GetMask("clickableLayer");
    }

    // Update is called once per frame
    void Update ()
    {
        SetDynamicValues();

        // Use the players central position with the cameras forward vector to create the ray
        Vector3 playerPosition = this.transform.GetChild(0).transform.position;
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GameObject tile = allTiles[row, column];

                // Movement
                TileComponent scriptedTile = tile.GetComponent<TileComponent>();
                if (scriptedTile.Row + 0.5f > playerPosition.x && playerPosition.x > scriptedTile.Row - 0.5f
                    && scriptedTile.Column + 0.5f > playerPosition.z && playerPosition.z > scriptedTile.Column - 0.5f)
                {
                    Tile dataTile = tileMap.rows[scriptedTile.Row].column[scriptedTile.Column];
                    switch (dataTile.tileType)
                    {
                        case TileType.finish:
                            {
                                SMS.NextLevel();
                            }
                            break;
                        case TileType.basic:
                        case TileType.moveable:
                            {
                            }
                            break;
                        default:
                            break;
                    }

                    currentTile.x = scriptedTile.Row;
                    currentTile.y = scriptedTile.Column;
                }
            }
        }

        //Debug.DrawRay(centerPoint.position, camera.TransformDirection(Vector3.forward), Color.red);

        //if (Physics.Raycast(centerTransform.position, centerTransform.TransformDirection(Vector3.forward), out RaycastHit rayHit, layerMask))
        if (Physics.Raycast(centerTransform.position, cameraTransform.TransformDirection(Vector3.forward), out RaycastHit rayHit, layerMask))
        {
            if(rayCastSphere != null)
                rayCastSphere.transform.position = rayHit.point;

            CrateScript crateScript = rayHit.collider.GetComponent<CrateScript>();
            if (crateScript != null)
            {
                // Highlighting
                crateScript.Highlight(rayHit.collider);

                if (PauseMenuManager.instance != null && !PauseMenuManager.instance.PauseorNot)
                {
                    // Selection
                    if (Input.GetMouseButtonDown(0))
                    {
                        crateScript.Select(rayHit.collider);
                    }
                }
            }
        }
    }
}
