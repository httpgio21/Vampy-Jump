using UnityEngine;
using UnityEngine.SceneManagement;

public class InfiniteScenarioCave : MonoBehaviour
{
    [Header("Movimento do Cenário")]
    public float speed = 2f;

    [Header("Último Cenário")]
    public Transform ultimoCenario;

    // quando o terceiro cenário chegar nessa posição
    public float stopX = -20f;

    [Header("Player")]
    public Rigidbody2D playerRb;
    public BatController batController;

    [Header("Ponto Central / Final")]
    public Transform centerPoint;
    public float flySpeed = 3f;

    [Header("Efeito de Flutuação (Final)")]
    public float amplitudeVelocidade = 3f;
    public float amplitudeAltura = 0.5f;

    [Header("UI - Fade Final")]
    [Tooltip("Arraste o Canvas Group do seu FadePanel aqui")]
    public CanvasGroup fadeCanvasGroup;
    [Tooltip("Velocidade com que a tela escurece")]
    public float fadeSpeed = 1f;

    [Header("Transição de Cena")]
    [Tooltip("Nome exato da próxima cena (Ex: Second-Floor-Cave)")]
    public string nomeDaProximaCena;

    private bool phaseEnded = false;
    private float initialY;
    private bool sceneLoadingStarted = false;

    void Start()
    {
        // Reseta todos os estados locais a cada vez que a cena é carregada.
        // Sem isso, na segunda volta do loop o phaseEnded e sceneLoadingStarted
        // continuam true do GameObject anterior — travando a cena no primeiro frame.
        phaseEnded = false;
        sceneLoadingStarted = false;

        if (fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = 0f;
            fadeCanvasGroup.blocksRaycasts = false;
        }
    }

    void Update()
    {
        // GAMEPLAY NORMAL
        if (!phaseEnded)
        {
            transform.position += Vector3.left * speed * Time.deltaTime;

            if (ultimoCenario.position.x <= stopX)
            {
                phaseEnded = true;

                // Desativa o GameObject inteiro do morcego para silenciar
                // colisores e VerificarLimitesDaTela() durante o fade.
                if (batController != null)
                    batController.gameObject.SetActive(false);

                // Congela física — necessário pois playerRb ainda existe
                // (só é destruído em CarregarProximaCena).
                playerRb.gravityScale = 0f;
                playerRb.linearVelocity = Vector2.zero;
                playerRb.angularVelocity = 0f;

                initialY = playerRb.transform.position.y;

                if (fadeCanvasGroup != null)
                    fadeCanvasGroup.blocksRaycasts = true;
            }
        }

        // Fade de saída
        if (phaseEnded && fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = Mathf.MoveTowards(
                fadeCanvasGroup.alpha, 1f, fadeSpeed * Time.deltaTime);

            if (fadeCanvasGroup.alpha >= 1f && !sceneLoadingStarted)
            {
                sceneLoadingStarted = true;
                CarregarProximaCena();
            }
        }
    }

    void FixedUpdate()
    {
        // Flutuação do morcego no centro enquanto o fade acontece.
        // playerRb pode ter sido desativado junto com o GameObject;
        // o null-check evita NullReferenceException.
        if (phaseEnded && playerRb != null)
        {
            float newY = initialY + Mathf.Sin(Time.time * amplitudeVelocidade) * amplitudeAltura;
            Vector2 targetPos = new Vector2(centerPoint.position.x, newY);

            Vector2 nextPosition = Vector2.MoveTowards(
                playerRb.position,
                targetPos,
                flySpeed * Time.fixedDeltaTime
            );

            playerRb.MovePosition(nextPosition);
        }
    }

    void CarregarProximaCena()
    {
        if (string.IsNullOrEmpty(nomeDaProximaCena))
        {
            Debug.LogError("InfiniteScenarioCave: nome da próxima cena não definido no Inspector!");
            return;
        }

        // Destrói o GameObject do morcego antes de trocar de cena para que
        // ele não apareça na Second-Floor-Cave por ter DontDestroyOnLoad
        // no PlayerHealth que o referencia indiretamente.
        if (playerRb != null)
            Destroy(playerRb.gameObject);

        SceneManager.LoadScene(nomeDaProximaCena);
    }
}
