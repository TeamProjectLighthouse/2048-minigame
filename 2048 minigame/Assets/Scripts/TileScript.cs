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
    }
    public GameObject tileTextureDatabasePrefab;
    TileTextureDatabaseScript tileTextureDatabase;
    List<Sprite> tileSpriteList;

    GridPosition gridCoords;
    void Start()
    {

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
    // Update is called once per frame
    /*void Update()
    {
        
     } */
}
