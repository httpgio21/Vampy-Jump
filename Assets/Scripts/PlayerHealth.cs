using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Collections.Generic;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

    [Header("Configurações de Atributos")]
    public int vidas = 3;

    [Header("Configurações de UI (Interface) - Corações")]
    public Image[] caixasCoracaoEsquerdo;
    public Image[] caixasCoracaoDireito;
    public Sprite coracaoCheio;
    public Sprite coracaoVazio;

    [Header("Configurações de UI (Interface) - Game Over")]
    public GameObject telaGameOverEsquerda;
    public GameObject telaGameOverDireita;

    [Header("Cenas")]
    public string nomeCenaInicio  = "Night-Vamp-Walking";
    public string nomeCenaRanking = "Ranking";

    private bool isGameOver = false;

    // Estrutura para salvar o estado original de múltiplos ScoreTexts (VR Left/Right)
    private class ScoreTextData
    {
        public RectTransform rect;
        public Transform paiOriginal;
        public Vector2 anchoredPosOriginal;
        public Vector2 anchorMinOriginal;
        public Vector2 anchorMaxOriginal;
        public Vector2 pivotOriginal;
    }
    private List<ScoreTextData> listaScoresModificados = new List<ScoreTextData>();

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
        listaScoresModificados.Clear();

        ReconectarComponentesDeUI();

        if (telaGameOverEsquerda != null) telaGameOverEsquerda.SetActive(false);
        if (telaGameOverDireita != null) telaGameOverDireita.SetActive(false);

        AtualizarCoracoesNaTela();
    }

    // ── Reconexão de UI Multi-Canvas (VR) ───────────────────────────
    void ReconectarComponentesDeUI()
    {
        // Busca todos os Canvas ativos e inativos da cena dividida
        Canvas[] todosOsCanvas = Object.FindObjectsByType<Canvas>(FindObjectsSortMode.None);
        if (todosOsCanvas.Length == 0)
        {
            Debug.LogError("Nenhum Canvas encontrado na cena atual!");
            return;
        }

        // Limpa referências antigas antes de remontar
        telaGameOverEsquerda = null;
        telaGameOverDireita = null;
        caixasCoracaoEsquerdo = null;
        caixasCoracaoDireito = null;

        foreach (Canvas canvas in todosOsCanvas)
        {
            // Define o lado verificando se o Canvas ou o pai dele contêm "left" no nome
            bool ehEsquerdo = canvas.gameObject.name.ToLower().Contains("left") || 
                              canvas.transform.parent?.name.ToLower().Contains("left") == true;

            // 1. Reconectar TelaGameOver
            Transform tGameOver = EncontrarFilhoInativoPorNome(canvas.transform, "TelaGameOver");
            if (tGameOver != null)
            {
                if (ehEsquerdo)
                {
                    telaGameOverEsquerda = tGameOver.gameObject;
                    ConectarBotoesGameOver(telaGameOverEsquerda);
                }
                else
                {
                    telaGameOverDireita = tGameOver.gameObject;
                    ConectarBotoesGameOver(telaGameOverDireita);
                }
            }

            // 2. Reconectar Corações (vampy_hearts)
            Transform painelCoracoes = EncontrarFilhoInativoPorNome(canvas.transform, "vampy_hearts");
            if (painelCoracoes != null)
            {
                if (ehEsquerdo)
                    caixasCoracaoEsquerdo = painelCoracoes.GetComponentsInChildren<Image>(true);
                else
                    caixasCoracaoDireito = painelCoracoes.GetComponentsInChildren<Image>(true);
            }

            // 3. Reconectar ScoreText e salvar dados originais
            Transform tScore = EncontrarFilhoInativoPorNome(canvas.transform, "ScoreText");
            if (tScore != null)
            {
                RectTransform rText = tScore.GetComponent<RectTransform>();
                if (rText != null)
                {
                    ScoreTextData data = new ScoreTextData
                    {
                        rect = rText,
                        paiOriginal = rText.parent,
                        anchoredPosOriginal = rText.anchoredPosition,
                        anchorMinOriginal = rText.anchorMin,
                        anchorMaxOriginal = rText.anchorMax,
                        pivotOriginal = rText.pivot
                    };
                    listaScoresModificados.Add(data);
                }
            }
        }

        // Avisos de debug para validação das duas telas
        if (telaGameOverEsquerda == null || telaGameOverDireita == null)
            Debug.LogWarning("Aviso: Certifique-se de que os Canvas VR contêm as palavras 'Left' e 'Right' para mapear ambos os GameOvers.");
    }

    void ConectarBotoesGameOver(GameObject tela)
    {
        Button[] botoes = tela.GetComponentsInChildren<Button>(true);
        foreach (Button btn in botoes)
        {
            btn.onClick.RemoveAllListeners();
            string nomeBotao = btn.gameObject.name.ToLower();

            if (nomeBotao.Contains("ranking") || nomeBotao.Contains("ver"))
                btn.onClick.AddListener(ClicouVerRanking);
            else
                btn.onClick.AddListener(ClicouNoBotaoReiniciar);
        }
    }

    // ── Mover / Restaurar ScoreTexts Simetricamente ────────────────
    void MoverScoreTextParaGameOver()
    {
        foreach (var score in listaScoresModificados)
        {
            if (score.rect == null) continue;

            // Identifica se este texto específico era do Canvas Left ou Right
            bool ehEsquerdo = score.paiOriginal.name.ToLower().Contains("left") || 
                              score.paiOriginal.root.name.ToLower().Contains("left") ||
                              score.rect.gameObject.name.ToLower().Contains("left");

            GameObject alvoGameOver = ehEsquerdo ? telaGameOverEsquerda : telaGameOverDireita;

            if (alvoGameOver != null)
            {
                score.rect.SetParent(alvoGameOver.transform, false);

                // Configuração simétrica inferior central dentro do respectivo Game Over
                score.rect.anchorMin = new Vector2(0.5f, 0f);
                score.rect.anchorMax = new Vector2(0.5f, 0f);
                score.rect.pivot     = new Vector2(0.5f, 0f);
                score.rect.anchoredPosition = new Vector2(100f, 450f); 
            }
        }
    }

    void RestaurarScoreText()
    {
        foreach (var score in listaScoresModificados)
        {
            if (score.rect == null || score.paiOriginal == null) continue;

            score.rect.SetParent(score.paiOriginal, false);
            score.rect.anchorMin        = score.anchorMinOriginal;
            score.rect.anchorMax        = score.anchorMaxOriginal;
            score.rect.pivot            = score.pivotOriginal;
            score.rect.anchoredPosition = score.anchoredPosOriginal;
        }
    }

    // ── Dano e Game Over VR ─────────────────────────────────────────
    public void TomarDano()
    {
        if (isGameOver) return;

        vidas--;
        Debug.Log("Vidas restantes: " + vidas);

        AtualizarCoracoesNaTela();

        if (vidas <= 0)
        {
            Debug.Log("GAME OVER VR!");
            isGameOver = true;

            if (ScoreManager.Instance != null)
                ScoreManager.Instance.PausarContagem();

            // Ativa simultaneamente ambas as telas para visualização em óculos
            if (telaGameOverEsquerda != null) telaGameOverEsquerda.SetActive(true);
            if (telaGameOverDireita != null) telaGameOverDireita.SetActive(true);

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

    public void AtributosReset()
    {
        vidas = 3;
        isGameOver = false;
    }

    public void AtualizarCoracoesNaTela()
    {
        // Atualiza Olho Esquerdo
        if (caixasCoracaoEsquerdo != null)
        {
            for (int i = 0; i < caixasCoracaoEsquerdo.Length; i++)
            {
                if (caixasCoracaoEsquerdo[i] == null) continue;
                caixasCoracaoEsquerdo[i].sprite = (i < vidas) ? coracaoCheio : coracaoVazio;
            }
        }

        // Atualiza Olho Direito
        if (caixasCoracaoDireito != null)
        {
            for (int i = 0; i < caixasCoracaoDireito.Length; i++)
            {
                if (caixasCoracaoDireito[i] == null) continue;
                caixasCoracaoDireito[i].sprite = (i < vidas) ? coracaoCheio : coracaoVazio;
            }
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