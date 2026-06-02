using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject[] obstaclePrefabs;

    // mesma ordem dos prefabs
    public float[] yOffsets;

    public float minTime = 1f;
    public float maxTime = 2f;

    private float timer;
    private float nextSpawnTime;

    public bool stopSpawn = false;

    void Start()
    {
        SetRandomTime();
    }

    void Update()
    {
        if (stopSpawn)
            return;

        timer += Time.deltaTime;

        if (timer >= nextSpawnTime)
        {
            SpawnObstacle();

            timer = 0;
            SetRandomTime();
        }
    }

    void SpawnObstacle()
    {
        int randomIndex = Random.Range(0, obstaclePrefabs.Length);

        Vector3 spawnPos = transform.position;

        if (randomIndex < yOffsets.Length)
        {
            spawnPos.y += yOffsets[randomIndex];
        }

        Instantiate(
            obstaclePrefabs[randomIndex],
            spawnPos,
            Quaternion.identity
        );
    }

    void SetRandomTime()
    {
        nextSpawnTime = Random.Range(minTime, maxTime);
    }
}