using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;

    public float minTime = 1f;
    public float maxTime = 2f;

    private float timer;
    private float nextSpawnTime;

    public bool stopSpawn = false;

    void Start()
    {
        // SPAWN IMEDIATO
        SpawnObstacle();

        SetRandomTime();
    }

    void Update()
    {
        if(stopSpawn)
            return;

        timer += Time.deltaTime;

        if(timer >= nextSpawnTime)
        {
            SpawnObstacle();

            timer = 0;

            SetRandomTime();
        }
    }

    void SpawnObstacle()
    {
        Instantiate(obstaclePrefab, transform.position, Quaternion.identity);
    }

    void SetRandomTime()
    {
        nextSpawnTime = Random.Range(minTime, maxTime);
    }
}