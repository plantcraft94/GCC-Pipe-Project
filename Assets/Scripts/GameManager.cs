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

    public static bool isPowerUp = false;
    public static int PowerCount = 2;

    [SerializeField] private GameObject _winText;

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
        yield return new WaitForSeconds(0.1f);
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
        _level.Column++;
        _level.Row++;
        GenerateLevelData(_level);
        yield return new WaitForSeconds(2f);
        PowerCount++;
        UnityEngine.SceneManagement.SceneManager.LoadScene(1);
    }


    private void GenerateLevelData(LevelData levelData)
    {
        levelData.Data.Clear();
        int totalCells = levelData.Row * levelData.Column;

        levelData.Data.Add(12);
        for (int i = 1; i < totalCells - 1; i++)
        {
            int rotation = Random.Range(0, 4);
            int pipeType = Random.Range(3, 6);
            int pipeData = rotation * 10 + pipeType;
            levelData.Data.Add(pipeData);
        }
        levelData.Data.Add(01);
    }
    public void ActivePowerUp()
    {
        if (PowerCount > 0)
        {
            isPowerUp = !isPowerUp;
        }
        print(isPowerUp);
    }
}
