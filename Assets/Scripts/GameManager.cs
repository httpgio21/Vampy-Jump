using UnityEngine;

public class GameManager : MonoBehaviour
{
    public PlayerAutoWalk player;
    public ObstacleSpawner obstacleSpawner;

    public MoveScenario floor1;
    public MoveScenario floor2;
    public MoveScenario floor3;
    public MoveScenario cave;

    // tempo até começar a sequência final
    public float tempoParaFinal = 12f;

    // quantos segundos antes da caverna parar os obstáculos
    public float tempoPararSpawns = 3f;

    void Start()
    {

        //ScoreManager.Instance.LimparRanking();

        // PARA OS SPAWNS ANTES
        Invoke(nameof(StopSpawners), tempoParaFinal - tempoPararSpawns);

        // COMEÇA FINAL
        Invoke(nameof(StartFinal), tempoParaFinal);
    }

    void StopSpawners()
    {
        obstacleSpawner.stopSpawn = true;
    }

    void StartFinal()
    {
        // pisos não reaparecem mais
        floor1.infiniteLoop = false;
        floor2.infiniteLoop = false;
        floor3.infiniteLoop = false;

        // espera a caverna entrar totalmente
        Invoke(nameof(StopScenario), 7f);
    }

    void StopScenario()
    {
        // para cenário inteiro
        floor1.stopMoving = true;
        floor2.stopMoving = true;
        floor3.stopMoving = true;
        cave.stopMoving = true;
        // player anda sozinho
        player.autoWalk = true;
    }
}