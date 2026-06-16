using TMPro;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// Popula a tela de ranking com as 6 melhores pontuações salvas.
///
/// HIERARQUIA ESPERADA NO CANVAS:
///
///   Canvas
///   └── RankingPanel
///       ├── Titulo           (TextMeshProUGUI)  — opcional
///       ├── Entry_0          (GameObject)
///       │   ├── Posicao      (TextMeshProUGUI)  ex: "#1"
///       │   ├── Nome         (TextMeshProUGUI)
///       │   ├── Score        (TextMeshProUGUI)
///       │   └── DataHora     (TextMeshProUGUI)
///       ├── Entry_1 ...
///       └── Entry_5
///
/// Os GameObjects devem se chamar Entry_0 até Entry_5.
/// Os filhos devem se chamar exatamente Posicao, Nome, Score, DataHora.
/// </summary>
public class RankingDisplay : MonoBehaviour
{
    [Header("Entradas do Ranking (arraste Entry_0 até Entry_5)")]
    [Tooltip("Arraste os 6 GameObjects de entrada em ordem. Deixe vazio para detecção automática.")]
    public GameObject[] entradasRanking = new GameObject[6];

    [Header("Destaque da última pontuação (opcional)")]
    [Tooltip("Cor de fundo da linha do jogador atual")]
    public Color corDestaque = new Color(1f, 0.85f, 0f, 0.25f);

    [Tooltip("Índice da última posição obtida (-1 = nenhum destaque). " +
             "Preenchido automaticamente por GameOverRankingSalvar.")]
    public int indiceMeuScore = -1;

    // ─────────────────────────────────────────────────────────────
    void Start()
    {
        // Lê o índice salvo pelo GameOverRankingSalvar (-1 se não houver)
        indiceMeuScore = PlayerPrefs.GetInt("ranking_ultimo_indice", -1);

        // Auto-detecção se os campos não foram preenchidos no Inspector
        if (entradasRanking == null || entradasRanking.Length == 0 || entradasRanking[0] == null)
            DetectarEntradas();

        PopularRanking();
    }

    // ─────────────────────────────────────────────────────────────
    void DetectarEntradas()
    {
        entradasRanking = new GameObject[6];
        for (int i = 0; i < 6; i++)
        {
            // Procura por "Entry_0", "Entry_1" etc. em toda a hierarquia
            Transform t = BuscarPorNome(transform.root, "Entry_" + i);
            if (t != null)
                entradasRanking[i] = t.gameObject;
            else
                Debug.LogWarning($"RankingDisplay: 'Entry_{i}' não encontrado na cena.");
        }
    }

    // ─────────────────────────────────────────────────────────────
    void PopularRanking()
    {
        if (ScoreManager.Instance == null)
        {
            Debug.LogError("RankingDisplay: ScoreManager não encontrado. " +
                           "Certifique-se de iniciar o jogo pela cena principal.");
            return;
        }

        ScoreManager.EntradaRanking[] dados = ScoreManager.Instance.CarregarRanking();

        for (int i = 0; i < entradasRanking.Length; i++)
        {
            if (entradasRanking[i] == null) continue;

            bool temDado = i < dados.Length;

            // Textos
            SetTexto(entradasRanking[i], "Posicao",  $"#{i + 1}");
            SetTexto(entradasRanking[i], "Nome",     temDado ? dados[i].nome             : "Vampiro");
            SetTexto(entradasRanking[i], "Score",    temDado ? dados[i].score.ToString() : "0");
            SetTexto(entradasRanking[i], "DataHora", temDado ? dados[i].dataHora         : "--/--/---- --:--");

            // Destaque visual na linha do jogador atual
            if (i == indiceMeuScore)
                AplicarDestaque(entradasRanking[i]);
        }
    }

    // ─────────────────────────────────────────────────────────────
    void SetTexto(GameObject entrada, string nomeFilho, string valor)
    {
        Transform filho = entrada.transform.Find(nomeFilho);
        if (filho == null)
        {
            Debug.LogWarning($"RankingDisplay: filho '{nomeFilho}' não encontrado em '{entrada.name}'.");
            return;
        }

        TextMeshProUGUI tmp = filho.GetComponent<TextMeshProUGUI>();
        if (tmp != null)
            tmp.text = valor;
    }

    // ─────────────────────────────────────────────────────────────
    void AplicarDestaque(GameObject entrada)
    {
        // Tenta colorir via Image de fundo da linha
        Image img = entrada.GetComponent<Image>();
        if (img != null)
        {
            img.color = corDestaque;
            return;
        }

        // Fallback: muda a cor de todos os textos da linha para amarelo
        foreach (TextMeshProUGUI txt in entrada.GetComponentsInChildren<TextMeshProUGUI>())
            txt.color = new Color(1f, 0.85f, 0f, 1f);
    }

    // ─────────────────────────────────────────────────────────────
    Transform BuscarPorNome(Transform raiz, string nome)
    {
        foreach (Transform filho in raiz.GetComponentsInChildren<Transform>(true))
            if (filho.name == nome) return filho;
        return null;
    }

    // ─────────────────────────────────────────────────────────────
    // Chame este método pelo botão "Limpar Ranking" (opcional)
    public void LimparRanking()
    {
        if (ScoreManager.Instance != null)
            ScoreManager.Instance.LimparRanking();

        PopularRanking();   // atualiza a tela imediatamente
    }
}
