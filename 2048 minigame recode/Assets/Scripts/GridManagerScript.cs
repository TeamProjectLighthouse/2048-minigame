using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using static System.Math;
using System;
using TMPro;
using UnityEditor;


public class GridManagerScript : MonoBehaviour
{


    public GameObject tilePrefab;
    //public GameObject tileTextureDatabasePrefab;
    public List<GameObject> tileList;

    private int score;
    public TextMeshProUGUI scoreText;
    public TextMeshProUGUI bestScoreText;

    private TextMeshProUGUI newScoreText;
    private TextMeshProUGUI newBestScoreText;

    public GameObject deathScreen;

    public Canvas startGameScreen;


    private enum GameState
    {
        StartScreen,
        InGame,
        GameOver
    }

    private GameState gameState = GameState.StartScreen;


    public GameObject background;

    private bool console = false;


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

        InstantiateTile(randomPos1, 1);
        InstantiateTile(randomPos2, 1);
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
        newTileScript.posAfterMoving = newTile.transform.position;
        if (console) { Debug.Log($"New tile with value of {(int)Pow(2, spriteValue)} (spriteValue of {spriteValue}) instantiated at {newTile.transform.position} (Grid Position: {pos[0]}, {pos[1]})");}
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
                if (console) { Debug.Log("up"); }
                return Direction.Up;
                
            } else if (downPressed) {
                if (console) { Debug.Log("down"); }
                return Direction.Down;
                
            } else if (leftPressed) {
                if (console) { Debug.Log("left"); }
                return Direction.Left;
                
            } else if (rightPressed) {
                if (console) { Debug.Log("right"); }
                return Direction.Right;
                
            } else {
                if (console) { Debug.Log("key press direction is broken HELP AHHHHHHH"); }
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
            TileScript tileScript = tile.GetComponent<TileScript>();
            if (tileScript != null) {
                tileScript.AlignMovement();
                tileScript.posAfterMoving = tile.transform.position;
            }
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
            availablePos.Remove(tile.GetComponent<TileScript>().posAfterMoving);
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
        foreach (GameObject tile in tileList) //deepcopying tileList to copyTileList so tiles can be removed from tileList in the loop below
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
                    if (script.MergeTile(directionVector3))
                    {
                        score += (int)Math.Pow(2, script.tileValue);
                    }
                    
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

    private bool HasValidMoves()
    {
        bool gridIsFull = (tileList.Count == 16);
        if (!gridIsFull)
        {
            return true;
        }

        foreach (GameObject tile in tileList)
        {
            TileScript tileScript = tile.GetComponent<TileScript>();
            if (tileScript != null)
            {
                foreach (GameObject otherTile in tileList)
                {
                    int distanceX = (int)Mathf.Abs(tileScript.posAfterMoving.x - otherTile.GetComponent<TileScript>().posAfterMoving.x);
                    int distanceY = (int)Mathf.Abs(tileScript.posAfterMoving.y - otherTile.GetComponent<TileScript>().posAfterMoving.y);

                    bool isOrthogonallyConnected = (distanceX == 1 && distanceY == 0) || (distanceX == 0 && distanceY == 1);
                    if (isOrthogonallyConnected)
                    {
                        
                        TileScript otherTileScript = otherTile.GetComponent<TileScript>();
                        if (otherTileScript != null)
                        {
                            List<int> tileGridPos = Vector3ToGridPosition(tileScript.posAfterMoving);
                            List<int> otherTileGridPos = Vector3ToGridPosition(otherTileScript.posAfterMoving);
                            if (tileScript.tileValue == otherTileScript.tileValue)
                            {
                                if (console) {

                                    Debug.Log($"Has valid moves with tiles at ({tileGridPos[0]}, {tileGridPos[1]}) and ({otherTileGridPos[0]}, {otherTileGridPos[1]})."); 
                                }
                                return true;
                            } else {
                            }
                        }
                    }
                }

            }
        }
        if (console) { Debug.Log("no valid moves D:"); }
        return false;
    }

    IEnumerator DisplayDeathScreen()
    {
        gameState = GameState.GameOver;
        yield return new WaitForSeconds(2f);
        var colour = deathScreen.GetComponent<Renderer>().material.color;
        colour.a = 0f;
        deathScreen.SetActive(true);
        colour.a = Mathf.Lerp(0f, 1f, 0.5f);
        newScoreText = Instantiate(scoreText, new Vector3(92.7f, -180.69f), Quaternion.identity, GameObject.Find("Score Text").transform);
        newBestScoreText = Instantiate(bestScoreText, new Vector3(92.7f, -220.7f), Quaternion.identity, GameObject.Find("Score Text").transform);
        newScoreText.GetComponent<RectTransform>().localPosition = new Vector3(92.7f, -180.69f);
        newBestScoreText.GetComponent<RectTransform>().localPosition = new Vector3(92.7f, -220.7f);
        newScoreText.color = new Color32(39, 39, 39, 255);
        newBestScoreText.color = new Color32(39, 39, 39, 255);

        var bestScore = PlayerPrefs.GetInt("highscore", 0);
        if (score > bestScore)
        {
            PlayerPrefs.SetInt("highscore", score);
            bestScoreText.text = PlayerPrefs.GetInt("highscore", 0).ToString();
        }
        if (Input.GetMouseButtonDown(0))
        {
            EditorApplication.isPlaying = false;
        }
        //TextMeshProUGUI deathText = deathScreen.GetComponentInChildren<TextMeshProUGUI>();
        //if (deathText != null)
        //{

        //    deathText.text = "Game Over (add UI here)";
        //} else {
        //    Debug.Log("deathtext is null");
        //}
    }

    private void SetupScore()
    {
        score = 0;
        scoreText.text = score.ToString();
    }


    private void RestartGame()
    {
        foreach (GameObject tile in tileList)
        {
            Destroy(tile);
        }
        tileList = new List<GameObject>();

        Destroy(newScoreText);
        Destroy(newBestScoreText);

        gameState = GameState.InGame;
        score = 0;
        deathScreen.SetActive(false);
    
        StartGame();
    }

    void Start()
    {
        startGameScreen.enabled = true;
        deathScreen.SetActive(false);
        background.SetActive(false);
        scoreText.color = new Color32(255,255,255,255);
        bestScoreText.color = new Color32(255, 255, 255, 255);
        scoreText.GetComponent<RectTransform>().localPosition = new Vector3(63.6f, 334.2f);
        bestScoreText.GetComponent<RectTransform>().localPosition = new Vector3(241f, 334.2f);
        bestScoreText.text = PlayerPrefs.GetInt("highscore", 0).ToString();
        //cooldownBetweenMove = 0.2f;

    }

    void Update()
    {
        if (gameState == GameState.StartScreen)
        {
            if (Input.GetMouseButtonDown(0))
            {
                startGameScreen.enabled = false;
                gameState = GameState.InGame;
                StartGame();
                background.SetActive(true);

            }
        } else if (gameState == GameState.InGame)
        {
            if (!HasValidMoves())
            {
                if (console) { Debug.LogWarning("YOU ARE DIE!"); }
                StartCoroutine(DisplayDeathScreen());
            } else {
                Direction inputDirection = DetectInput();
                if (inputDirection != Direction.None)
                {
                    
                    UpdateGrid(inputDirection);
                    scoreText.text = score.ToString();
                    int bestScore = PlayerPrefs.GetInt("highscore", 0);
                    if (score > bestScore)
                    {
                        PlayerPrefs.SetInt("highscore", score);
                        bestScoreText.text = PlayerPrefs.GetInt("highscore", 0).ToString();
                    }

                }
                if (startGameScreen.enabled)
                {
                    scoreText.color = Color.white;
                    bestScoreText.color = Color.white;
                    startGameScreen.enabled = false;
                }
            }
        } else if (gameState == GameState.GameOver)
        {
            if (Input.GetMouseButtonDown(0))
            {
                RestartGame();
            }
        }

    }


}