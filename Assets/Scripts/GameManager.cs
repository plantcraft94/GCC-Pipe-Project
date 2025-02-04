using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance;

    [SerializeField] private LevelData _level;
    [SerializeField] private Pipe _cellPrefab;

    private bool hasGameFinished;
    private Pipe[,] pipes;
    private List<Pipe> startPipes;
    public bool Win = false;

    public static bool isPowerUp = false;
    public static int PowerCount = 2;

    [SerializeField] private GameObject _winText;

    [SerializeField] private Timer timer;

    private void Awake()
    {
        Instance = this;
        hasGameFinished = false;
        _winText.SetActive(false);
        SpawnLevel();
    }

    private void SpawnLevel()
    {
        pipes = new Pipe[_level.Row, _level.Column];
        startPipes = new List<Pipe>();

        for (int i = 0; i < _level.Row; i++)
        {
            for (int j = 0; j < _level.Column; j++)
            {
                Vector2 spawnPos = new Vector2(j + 0.5f, i + 0.5f);
                Pipe tempPipe = Instantiate(_cellPrefab);
                tempPipe.transform.position = spawnPos;
                tempPipe.Init(_level.Data[i * _level.Column + j]);
                pipes[i, j] = tempPipe;
                if (tempPipe.PipeType == 1)
                {
                    startPipes.Add(tempPipe);
                }
            }
        }

        Camera.main.orthographicSize = Mathf.Max(_level.Row, _level.Column)/2f + 2f;
        Vector3 cameraPos = Camera.main.transform.position;
        cameraPos.x = _level.Column * 0.5f;
        cameraPos.y = _level.Row * 0.5f;
        Camera.main.transform.position = cameraPos;

        StartCoroutine(ShowHint());
    }

    private void Update()
    {
        if (hasGameFinished) return;
        if (!IsMouseInView()) return;

        Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        int row = Mathf.FloorToInt(mousePos.y);
        int col = Mathf.FloorToInt(mousePos.x);
        if (row < 0 || col < 0 || row >= _level.Row || col >= _level.Column) return;

        if (Input.GetMouseButtonDown(0))
        {
            pipes[row, col].UpdateInput();
            StartCoroutine(ShowHint());
        }
    }
    private bool IsMouseInView()
    {
        Vector3 mousePos = Input.mousePosition;
        Rect screenRect = new Rect(0, 0, Screen.width, Screen.height);
        return screenRect.Contains(mousePos);
    }

    private IEnumerator ShowHint()
    {
        yield return new WaitForSeconds(0.2f);// thời gian mà ống xoay
        CheckFill();
        CheckWin();
    }

    private void CheckFill()
    {
        for (int i = 0; i < _level.Row; i++)
        {
            for (int j = 0; j < _level.Column; j++)
            {
                Pipe tempPipe = pipes[i, j];
                if (tempPipe.PipeType != 0)
                {
                    tempPipe.IsFilled = false;
                }
            }
        }

        Queue<Pipe> check = new Queue<Pipe>();
        HashSet<Pipe> finished = new HashSet<Pipe>();
        foreach (Pipe pipe in startPipes)
        {
            check.Enqueue(pipe);
        }

        while (check.Count > 0)
        {
            Pipe pipe = check.Dequeue();
            finished.Add(pipe);
            List<Pipe> connected = pipe.ConnectedPipes();
            foreach (var connectedPipe in connected)
            {
                if (!finished.Contains(connectedPipe))
                {
                    check.Enqueue(connectedPipe);
                }
            }
        }

        foreach (var filled in finished)
        {
            filled.IsFilled = true;
        }

        for (int i = 0; i < _level.Row; i++)
        {
            for (int j = 0; j < _level.Column; j++)
            {
                Pipe tempPipe = pipes[i, j];
                tempPipe.UpdateFilled();
            }
        }

    }

    private void CheckWin()
    {
        for (int i = 0; i < _level.Row; i++)
        {
            for (int j = 0; j < _level.Column; j++)
            {
                if (pipes[i, j].PipeType == 2 && pipes[i, j].IsFilled)
                {
                    hasGameFinished = true;
                }
            }
        }
        if (hasGameFinished)
        {
            StartCoroutine(GameFinished());
        }

    }

    private IEnumerator GameFinished()
    {
        _winText.SetActive(true);
        Debug.Log("win");
        Win = true;
        _level.Column++;
        _level.Row++;
        GenerateLevelData(_level);
        yield return new WaitForSeconds(2f);
        PowerCount++;
        timer.AddTime(30f);
        Win = false;
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }


    public void GenerateLevelData(LevelData levelData)
    {
            levelData.Data.Clear();
            int totalCells = levelData.Row * levelData.Column;

            levelData.Data.Add(11);

            for (int i = 1; i < totalCells - 1; i++)
            {
                int rotation = Random.Range(0, 4);
                int pipeType = Random.Range(3, 6);
                int rand = Random.Range(0, 100);
                if (rand == 0)
                {
                    pipeType = 0;
                }
                int pipeData = rotation * 10 + pipeType;
                levelData.Data.Add(pipeData);
            }
            levelData.Data.Add(32);
        EditorUtility.SetDirty(levelData);
    }

    public void ActivePowerUp()
    {
        if (PowerCount > 0)
        {
            isPowerUp = !isPowerUp;
        }
        print(isPowerUp);
    }
    private bool CheckValidPath(LevelData levelData)
    {
        int rows = levelData.Row;
        int cols = levelData.Column;

        Pipe[,] pipes = new Pipe[rows, cols];
        Vector2Int startPos = Vector2Int.zero;
        Vector2Int endPos = new Vector2Int(rows - 1, cols - 1);

        for (int i = 0; i < rows; i++)
        {
            for (int j = 0; j < cols; j++)
            {
                pipes[i, j] = GameObject.Instantiate(_cellPrefab);
                pipes[i, j].Init(levelData.Data[i * cols + j]);

                if (pipes[i, j].PipeType == 1) startPos = new Vector2Int(i, j);
                if (pipes[i, j].PipeType == 2) endPos = new Vector2Int(i, j);
            }
        }

        //BFS
        Queue<Vector2Int> queue = new Queue<Vector2Int>();
        HashSet<Vector2Int> visited = new HashSet<Vector2Int>();

        queue.Enqueue(startPos);
        visited.Add(startPos);

        while (queue.Count > 0)
        {
            Vector2Int current = queue.Dequeue();

            if (current == endPos)
            {
                return true;
            }

            List<Vector2Int> neighbors = GetConnectedNeighbors(current, pipes, rows, cols);
            foreach (var neighbor in neighbors)
            {
                if (!visited.Contains(neighbor))
                {
                    queue.Enqueue(neighbor);
                    visited.Add(neighbor);
                }
            }
        }

        return false;
    }
    private List<Vector2Int> GetConnectedNeighbors(Vector2Int pos, Pipe[,] pipes, int rows, int cols)
    {
        List<Vector2Int> neighbors = new List<Vector2Int>();

        Vector2Int[] directions = new Vector2Int[]
        {
        new Vector2Int(-1, 0),
        new Vector2Int(1, 0),
        new Vector2Int(0, -1),
        new Vector2Int(0, 1)
        };

        foreach (var dir in directions)
        {
            Vector2Int neighbor = pos + dir;

            if (neighbor.x >= 0 && neighbor.x < rows &&
                neighbor.y >= 0 && neighbor.y < cols &&
                pipes[pos.x, pos.y].IsConnectedTo(pipes[neighbor.x, neighbor.y], dir))
            {
                neighbors.Add(neighbor);
            }
        }

        return neighbors;
    }

}

