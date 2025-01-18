using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenu : MonoBehaviour
{
    string Name;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        Name = SceneManager.GetActiveScene().name;
    }

    // Update is called once per frame
    void Update()
    {
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
