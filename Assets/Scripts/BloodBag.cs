using UnityEngine;

public class BloodBag : MonoBehaviour
{
    [Header("Configurações do Item")]
    [Tooltip("Quantidade de pontos que o vampiro ganha ao coletar a bolsa.")]
    public int pontosDestaBolsa = 20;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        // Garanta que a Tag do seu Personagem no Unity esteja definida como "Player"
        if (collision.CompareTag("Player"))
        {
            // Envia os 20 pontos diretamente para o ScoreManager
            if (ScoreManager.Instance != null)
            {
                ScoreManager.Instance.AdicionarPontos(pontosDestaBolsa);
            }
            else
            {
                Debug.LogWarning("Aviso: ScoreManager não foi encontrado na cena!");
            }

            // Destrói o objeto para sumir do mapa ao ser pego
            Destroy(gameObject);
        }
    }
}