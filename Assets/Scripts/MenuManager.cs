using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public SceneTransition transition;

    public void Jogar()
    {
        // Reseta vida e pontuação antes de iniciar, independente de onde o jogador veio
        if (PlayerHealth.Instance != null)
            PlayerHealth.Instance.AtributosReset();

        if (ScoreManager.Instance != null)
            ScoreManager.Instance.ResetarPontuacao();

        transition.CarregarCena("Night-Vamp-Walking");
    }

    public void Integrantes()
    {
        transition.CarregarCena("Integrantes");
    }

    public void Menu()
    {
        transition.CarregarCena("Menu");
    }
}
