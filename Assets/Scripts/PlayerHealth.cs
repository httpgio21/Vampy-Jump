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

    private bool isGameOver = false;

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
            Transform tGameOver = EncontrarFilhoInativoPorNome(canvasDaCena.transform, "TelaGameOver");
            if (tGameOver != null)
            {
                telaGameOver = tGameOver.gameObject;

                Button botaoReiniciar = telaGameOver.GetComponentInChildren<Button>(true);
                if (botaoReiniciar != null)
                {
                    botaoReiniciar.onClick.RemoveAllListeners();
                    botaoReiniciar.onClick.AddListener(ClicouNoBotaoReiniciar);
                    Debug.Log("Botão Reiniciar conectado automaticamente com sucesso!");
                }
                else
                {
                    Debug.LogWarning("Aviso: Nenhum componente 'Button' foi encontrado dentro da TelaGameOver.");
                }
            }
            else
            {
                Debug.LogWarning("Aviso: 'TelaGameOver' não foi encontrado nesta cena.");
            }

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