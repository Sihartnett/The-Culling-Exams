﻿using System;
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

    // variable to store the max fatigue & checking the blood vignette
    private int initFatigue = 0;
    private int bloodFatigue = 0;
    private bool vignetteAppliedFirst = false;

    // Start is called before the first frame update
    void Start()
    {
        if (tileMap != null && (tileMap.rowCount != rows || tileMap.columnCount != columns))
        {
            Debug.Log("ScirptableObjectDoes not match tiles");
            return;
        }

        // Create all GameObjects

        initFatigue = fatigue;
        vignetteAppliedFirst = false;
        bloodFatigue = 0;
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

        CrateScript.Deselect();
    }

    // Update is called once per frame
    void Update()
    {
        if (Menus.PauseMenuManager.Instance != null && Menus.PauseMenuManager.Instance.Paused)
            return;

        // If the player moves make sure to deselect everything
        player.Movement(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"), player.gameObject.transform);
        player.CameraOrbit(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y"));

        // Managing the blood vignette based on fatigue

        if(fatigue < initFatigue * 0.3f)
        {
            if(player.postBehavior && !vignetteAppliedFirst)
            {
                player.vigMod.intensity = 0.5f;                
                vignetteAppliedFirst = true;
                bloodFatigue = fatigue;
            }
            if(vignetteAppliedFirst && player.isMoving)
            {
                player.vigMod.intensity = 0.5f + (0.5f * (bloodFatigue - fatigue) / bloodFatigue);
            }
            player.postBehavior.profile.vignette.settings = player.vigMod;
        }

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

                if (Input.GetMouseButtonDown(0))
                {
                    crateScript.Select();
                }

                //if (Input.GetMouseButtonDown(1))
                //{
                //    crateScript.ShootRay(this, player.gameObject);
                //}
            }
        }
    }

    public void SetCrate(int row, int column, CrateType type, ObjectState state)
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
                tile.transform.GetChild(7).gameObject.SetActive(true);
                meshes[7].material = crateHighlightMaterial;
                break;
            case ObjectState.selected:
                tile.transform.GetChild(1).gameObject.SetActive(true);
                tile.transform.GetChild(7).gameObject.SetActive(true);
                meshes[7].material = crateSelectedMaterial;
                break;
            case ObjectState.ghost:
                tile.transform.GetChild(1).gameObject.SetActive(true);
                tile.transform.GetChild(7).gameObject.SetActive(false);
                meshes[1].material = crateGhostMaterial;
                break;
            case ObjectState.ghostHighlighted:
                tile.transform.GetChild(1).gameObject.SetActive(true);
                tile.transform.GetChild(7).gameObject.SetActive(true);
                meshes[7].material = crateHighlightMaterial;
                break;
            case ObjectState.none:
            default:
                {
                    tile.transform.GetChild(7).gameObject.SetActive(false);

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
                        case CrateType.orangeCrate:
                            tile.transform.GetChild(1).gameObject.SetActive(true);
                            meshes[1].material = orangeCrateMaterial;
                            break;
                        case CrateType.lightBlueCrate:
                            tile.transform.GetChild(1).gameObject.SetActive(true);
                            meshes[1].material = lightBlueCrateMaterial;
                            break;
                        case CrateType.mirror:
                            tile.transform.GetChild(1).gameObject.SetActive(true);
                            meshes[1].material = mirrorMaterial;
                            break;
                        case CrateType.none:
                        default:
                            tile.transform.GetChild(1).gameObject.SetActive(false);
                            meshes[1].material = crateMaterial;
                            break;
                    }
                }
                break;
        }
    }

    public void SetTile(int row, int column, TileType type, ObjectState state)
    {
        GameObject tile = allTiles[row, column];

        TileComponent tileComponent = tile.GetComponent<TileComponent>();
        tileComponent.Tile.tileType = type;
        tileComponent.Tile.tileState = state;

        MeshRenderer[] meshes = tile.GetComponentsInChildren<MeshRenderer>(true);

        switch (state)
        {
            case ObjectState.highlighted:
                tile.transform.GetChild(8).gameObject.SetActive(true);
                meshes[8].material = tileHighlightMaterial;
                break;
            case ObjectState.selected:
                tile.transform.GetChild(8).gameObject.SetActive(true);
                meshes[8].material = tileSelectedMaterial;
                break;
            case ObjectState.ghost:
                tile.transform.GetChild(8).gameObject.SetActive(false);
                meshes[0].material = tileGhostMaterial;
                break;
            case ObjectState.ghostHighlighted:
                tile.transform.GetChild(8).gameObject.SetActive(true);
                meshes[8].material = tileHighlightMaterial;
                break;
            case ObjectState.none:
            default:
                {
                    tile.transform.GetChild(8).gameObject.SetActive(false);

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
                        case TileType.orangeTile:
                            meshes[0].material = orangeTileMaterial;
                            break;
                        case TileType.lightBlueTile:
                            meshes[0].material = lightBlueTileMaterial;
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

    public void SetNorthWall(int row, int column, WallType type)
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
            case WallType.orangeDoor:
                tile.transform.GetChild(2).gameObject.SetActive(true);
                meshes[2].material = orangeDoorMaterial;
                break;
            case WallType.lightBlueDoor:
                tile.transform.GetChild(2).gameObject.SetActive(true);
                meshes[2].material = lightBlueDoorMaterial;
                break;
            case WallType.none:
            default:
                tile.transform.GetChild(2).gameObject.SetActive(false);
                break;
        }
    }

    public void SetSouthWall(int row, int column, WallType type)
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
            case WallType.orangeDoor:
                tile.transform.GetChild(3).gameObject.SetActive(true);
                meshes[3].material = orangeDoorMaterial;
                break;
            case WallType.lightBlueDoor:
                tile.transform.GetChild(3).gameObject.SetActive(true);
                meshes[3].material = lightBlueDoorMaterial;
                break;
            case WallType.none:
            default:
                tile.transform.GetChild(3).gameObject.SetActive(false);
                break;
        }

    }

    public void SetEastWall(int row, int column, WallType type)
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
            case WallType.orangeDoor:
                tile.transform.GetChild(4).gameObject.SetActive(true);
                meshes[4].material = orangeDoorMaterial;
                break;
            case WallType.lightBlueDoor:
                tile.transform.GetChild(4).gameObject.SetActive(true);
                meshes[4].material = lightBlueDoorMaterial;
                break;
            case WallType.none:
            default:
                tile.transform.GetChild(4).gameObject.SetActive(false);
                break;
        }

    }

    public void SetWestWall(int row, int column, WallType type)
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
            case WallType.orangeDoor:
                tile.transform.GetChild(5).gameObject.SetActive(true);
                meshes[5].material = orangeDoorMaterial;
                break;
            case WallType.lightBlueDoor:
                tile.transform.GetChild(5).gameObject.SetActive(true);
                meshes[5].material = lightBlueDoorMaterial;
                break;
            case WallType.none:
            default:
                tile.transform.GetChild(5).gameObject.SetActive(false);
                break;
        }
    }

    public void SetPickupType(int row, int column, PickupType type, float pickupCount)
    {
        GameObject tile = allTiles[row, column];
        TileComponent tileComponent = tile.GetComponent<TileComponent>();

        MeshRenderer[] meshes = tile.GetComponentsInChildren<MeshRenderer>(true);

        tileComponent.Tile.pickupType = type;
        tileComponent.Tile.pickupCount = pickupCount;

        switch (type)
        {
            case PickupType.fatiguePickup:
                tile.transform.GetChild(6).gameObject.SetActive(true);
                meshes[6].material = fatiguePickupMaterial;
                break;
            case PickupType.crateMovePickup:
                tile.transform.GetChild(6).gameObject.SetActive(true);
                meshes[6].material = crateMovePickupMaterial;
                break;
            case PickupType.timePickup:
                tile.transform.GetChild(6).gameObject.SetActive(true);
                meshes[6].material = timePickupMaterial;
                break;
            case PickupType.none:
            default:
                tile.transform.GetChild(6).gameObject.SetActive(false);
                break;
        }
    }
}
