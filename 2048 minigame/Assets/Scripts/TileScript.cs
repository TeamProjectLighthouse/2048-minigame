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
        GridManagerScript.GridPosition oldTile = new GridManagerScript.GridPosition(tile.row, tile.column);
        GridManagerScript.GridPosition newTile = UpdateTilePos(tile, direction);

        StartCoroutine(Animate(gridScript.GridPositionToVector3(newTile)));

        gridScript.UpdateGridTiles(newTile, oldTile);

        Debug.Log($"Moved tile from ({oldTile.row},{oldTile.column}) to ({newTile.row},{newTile.column})");

        return newTile;
    }

    private GridManagerScript.GridPosition UpdateTilePos(GridManagerScript.GridPosition newTile, GridManagerScript.Direction direction)
    {
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

    private IEnumerator Animate(Vector3 to)
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
    }
}
