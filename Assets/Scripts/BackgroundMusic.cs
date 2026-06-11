using UnityEngine;

public class BackgroundMusic : MonoBehaviour
{
    private static BackgroundMusic instance;

    void Awake()
    {
        // Verifica se já existe uma instância deste objeto no jogo
        if (instance == null)
        {
            instance = this;
            // Garante que o objeto não seja destruído ao mudar de cena
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            // Se já existir uma música tocando, destrói a duplicata
            Destroy(gameObject);
        }
    }
}