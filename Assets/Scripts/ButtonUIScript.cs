using UnityEngine;
using UnityEngine.UI;

public class ButtonUIScript : MonoBehaviour
{
    Button PowerUpButtonUI;
    [SerializeField] GameObject Hint;
    private void Start()
    {
        PowerUpButtonUI = GetComponent<Button>();
    }
    void Update()
    {
        if (GameManager.isPowerUp == true)
        {
            ColorBlock cb = PowerUpButtonUI.colors;
            cb.normalColor = new Color (0.5896226f, 0.9816263f,1f,1f);
            cb.highlightedColor = new Color(0.5896226f, 0.9816263f, 1f, 1f);
            cb.selectedColor = new Color(0.5896226f, 0.9816263f, 1f, 1f);
            PowerUpButtonUI.colors = cb;
        }
        else if(GameManager.isPowerUp == false)
        {
            ColorBlock cb = PowerUpButtonUI.colors;
            cb.normalColor = Color.white;
            cb.highlightedColor = Color.white;
            cb.selectedColor = Color.white;
            PowerUpButtonUI.colors = cb;
        }
        Hint.SetActive(GameManager.isPowerUp);
    }
}
