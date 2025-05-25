using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class PlayerHealth : MonoBehaviour
{
    public DamageVignetteController vignette;
    public float maxHealth = 100f;
    public float currentHealth;
    public TextMeshProUGUI hpText;
    public GameObject crosshair;

    private bool isDead = false;

    void Start()
    {
        currentHealth = maxHealth;
        UpdateHPText();

        if (vignette == null)
            vignette = FindFirstObjectByType<DamageVignetteController>();
    }

    void Update()
    {
        UpdateHPText();
        if (!isDead && transform.position.y < -20f)
        {
            Debug.Log("☠️ 玩家掉出地圖，判定死亡");
            TakeDamage(maxHealth); // 或直接呼叫 Die();
        }
    }

    public void TakeDamage(float amount)
    {
        if (isDead) return;

        if (vignette != null)
            vignette.Flash();

        currentHealth -= amount;
        currentHealth = Mathf.Clamp(currentHealth, 0, maxHealth);
        Debug.Log("Player HP: " + currentHealth);

        if (currentHealth <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        if (isDead) return;
        isDead = true;

        Debug.Log("💀 Player died.");

        // 停用玩家操作與準心
        GetComponent<PlayerController>().enabled = false;
        if (crosshair != null)
            crosshair.SetActive(false);

        // 顯示滑鼠
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;

        // 設定結果並切換場景
        PlayerPrefs.SetString("GameResult", "Lose");
        PlayerPrefs.Save();
        UnityEngine.SceneManagement.SceneManager.LoadScene("ResultScene");
    }

    void UpdateHPText()
    {
        if (hpText != null)
            hpText.text = Mathf.CeilToInt(currentHealth) + " / " + Mathf.CeilToInt(maxHealth);
    }
}
