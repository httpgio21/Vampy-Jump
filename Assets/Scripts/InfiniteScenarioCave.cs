using UnityEngine;

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
    public CanvasGroup fadeCanvasGroup; // Corrigido aqui!
    [Tooltip("Velocidade com que a tela escurece")]
    public float fadeSpeed = 1f;

    private bool phaseEnded = false;
    private float initialY; 

    void Start()
    {
        // Garante que o painel comece invisível e sem bloquear cliques no início
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
            // MOVE O CENÁRIO
            transform.position += Vector3.left * speed * Time.deltaTime;

            // verifica se o último cenário chegou no limite
            if (ultimoCenario.position.x <= stopX)
            {
                phaseEnded = true;

                // desativa controle do jogador
                batController.enabled = false;

                // remove gravidade e zera velocidades físicas
                playerRb.gravityScale = 0f;
                playerRb.linearVelocity = Vector2.zero;
                playerRb.angularVelocity = 0f; 

                // Salva a altura em que o morcego parou para iniciar a flutuação
                initialY = playerRb.transform.position.y;

                // Ativa o bloqueio de cliques do painel
                if (fadeCanvasGroup != null)
                {
                    fadeCanvasGroup.blocksRaycasts = true;
                }
            }
        }
        
        // Se a fase acabou, faz o Fade acontecer continuamente no Update
        if (phaseEnded && fadeCanvasGroup != null)
        {
            // Aumenta o Alpha de 0 até 1 suavemente
            fadeCanvasGroup.alpha = Mathf.MoveTowards(fadeCanvasGroup.alpha, 1f, fadeSpeed * Time.deltaTime);
        }
    }

    void FixedUpdate()
    {
        // FINAL DA FASE (Movimento do Morcego)
        if (phaseEnded)
        {
            // 1. Calcula a nova altura (Y) de flutuação suave
            float newY = initialY + Mathf.Sin(Time.time * amplitudeVelocidade) * amplitudeAltura;

            // 2. Alvo combina o X do ponto central e o Y da flutuação
            Vector2 targetPos = new Vector2(centerPoint.position.x, newY);

            // 3. Move o Rigidbody do morcego suavemente usando física
            Vector2 nextPosition = Vector2.MoveTowards(
                playerRb.position,
                targetPos,
                flySpeed * Time.fixedDeltaTime
            );

            playerRb.MovePosition(nextPosition);
        }
    }
}