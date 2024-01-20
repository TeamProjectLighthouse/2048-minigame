using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{

    private GameObject grid;
    public GameObject tileTextureDatabasePrefab;
    TileTextureDatabaseScript tileTextureDatabase;
    List<Sprite> tileSpriteList;

    GridManagerScript.GridPosition gridCoords;
    GridManagerScript gridScript;

    void Start()
    {
        grid = GameObject.Find("Grid Manager");
        gridScript = grid.GetComponent<GridManagerScript>();
    }   
    public void Initiation()
    {
        tileTextureDatabase = tileTextureDatabasePrefab.GetComponent<TileTextureDatabaseScript>();
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        tileSpriteList = new List<Sprite>();
        tileSpriteList.AddRange(new Sprite[] { tileTextureDatabase.tile2, tileTextureDatabase.tile4, tileTextureDatabase.tile8, tileTextureDatabase.tile16, tileTextureDatabase.tile32, tileTextureDatabase.tile64, tileTextureDatabase.tile128, tileTextureDatabase.tile256, tileTextureDatabase.tile512, tileTextureDatabase.tile1024, tileTextureDatabase.tile2048 });
    }
    public void UpdateSprite(int spriteValue)
    {
        SpriteRenderer spriteRenderer = gameObject.GetComponent<SpriteRenderer>();
        if (tileSpriteList == null)
        {
            Debug.Log("it is null");
        }
        spriteRenderer.sprite = tileSpriteList[spriteValue-1];
    }

    public GridManagerScript.GridPosition MoveTile(GridManagerScript.GridPosition tile, GridManagerScript.Direction direction)
    {
        GridManagerScript.GridPosition newTile = UpdateTilePos(tile, direction);
        
        transform.position = gridScript.GridPositionToVector3(newTile);
        return newTile;
    }

    private GridManagerScript.GridPosition UpdateTilePos(GridManagerScript.GridPosition newTile, GridManagerScript.Direction direction)
    {
        GridManagerScript.GridPosition lastCheckedTile = new GridManagerScript.GridPosition(newTile.row, newTile.column);
        if (direction == GridManagerScript.Direction.Up)
        {
            newTile.row -= 1;
        }
        else if (direction == GridManagerScript.Direction.Down)
        {
            newTile.row += 1;
        }
        else if (direction == GridManagerScript.Direction.Left)
        {
            newTile.column -= 1;
        }
        else if (direction == GridManagerScript.Direction.Right)
        {
            newTile.column += 1;
        }


        while (!gridScript.HasTile(newTile.row, newTile.column) && IsWithinGridBounds(newTile))
        {
            if (!IsWithinGridBounds(newTile) || gridScript.HasTile(newTile.row, newTile.column))
            {
                break;
            }
            //Debug.Log($"checking ({newTile.row}, {newTile.column})");
            lastCheckedTile = new GridManagerScript.GridPosition(newTile.row, newTile.column);
            if (direction == GridManagerScript.Direction.Up)
            {
                newTile.row -= 1;

            }
            else if (direction == GridManagerScript.Direction.Down)
            {
                newTile.row += 1;

            }
            else if (direction == GridManagerScript.Direction.Left)
            {
                newTile.column -= 1;

            }
            else if (direction == GridManagerScript.Direction.Right)
            {
                newTile.column += 1;

            }



        }
        Debug.Log($"Final tile: ({lastCheckedTile.row}, {lastCheckedTile.column})");
        return lastCheckedTile;
    }

    private bool IsWithinGridBounds(GridManagerScript.GridPosition position)
    {
        return position.row >= 1 && position.row <= 4 && position.column >= 1 && position.column <= 4;
    }


    //public void Merge(GridManagerScript.Direction direction)
    //{
    //    Debug.Log("ok");
    //    GridManagerScript.GridPosition gridPosition = Vector3ToGridPosition(transform.position);
    //    Debug.Log(gridPosition.row);
    //    TileScript adjacent = FindAdjacent(gridPosition, direction);
    //}

    //private TileScript FindAdjacent(GridManagerScript.GridPosition tile, GridManagerScript.Direction direction)
    //{
    //    //if (direction == GridManagerScript.Direction.Right)
    //    //{
    //        List<GridManagerScript.GridPosition> sameRow;
    //        sameRow = new List<GridManagerScript.GridPosition>();

    //    foreach (var pos in grid.gridTiles.Keys)
    //    {
    //        if (tile.row == pos.row)
    //        {
    //            sameRow.Add(pos);
    //        }
    //    }
    //    Debug.Log(sameRow);

    //        return null;    
    //   // }
    //}


    // Update is called once per frame
    /*void Update()
    {
        
     } */
}
