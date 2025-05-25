using UnityEngine;
using UnityEngine.SceneManagement;

public class SceneController : MonoBehaviour
{
    public void ReturnToMainMenu()
    {
        Debug.Log("✅ Button clicked: ReturnToMainMenu");
        // ✅ 確保遊戲恢復正常時間（避免還是暫停狀態）
        Time.timeScale = 1f;
        SceneManager.LoadScene("MainMenu");
    }
}
