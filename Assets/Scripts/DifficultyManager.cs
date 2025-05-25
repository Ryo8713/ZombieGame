using UnityEngine;

public class DifficultyManager : MonoBehaviour
{
    public static DifficultyManager Instance;

    public enum Difficulty { Easy, Medium, Hard }
    public Difficulty selectedDifficulty = Difficulty.Medium;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void SetDifficulty(string difficultyStr)
    {
        if (System.Enum.TryParse(difficultyStr, out Difficulty result))
        {
            selectedDifficulty = result;
            Debug.Log($"Selected difficulty: {selectedDifficulty}");
        }
    }
}
