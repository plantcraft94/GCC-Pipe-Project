using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Pipe : MonoBehaviour
{

    [HideInInspector] public bool IsFilled;
    [HideInInspector] public int PipeType;

    [SerializeField] private Transform[] _pipePrefabs;

    private Transform currentPipe;
    private int rotation;
    public bool isRotating = false;

    private SpriteRenderer emptySprite;
    private SpriteRenderer filledSprite;
    private List<Transform> connectBoxes;

    private const int minRotation = 0;
    private const int maxRotation = 3;
    private const int rotationMultiplier = 90;

    public float timeCount = 0.0f;
    public Quaternion CurrentRotation;
    public Quaternion TargetRotation;

    AudioSource pipeTurnSound;


    public void Init(int pipe)
    {
        PipeType = pipe % 10;
        currentPipe = Instantiate(_pipePrefabs[PipeType], transform);
        currentPipe.transform.localPosition = Vector3.zero;
        if (PipeType == 1 || PipeType == 2)
        {
            rotation = pipe / 10;
        }
        else
        {
            rotation = Random.Range(minRotation, maxRotation + 1);
        }
        currentPipe.transform.eulerAngles = new Vector3(0, 0, rotation * rotationMultiplier);

        if (PipeType == 0 || PipeType == 1)
        {
            IsFilled = true;
        }

        if (PipeType == 0)
        {
            return;
        }

        emptySprite = currentPipe.GetChild(1).GetComponent<SpriteRenderer>();
        emptySprite.gameObject.SetActive(!IsFilled);
        filledSprite = currentPipe.GetChild(0).GetComponent<SpriteRenderer>();
        filledSprite.gameObject.SetActive(IsFilled);

        connectBoxes = new List<Transform>();
        for (int i = 2; i < currentPipe.childCount; i++)
        {
            connectBoxes.Add(currentPipe.GetChild(i));
        }
    }

    private void Awake()
    {
        pipeTurnSound = GetComponent<AudioSource>();
    }

    private void Update()
    {
        if (isRotating == true)
        {
            currentPipe.transform.rotation = Quaternion.Slerp(CurrentRotation, TargetRotation, timeCount);
            timeCount = timeCount * 1.2f + Time.deltaTime;
            if (currentPipe.transform.rotation.eulerAngles == TargetRotation.eulerAngles)
            {
                timeCount = 0f;
                isRotating = false;
            }
        }
    }
    public void UpdateInput()
    {
        if (PipeType == 0 || PipeType == 1 || PipeType == 2)
        {
            return;
        }

        else if (GameManager.isPowerUp == true)
        {
            connectBoxes.Clear();
            Destroy(currentPipe.transform.gameObject);
            currentPipe = Instantiate(_pipePrefabs[6], transform);
            currentPipe.transform.localPosition = Vector3.zero;
            emptySprite = currentPipe.GetChild(1).GetComponent<SpriteRenderer>();
            emptySprite.gameObject.SetActive(!IsFilled);
            filledSprite = currentPipe.GetChild(0).GetComponent<SpriteRenderer>();
            filledSprite.gameObject.SetActive(IsFilled);
            for (int i = 2; i < currentPipe.childCount; i++)
            {
                connectBoxes.Add(currentPipe.GetChild(i));
            }

            GameManager.PowerCount--;
            GameManager.isPowerUp = false;
            return;
        }

        CurrentRotation = currentPipe.transform.rotation;
        rotation = (rotation + 1) % (maxRotation + 1);
        TargetRotation = Quaternion.Euler(0, 0, rotation * rotationMultiplier);
        isRotating = true;

        pipeTurnSound.Play();
    }

    public void UpdateFilled()
    {
        if (PipeType == 0) return;
        emptySprite.gameObject.SetActive(!IsFilled);
        filledSprite.gameObject.SetActive(IsFilled);
    }

    public List<Pipe> ConnectedPipes()
    {
        List<Pipe> result = new List<Pipe>();

        foreach (Transform box in connectBoxes)
        {

            RaycastHit2D[] hit = Physics2D.RaycastAll(box.transform.position, Vector2.zero, 0.1f);
            for (int i = 0; i < hit.Length; i++)
            {
                result.Add(hit[i].collider.transform.parent.parent.GetComponent<Pipe>());
            }
        }

        return result;
    }
    public bool IsConnectedTo(Pipe otherPipe, Vector2Int direction)
    {
        // Giả sử chúng ta có một danh sách các hướng hợp lệ cho từng loại pipe
        Dictionary<int, Vector2Int[]> pipeConnections = new Dictionary<int, Vector2Int[]>()
    {
        { 3, new Vector2Int[] { new Vector2Int(1, 0), new Vector2Int(-1, 0) } }, // Pipe thẳng dọc
        { 4, new Vector2Int[] { new Vector2Int(0, 1), new Vector2Int(-1, 0) } }, // Pipe chữ L
        { 5, new Vector2Int[] { new Vector2Int(0, -1), new Vector2Int(1, 0), new Vector2Int(0, 1) } }, // Pipe 3 đường
        { 6, new Vector2Int[] { new Vector2Int(-1, 0), new Vector2Int(1, 0), new Vector2Int(0, -1), new Vector2Int(0, 1) } } // Pipe 4 đường
    };

        if (!pipeConnections.ContainsKey(this.PipeType) || !pipeConnections.ContainsKey(otherPipe.PipeType))
            return false;

        // Kiểm tra xem direction có nằm trong danh sách hướng kết nối không
        return pipeConnections[this.PipeType].Contains(direction) &&
               pipeConnections[otherPipe.PipeType].Contains(-direction);
    }

}
