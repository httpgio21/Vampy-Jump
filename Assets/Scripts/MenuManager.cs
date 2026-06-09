using UnityEngine;

public class MenuManager : MonoBehaviour
{
    public SceneTransition transition;

    public void Jogar()
    {
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