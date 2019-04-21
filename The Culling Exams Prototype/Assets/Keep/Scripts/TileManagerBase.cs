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

    public Material crateMaterial;
    public Material mirrorMaterial;

    public Material wallMaterial;
    public Material doorMaterial;
    public Material windowMaterial;

    // Dynamic Materials
    public Material normalMaterial;
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
                case TileType.barrier:
                    tile.transform.GetChild(6).gameObject.SetActive(true);
                    meshes[0].material = barrierMaterial;
                    break;
                case TileType.finish:
                    tile.transform.GetChild(6).gameObject.SetActive(false);
                    meshes[0].material = finishMaterial;
                    break;
                case TileType.start:
                    tile.transform.GetChild(6).gameObject.SetActive(false);
                    meshes[0].material = startMaterial;
                    break;
                case TileType.moveable:
                    tile.transform.GetChild(6).gameObject.SetActive(false);
                    meshes[0].material = moveableMaterial;
                    break;
                case TileType.fall:
                    tile.transform.GetChild(6).gameObject.SetActive(false);
                    meshes[0].material = fallMaterial;
                    break;

                case TileType.basic:
                default:
                    tile.transform.GetChild(6).gameObject.SetActive(false);
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
                case WallType.door:
                    tile.transform.GetChild(2).gameObject.SetActive(true);
                    meshes[2].material = doorMaterial;
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
                case WallType.door:
                    tile.transform.GetChild(3).gameObject.SetActive(true);
                    meshes[3].material = doorMaterial;
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
                case WallType.door:
                    tile.transform.GetChild(4).gameObject.SetActive(true);
                    meshes[4].material = doorMaterial;
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
                case WallType.door:
                    tile.transform.GetChild(5).gameObject.SetActive(true);
                    meshes[5].material = doorMaterial;
                    break;
                case WallType.none:
                default:
                    tile.transform.GetChild(5).gameObject.SetActive(false);
                    break;
            }
        }
    }

    protected void SetDynamicValues ()
    {
        for (int row = 0; row < rows; row++)
        {
            for (int column = 0; column < columns; column++)
            {
                GameObject tile = allTiles[row, column];

                TileComponent tileComponent = tile.GetComponent<TileComponent>();
                MeshRenderer[] meshes = tile.GetComponentsInChildren<MeshRenderer>(true);

                switch (tileComponent.Tile.crateType)
                {
                    case CrateType.crate:
                        tile.transform.GetChild(1).gameObject.SetActive(true);
                        meshes[1].material = crateMaterial;
                        break;
                    case CrateType.mirror:
                        tile.transform.GetChild(1).gameObject.SetActive(true);
                        meshes[1].material = mirrorMaterial;
                        break;
                    case CrateType.crateHighlighted:
                        tile.transform.GetChild(1).gameObject.SetActive(true);
                        meshes[1].material = highlightMaterial;
                        break;
                    case CrateType.crateSelected:
                        tile.transform.GetChild(1).gameObject.SetActive(true);
                        meshes[1].material = selectedMaterial;
                        break;
                    case CrateType.crateGhost:
                        tile.transform.GetChild(1).gameObject.SetActive(true);
                        meshes[1].material = ghostMaterial;
                        break;
                    case CrateType.none:
                    default:
                        tile.transform.GetChild(1).gameObject.SetActive(false);
                        break;
                }
            }
        }
    }
}
