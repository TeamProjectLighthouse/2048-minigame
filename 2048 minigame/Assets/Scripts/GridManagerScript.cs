using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;
using System;
using System.Linq;

public class GridManagerScript : MonoBehaviour
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
    public GameObject tilePrefab;
    public GameObject tileTextureDatabasePrefab;
    public Dictionary<GameObject, int> gridValues = new Dictionary<GameObject, int>();
    public Dictionary<GridPosition, GameObject> gridTiles = new Dictionary<GridPosition, GameObject>();



    private void StartGame()
    {
        InstantiateTile(1);
        InstantiateTile(1);
    }

    public GridPosition GetRandomGridPosition()
    {
        GridPosition randomPos = new GridPosition(UnityEngine.Random.Range(1, 4), UnityEngine.Random.Range(1, 4));
        while (HasTile(randomPos.row,randomPos.column))
        {
            randomPos = new GridPosition(UnityEngine.Random.Range(1, 4), UnityEngine.Random.Range(1, 4));
        }
        return randomPos;
    }

    public Vector3 GridPositionToVector3(GridPosition pos)
    {
        List<int> gridPos = pos.GetGridPosition();
        Vector3 vector3Pos = new Vector3((-1.8f + (gridPos[1] - 1) * 1.2f), (1.8f - (gridPos[0] - 1) * 1.2f), 0);
        return vector3Pos;
    }


    public GridPosition Vector3ToGridPosition(Vector3 vector3)
    {
        Vector3 firstPos = new Vector3(-1.8f, 1.8f);
        float x = vector3.x;
        float y = vector3.y;
        int gridX = (int)((y - firstPos.y) / -1.2f + 1);
        int gridY = (int)((x - firstPos.x) / 1.2f + 1);
        GridManagerScript.GridPosition finalPos = new GridManagerScript.GridPosition(gridX, gridY);
        return finalPos;
    }

    private void InstantiateTile(int spriteValue)
    {
        GridPosition pos = GetRandomGridPosition();
        GameObject newTile = Instantiate(tilePrefab);
        gridValues[newTile] = spriteValue;
        TileScript newTileScript = newTile.GetComponent<TileScript>();
        newTileScript.Initiation();
        gridTiles[pos] = newTile;
        newTileScript.UpdateSprite(spriteValue);
        newTile.transform.position = GridPositionToVector3(pos);
        Debug.Log($"New tile with value of {(int)Pow(2, spriteValue)} (spriteValue of {spriteValue}) instantiated at {GridPositionToVector3(pos)} (gridPosition: {pos.GetGridPosition()[0]}, {pos.GetGridPosition()[1]})");
    }

    public enum Direction
    {
        Up,
        Down,
        Left,
        Right,
        None
    }
    private Direction DetectInput()
    {
        bool upPressed = false;
        bool downPressed = false;
        bool leftPressed = false;
        bool rightPressed = false;
        if (Input.GetKeyDown(KeyCode.W)|| Input.GetKeyDown(KeyCode.UpArrow))
        {
            upPressed = true;
        }
        else if (Input.GetKeyDown(KeyCode.A)|| Input.GetKeyDown(KeyCode.LeftArrow))
        {
            leftPressed = true;
        }
        else if (Input.GetKeyDown(KeyCode.S)|| Input.GetKeyDown(KeyCode.DownArrow))
        {
            downPressed = true;
        }
        else if (Input.GetKeyDown(KeyCode.D)|| Input.GetKeyDown(KeyCode.RightArrow))
        {
            rightPressed = true;
        }

        int keysPressedCount = Convert.ToInt32(upPressed) + Convert.ToInt32(downPressed) + Convert.ToInt32(leftPressed) + Convert.ToInt32(rightPressed);
        if (keysPressedCount == 1)
        {
            if (upPressed)
            {
                MoveTiles(Direction.Up, 4, -1, 1, 1);
                return Direction.Up;
                
            } else if (downPressed) {
                MoveTiles(Direction.Down, 1, 1, 1, 1);
                return Direction.Down;
                
            } else if (leftPressed) {
                MoveTiles(Direction.Left, 1, 1, 1, 1);
                return Direction.Left;
                
            } else if (rightPressed) {
                MoveTiles(Direction.Right, 1, 1, 4, -1);
                return Direction.Right;
                
            } else {
                Debug.LogError("key press direction is broken HELP AHHHHHHH");
                return Direction.None;
            }

        }

        return Direction.None;
    }

    private void MoveTiles(Direction direction, int startRow, int incrementRow, int startColumn, int incrementColumn)
    {
        //Debug.Log(tiles);
        //foreach(var x in gridTiles.Values)
        //{
        //    x.GetComponent<TileScript>().Merge(direction);
        //}

        bool changed = false;

        for (int x = startRow; x >= 1 && x <= 4; x += incrementRow)
        {
            for (int y = startColumn; y >= 1 && y <= 4; y += incrementColumn)
            {
                if (HasTile(x, y)) 
                {
                    GameObject tile = GetTile(x, y);
                    GridPosition pos = new GridPosition(x, y);
                    TileScript script = tile.GetComponent<TileScript>();
                    Debug.Log($"Moving tile at ({pos.GetGridPosition()[0]}, {pos.GetGridPosition()[1]}) in {direction} direction");
                    changed |= script.MoveTile(pos, direction);
                    //Debug.Log($"{newGridPos.row}, {newGridPos.column}");
                    //gridTiles.Remove(pos);
                    //gridTiles[newGridPos] = tile;
                }
            }
        }

        if (changed)
        {
            for (int x = startRow; x >= 1 && x <= 4; x += incrementRow)
            {
                for (int y = startColumn; y >= 1 && y <= 4; y += incrementColumn)
                {
                    if (HasTile(x, y))
                    {
                        GameObject tile = GetTile(x, y);
                        tile.GetComponent<TileScript>().locked = false;
                    }
                }
            }
        }

        InstantiateTile(1);

        //Dictionary<GridPosition, GameObject> newGridTiles = new Dictionary<GridPosition, GameObject>();
        //foreach (KeyValuePair<GridPosition, GameObject> kvp in gridTiles)
        //{
        //    GridPosition oldGridPos = kvp.Key;
        //    GameObject tile = kvp.Value;
        //    Vector3 newVectorPos = tile.transform.position;
        //    GridPosition newGridPos = Vector3ToGridPosition(newVectorPos);
        //    newGridTiles[newGridPos] = tile;
        //    Debug.Log($"Tile originally at ({oldGridPos.row}, {oldGridPos.column}) updated to ({newGridPos.row}, {newGridPos.column})");
        //}
        //gridTiles = newGridTiles;

    }

    public void UpdateGridTiles(GridPosition oldTile, GridPosition newTile)
    {
        gridTiles = gridTiles
            .Select(kvp => new KeyValuePair<GridPosition, GameObject>(kvp.Key == newTile ? oldTile : kvp.Key, kvp.Value))
            .ToDictionary(kvp => kvp.Key, kvp => kvp.Value);
    }

    public bool HasTile(int x, int y)
    {
        return gridTiles.ContainsKey(new GridPosition(x,y));
    }

    private GameObject GetTile(int x, int y)
    {
        var position = new GridPosition(x, y);
        return gridTiles[position];
    }

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        Direction inputDirection = DetectInput();
    }
}
