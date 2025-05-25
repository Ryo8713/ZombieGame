using UnityEngine;

public class ResultManager : MonoBehaviour
{
    public CanvasGroup winPanel;
    public CanvasGroup deathPanel;

    void Start()
    {
        string result = PlayerPrefs.GetString("GameResult", "None");

        if (result == "Win")
            ShowPanel(winPanel);
        else if (result == "Lose")
            ShowPanel(deathPanel);
    }

    void ShowPanel(CanvasGroup panel)
    {
        panel.alpha = 1f;
        panel.interactable = true;
        panel.blocksRaycasts = true;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        Time.timeScale = 1f;
    }

    public void ReturnToMainMenu()
    {
        UnityEngine.SceneManagement.SceneManager.LoadScene("MainMenu");
    }
}
