using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerHealth : MonoBehaviour
{
    public int vidas = 3; // Vampy começa com 3 vidas agora!

    [Header("Configurações de UI (Interface)")]
    public Image[] caixasDeCoracao;
    public Sprite coracaoCheio;
    public Sprite coracaoVazio;
    public GameObject telaGameOver;

    private bool isGameOver = false; // Controla se o jogador perdeu

    public void TomarDano()
    {
        if (isGameOver) return; // Se o jogo já acabou, ignora novos danos

        vidas--; // Perde 1 vida
        Debug.Log("Vampy levou dano! Vidas restantes: " + vidas);

        // Atualiza o visual dos corações na tela
        AtualizarCoracoesNaTela();

        // Quando as vidas chegam a 0, ativa o Game Over!
        if (vidas <= 0)
        {
            Debug.Log("GAME OVER! Clique no botão Tente Novamente para reiniciar.");
            isGameOver = true;

            if (telaGameOver != null)
            {
                telaGameOver.SetActive(true); // Faz a tela de Game Over aparecer
            }

            Time.timeScale = 0f; // Congela o tempo do jogo 
        }
    }

    void AtualizarCoracoesNaTela()
    {
        for (int i = 0; i < caixasDeCoracao.Length; i++)
        {
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

    // NOVA FUNÇÃO PÚBLICA: O seu botão "Tente Novamente" vai chamar isso aqui!
    public void ClicouNoBotaoReiniciar()
    {
        Time.timeScale = 1f; // Descongela o jogo antes de recarregar
        SceneManager.LoadScene(SceneManager.GetActiveScene().name); // Recarrega a fase atual
    }
}