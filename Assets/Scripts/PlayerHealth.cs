using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [Header("Configurações de Atributos")]
    public int vidas = 3;

    [Header("Configurações de UI (Interface)")]
    public Image[] caixasDeCoracao;
    public Sprite coracaoCheio;
    public Sprite coracaoVazio;
    public GameObject telaGameOver;

    [Header("Cenas")]
    public string nomeCenaInicio  = "Night-Vamp-Walking";
    public string nomeCenaRanking = "Ranking";

    private bool isGameOver = false;

    // Estado original do ScoreText para restaurar ao reiniciar
    private RectTransform scoreTextRect;
    private Transform     scoreTextPaiOriginal;
    private Vector2       scoreTextAnchoredPosOriginal;
    private Vector2       scoreTextAnchorMinOriginal;
    private Vector2       scoreTextAnchorMaxOriginal;
    private Vector2       scoreTextPivotOriginal;

    // ── Singleton ─────────────────────────────────────────────────
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
            SceneManager.sceneLoaded += AoCarregarNovaCena;
        }
        else
        {
            Destroy(gameObject);
            return;
        }
    }

    void OnDestroy()
    {
        SceneManager.sceneLoaded -= AoCarregarNovaCena;
    }

    void AoCarregarNovaCena(Scene cena, LoadSceneMode modo)
    {
        Time.timeScale = 1f;
        isGameOver = false;
        scoreTextRect = null;

        ReconectarComponentesDeUI();

        if (telaGameOver != null)
            telaGameOver.SetActive(false);

        AtualizarCoracoesNaTela();
    }

    // ── Reconexão de UI ───────────────────────────────────────────
    void ReconectarComponentesDeUI()
    {
        Canvas canvasDaCena = Object.FindFirstObjectByType<Canvas>();
        if (canvasDaCena == null)
        {
            Debug.LogError("Nenhum Canvas encontrado na cena atual!");
            return;
        }

        // TelaGameOver
        Transform tGameOver = EncontrarFilhoInativoPorNome(canvasDaCena.transform, "TelaGameOver");
        if (tGameOver != null)
        {
            telaGameOver = tGameOver.gameObject;
            ConectarBotoesGameOver(telaGameOver);
        }
        else
        {
            Debug.LogWarning("Aviso: 'TelaGameOver' não foi encontrado nesta cena.");
        }

        // Corações
        Transform painelCoracoes = EncontrarFilhoInativoPorNome(canvasDaCena.transform, "vampy_hearts");
        if (painelCoracoes != null)
        {
            caixasDeCoracao = painelCoracoes.GetComponentsInChildren<Image>(true);
            Debug.Log("Corações conectados. Total: " + caixasDeCoracao.Length);
        }
        else
        {
            Debug.LogError("Erro Crítico: 'vampy_hearts' não encontrado no Canvas desta cena!");
        }

        // ScoreText — salva estado original para restaurar depois
        Transform tScore = EncontrarFilhoInativoPorNome(canvasDaCena.transform, "ScoreText");
        if (tScore != null)
        {
            scoreTextRect = tScore.GetComponent<RectTransform>();
            if (scoreTextRect != null)
            {
                scoreTextPaiOriginal         = scoreTextRect.parent;
                scoreTextAnchoredPosOriginal = scoreTextRect.anchoredPosition;
                scoreTextAnchorMinOriginal   = scoreTextRect.anchorMin;
                scoreTextAnchorMaxOriginal   = scoreTextRect.anchorMax;
                scoreTextPivotOriginal       = scoreTextRect.pivot;
            }
        }
    }

    void ConectarBotoesGameOver(GameObject tela)
    {
        Button[] botoes = tela.GetComponentsInChildren<Button>(true);
        if (botoes.Length == 0)
        {
            Debug.LogWarning("Nenhum botão encontrado dentro da TelaGameOver.");
            return;
        }

        foreach (Button btn in botoes)
        {
            btn.onClick.RemoveAllListeners();
            string nomeBotao = btn.gameObject.name.ToLower();

            if (nomeBotao.Contains("ranking") || nomeBotao.Contains("ver"))
            {
                btn.onClick.AddListener(ClicouVerRanking);
                Debug.Log($"Botão '{btn.gameObject.name}' → Ver Ranking");
            }
            else
            {
                btn.onClick.AddListener(ClicouNoBotaoReiniciar);
                Debug.Log($"Botão '{btn.gameObject.name}' → Tentar Novamente");
            }
        }
    }

    // ── Mover / Restaurar ScoreText ───────────────────────────────

    void MoverScoreTextParaGameOver()
    {
        if (scoreTextRect == null || telaGameOver == null) return;

        // Move para dentro da TelaGameOver
        scoreTextRect.SetParent(telaGameOver.transform, false);

        // Inferior central
        scoreTextRect.anchorMin        = new Vector2(0.5f, 0f);
        scoreTextRect.anchorMax        = new Vector2(0.5f, 0f);
        scoreTextRect.pivot            = new Vector2(0.5f, 0f);
        scoreTextRect.anchoredPosition = new Vector2(100f, 450f);
    }

    void RestaurarScoreText()
    {
        if (scoreTextRect == null || scoreTextPaiOriginal == null) return;

        scoreTextRect.SetParent(scoreTextPaiOriginal, false);
        scoreTextRect.anchorMin        = scoreTextAnchorMinOriginal;
        scoreTextRect.anchorMax        = scoreTextAnchorMaxOriginal;
        scoreTextRect.pivot            = scoreTextPivotOriginal;
        scoreTextRect.anchoredPosition = scoreTextAnchoredPosOriginal;
    }

    // ── Dano e Game Over ──────────────────────────────────────────
    public void TomarDano()
    {
        if (isGameOver) return;

        vidas--;
        Debug.Log("Vidas restantes: " + vidas);

        AtualizarCoracoesNaTela();

        if (vidas <= 0)
        {
            Debug.Log("GAME OVER!");
            isGameOver = true;

            if (ScoreManager.Instance != null)
                ScoreManager.Instance.PausarContagem();

            if (telaGameOver != null)
                telaGameOver.SetActive(true);

            MoverScoreTextParaGameOver();

            Time.timeScale = 0f;
        }
    }

    // ── Botões ────────────────────────────────────────────────────

    public void ClicouNoBotaoReiniciar()
    {
        SalvarNaRanking();
        RestaurarScoreText();

        Time.timeScale = 1f;
        AtributosReset();

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetarPontuacao();

        SceneManager.LoadScene(nomeCenaInicio);
    }

    public void ClicouVerRanking()
    {
        SalvarNaRanking();

        Time.timeScale = 1f;
        SceneManager.LoadScene(nomeCenaRanking);
    }

    private void SalvarNaRanking()
    {
        if (ScoreManager.Instance == null) return;

        int posicao = ScoreManager.Instance.SalvarNaRanking("Vampiro");
        PlayerPrefs.SetInt("ranking_ultimo_indice", posicao);
        PlayerPrefs.Save();
    }

    // ── Helpers ───────────────────────────────────────────────────
    public void AtributosReset()
    {
        vidas = 3;
        isGameOver = false;
    }

    public void AtualizarCoracoesNaTela()
    {
        if (caixasDeCoracao == null || caixasDeCoracao.Length == 0)
        {
            Debug.LogWarning("Nenhum coração associado para atualizar na tela!");
            return;
        }

        for (int i = 0; i < caixasDeCoracao.Length; i++)
        {
            if (caixasDeCoracao[i] == null) continue;
            caixasDeCoracao[i].sprite = (i < vidas) ? coracaoCheio : coracaoVazio;
        }
    }

    Transform EncontrarFilhoInativoPorNome(Transform pai, string nomeAlvo)
    {
        foreach (Transform filho in pai.GetComponentsInChildren<Transform>(true))
            if (filho.gameObject.name.ToLower() == nomeAlvo.ToLower())
                return filho;
        return null;
    }
}
