using System.Collections;
using TMPro;
using UnityEngine;

public class Timer : MonoBehaviour
{
    public static float pubTime = 45f;
    static float timer = pubTime;
    GameManager GM;
    public GameObject Losetext;
    TextMeshProUGUI textMeshProUGUI;
    [SerializeField] private LevelData _level;
    private void Start()
    {
        Losetext.SetActive(false);
        GM = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManager>();
        textMeshProUGUI = GetComponent<TextMeshProUGUI>();
    }
    private void Update()
    {
        timer = timer - Time.deltaTime;
        if(timer <= 0)
        {
            timer = 0;
            StartCoroutine(Lose());
        }
        string text = $"{((int)timer)}s left";
        textMeshProUGUI.text = text;
    }

    IEnumerator Lose()
    {
        if (timer <= 0 && !GM.Win)
        {
            Losetext.SetActive(true);
            _level.Column = 3;
            _level.Row = 3;
            GM.GenerateLevelData(_level);
            yield return new WaitForSeconds(2f);
            GameManager.PowerCount = 2;
            timer = pubTime;
            UnityEngine.SceneManagement.SceneManager.LoadScene(1);
        }
    }
    public void AddTime(float time)
    {
        timer += time;
    }
}
