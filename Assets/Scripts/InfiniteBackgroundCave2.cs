using UnityEngine;
using UnityEngine.SceneManagement;

public class InfiniteBackgroundCave2 : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 2f;
    public float larguraSprite = 20f;

    [Header("Duração da Cena")]
    public float tempoDeCena = 20f;

    [Tooltip("Quantos segundos ANTES do fim da cena o spawner deve parar? (Ex: 5)")]
    public float antecedenciaPararSpawn = 5f;

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
    private bool spawnerJaParou = false;

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
        // Contador da cena
        timer += Time.deltaTime;

        // 1. Para a criação de obstáculos antes do tempo final da cena
        if (timer >= (tempoDeCena - antecedenciaPararSpawn) && !spawnerJaParou)
        {
            spawnerJaParou = true;
            if (obstacleSpawner != null)
            {
                obstacleSpawner.stopSpawn = true;
            }
        }

        // Movimento infinito do fundo
        if (!iniciandoFade)
        {
            transform.Translate(Vector3.left * velocidade * Time.deltaTime);

            if (transform.position.x <= -larguraSprite)
            {
                transform.position += new Vector3(larguraSprite * 2f, 0, 0);
            }
        }

        // Fim do tempo da cena
        if (timer >= tempoDeCena && !iniciandoFade)
        {
            iniciandoFade = true;

            // Para o fundo
            velocidade = 0f;

            // Faz o Vampiro andar sozinho
            if (playerAutoWalk != null)
            {
                playerAutoWalk.walkSpeed = 3f;
                playerAutoWalk.autoWalk = true;
            }

            // REMOVIDO: Não deleta mais os obstáculos na tela de forma abrupta,
            // eles já saíram da tela sozinhos por causa do tempo de antecedência.
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