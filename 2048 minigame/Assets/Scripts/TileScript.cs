using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{
    public class GridPosition
    {
        public int row { get; set; }
        public int column { get; set; }

        public GridPosition(int x, int y)
        {
            row = x;
            column = y;
        }

        public static bool operator ==(GridPosition pos1, GridPosition pos2)
        {
            if (ReferenceEquals(pos1, pos2))
            {
                return true;
            }

            if (ReferenceEquals(pos1, null) || ReferenceEquals(pos2, null))
            {
                return false;
            }

            return pos1.row == pos2.row && pos1.column == pos2.column;
        }

        public static bool operator !=(GridPosition pos1, GridPosition pos2)
        {
            return !(pos1 == pos2);
        }

        public override bool Equals(object obj)
        {
            if (obj == null || GetType() != obj.GetType())
            {
                return false;
            }

            GridPosition other = (GridPosition)obj;
            return row == other.row && column == other.column;
        }

        public override int GetHashCode()
        {
            unchecked
            {
                int hash = 17;
                hash = hash * 23 + row.GetHashCode();
                hash = hash * 23 + column.GetHashCode();
                return hash;
            }
        }

        public List<int> GetGridPosition()
        {
            List<int> posList = new List<int> {row, column};
            return posList;
        }
    }

    private GameObject grid;
    public GameObject tileTextureDatabasePrefab;
    TileTextureDatabaseScript tileTextureDatabase;
    List<Sprite> tileSpriteList;

    GridPosition gridCoords;
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

    public void MoveTile(GridManagerScript.GridPosition tile, GridManagerScript.Direction direction)
    {
        GridManagerScript.GridPosition newTile = UpdateTilePos(tile, direction);
        transform.position = gridScript.GridPositionToVector3(newTile);
    }

    private GridManagerScript.GridPosition UpdateTilePos(GridManagerScript.GridPosition newTile, GridManagerScript.Direction direction)
    {
        while (!gridScript.HasTile(newTile.column, newTile.row))
        {
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

            // Check if newTile is within grid bounds
            if (!IsWithinGridBounds(newTile))
            {
                break;
            }

            Debug.Log($"{newTile.column}, {newTile.row}");
        }

        return newTile;
    }

    private bool IsWithinGridBounds(GridManagerScript.GridPosition position)
    {
        return position.row >= 0 && position.row < 4 && position.column >= 0 && position.column < 4;
    }

    //public GridManagerScript.GridPosition Vector3ToGridPosition(Vector3 vector3)
    //{
    //    Vector3 firstPos = new Vector3(1.8f, 1.8f);
    //    float x = vector3.x;
    //    float y = vector3.y;
    //    int gridX = (int)((y - firstPos.y) / 1.2f + 1);
    //    int gridY = (int)((x - firstPos.x) / 1.2f + 1);
    //    GridManagerScript.GridPosition finalPos = new GridManagerScript.GridPosition(gridX, gridY);
    //    return finalPos;
    //}

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
