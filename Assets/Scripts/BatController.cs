using UnityEngine;
using UnityEngine.SceneManagement; 
using System.Collections; 

public class BatController : MonoBehaviour
{
    public float flyForce = 6f;

    [Header("Animação de Batimento de Asas")]
    public Sprite[] spritesAsas;
    public float tempoPorFrame = 0.15f;

    [Header("UI de Game Over (Mesma Cena)")]
    public GameObject telaGameOverObjeto;

    [Header("Configurações do Glitch")]
    public float tempoGlitch = 1.5f; 
    public string tagDoObstaculo = "Obstacle";

    private Rigidbody2D rb;
    private Collider2D meuCollider; 
    private SpriteRenderer spriteRenderer;
    private int indexSpriteAtual = 0;
    private float cronometroAnimação = 0f;

    private Camera cam;
    private bool jogadorMorreu = false;
    private bool estaEmGlitch = false; 

    // NOVO: Tempo de segurança para evitar Game Over instantâneo ao carregar a cena
    private float tempoImunidadeBorda = 0.5f; 

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        meuCollider = GetComponent<Collider2D>(); 
        spriteRenderer = GetComponent<SpriteRenderer>();
        cam = Camera.main;

        rb.gravityScale = 2f;

        if (telaGameOverObjeto != null)
        {
            telaGameOverObjeto.SetActive(false);
        }

        if (spritesAsas != null && spritesAsas.Length > 0 && spriteRenderer != null)
        {
            spriteRenderer.sprite = spritesAsas[indexSpriteAtual];
        }
    }

    void Update()
    {
        if (jogadorMorreu) return;

        // Diminui o tempo de imunidade inicial a cada frame
        if (tempoImunidadeBorda > 0)
        {
            tempoImunidadeBorda -= Time.deltaTime;
        }

        // 1. CONTROLE DE PULO
        bool flyInput =
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetMouseButtonDown(0) ||
            Input.touchCount > 0 &&
            Input.GetTouch(0).phase == TouchPhase.Began;

        if (flyInput)
        {
            rb.linearVelocity = Vector2.zero;
            rb.AddForce(Vector2.up * flyForce, ForceMode2D.Impulse);
        }

        // 2. ANIMAÇÃO
        AnimarAsas();

        // 3. VERIFICAÇÃO DOS LIMITES DA TELA (Só roda após o tempo de segurança acabar)
        if (tempoImunidadeBorda <= 0)
        {
            VerificarLimitesDaTela();
        }
    }

    // Detecta quando bate no obstáculo físico
    void OnCollisionEnter2D(Collision2D colisao)
    {
        if (jogadorMorreu || estaEmGlitch) return;

        // Checa se bateu no obstáculo "Obstacle"
        if (colisao.gameObject.CompareTag(tagDoObstaculo) || 
            (colisao.transform.parent != null && colisao.transform.parent.CompareTag(tagDoObstaculo)))
        {
            // --- Conversa com o seu PlayerHealth verdadeiro ---
            if (PlayerHealth.Instance != null)
            {
                PlayerHealth.Instance.TomarDano(); // Aplica o dano global

                // Se a vida chegou a zero após o dano, para o morcego por segurança
                if (PlayerHealth.Instance.vidas <= 0)
                {
                    jogadorMorreu = true;
                    return;
                }
            }
            else
            {
                Debug.LogWarning("⚠️ PlayerHealth não foi encontrado vindo da cena anterior!");
            }

            // Inicia o glitch e atravessa o objeto
            StartCoroutine(AtivarEfeitoGlitch());
        }
    }

    // Controla o estado de glitch e intangibilidade
    IEnumerator AtivarEfeitoGlitch()
    {
        estaEmGlitch = true;
        meuCollider.isTrigger = true;

        float tempoDecorrido = 0f;

        while (tempoDecorrido < tempoGlitch)
        {
            spriteRenderer.color = new Color(1f, 1f, 1f, 0.2f);
            yield return new WaitForSeconds(0.08f);

            spriteRenderer.color = new Color(1f, 1f, 1f, 0.8f);
            yield return new WaitForSeconds(0.08f);

            tempoDecorrido += 0.16f;
        }

        spriteRenderer.color = new Color(1f, 1f, 1f, 1f);
        meuCollider.isTrigger = false;
        estaEmGlitch = false;
    }

    void AnimarAsas()
    {
        if (spritesAsas == null || spritesAsas.Length == 0 || spriteRenderer == null) return;

        cronometroAnimação += Time.deltaTime;

        if (cronometroAnimação >= tempoPorFrame)
        {
            cronometroAnimação = 0f;
            indexSpriteAtual++;

            if (indexSpriteAtual >= spritesAsas.Length)
            {
                indexSpriteAtual = 0;
            }

            spriteRenderer.sprite = spritesAsas[indexSpriteAtual];
        }
    }

    void VerificarLimitesDaTela()
    {
        if (cam == null) return;

        float limiteInferiorY = cam.ViewportToWorldPoint(new Vector3(0, 0, 0)).y;
        float limiteSuperiorY = cam.ViewportToWorldPoint(new Vector3(0, 1, 0)).y;

        float playerY = transform.position.y;

        if (playerY <= limiteInferiorY || playerY >= limiteSuperiorY)
        {
            AcionarGameOver();
        }
    }

    void AcionarGameOver()
    {
        jogadorMorreu = true;

        rb.linearVelocity = Vector2.zero;
        rb.gravityScale = 0f;

        // Ativa a tela de Game Over local da caverna (caso morra por cair no chão/teto)
        if (telaGameOverObjeto != null)
        {
            telaGameOverObjeto.SetActive(true);
        }
    }

    public void ClicouNoBotaoReiniciar()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); 
    }
}