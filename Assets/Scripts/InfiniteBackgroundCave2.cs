using UnityEngine;
using UnityEngine.SceneManagement;

public class InfiniteBackgroundCave2 : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 2f;
    public float larguraSprite = 20f;

    [Header("Duração da Cena")]
    public float tempoDeCena = 20f;

    [Header("Fade")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeSpeed = 1f;

    [Header("Próxima Cena")]
    public string nomeDaCena;

    [Header("Spawner")]
    public ObstacleSpawner obstacleSpawner;

    [Header("Player")]
    public PlayerAutoWalk playerAutoWalk;

    private float timer;
    private bool iniciandoFade = false;

    void Start()
    {
        timer = 0f;

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }

    void Update()
    {
        // Movimento infinito do fundo
        if (!iniciandoFade)
        {
            transform.Translate(Vector3.left * velocidade * Time.deltaTime);

            if (transform.position.x <= -larguraSprite)
            {
                transform.position += new Vector3(larguraSprite * 2f, 0, 0);
            }
        }

        // Contador da cena
        timer += Time.deltaTime;

        if (timer >= tempoDeCena && !iniciandoFade)
        {
            iniciandoFade = true;

            // Para o fundo
            velocidade = 0f;

            // Para o spawner
            if (obstacleSpawner != null)
            {
                obstacleSpawner.stopSpawn = true;
            }

            // Faz o Vampiro andar sozinho
            if (playerAutoWalk != null)
            {
                playerAutoWalk.walkSpeed = 3f;
                playerAutoWalk.autoWalk = true;
            }

            // Remove todas as gosmas da tela
            MoveGosma[] obstacles = FindObjectsOfType<MoveGosma>();

            foreach (MoveGosma obstacle in obstacles)
            {
                Destroy(obstacle.gameObject);
            }
        }

        // Fade
        if (iniciandoFade && fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = true;

            fadeCanvasGroup.alpha = Mathf.MoveTowards(
                fadeCanvasGroup.alpha,
                1f,
                fadeSpeed * Time.deltaTime
            );

            // Quando a tela estiver totalmente escura
            if (fadeCanvasGroup.alpha >= 0.99f)
            {
                PlayerJump vampiro = FindFirstObjectByType<PlayerJump>();

                if (vampiro != null)
                {
                    Destroy(vampiro.gameObject);
                }

                SceneManager.LoadScene(nomeDaCena);
            }
        }
    }
}