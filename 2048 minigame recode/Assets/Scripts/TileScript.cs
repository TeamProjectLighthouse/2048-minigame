using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TileScript : MonoBehaviour
{

    public GameObject tileTextureDatabasePrefab;
    TileTextureDatabaseScript tileTextureDatabase;
    List<Sprite> tileSpriteList;

    public GameObject gridManager;
    private GridManagerScript gridManagerScript;

    public Vector3 posAfterMoving;

    public bool isDeletedNextFrame = false;
    public bool alreadyMergedThisMove = false;
    public int tileValue;
    public void Initiation()
    {
        gridManager = GameObject.Find("Grid Manager");
        gridManagerScript = gridManager.GetComponent<GridManagerScript>();
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

    public void MoveTile(Vector3 moveDirection)
    {
        if (!isDeletedNextFrame)
        {
            //Debug.Log($"Original posAfterMoving: {posAfterMoving}");
            posAfterMoving = FindPosToMove(moveDirection);
            //Debug.Log($"posAfterMoving: {posAfterMoving}");
        }

    }

    public void Move(Vector3 finalPos)
    {
        transform.position = finalPos;
    }

    public Vector3 FindPosToMove(Vector3 moveDirection)
    {
        Vector3 originalPosition = posAfterMoving;
        Vector3 finalPosition = originalPosition;
        while (!Vector3IsOutOfBounds(finalPosition))
        {
            Vector3 nextPosition = finalPosition+moveDirection;
            if (Vector3IsOutOfBounds(nextPosition))
            {
                return finalPosition;
            }
            foreach (GameObject tile in gridManagerScript.tileList)
            {
                if (tile.GetComponent<TileScript>().posAfterMoving == nextPosition && !tile.GetComponent<TileScript>().isDeletedNextFrame)
                {
                    return finalPosition;
                }
            }
            finalPosition += moveDirection;
        }
        return finalPosition - moveDirection;
    }

    public bool MergeTile(Vector3 moveDirection)
    {
        Vector3 posToCheck = posAfterMoving + moveDirection;
        GameObject tileToMerge = FindTileWithPos(posToCheck);
        if (tileToMerge != null)
        {
            TileScript targetTileScript = tileToMerge.GetComponent<TileScript>();
            if (targetTileScript == null)
            {
                Debug.LogError("script of target tile in merge is null HELP AHHH");
            }
            int targetTileValue = targetTileScript.tileValue;
            if (tileValue == targetTileValue && !targetTileScript.alreadyMergedThisMove)
            {
                posAfterMoving = posToCheck;
                targetTileScript.isDeletedNextFrame = true;
                tileValue += 1;
                UpdateSprite(tileValue);
                alreadyMergedThisMove = true;
                return true;
            }
        }
        return false;
    }

    public GameObject FindTileWithPos(Vector3 pos)
    {
        foreach (GameObject tile in gridManagerScript.tileList)
        {
            if (tile.GetComponent<TileScript>().posAfterMoving == pos)
            {
                return tile;
            }
        }
        return null;
    }
    public int GetPriority(Vector3 moveDirection)
    {
        Vector3 pos = posAfterMoving;
        int priority;
        if (moveDirection == new Vector3(0, 1, 0)) //up
        {
            priority = (int)(pos.x + (-pos.y)*4);
        } else if (moveDirection == new Vector3(0, -1, 0)) { //down
            priority = (int)(pos.x + (5 - (-pos.y))*4);
        } else if (moveDirection == new Vector3(-1, 0, 0)) { //left
            priority = (int)((pos.x)*4 + (-pos.y));
        } else { //right
            priority = (int)((5-pos.x)*4 + (-pos.y));
        }
        priority -= 4;
        return priority;
    }

    private bool Vector3IsOutOfBounds(Vector3 pos)
    {
        return pos.x > 4 || pos.x < 1 || -pos.y > 4 || -pos.y < 1;
    }

    void Start()
    {

    }  
}
