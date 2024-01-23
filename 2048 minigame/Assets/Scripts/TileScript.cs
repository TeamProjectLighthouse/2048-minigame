using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class TileScript : MonoBehaviour
{

    private GameObject grid;
    public GameObject tileTextureDatabasePrefab;
    TileTextureDatabaseScript tileTextureDatabase;
    List<Sprite> tileSpriteList;

    GridManagerScript.GridPosition gridCoords;
    GridManagerScript gridScript;

    public bool locked;

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

    public bool MoveTile(GridManagerScript.GridPosition tile, GridManagerScript.Direction direction)
    {
        GridManagerScript.GridPosition oldTile = new GridManagerScript.GridPosition(tile.row, tile.column);
        GridManagerScript.GridPosition newTile = UpdateTilePos(tile, direction);
        
        StartCoroutine(Animate(gridScript.GridPositionToVector3(newTile), false));

        gridScript.UpdateGridTiles(newTile, oldTile);

        Debug.Log($"Moved tile from ({oldTile.row},{oldTile.column}) to ({newTile.row},{newTile.column})");

        GameObject adjacentTile = GetAdjacentCell(newTile, direction);
        TileScript adjacentScript = adjacentTile.GetComponent<TileScript>();

        if (!(adjacentTile == null))
        {
            if (gridScript.gridValues[adjacentTile] == gridScript.gridValues[gameObject] && !adjacentScript.locked)
            {
                Merge(gameObject, adjacentTile);
                return true;
            }
        }

        return false;
    }

    private GridManagerScript.GridPosition UpdateTilePos(GridManagerScript.GridPosition newTile, GridManagerScript.Direction direction)
    {
        GameObject adjacentTile = GetAdjacentCell(newTile, direction);
        TileScript adjacentScript = adjacentTile.GetComponent<TileScript>();

        //GridManagerScript.GridPosition adjacentPos = gridScript.gridTiles.FirstOrDefault(x => x.Value == adjacentTile).Key;

        GridManagerScript.GridPosition lastCheckedTile = new GridManagerScript.GridPosition(newTile.row, newTile.column);
        MoveTileInGridTile(newTile, direction);

        while (!gridScript.HasTile(newTile.row, newTile.column) && IsWithinGridBounds(newTile))
        {
            if (!IsWithinGridBounds(newTile) || gridScript.HasTile(newTile.row, newTile.column))
            {
                break;
            }
            lastCheckedTile = new GridManagerScript.GridPosition(newTile.row, newTile.column);
            MoveTileInGridTile(newTile, direction);
        }
        return lastCheckedTile;
    }

    private bool IsWithinGridBounds(GridManagerScript.GridPosition position)
    {
        return position.row >= 1 && position.row <= 4 && position.column >= 1 && position.column <= 4;
    }

    private void MoveTileInGridTile(GridManagerScript.GridPosition newTile, GridManagerScript.Direction direction)
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
    }

    private IEnumerator Animate(Vector3 to, bool merging)
    {
        float elapsed = 0f;
        float duration = 0.1f;

        Vector3 from = transform.position;

        while (elapsed < duration)
        {
            transform.position = Vector3.Lerp(from, to, elapsed / duration);
            elapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = to;

        if (merging)
        {
            Destroy(gameObject);
        }
    }

    private GameObject GetAdjacentCell(GridManagerScript.GridPosition tile, GridManagerScript.Direction direction)
    {
        GridManagerScript.GridPosition adjacent = new GridManagerScript.GridPosition(tile.row, tile.column);

        if (direction == GridManagerScript.Direction.Up)
        {
            adjacent.row -= 1;
        }
        else if (direction == GridManagerScript.Direction.Down)
        {
            adjacent.row += 1;
        }
        else if (direction == GridManagerScript.Direction.Left)
        {
            adjacent.column -= 1;
        }
        else if (direction == GridManagerScript.Direction.Right)
        {
            adjacent.column += 1;
        }
        else
        {
            return null; // won't happen
        }

        return gridScript.gridTiles[adjacent];
    }

    private void Merge(GameObject a, GameObject b)
    {
        GridManagerScript.GridPosition aPos = gridScript.gridTiles.FirstOrDefault(x => x.Value == a).Key;
        GridManagerScript.GridPosition bPos = gridScript.gridTiles.FirstOrDefault(x => x.Value == b).Key;

        TileScript aScript = a.GetComponent<TileScript>();
        TileScript bScript = b.GetComponent<TileScript>();

        gridScript.gridTiles.Remove(aPos);
        bScript.locked = true;
        gridScript.gridValues[b] = 2;
        bScript.UpdateSprite(2);

        StartCoroutine(Animate(gridScript.GridPositionToVector3(bPos), true));
    }
}
