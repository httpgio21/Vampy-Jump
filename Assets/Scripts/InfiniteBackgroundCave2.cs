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
    private bool sceneLoadingStarted = false; // guard para LoadScene ser chamado 1x só

    // walkSpeed original do Inspector — capturado em Awake antes de Start()s concorrentes
    private float walkSpeedPadrao;

    void Awake()
    {
        // Awake roda antes de qualquer Start(), então captura o valor limpo do Inspector
        // antes que PlayerAutoWalk.Start() force walkSpeed = 2f.
        walkSpeedPadrao = (playerAutoWalk != null) ? playerAutoWalk.walkSpeed : 2f;
    }

    void Start()
    {
        // Reseta todos os estados a cada vez que a cena é carregada.
        // Crítico para o loop: na segunda volta os bools continuariam true
        // se não fossem resetados aqui, travando a cena imediatamente.
        timer = 0f;
        iniciandoFade = false;
        spawnerJaParou = false;
        sceneLoadingStarted = false;

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

        // Para a criação de novos obstáculos antes do fim
        if (timer >= (tempoDeCena - antecedenciaPararSpawn) && !spawnerJaParou)
        {
            spawnerJaParou = true;
            if (obstacleSpawner != null)
                obstacleSpawner.stopSpawn = true;
        }

        // Movimento infinito do fundo
        if (!iniciandoFade)
        {
            transform.Translate(Vector3.left * velocidade * Time.deltaTime);

            if (transform.position.x <= -larguraSprite)
                transform.position += new Vector3(larguraSprite * 2f, 0, 0);
        }

        // Fim do tempo da cena
        if (timer >= tempoDeCena && !iniciandoFade)
        {
            iniciandoFade = true;
            velocidade = 0f;

            if (playerAutoWalk != null)
            {
                playerAutoWalk.walkSpeed = 3f;
                playerAutoWalk.autoWalk = true;
            }

            // Desativa obstáculos que ainda estão na tela
            foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Obstacle"))
                obj.SetActive(false);
        }

        // Fade de saída
        if (iniciandoFade && fadeCanvasGroup != null)
        {
            fadeCanvasGroup.blocksRaycasts = true;

            fadeCanvasGroup.alpha = Mathf.MoveTowards(
                fadeCanvasGroup.alpha, 1f, fadeSpeed * Time.deltaTime);

            if (fadeCanvasGroup.alpha >= 0.99f && !sceneLoadingStarted)
            {
                sceneLoadingStarted = true;

                // Limpa o Vampiro antes de trocar de cena para não
                // carregar estado sujo para a CaveBat
                PlayerJump vampiro = FindFirstObjectByType<PlayerJump>();
                if (vampiro != null)
                {
                    PlayerAutoWalk autoWalkComp = vampiro.GetComponent<PlayerAutoWalk>();
                    if (autoWalkComp != null)
                    {
                        autoWalkComp.autoWalk = false;
                        autoWalkComp.walkSpeed = walkSpeedPadrao;
                    }

                    Destroy(vampiro.gameObject);
                }

                SceneManager.LoadScene(nomeDaCena);
            }
        }
    }
}
