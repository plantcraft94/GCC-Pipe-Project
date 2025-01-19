using UnityEngine;
using UnityEngine.SceneManagement;
using TMPro;

public class MainMenu : MonoBehaviour
{
    string Name;
    public GameObject PowerCounter;
    TextMeshProUGUI textMeshProUGUI;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Name = SceneManager.GetActiveScene().name;
        if (PowerCounter != null)
        {
            textMeshProUGUI = PowerCounter.GetComponent<TextMeshProUGUI>();

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (PowerCounter != null)
        {
            textMeshProUGUI.text = GameManager.PowerCount.ToString();

        }
        if (Input.GetMouseButtonDown(0) && Name == "MainMenu")
        {
            SceneManager.LoadScene("Gameplay");
        }

    }
    public void Restart()
    {
        SceneManager.LoadScene("Gameplay");
    }
}
