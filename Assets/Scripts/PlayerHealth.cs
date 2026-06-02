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

    private bool isGameOver = false;

    void Awake()
    {
        // Padrão Singleton para persistir entre as cenas
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);

            // Inscreve o método para rodar sempre que uma nova cena carregar
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
        // Desinscreve o método para evitar vazamento de memória
        SceneManager.sceneLoaded -= AoCarregarNovaCena;
    }

    void AoCarregarNovaCena(Scene cena, LoadSceneMode modo)
    {
        Time.timeScale = 1f;
        isGameOver = false;

        // Executa a busca automática dos componentes visuais na nova cena
        ReconectarComponentesDeUI();

        if (telaGameOver != null)
        {
            telaGameOver.SetActive(false);
        }

        AtualizarCoracoesNaTela();
    }

    void ReconectarComponentesDeUI()
    {
        Canvas canvasDaCena = Object.FindFirstObjectByType<Canvas>();

        if (canvasDaCena != null)
        {
            // 1. Busca a Tela de Game Over de forma independente
            Transform tGameOver = EncontrarFilhoInativoPorNome(canvasDaCena.transform, "TelaGameOver");
            if (tGameOver != null)
            {
                telaGameOver = tGameOver.gameObject;
            }
            else
            {
                Debug.LogWarning("Aviso: 'TelaGameOver' não foi encontrado nesta cena.");
            }

            // 2. Busca o vampy_hearts de forma totalmente isolada
            Transform painelCoracoes = EncontrarFilhoInativoPorNome(canvasDaCena.transform, "vampy_hearts");
            if (painelCoracoes != null)
            {
                caixasDeCoracao = painelCoracoes.GetComponentsInChildren<Image>(true);
                Debug.Log("Corações conectados com sucesso! Total encontrado: " + caixasDeCoracao.Length);
            }
            else
            {
                Debug.LogError("Erro Crítico: 'vampy_hearts' não foi encontrado no Canvas desta cena!");
            }
        }
        else
        {
            Debug.LogError("Nenhum Canvas encontrado na cena atual!");
        }
    }

    Transform EncontrarFilhoInativoPorNome(Transform pai, string nomeAlvo)
    {
        foreach (Transform filho in pai.GetComponentsInChildren<Transform>(true))
        {
            if (filho.gameObject.name.ToLower() == nomeAlvo.ToLower())
            {
                return filho;
            }
        }
        return null;
    }

    public void TomarDano()
    {
        Debug.Log("TomarDano foi chamado!");

        if (isGameOver)
            return;

        vidas--;
        Debug.Log("Vidas restantes: " + vidas);

        AtualizarCoracoesNaTela();

        if (vidas <= 0)
        {
            Debug.Log("GAME OVER!");
            isGameOver = true;

            if (telaGameOver != null)
            {
                telaGameOver.SetActive(true);
            }

            Time.timeScale = 0f;
        }
    }

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
            if (caixasDeCoracao[i] == null)
                continue;

            // Define se o coração vai mostrar o sprite cheio ou vazio
            caixasDeCoracao[i].sprite = (i < vidas) ? coracaoCheio : coracaoVazio;
        }
    }

    public void ClicouNoBotaoReiniciar()
    {
        Time.timeScale = 1f;
        AtributosReset();
        SceneManager.LoadScene("Night-Vamp-Walking");
    }
}