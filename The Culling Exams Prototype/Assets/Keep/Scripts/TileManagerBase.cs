using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileManagerBase : MonoBehaviour
{
    public int rows;
    public int columns;

    public GameObject tilePrefab;

    public GameObject[,] allTiles;
    public TileMap tileMap;

    // Map Editor Materials
    public Material defaultMaterial;
    public Material startMaterial;
    public Material finishMaterial;
    public Material barrierMaterial;
    public Material fallMaterial;
    public Material moveableMaterial;
    public Material moveableBarrierMaterial;
    public Material moveableCrateMaterial;
    public Material blueTileMaterial;
    public Material redTileMaterial;
    public Material purpleTileMaterial;
    public Material brownTileMaterial;

    public Material mirrorMaterial;
    public Material crateMaterial;
    public Material redCrateMaterial;
    public Material blueCrateMaterial;
    public Material purpleCrateMaterial;
    public Material brownCrateMaterial;

    public Material wallMaterial;
    public Material redDoorMaterial;
    public Material blueDoorMaterial;
    public Material purpleDoorMaterial;
    public Material brownDoorMaterial;
    public Material windowMaterial;

    // Pickup Materials
    public Material fatiguePickupMaterial;
    public Material crateMovePickupMaterial;
    public Material timePickupMaterial;

    // Dynamic Materials
    public Material highlightMaterial;
    public Material selectedMaterial;
    public Material ghostMaterial;

    // Start is called before the first frame update
    void Start()
    {
        throw new NotImplementedException();
    }

    // Update is called once per frame
    void Update()
    {
        throw new NotImplementedException();
    }

    protected void SetMapValues ()
    {
        foreach (GameObject tile in allTiles)
        {
            TileComponent tileComponent = tile.GetComponent<TileComponent>();
            MeshRenderer[] meshes = tile.GetComponentsInChildren<MeshRenderer>(true);

            int row = tileComponent.Row;
            int column = tileComponent.Column;

            Tile tileMapTile = tileMap.rows[row].column[column];

            // Switch on tile type
            switch (tileMapTile.tileType)
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
                case TileType.moveableCrateTile:
                    meshes[0].material = moveableCrateMaterial;
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

            // Switch on crate type
            transform.rotation = Quaternion.Euler(tileMapTile.rotation);
            switch (tileMapTile.crateType)
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
                case CrateType.purpleCrate:
                    tile.transform.GetChild(1).gameObject.SetActive(true);
                    meshes[1].material = purpleCrateMaterial;
                    break;
                case CrateType.brownCrate:
                    tile.transform.GetChild(1).gameObject.SetActive(true);
                    meshes[1].material = brownCrateMaterial;
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

            // Switch for all the walls
            switch (tileMapTile.northWallType)
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
            switch (tileMapTile.southWallType)
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
                case WallType.brownDoor:
                    tile.transform.GetChild(3).gameObject.SetActive(true);
                    meshes[3].material = brownDoorMaterial;
                    break;
                case WallType.purpleDoor:
                    tile.transform.GetChild(3).gameObject.SetActive(true);
                    meshes[3].material = purpleDoorMaterial;
                    break;
                case WallType.none:
                default:
                    tile.transform.GetChild(3).gameObject.SetActive(false);
                    break;
            }
            switch (tileMapTile.eastWallType)
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
            switch (tileMapTile.westWallType)
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
                case WallType.brownDoor:
                    tile.transform.GetChild(5).gameObject.SetActive(true);
                    meshes[5].material = brownDoorMaterial;
                    break;
                case WallType.purpleDoor:
                    tile.transform.GetChild(5).gameObject.SetActive(true);
                    meshes[5].material = purpleDoorMaterial;
                    break;
                case WallType.none:
                default:
                    tile.transform.GetChild(5).gameObject.SetActive(false);
                    break;
            }

            // TODO: When the prefab is updated then this should work
            //switch (tileMapTile.pickupType)
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
}
