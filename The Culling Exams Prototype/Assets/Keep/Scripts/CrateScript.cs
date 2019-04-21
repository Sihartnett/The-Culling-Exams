using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CrateScript : MonoBehaviour
{
    void Start ()
    {
    }

    public void Highlight ( bool highlight )
    {
        TileComponent tileComponent = this.transform.parent.GetComponent<TileComponent>();
        TilePlayManager playManager = this.transform.parent.transform.parent.GetComponent<TilePlayManager>();
        
        if (tileComponent.Row == playManager.currentTile.x && playManager.currentTile.y == tileComponent.Column)
            return;

        if (tileComponent.Tile.crateType == CrateType.none)
            return;

        // Deselect any highlight
        if (!highlight)
        {
            if(tileComponent.Tile.crateType == CrateType.crateHighlighted)
                tileComponent.Tile.crateType = CrateType.crate;
        }

        // Highlight Ranges Nothing Selected
        if (playManager.selectedTile == null)
        {
            if (tileComponent.Row >= playManager.currentTile.x - 1 && playManager.currentTile.x + 1 >= tileComponent.Row
                && tileComponent.Column >= playManager.currentTile.y - 1 && playManager.currentTile.y + 1 >= tileComponent.Column)
            {
                if (highlight)
                {
                    tileComponent.Tile.crateType = CrateType.crateHighlighted;
                }
            }
        }
        else
        {
            //if (tileComponent.Row >= playManager.selectedTile.Value.x - 1 && playManager.selectedTile.Value.x + 1 >= tileComponent.Row
            //    && tileComponent.Column >= playManager.selectedTile.Value.y - 1 && playManager.selectedTile.Value.y + 1 >= tileComponent.Column)
            //{
            //    if (highlight)
            //        tileComponent.Tile.crateType = CrateType.crateHighlighted;
            //    else
            //        tileComponent.Tile.crateType = CrateType.crateGhost;
            //}
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

                // Set the tiles around me as ghosts

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

                // remove the tiles around me as ghosts
            }
        }
    }
}
