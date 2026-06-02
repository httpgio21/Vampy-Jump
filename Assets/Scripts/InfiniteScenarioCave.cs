using UnityEngine;
// 1. IMPORTANTE: Adicione a biblioteca de gerenciamento de cenas
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

    // 2. ADICIONE: Nome da terceira cena exatamente como está na Unity
    [Header("Transição de Cena")]
    [Tooltip("Nome exato da terceira cena (Digite: Second-Floor-Cave)")]
    public string nomeDaProximaCena;

    private bool phaseEnded = false;
    private float initialY; 
    private bool sceneLoadingStarted = false; // Evita carregar a cena múltiplas vezes

    void Start()
    {
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

                batController.enabled = false;

                playerRb.gravityScale = 0f;
                playerRb.linearVelocity = Vector2.zero;
                playerRb.angularVelocity = 0f; 

                initialY = playerRb.transform.position.y;

                if (fadeCanvasGroup != null)
                {
                    fadeCanvasGroup.blocksRaycasts = true;
                }
            }
        }
        
        // Se a fase acabou, faz o Fade acontecer continuamente
        if (phaseEnded && fadeCanvasGroup != null)
        {
            fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, 1f, fadeSpeed * Time.deltaTime);

            // 3. NOVO: Se a tela ficou totalmente escura, muda para a terceira cena
            if (fadeCanvasGroup.alpha >= 1f && !sceneLoadingStarted)
            {
                sceneLoadingStarted = true;
                CarregarProximaCena();
            }
        }
    }

    void FixedUpdate()
    {
        if (phaseEnded)
        {
            // Calcula a flutuação do morcego no final da fase
            float newY = initialY + Mathf.Sin(Time.time * amplitudeVelocidade) * amplitudeAltura;
            
            // LINHA CORRIGIDA: Criando o vetor de posição corretamente
            Vector2 targetPos = new Vector2(centerPoint.position.x, newY);

            Vector2 nextPosition = Vector2.MoveTowards(
                playerRb.position,
                targetPos,
                flySpeed * Time.fixedDeltaTime
            );

            playerRb.MovePosition(nextPosition);
        }
    }

    // 4. METODO ATUALIZADO: Evita que o morcego invada a Cena 3 e quebre o Vampiro nativo
    void CarregarProximaCena()
    {
        if (!string.IsNullOrEmpty(nomeDaProximaCena))
        {
            // DESTRÓI O CORPO DO MORCEGO ANTES DA MUDANÇA:
            // Isso impede que o DontDestroyOnLoad da vida traga o corpo do morcego junto para a Cena 3
            if (playerRb != null)
            {
                Destroy(playerRb.gameObject);
            }

            SceneManager.LoadScene(nomeDaProximaCena);
        }
        else
        {
            Debug.LogError("Esqueceu de colocar o nome da terceira cena no Inspector!");
        }
    }
}