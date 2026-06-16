using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Adicione este componente na TelaGameOver (junto ou no lugar do botão existente).
///
/// HIERARQUIA ESPERADA dentro da TelaGameOver:
///
///   TelaGameOver
///   ├── PontuacaoFinal     (TextMeshProUGUI)  — exibe o score da run
///   ├── CampoNome          (TMP_InputField)   — jogador digita o nome
///   ├── BotaoTentarNovamente (Button)         → chama SalvarETentarNovamente()
///   └── BotaoVerRanking      (Button)         → chama SalvarEVerRanking()
///
/// O botão original "Tentar Novamente" do PlayerHealth.ClicouNoBotaoReiniciar()
/// deve ser SUBSTITUÍDO por SalvarETentarNovamente() abaixo.
/// </summary>
public class GameOverRankingSalvar : MonoBehaviour
{
    [Header("UI da Tela de Game Over")]
    [Tooltip("Texto que mostra a pontuação da run. Detectado automaticamente se vazio.")]
    public TextMeshProUGUI textoPontuacaoFinal;

    [Tooltip("Campo onde o jogador digita o nome. Detectado automaticamente se vazio.")]
    public TMP_InputField campoNome;

    [Header("Cenas")]
    public string nomeCenaInicio  = "Night-Vamp-Walking";
    public string nomeCenaRanking = "Ranking";

    private const string NOME_PADRAO        = "Vampiro";
    private const string CHAVE_ULTIMO_INDICE = "ranking_ultimo_indice";

    // ─────────────────────────────────────────────────────────────
    void OnEnable()
    {
        // Busca os componentes automaticamente se não foram arrastados no Inspector
        if (textoPontuacaoFinal == null)
            textoPontuacaoFinal = BuscarFilho<TextMeshProUGUI>("PontuacaoFinal");

        if (campoNome == null)
            campoNome = BuscarFilho<TMP_InputField>("CampoNome");

        // Mostra o score da run assim que a tela aparece
        if (textoPontuacaoFinal != null && ScoreManager.Instance != null)
            textoPontuacaoFinal.text = $"Score: {ScoreManager.Instance.PontuacaoAtual}";
    }

    // ─────────────────────────────────────────────────────────────
    // Salva no ranking e REINICIA o jogo (substitui o botão original)
    public void SalvarETentarNovamente()
    {
        SalvarRanking();
        Reiniciar();
    }

    // ─────────────────────────────────────────────────────────────
    // Salva no ranking e vai para a tela de Ranking
    public void SalvarEVerRanking()
    {
        SalvarRanking();

        Time.timeScale = 1f;
        SceneManager.LoadScene(nomeCenaRanking);
    }

    // ─────────────────────────────────────────────────────────────
    // Lógica comum de salvamento
    private void SalvarRanking()
    {
        if (ScoreManager.Instance == null) return;

        string nome = (campoNome != null && !string.IsNullOrWhiteSpace(campoNome.text))
            ? campoNome.text.Trim()
            : NOME_PADRAO;

        int posicao = ScoreManager.Instance.SalvarNaRanking(nome);

        // Guarda o índice para o RankingDisplay destacar a linha do jogador
        PlayerPrefs.SetInt(CHAVE_ULTIMO_INDICE, posicao);
        PlayerPrefs.Save();
    }

    // ─────────────────────────────────────────────────────────────
    // Reiniciar: mesmo comportamento do PlayerHealth.ClicouNoBotaoReiniciar()
    private void Reiniciar()
    {
        Time.timeScale = 1f;

        if (PlayerHealth.Instance != null)
            PlayerHealth.Instance.AtributosReset();

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetarPontuacao();

        SceneManager.LoadScene(nomeCenaInicio);
    }

    // ─────────────────────────────────────────────────────────────
    T BuscarFilho<T>(string nomeAlvo) where T : Component
    {
        foreach (Transform filho in GetComponentsInChildren<Transform>(true))
            if (filho.name == nomeAlvo)
            {
                T comp = filho.GetComponent<T>();
                if (comp != null) return comp;
            }
        return null;
    }
}
