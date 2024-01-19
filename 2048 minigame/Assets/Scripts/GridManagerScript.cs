using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;
using System;
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
    Dictionary<GridPosition, int> gridValues = new Dictionary<GridPosition, int>();
    Dictionary<GridPosition, GameObject> gridTiles = new Dictionary<GridPosition, GameObject>();


    private void StartGame()
    {
        GridPosition randomPos1 = new GridPosition(UnityEngine.Random.Range(1,4), UnityEngine.Random.Range(1,4));
        GridPosition randomPos2 = new GridPosition(UnityEngine.Random.Range(1,4), UnityEngine.Random.Range(1,4));
        while (randomPos1 == randomPos2)
        {
            randomPos2 = new GridPosition(UnityEngine.Random.Range(1,4), UnityEngine.Random.Range(1,4));
        }

        InstantiateTile(randomPos1, 1);
        InstantiateTile(randomPos2, 1);
             
        
    }

    private Vector3 GridPositionToVector3(GridPosition pos)
    {
        List<int> gridPos = pos.GetGridPosition();
        Vector3 vector3Pos = new Vector3((-1.8f + (gridPos[0] - 1) * 1.2f), (1.8f - (gridPos[1] - 1) * 1.2f), 0);
        return vector3Pos;
    }

    private void InstantiateTile(GridPosition pos, int spriteValue)
    {
        gridValues[pos] = spriteValue;
        GameObject newTile = Instantiate(tilePrefab);
        TileScript newTileScript = newTile.GetComponent<TileScript>();
        newTileScript.Initiation();
        gridTiles[pos] = newTile;
        newTileScript.UpdateSprite(spriteValue);
        newTile.transform.position = GridPositionToVector3(pos);
        Debug.Log($"New tile with value of {(int)Pow(2, spriteValue)} (spriteValue of {spriteValue}) instantiated at {GridPositionToVector3(pos)} (gridPosition: {pos.GetGridPosition()[0]}, {pos.GetGridPosition()[1]})");
    }

    private enum Direction
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
                Debug.Log("up");
                return Direction.Up;
                
            } else if (downPressed) {
                Debug.Log("down");
                return Direction.Down;
                
            } else if (leftPressed) {
                Debug.Log("left");
                return Direction.Left;
                
            } else if (rightPressed) {
                Debug.Log("right");
                return Direction.Right;
                
            } else {
                Debug.LogError("key press direction is broken HELP AHHHHHHH");
                return Direction.None;
            }
        }
        //Debug.Log("none");
        return Direction.None;
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
