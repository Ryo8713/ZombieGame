using UnityEngine;

public class ZombieSpawner : MonoBehaviour
{
    public GameObject zombiePrefab;
    public float spawnRadius = 20f;

    private float spawnInterval = 5f;
    private float minInterval = 1f;
    private float acceleration = 0.2f;
    private float timer = 0f;
    private float elapsed = 0f;
    private float surviveTime;

    void Start()
    {
        var difficulty = DifficultyManager.Instance.selectedDifficulty;

        switch (difficulty)
        {
            case DifficultyManager.Difficulty.Easy:
                spawnInterval = 6f;
                minInterval = 2f;
                acceleration = 0.1f;
                break;
            case DifficultyManager.Difficulty.Medium:
                spawnInterval = 4f;
                minInterval = 1f;
                acceleration = 0.2f;
                break;
            case DifficultyManager.Difficulty.Hard:
                spawnInterval = 2.5f;
                minInterval = 0.5f;
                acceleration = 0.3f;
                break;
        }

        surviveTime = FindFirstObjectByType<GameManager>().GetSurviveTime();

        Debug.Log($"🧟 生成初始：{spawnInterval}s，加速率：{acceleration}, 最小間隔：{minInterval}");
    }

    void Update()
    {
        timer += Time.deltaTime;
        elapsed += Time.deltaTime;

        if (elapsed >= surviveTime) return;

        if (timer >= spawnInterval)
        {
            SpawnZombie();
            timer = 0f;

            spawnInterval = Mathf.Max(minInterval, spawnInterval - acceleration);
        }
    }

    void SpawnZombie()
    {
        Vector3 dir = Random.insideUnitSphere;
        dir.y = 0;
        Vector3 pos = transform.position + dir.normalized * spawnRadius;
        Instantiate(zombiePrefab, pos, Quaternion.identity);
    }
}
