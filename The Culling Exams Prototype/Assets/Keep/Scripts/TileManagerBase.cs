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
    public Material currentMaterial;
    
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
            Tile scriptedTile = tile.GetComponent<Tile>();
            MeshRenderer[] meshes = tile.GetComponentsInChildren<MeshRenderer>(true);

            int row = scriptedTile.Row;
            int column = scriptedTile.Column;

            TileMap.Row.Tile tileMapTile = tileMap.rows[row].column[column];

            // Switch on tile type
            switch (tileMapTile.tileType)
            {
                case TileMap.TileType.barrier:
                    tile.transform.GetChild(6).gameObject.SetActive(true);
                    meshes[0].material = barrierMaterial;
                    break;
                case TileMap.TileType.finish:
                    tile.transform.GetChild(6).gameObject.SetActive(false);
                    meshes[0].material = finishMaterial;
                    break;
                case TileMap.TileType.start:
                    tile.transform.GetChild(6).gameObject.SetActive(false);
                    meshes[0].material = startMaterial;
                    break;
                case TileMap.TileType.moveable:
                    tile.transform.GetChild(6).gameObject.SetActive(false);
                    meshes[0].material = moveableMaterial;
                    break;
                case TileMap.TileType.fall:
                    tile.transform.GetChild(6).gameObject.SetActive(false);
                    meshes[0].material = fallMaterial;
                    break;

                case TileMap.TileType.basic:
                default:
                    tile.transform.GetChild(6).gameObject.SetActive(false);
                    meshes[0].material = defaultMaterial;
                    break;
            }

            // Switch on crate type
            transform.rotation = Quaternion.Euler(tileMapTile.rotation);
            switch (tileMapTile.crateType)
            {
                case TileMap.CrateType.crate:
                    tile.transform.GetChild(1).gameObject.SetActive(true);
                    meshes[1].material = crateMaterial;
                    break;
                case TileMap.CrateType.mirror:
                    tile.transform.GetChild(1).gameObject.SetActive(true);
                    meshes[1].material = mirrorMaterial;
                    break;
                case TileMap.CrateType.none:
                default:
                    tile.transform.GetChild(1).gameObject.SetActive(false);
                    break;
            }

            // Switch for all the walls

            switch (tileMapTile.northWallType)
            {
                case TileMap.WallType.wall:
                    tile.transform.GetChild(2).gameObject.SetActive(true);
                    meshes[2].material = wallMaterial;
                    break;
                case TileMap.WallType.window:
                    tile.transform.GetChild(2).gameObject.SetActive(true);
                    meshes[2].material = windowMaterial;
                    break;
                case TileMap.WallType.door:
                    tile.transform.GetChild(2).gameObject.SetActive(true);
                    meshes[2].material = doorMaterial;
                    break;
                case TileMap.WallType.none:
                default:
                    tile.transform.GetChild(2).gameObject.SetActive(false);
                    break;
            }
            switch (tileMapTile.southWallType)
            {
                case TileMap.WallType.wall:
                    tile.transform.GetChild(3).gameObject.SetActive(true);
                    meshes[3].material = wallMaterial;
                    break;
                case TileMap.WallType.window:
                    tile.transform.GetChild(3).gameObject.SetActive(true);
                    meshes[3].material = windowMaterial;
                    break;
                case TileMap.WallType.door:
                    tile.transform.GetChild(3).gameObject.SetActive(true);
                    meshes[3].material = doorMaterial;
                    break;
                case TileMap.WallType.none:
                default:
                    tile.transform.GetChild(3).gameObject.SetActive(false);
                    break;
            }
            switch (tileMapTile.eastWallType)
            {
                case TileMap.WallType.wall:
                    tile.transform.GetChild(4).gameObject.SetActive(true);
                    meshes[4].material = wallMaterial;
                    break;
                case TileMap.WallType.window:
                    tile.transform.GetChild(4).gameObject.SetActive(true);
                    meshes[4].material = windowMaterial;
                    break;
                case TileMap.WallType.door:
                    tile.transform.GetChild(4).gameObject.SetActive(true);
                    meshes[4].material = doorMaterial;
                    break;
                case TileMap.WallType.none:
                default:
                    tile.transform.GetChild(4).gameObject.SetActive(false);
                    break;
            }
            switch (tileMapTile.westWallType)
            {
                case TileMap.WallType.wall:
                    tile.transform.GetChild(5).gameObject.SetActive(true);
                    meshes[5].material = wallMaterial;
                    break;
                case TileMap.WallType.window:
                    tile.transform.GetChild(5).gameObject.SetActive(true);
                    meshes[5].material = windowMaterial;
                    break;
                case TileMap.WallType.door:
                    tile.transform.GetChild(5).gameObject.SetActive(true);
                    meshes[5].material = doorMaterial;
                    break;
                case TileMap.WallType.none:
                default:
                    tile.transform.GetChild(5).gameObject.SetActive(false);
                    break;
            }
        }
    }

    protected void SetDynamicValues ()
    {

        foreach (GameObject tile in allTiles)
        {
            Tile scriptedTile = tile.GetComponent<Tile>();
            MeshRenderer[] meshes = tile.GetComponentsInChildren<MeshRenderer>(true);

            int row = scriptedTile.Row;
            int column = scriptedTile.Column;

            TileMap.Row.Tile tileMapTile = tileMap.rows[row].column[column];
            
            tile.transform.GetChild(1).gameObject.SetActive(false);
        }
    }
}
