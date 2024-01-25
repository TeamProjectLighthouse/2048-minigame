using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;
using System;



public class GridManagerScript : MonoBehaviour
{


    public GameObject tilePrefab;
    public GameObject tileTextureDatabasePrefab;
    public List<GameObject> tileList;

    private void StartGame()
    {
        List<int> randomPos1 = new List<int>();
        randomPos1.Add(UnityEngine.Random.Range(1,4));
        randomPos1.Add(UnityEngine.Random.Range(1,4));
        List<int> randomPos2 = new List<int>();
        randomPos2.Add(UnityEngine.Random.Range(1,4));
        randomPos2.Add(UnityEngine.Random.Range(1,4));
        while (randomPos1[0] == randomPos2[0] && randomPos1[1] == randomPos2[1])
        {
            randomPos2 = new List<int>();
            randomPos2.Add(UnityEngine.Random.Range(1,4));
            randomPos2.Add(UnityEngine.Random.Range(1,4));
        }

        //InstantiateTile(randomPos1, 1);
        //InstantiateTile(randomPos2, 1);
        List<int> pos1 = new List<int>();
        List<int> pos2 = new List<int>();
        List<int> pos3 = new List<int>  ();
        pos1.Add(1);
        pos2.Add(1);
        pos3.Add(1);
        pos1.Add(1);
        pos2.Add(2);
        pos3.Add(3);
        InstantiateTile(pos1, 1);
        InstantiateTile(pos2, 1);
        InstantiateTile(pos3, 1);
    }

    private Vector3 GridPositionToVector3(List<int> pos)
    {
        Vector3 vector3Pos = new Vector3(pos[1], -pos[0], 0);
        return vector3Pos;
    }

    private List<int> Vector3ToGridPosition(Vector3 pos)
    {
        List<int> listPos = new List<int>();
        listPos.Add((int)(-pos.y));
        listPos.Add((int)pos.x);
        return listPos;
    }
    private void InstantiateTile(List<int> pos, int spriteValue)
    {
        GameObject newTile = Instantiate(tilePrefab);
        tileList.Add(newTile);
        TileScript newTileScript = newTile.GetComponent<TileScript>();
        newTileScript.Initiation();
        newTileScript.tileValue = spriteValue;
        newTileScript.posAfterMoving = newTile.transform.position;
        newTileScript.UpdateSprite(spriteValue);
        newTile.transform.position = GridPositionToVector3(pos);
        Debug.Log($"New tile with value of {(int)Pow(2, spriteValue)} (spriteValue of {spriteValue}) instantiated at {newTile.transform.position} (Grid Position: {pos[0]}, {pos[1]})");
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

    public Vector3 DirectionToVector3(Direction direction)
    {
        Vector3 directionVector3;
        if (direction == Direction.Up)
        {
            directionVector3 = new Vector3(0, 1, 0);
        } else if (direction == Direction.Down) {
            directionVector3 = new Vector3(0, -1, 0);
        } else if (direction == Direction.Left) {
            directionVector3 = new Vector3(-1, 0, 0);
        } else {
            directionVector3 = new Vector3(1, 0, 0);
        }
        return directionVector3;
    }
    private void UpdateGrid(Direction direction)
    {

        foreach (GameObject tile in tileList)
        {
            tile.GetComponent<TileScript>().posAfterMoving = tile.transform.position;
        }

        CompressTiles(direction);
        MergeTiles(direction);
        CompressTiles(direction);

        bool gridHasChanged = UpdateTiles();
        if (gridHasChanged)
        {
            SpawnRandomTile();
        }
        
    }

    private void SpawnRandomTile()
    {
        List<Vector3> availablePos = new List<Vector3>();
        for (int row = 1; row <= 4; row++)
        {
            for (int col = 1; col <=4; col++)
            {
                List<int> gridPos = new List<int>();
                gridPos.Add(row);
                gridPos.Add(col);

                Vector3 pos = GridPositionToVector3(gridPos);
                availablePos.Add(pos);
            }
        }

        foreach (GameObject tile in tileList)
        {
            availablePos.Remove(tile.transform.position);
        }

        Vector3 randomPos = availablePos[UnityEngine.Random.Range(1, availablePos.Count)-1];

        int tileValueRandom = UnityEngine.Random.Range(1,10);
        int tileValue;
        if (tileValueRandom <= 9)
        {
            tileValue = 1;
        } else {
            tileValue = 2;
        }

        InstantiateTile(Vector3ToGridPosition(randomPos),tileValue);
    }

    private bool UpdateTiles()
    {
        bool gridHasChanged = false;
        List<GameObject> copyTileList = new List<GameObject>();
        foreach (GameObject tile in tileList) //deepcopying tileList to copyTileList so tile can be removed from tileList in the loop below
        {
            copyTileList.Add(tile);
        }

        foreach (GameObject tile in copyTileList)
        {
            TileScript script = tile.GetComponent<TileScript>();
            if (script != null)
            {
                if (script.isDeletedNextFrame)
                {
                    gridHasChanged = true;
                    tileList.Remove(tile);
                    Destroy(tile);
                    continue;
                }
                if (tile.transform.position != script.posAfterMoving) 
                {
                    script.Move(script.posAfterMoving);
                    gridHasChanged = true;
                }
                if (script.alreadyMergedThisMove) 
                {
                    script.alreadyMergedThisMove = false;
                    gridHasChanged = true;
                }
                
            }
        }
        return gridHasChanged;
    }

    private void CompressTiles(Direction direction)
    {
        Vector3 directionVector3 = DirectionToVector3(direction);
        Dictionary<int, GameObject> tilePriority = GetPriorityList(directionVector3);

        
        for (int priority = 1; priority <= 16; priority++)
        {
            if (tilePriority.ContainsKey(priority))
            {
                GameObject tile = tilePriority[priority];
                TileScript script = tile.GetComponent<TileScript>();
                if (script != null) {
                    script.MoveTile(directionVector3);
                }
            }

        }
        
    }

    private void MergeTiles(Direction direction)
    {
        Vector3 directionVector3 = DirectionToVector3(direction);
        Dictionary<int, GameObject> tilePriority = GetPriorityList(directionVector3);
        for (int priority = 1; priority <= 16; priority++)
        {
            if (tilePriority.ContainsKey(priority))
            {
                GameObject tile = tilePriority[priority];
                TileScript script = tile.GetComponent<TileScript>();
                if (script != null) {
                    script.MergeTile(directionVector3);
                }
            }

        }
    }

    private Dictionary<int, GameObject> GetPriorityList(Vector3 moveDirection)
    {
        Dictionary<int, GameObject> tilePriority = new Dictionary<int, GameObject>();
        foreach (GameObject tile in tileList)
        {
            TileScript script = tile.GetComponent<TileScript>();
            if (script != null) {
                int priority = script.GetPriority(moveDirection);
                tilePriority[priority] = tile;
            }
        }

        return tilePriority;
    }

    void Start()
    {
        StartGame();
    }

    void Update()
    {
        Direction inputDirection = DetectInput();
        if (inputDirection != Direction.None)
        {
            UpdateGrid(inputDirection);
        }
    }


}
