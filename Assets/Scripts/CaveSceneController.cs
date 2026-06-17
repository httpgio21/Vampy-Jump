using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Controlador da cena Second-Floor-Cave.
/// Separa a lógica de cena do movimento do background.
/// Coloque este script num GameObject vazio chamado "SceneController".
/// O background deve ter o script CaveBackgroundLoop (separado).
/// </summary>
public class CaveSceneController : MonoBehaviour
{
    [Header("Duração da Cena")]
    public float tempoDeCena = 20f;

    [Tooltip("Segundos antes do fim para parar o spawner")]
    public float antecedenciaPararSpawn = 5f;

    [Header("Fade")]
    public CanvasGroup fadeCanvasGroup;
    public float fadeSpeed = 1f;

    [Header("Próxima Cena")]
    public string nomeDaCena = "Cave-Bat";

    [Header("Referências")]
    public ObstacleSpawner obstacleSpawner;
    public PlayerAutoWalk playerAutoWalk;

    private float timer;
    private bool iniciandoFade;
    private bool spawnerJaParou;
    private bool sceneLoadingStarted;
    private float walkSpeedPadrao;

    void Awake()
    {
        // Captura walkSpeed antes de qualquer Start() concorrente
        walkSpeedPadrao = (playerAutoWalk != null) ? playerAutoWalk.walkSpeed : 2f;
    }

    void Start()
    {
        // Reset explícito de todo estado — garante loop infinito correto
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
        timer += Time.deltaTime;

        // Para spawner antes do fim
        if (!spawnerJaParou && timer >= tempoDeCena - antecedenciaPararSpawn)
        {
            spawnerJaParou = true;
            if (obstacleSpawner != null)
                obstacleSpawner.stopSpawn = true;
        }

        // Fim da cena: inicia saída
        if (!iniciandoFade && timer >= tempoDeCena)
        {
            iniciandoFade = true;

            if (playerAutoWalk != null)
            {
                playerAutoWalk.walkSpeed = 4f;
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
                IrParaProximaCena();
            }
        }
    }

    void IrParaProximaCena()
    {
        // Limpa o Vampiro antes de sair para não vazar estado para a CaveBat
        PlayerJump vampiro = FindFirstObjectByType<PlayerJump>();
        if (vampiro != null)
        {
            PlayerAutoWalk aw = vampiro.GetComponent<PlayerAutoWalk>();
            if (aw != null)
            {
                aw.autoWalk = false;
                aw.walkSpeed = walkSpeedPadrao;
            }
            Destroy(vampiro.gameObject);
        }

        SceneManager.LoadScene(nomeDaCena);
    }
}
