using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateScript : MonoBehaviour
{
    public SelectionType type;

    void Start ()
    {
    }

    public void Highlight ( bool highlight )
    {
        TileComponent tileComponent = this.transform.parent.GetComponent<TileComponent>();
        TilePlayManager playManager = this.transform.parent.transform.parent.GetComponent<TilePlayManager>();
        
        // Do nothing Ever if im standing on the tile
        if (tileComponent.Row == playManager.currentTile.x && playManager.currentTile.y == tileComponent.Column)
            return;
        
        // Deselect any highlight
        if (!highlight)
        {
            if (tileComponent.Tile.crateType == CrateType.crateHighlighted)
                tileComponent.Tile.crateType = CrateType.crate;
            if (tileComponent.Tile.crateType == CrateType.crateGhostHighlighted)
                tileComponent.Tile.crateType = CrateType.crateGhost;

            if (tileComponent.Tile.crateType == CrateType.mirrorHighlighted)
                tileComponent.Tile.crateType = CrateType.mirror;
            if (tileComponent.Tile.crateType == CrateType.mirrorGhostHighlighted)
                tileComponent.Tile.crateType = CrateType.mirrorGhost;

            if (tileComponent.Tile.tileType == TileType.moveableHighlighted)
                tileComponent.Tile.tileType = TileType.moveable;
            if (tileComponent.Tile.tileType == TileType.fallHighlighted)
                tileComponent.Tile.tileType = TileType.fallGhost;

            playManager.selectionType = SelectionType.none;
        }

        // If I have a selection then I can only select on thoes types
        if (playManager.selectedTile != null && playManager.selectionType != this.type)
            return;

        // Highlight Ranges Nothing Selected
        if (playManager.selectedTile == null)
        {
            if (tileComponent.Row >= playManager.currentTile.x - 1 && playManager.currentTile.x + 1 >= tileComponent.Row
                && tileComponent.Column >= playManager.currentTile.y - 1 && playManager.currentTile.y + 1 >= tileComponent.Column)
            {
                if (highlight)
                {
                    if (this.type == SelectionType.crate)
                        tileComponent.Tile.crateType = CrateType.crateHighlighted;

                    if (this.type == SelectionType.tile)
                        tileComponent.Tile.tileType = TileType.moveableHighlighted;
                }
            }
        }
        else
        {
            // This is when there is a current selection
            // Only show highlights of the currently selected object

            if (tileComponent.Row >= playManager.selectedTile.Value.x - 1 && playManager.selectedTile.Value.x + 1 >= tileComponent.Row
                && tileComponent.Column >= playManager.selectedTile.Value.y - 1 && playManager.selectedTile.Value.y + 1 >= tileComponent.Column)
            {
                if (highlight)
                {
                    if (this.type == SelectionType.crate)
                    {
                        tileComponent.Tile.crateType = CrateType.crateGhostHighlighted;
                    }
                    else if (this.type == SelectionType.tile)
                    {
                        tileComponent.Tile.tileType = TileType.fallHighlighted;
                    }
                }
            }
        }
    }

    public void Select (bool select)
    {
        TileComponent tileComponent = this.transform.parent.GetComponent<TileComponent>();
        TilePlayManager playManager = this.transform.parent.transform.parent.GetComponent<TilePlayManager>();

        if (select)
        {
            if (playManager.selectedTile == null)
            {
                playManager.selectedTile = new Vector2(tileComponent.Row, tileComponent.Column);
                tileComponent.Tile.crateType = CrateType.crateSelected;

                // check all four directions for empty slots 

                TileComponent tile = playManager.allTiles[tileComponent.Row - 1, tileComponent.Column].GetComponent<TileComponent>();
                if (tile.Row != playManager.currentTile.x && tile.Column != playManager.currentTile.y &&
                    tile.Tile.crateType == CrateType.none && tile.Tile.tileType == TileType.basic)
                    tile.Tile.crateType = CrateType.crateGhost;

                tile = playManager.allTiles[tileComponent.Row + 1, tileComponent.Column].GetComponent<TileComponent>();
                if (tile.Row != playManager.currentTile.x && tile.Column != playManager.currentTile.y && 
                    tile.Tile.crateType == CrateType.none && tile.Tile.tileType == TileType.basic)
                    tile.Tile.crateType = CrateType.crateGhost;

                tile = playManager.allTiles[tileComponent.Row, tileComponent.Column + 1].GetComponent<TileComponent>();
                if (tile.Row != playManager.currentTile.x && tile.Column != playManager.currentTile.y && 
                    tile.Tile.crateType == CrateType.none && tile.Tile.tileType == TileType.basic)
                    tile.Tile.crateType = CrateType.crateGhost;

                tile = playManager.allTiles[tileComponent.Row, tileComponent.Column - 1].GetComponent<TileComponent>();
                if (tile.Row != playManager.currentTile.x && tile.Column != playManager.currentTile.y && 
                    tile.Tile.crateType == CrateType.none && tile.Tile.tileType == TileType.basic)
                    tile.Tile.crateType = CrateType.crateGhost;

            }
            else
            {
                // if im clicking on a valid selection from the selected tile do the stuff
            }
        }
        else
        {
            if (playManager.selectedTile != null)
            {
                playManager.selectedTile = null;
                tileComponent.Tile.crateType = CrateType.crate;

                TileComponent tile = playManager.allTiles[tileComponent.Row - 1, tileComponent.Column].GetComponent<TileComponent>();
                if (tile.Tile.crateType == CrateType.crateGhost)
                    tile.Tile.crateType = CrateType.none;

                tile = playManager.allTiles[tileComponent.Row + 1, tileComponent.Column].GetComponent<TileComponent>();
                if (tile.Tile.crateType == CrateType.crateGhost)
                    tile.Tile.crateType = CrateType.none;

                tile = playManager.allTiles[tileComponent.Row, tileComponent.Column + 1].GetComponent<TileComponent>();
                if (tile.Tile.crateType == CrateType.crateGhost)
                    tile.Tile.crateType = CrateType.none;

                tile = playManager.allTiles[tileComponent.Row, tileComponent.Column - 1].GetComponent<TileComponent>();
                if (tile.Tile.crateType == CrateType.crateGhost)
                    tile.Tile.crateType = CrateType.none;
            }
        }
    }
}
