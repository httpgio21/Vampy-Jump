using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public static PlayerHealth Instance { get; private set; }

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
            DontDestroyOnLoad(gameObject); // Salva o GameManager e o Canvas!
            
            // Avisa o Unity para rodar uma função sempre que mudar de cena
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

    // Roda automaticamente assim que entra na cena "cave-bat"
    void AoCarregarNovaCena(Scene cena, LoadSceneMode modo)
    {
        // Força o jogo a despausar e limpa estados antigos
        Time.timeScale = 1f; 
        isGameOver = false;

        // Procura e reconecta os corações e a tela de Game Over usando sua nova Hierarchy
        ReconectarComponentesDeUI();

        if (telaGameOver != null) 
        {
            telaGameOver.SetActive(false); // Garante que começa escondida!
        }

        AtualizarCoracoesNaTela();
    }

    void ReconectarComponentesDeUI()
{
    // Em vez de procurar só nos filhos do GameObject atual, 
    // procuramos por QUALQUER Canvas ativo na nova cena.
    Canvas canvasDaCena = Object.FindFirstObjectByType<Canvas>();

    if (canvasDaCena != null)
    {
        // 1. Encontra a TelaGameOver dentro do Canvas da cena atual
        Transform tGameOver = EncontrarFilhoInativoPorNome(canvasDaCena.transform, "TelaGameOver");
        if (tGameOver != null) 
        {
            telaGameOver = tGameOver.gameObject;
        }

        // 2. Encontra o objeto "vampy_hearts" dentro do Canvas da cena atual
        Transform painelCoracoes = EncontrarFilhoInativoPorNome(canvasDaCena.transform, "vampy_hearts");
        
        if (painelCoracoes != null)
        {
            // Pega as imagens dos corações (heart1, heart2, heart3)
            caixasDeCoracao = painelCoracoes.GetComponentsInChildren<Image>(true);
        }
        else
        {
            Debug.LogWarning("Aviso: 'vampy_hearts' não foi encontrado no Canvas desta cena!");
        }
    }
    else
    {
        Debug.LogError("Erro: Nenhum Canvas foi encontrado nesta cena!");
    }
}

    // Função especial para achar objetos na Hierarchy mesmo que estejam desativados
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
        if (isGameOver) return; 

        vidas--; 
        Debug.Log("Vampy levou dano! Vidas restantes: " + vidas);

        AtualizarCoracoesNaTela();

        if (vidas <= 0)
        {
            Debug.Log("GAME OVER!");
            isGameOver = true;

            if (telaGameOver != null)
            {
                telaGameOver.SetActive(true); 
            }

            Time.timeScale = 0f; // Congela o jogo
        }
    }

    public void AtuaisAparecer()
    {
        AtualizarCoracoesNaTela();
    }

    public void AtualizarCoracoesNaTela()
    {
        if (caixasDeCoracao == null || caixasDeCoracao.Length == 0) return;

        for (int i = 0; i < caixasDeCoracao.Length; i++)
        {
            if (caixasDeCoracao[i] == null) continue;

            if (i < vidas)
            {
                caixasDeCoracao[i].sprite = coracaoCheio;
            }
            else
            {
                caixasDeCoracao[i].sprite = coracaoVazio;
            }
        }
    }

    public void ClicouNoBotaoReiniciar()
    {
        Time.timeScale = 1f; 
        SceneManager.LoadScene("Night-Vamp-Walking"); // Nome exato da sua primeira cena
    }
}