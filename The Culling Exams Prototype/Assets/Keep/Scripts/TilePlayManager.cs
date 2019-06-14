using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class TilePlayManager : TileManagerBase
{
    private Transform cameraTransform;

    public GameObject rayCastSphere;
    public float rayCastYoffset = 1.3f;

    public PlayerMovement player;

    [NonSerialized]
    public Vector2 currentTile = new Vector2();

    // Current Counters Set at the begining of every level
    public int fatigue = 0;
    public int timeLeft = 0;
    public int crateMoves = 0;

    // needed for ray cast collision culling
    private int layerMask;

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
                Tile dataTile = tileMap.rows[row].column[column];

                allTiles[row, column] = Instantiate(tilePrefab, new Vector3(row, 0, column), transform.rotation, transform);

                TileComponent tileComponent = allTiles[row, column].GetComponent<TileComponent>();
                tileComponent.Row = row;
                tileComponent.Column = column;
                tileComponent.Tile = new Tile(dataTile);

                if (dataTile.tileType == TileType.startTile)
                {
                    transform.GetChild(0).transform.SetPositionAndRotation(dataTile.CenterPoint, Quaternion.identity);
                    currentTile.x = row;
                    currentTile.y = column;
                }
            }
        }

        SetMapValues();

        cameraTransform = Camera.main.transform;
        
        player = FindObjectOfType<PlayerMovement>();
        layerMask = LayerMask.GetMask("clickableLayer");
    }
    
    // Update is called once per frame
    void Update ()
    {
        player.Movement(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), player.gameObject.transform);
        
        if (Physics.Raycast(
            new Vector3(player.transform.position.x, player.transform.position.y + rayCastYoffset, player.transform.position.z),
            cameraTransform.TransformDirection(Vector3.forward), out RaycastHit rayHit, layerMask))
        {
            if (rayCastSphere != null)
                rayCastSphere.transform.position = rayHit.point;

            CrateScript crateScript = rayHit.collider.GetComponent<CrateScript>();
            if (crateScript != null)
            {
                // Highlighting
                crateScript.Highlight(rayHit.collider);

                //if (PauseMenuManager.instance != null && !PauseMenuManager.instance.PauseorNot)
                {
                    if (Input.GetMouseButtonDown(0))
                    {
                        crateScript.Select(rayHit.collider);
                    }

                    if (Input.GetMouseButtonDown(1))
                    {
                        crateScript.ShootRay(this, player.gameObject);
                    }
                }
            }
        }
    }

    public void SetCrate ( int row, int column, CrateType type, ObjectState state )
    {
        GameObject tile = allTiles[row, column];

        TileComponent tileComponent = tile.GetComponent<TileComponent>();
        tileComponent.Tile.crateType = type;
        tileComponent.Tile.crateState = state;

        MeshRenderer[] meshes = tile.GetComponentsInChildren<MeshRenderer>(true);

        switch (state)
        {
            case ObjectState.highlighted:
                tile.transform.GetChild(1).gameObject.SetActive(true);
                meshes[1].material = highlightMaterial;
                break;
            case ObjectState.selected:
                tile.transform.GetChild(1).gameObject.SetActive(true);
                meshes[1].material = selectedMaterial;
                break;
            case ObjectState.ghost:
                tile.transform.GetChild(1).gameObject.SetActive(true);
                meshes[1].material = ghostMaterial; break;
            case ObjectState.ghostHighlighted:
                tile.transform.GetChild(1).gameObject.SetActive(true);
                meshes[1].material = highlightMaterial;
                break;
            case ObjectState.none:
            default:
                {
                    switch (type)
                    {
                        case CrateType.crate:
                            tile.transform.GetChild(1).gameObject.SetActive(true);
                            meshes[1].material = crateMaterial;
                            break;
                        case CrateType.redCrate:
                            tile.transform.GetChild(1).gameObject.SetActive(true);
                            meshes[1].material = redCrateMaterial;
                            break;
                        case CrateType.blueCrate:
                            tile.transform.GetChild(1).gameObject.SetActive(true);
                            meshes[1].material = blueCrateMaterial;
                            break;
                        case CrateType.brownCrate:
                            tile.transform.GetChild(1).gameObject.SetActive(true);
                            meshes[1].material = brownCrateMaterial;
                            break;
                        case CrateType.purpleCrate:
                            tile.transform.GetChild(1).gameObject.SetActive(true);
                            meshes[1].material = purpleCrateMaterial;
                            break;
                        case CrateType.mirror:
                            tile.transform.GetChild(1).gameObject.SetActive(true);
                            meshes[1].material = mirrorMaterial;
                            break;
                        case CrateType.none:
                        default:
                            tile.transform.GetChild(1).gameObject.SetActive(false);
                            break;
                    }
                }
                break;
        }
    }

    public void SetTile ( int row, int column, TileType type, ObjectState state )
    {
        GameObject tile = allTiles[row, column];

        TileComponent tileComponent = tile.GetComponent<TileComponent>();
        tileComponent.Tile.tileType = type;
        tileComponent.Tile.tileState = state;
        
        MeshRenderer[] meshes = tile.GetComponentsInChildren<MeshRenderer>(true);

        switch (state)
        {
            case ObjectState.highlighted:
                meshes[0].material = highlightMaterial;
                break;
            case ObjectState.selected:
                meshes[0].material = selectedMaterial;
                break;
            case ObjectState.ghost:
                meshes[0].material = ghostMaterial;
                break;
            case ObjectState.ghostHighlighted:
                meshes[0].material = highlightMaterial;
                break;
            case ObjectState.none:
            default:
                {
                    switch (type)
                    {
                        case TileType.barrierTile:
                            meshes[0].material = barrierMaterial;
                            break;
                        case TileType.finishTile:
                            meshes[0].material = finishMaterial;
                            break;
                        case TileType.startTile:
                            meshes[0].material = startMaterial;
                            break;
                        case TileType.moveableTile:
                            meshes[0].material = moveableMaterial;
                            break;
                        case TileType.moveableBarrierTile:
                            meshes[0].material = moveableBarrierMaterial;
                            break;
                        case TileType.fallTile:
                            meshes[0].material = fallMaterial;
                            break;
                        case TileType.blueTile:
                            meshes[0].material = blueTileMaterial;
                            break;
                        case TileType.redTile:
                            meshes[0].material = redTileMaterial;
                            break;
                        case TileType.purpleTile:
                            meshes[0].material = purpleTileMaterial;
                            break;
                        case TileType.brownTile:
                            meshes[0].material = brownTileMaterial;
                            break;
                        case TileType.basicTile:
                        default:
                            meshes[0].material = defaultMaterial;
                            break;
                    }
                }
                break;
        }
    }

    public void SetNorthWall ( int row, int column, WallType type )
    {
        GameObject tile = allTiles[row, column];
        TileComponent tileComponent = tile.GetComponent<TileComponent>();
        tileComponent.Tile.northWallType = type;

        MeshRenderer[] meshes = tile.GetComponentsInChildren<MeshRenderer>(true);

        switch (type)
        {
            case WallType.wall:
                tile.transform.GetChild(2).gameObject.SetActive(true);
                meshes[2].material = wallMaterial;
                break;
            case WallType.window:
                tile.transform.GetChild(2).gameObject.SetActive(true);
                meshes[2].material = windowMaterial;
                break;
            case WallType.redDoor:
                tile.transform.GetChild(2).gameObject.SetActive(true);
                meshes[2].material = redDoorMaterial;
                break;
            case WallType.blueDoor:
                tile.transform.GetChild(2).gameObject.SetActive(true);
                meshes[2].material = blueDoorMaterial;
                break;
            case WallType.purpleDoor:
                tile.transform.GetChild(2).gameObject.SetActive(true);
                meshes[2].material = purpleDoorMaterial;
                break;
            case WallType.brownDoor:
                tile.transform.GetChild(2).gameObject.SetActive(true);
                meshes[2].material = brownDoorMaterial;
                break;
            case WallType.none:
            default:
                tile.transform.GetChild(2).gameObject.SetActive(false);
                break;
        }
    }

    public void SetSouthWall ( int row, int column, WallType type )
    {
        GameObject tile = allTiles[row, column];

        TileComponent tileComponent = tile.GetComponent<TileComponent>();
        tileComponent.Tile.southWallType = type;
        MeshRenderer[] meshes = tile.GetComponentsInChildren<MeshRenderer>(true);

        switch (type)
        {
            case WallType.wall:
                tile.transform.GetChild(3).gameObject.SetActive(true);
                meshes[3].material = wallMaterial;
                break;
            case WallType.window:
                tile.transform.GetChild(3).gameObject.SetActive(true);
                meshes[3].material = windowMaterial;
                break;
            case WallType.redDoor:
                tile.transform.GetChild(3).gameObject.SetActive(true);
                meshes[3].material = redDoorMaterial;
                break;
            case WallType.blueDoor:
                tile.transform.GetChild(3).gameObject.SetActive(true);
                meshes[3].material = blueDoorMaterial;
                break;
            case WallType.purpleDoor:
                tile.transform.GetChild(3).gameObject.SetActive(true);
                meshes[3].material = purpleDoorMaterial;
                break;
            case WallType.brownDoor:
                tile.transform.GetChild(3).gameObject.SetActive(true);
                meshes[3].material = brownDoorMaterial;
                break;
            case WallType.none:
            default:
                tile.transform.GetChild(3).gameObject.SetActive(false);
                break;
        }

    }

    public void SetEastWall ( int row, int column, WallType type )
    {
        GameObject tile = allTiles[row, column];
        TileComponent tileComponent = tile.GetComponent<TileComponent>();
        tileComponent.Tile.eastWallType = type;

        MeshRenderer[] meshes = tile.GetComponentsInChildren<MeshRenderer>(true);

        switch (type)
        {
            case WallType.wall:
                tile.transform.GetChild(4).gameObject.SetActive(true);
                meshes[4].material = wallMaterial;
                break;
            case WallType.window:
                tile.transform.GetChild(4).gameObject.SetActive(true);
                meshes[4].material = windowMaterial;
                break;
            case WallType.redDoor:
                tile.transform.GetChild(4).gameObject.SetActive(true);
                meshes[4].material = redDoorMaterial;
                break;
            case WallType.blueDoor:
                tile.transform.GetChild(4).gameObject.SetActive(true);
                meshes[4].material = blueDoorMaterial;
                break;
            case WallType.brownDoor:
                tile.transform.GetChild(4).gameObject.SetActive(true);
                meshes[4].material = brownDoorMaterial;
                break;
            case WallType.purpleDoor:
                tile.transform.GetChild(4).gameObject.SetActive(true);
                meshes[4].material = purpleDoorMaterial;
                break;
            case WallType.none:
            default:
                tile.transform.GetChild(4).gameObject.SetActive(false);
                break;
        }

    }

    public void SetWestWall ( int row, int column, WallType type )
    {
        GameObject tile = allTiles[row, column];
        TileComponent tileComponent = tile.GetComponent<TileComponent>();
        tileComponent.Tile.westWallType = type;

        MeshRenderer[] meshes = tile.GetComponentsInChildren<MeshRenderer>(true);

        switch (type)
        {
            case WallType.wall:
                tile.transform.GetChild(5).gameObject.SetActive(true);
                meshes[5].material = wallMaterial;
                break;
            case WallType.window:
                tile.transform.GetChild(5).gameObject.SetActive(true);
                meshes[5].material = windowMaterial;
                break;
            case WallType.redDoor:
                tile.transform.GetChild(5).gameObject.SetActive(true);
                meshes[5].material = redDoorMaterial;
                break;
            case WallType.blueDoor:
                tile.transform.GetChild(5).gameObject.SetActive(true);
                meshes[5].material = blueDoorMaterial;
                break;
            case WallType.purpleDoor:
                tile.transform.GetChild(5).gameObject.SetActive(true);
                meshes[5].material = purpleDoorMaterial;
                break;
            case WallType.brownDoor:
                tile.transform.GetChild(5).gameObject.SetActive(true);
                meshes[5].material = brownDoorMaterial;
                break;
            case WallType.none:
            default:
                tile.transform.GetChild(5).gameObject.SetActive(false);
                break;
        }
    }

    public void SetPickupType ( int row, int column, PickupType type, float pickupCount )
    {
        GameObject tile = allTiles[row, column];
        TileComponent tileComponent = tile.GetComponent<TileComponent>();

        MeshRenderer[] meshes = tile.GetComponentsInChildren<MeshRenderer>(true);

        tileComponent.Tile.pickupType = type;
        tileComponent.Tile.pickupCount = pickupCount;

        // TODO: comment back in when prefab fixed
        //switch (type)
        //{
        //    case PickupType.fatiguePickup:
        //        tile.transform.GetChild(6).gameObject.SetActive(true);
        //        meshes[6].material = fatiguePickupMaterial;
        //        break;
        //    case PickupType.crateMovePickup:
        //        tile.transform.GetChild(6).gameObject.SetActive(true);
        //        meshes[6].material = crateMovePickupMaterial;
        //        break;
        //    case PickupType.timePickup:
        //        tile.transform.GetChild(6).gameObject.SetActive(true);
        //        meshes[6].material = timePickupMaterial;
        //        break;
        //    case PickupType.none:
        //    default:
        //        tile.transform.GetChild(6).gameObject.SetActive(false);
        //        break;
        //}
    }
}
