using UnityEngine;
using UnityEngine.SceneManagement;

public class MainMenuUI : MonoBehaviour
{
    public void SelectEasy()   => LoadGame("Easy");
    public void SelectMedium() => LoadGame("Medium");
    public void SelectHard()   => LoadGame("Hard");

    private void LoadGame(string difficulty)
    {
        if (DifficultyManager.Instance != null)
            DifficultyManager.Instance.SetDifficulty(difficulty);

        SceneManager.LoadScene("GameScene"); // ← 請改成你的實際遊戲場景名稱
    }
}
