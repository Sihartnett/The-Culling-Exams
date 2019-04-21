using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TilePlayManager : TileManagerBase
{
    public GameObject cratePrefab;
    public GameObject rayCast;

    private GameObject currentHighlight;
    private GameObject currentSelected;

    Transform cameraTransform;
    Transform centerTransform;

    //Scene Manager
    private SceneManagerSystem SMS;

    public Vector2 currentTile = new Vector2();
    public Vector2? selectedTile = null;

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
                    // You changed tiles
                    // Drop any Selected Items
                    // Reset Selections

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
        if (Physics.Raycast(centerTransform.position, cameraTransform.TransformDirection(Vector3.forward), out RaycastHit rayHit))
        {
            rayCast.transform.position = rayHit.point;

            // For Highlighting
            if (currentHighlight != rayHit.collider.gameObject)
            {
                if (currentHighlight != null)
                {
                    CrateScript currentCrateScript = currentHighlight.GetComponent<CrateScript>();
                    if (currentCrateScript != null)
                        currentCrateScript.Highlight(false);
                }

                // For Highlighting
                CrateScript crateScript = rayHit.collider.GetComponent<CrateScript>();
                if (crateScript != null)
                {
                    crateScript.Highlight(true);
                    currentHighlight = rayHit.collider.gameObject;
                }
            }

            // For Selection
            if (Input.GetMouseButtonDown(0))
            {
                // For Selection
                if (currentSelected != rayHit.collider.gameObject)
                {
                    if (currentSelected != null)
                    {
                        CrateScript currentCrate = currentSelected.GetComponent<CrateScript>();
                        if (currentCrate != null)
                            currentCrate.Select(false);
                    }

                    // For Selection
                    CrateScript clickOnCrate = rayHit.collider.GetComponent<CrateScript>();
                    if (clickOnCrate != null)
                        clickOnCrate.Select(true);

                    currentSelected = rayHit.collider.gameObject;
                }
            }
        }
    }
}
