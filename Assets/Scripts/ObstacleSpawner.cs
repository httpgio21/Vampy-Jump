using UnityEngine;

public class ObstacleSpawner : MonoBehaviour
{
    public GameObject obstaclePrefab;

    public float minTime = 2.5f;
    public float maxTime = 5f;

    private float timer;
    private float nextSpawnTime;

    void Start()
    {
        SetRandomTime();
    }

    void Update()
    {
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