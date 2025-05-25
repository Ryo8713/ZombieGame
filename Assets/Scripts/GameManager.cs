using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class GameManager : MonoBehaviour
{
    [Header("生存時間設定")]
    public float surviveTime = 60f;
    private float timeLeft;
    private bool gameEnded = false;

    [Header("UI 設定")]
    public TextMeshProUGUI timerText;

    [Header("其他 UI 元件")]
    public GameObject crosshair;

    void Start()
    {   
        var difficulty = DifficultyManager.Instance.selectedDifficulty;

        switch (difficulty)
        {
            case DifficultyManager.Difficulty.Easy:
                surviveTime = 45f;
                break;
            case DifficultyManager.Difficulty.Medium:
                surviveTime = 60f;
                break;
            case DifficultyManager.Difficulty.Hard:
                surviveTime = 90f;
                break;
        }

        timeLeft = surviveTime;

        // 更新滑鼠狀態
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    void Update()
    {
        if (gameEnded) return;

        timeLeft -= Time.deltaTime;
        if (timeLeft <= 0f)
        {
            timeLeft = 0f;
            TriggerWin();
        }

        if (timerText != null)
            timerText.text = $"Survive: {Mathf.CeilToInt(timeLeft)}s";
    }

    void TriggerWin()
    {
        gameEnded = true;

        // ✅ 儲存結果資訊
        PlayerPrefs.SetString("GameResult", "Win");
        PlayerPrefs.SetFloat("SurviveTime", surviveTime);
        PlayerPrefs.Save();

        // ✅ 切換場景顯示勝利畫面
        UnityEngine.SceneManagement.SceneManager.LoadScene("ResultScene");

        // 關閉準心（若有殘留）
        if (crosshair != null)
            crosshair.SetActive(false);

        // 解鎖滑鼠
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        Debug.Log("🏆 玩家成功存活，切換至結果畫面");
    }

    public bool IsGameOver() => gameEnded;

    public float GetSurviveTime() => surviveTime;
}
