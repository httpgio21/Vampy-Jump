using System;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ScoreManager : MonoBehaviour
{
    public static ScoreManager Instance { get; private set; }

    // ── Configuração ──────────────────────────────────────────────
    [Header("Pontuação")]
    [Tooltip("Pontos ganhos por segundo de sobrevivência")]
    public float pontosPorSegundo = 10f;

    // Ranking fixo em 6 entradas conforme solicitado
    private const int TAMANHO_RANKING = 6;

    // ── Estado interno ────────────────────────────────────────────
    private float pontuacaoAtual = 0f;
    private bool contando = false;

    // Chaves do PlayerPrefs
    private const string CHAVE_TOTAL = "ranking_total";
    private const string CHAVE_NOME = "ranking_{0}_nome";
    private const string CHAVE_SCORE = "ranking_{0}_score";
    private const string CHAVE_DATA = "ranking_{0}_data";  // salva como string ISO

    // ── Singleton + persistência entre cenas ─────────────────────
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
        if (cena.name == "Menu" || cena.name == "Integrantes" || cena.name == "Ranking")
            PausarContagem();
        else
            RetomarContagem();
    }

    // ── Update ────────────────────────────────────────────────────
    void Update()
    {
        if (contando && Time.timeScale > 0f)
            pontuacaoAtual += pontosPorSegundo * Time.deltaTime;
    }

    // ── API pública ───────────────────────────────────────────────

    public int PontuacaoAtual => Mathf.FloorToInt(pontuacaoAtual);

    // FUNÇÃO QUE A BOLSA DE SANGUE CHAMARÁ PARA ADICIONAR PONTOS
    public void AdicionarPontos(int quantidade)
    {
        if (contando)
        {
            pontuacaoAtual += quantidade;
            Debug.Log($"🩸 Bolsa coletada! +{quantidade} pontos. Total atual: {PontuacaoAtual}");
        }
    }

    public void RetomarContagem() => contando = true;
    public void PausarContagem() => contando = false;

    public void ResetarPontuacao()
    {
        pontuacaoAtual = 0f;
        contando = true;
    }

    // Salva pontuação com data/hora atual e retorna a posição no ranking (0 = 1º lugar)
    // Retorna -1 se a pontuação não entrou no top 6
    public int SalvarNaRanking(string nomeJogador)
    {
        PausarContagem();

        int score = PontuacaoAtual;
        string dataHora = DateTime.Now.ToString("dd/MM/yyyy HH:mm");

        // Carrega ranking existente
        int total = PlayerPrefs.GetInt(CHAVE_TOTAL, 0);

        string[] nomes = new string[total];
        int[] scores = new int[total];
        string[] datas = new string[total];

        for (int i = 0; i < total; i++)
        {
            nomes[i] = PlayerPrefs.GetString(string.Format(CHAVE_NOME, i), "---");
            scores[i] = PlayerPrefs.GetInt(string.Format(CHAVE_SCORE, i), 0);
            datas[i] = PlayerPrefs.GetString(string.Format(CHAVE_DATA, i), "");
        }

        // Insere a nova entrada
        Array.Resize(ref nomes, total + 1);
        Array.Resize(ref scores, total + 1);
        Array.Resize(ref datas, total + 1);
        nomes[total] = nomeJogador;
        scores[total] = score;
        datas[total] = dataHora;

        // Ordena por score decrescente (insertion sort simples)
        for (int i = 1; i < scores.Length; i++)
        {
            int tmpS = scores[i];
            string tmpN = nomes[i];
            string tmpD = datas[i];
            int j = i - 1;
            while (j >= 0 && scores[j] < tmpS)
            {
                scores[j + 1] = scores[j];
                nomes[j + 1] = nomes[j];
                datas[j + 1] = datas[j];
                j--;
            }
            scores[j + 1] = tmpS;
            nomes[j + 1] = tmpN;
            datas[j + 1] = tmpD;
        }

        // Descobre posição final da entrada recém inserida
        int posicao = -1;
        for (int i = 0; i < scores.Length; i++)
        {
            if (scores[i] == score && nomes[i] == nomeJogador && datas[i] == dataHora)
            {
                posicao = i;
                break;
            }
        }

        // Limita ao top 6 e persiste
        int novoTotal = Mathf.Min(scores.Length, TAMANHO_RANKING);

        for (int i = 0; i < novoTotal; i++)
        {
            PlayerPrefs.SetString(string.Format(CHAVE_NOME, i), nomes[i]);
            PlayerPrefs.SetInt(string.Format(CHAVE_SCORE, i), scores[i]);
            PlayerPrefs.SetString(string.Format(CHAVE_DATA, i), datas[i]);
        }

        PlayerPrefs.SetInt(CHAVE_TOTAL, novoTotal);
        PlayerPrefs.Save();

        // Retorna -1 se ficou fora do top 6
        return posicao < TAMANHO_RANKING ? posicao : -1;
    }

    // Struct pública para que o RankingDisplay acesse os dados com type safety
    public struct EntradaRanking
    {
        public string nome;
        public int score;
        public string dataHora;
    }

    // Retorna as entradas salvas (máx. 6), já ordenadas por score
    public EntradaRanking[] CarregarRanking()
    {
        int total = PlayerPrefs.GetInt(CHAVE_TOTAL, 0);
        var ranking = new EntradaRanking[total];

        for (int i = 0; i < total; i++)
        {
            ranking[i].nome = PlayerPrefs.GetString(string.Format(CHAVE_NOME, i), "---");
            ranking[i].score = PlayerPrefs.GetInt(string.Format(CHAVE_SCORE, i), 0);
            ranking[i].dataHora = PlayerPrefs.GetString(string.Format(CHAVE_DATA, i), "--/--/---- --:--");
        }

        return ranking;
    }

    // Apaga todos os dados do ranking (útil para debug)
    public void LimparRanking()
    {
        int total = PlayerPrefs.GetInt(CHAVE_TOTAL, 0);
        for (int i = 0; i < total; i++)
        {
            PlayerPrefs.DeleteKey(string.Format(CHAVE_NOME, i));
            PlayerPrefs.DeleteKey(string.Format(CHAVE_SCORE, i));
            PlayerPrefs.DeleteKey(string.Format(CHAVE_DATA, i));
        }
        PlayerPrefs.DeleteKey(CHAVE_TOTAL);
        PlayerPrefs.Save();
    }
}