using UnityEngine;

public class MoveBolsa : MonoBehaviour
{
    [Header("Movimento")]
    public float velocidade = 4f; // Sincronizada com a caverna e os obstáculos

    [Header("Limite da Tela")]
    public float limiteEsquerda = -25f; // Ponto onde a bolsa sai totalmente da tela

    // Mudamos para LateUpdate e alteração direta de position para rodar liso com o cenário
    void LateUpdate()
    {
        // Move a bolsa de forma estável usando position diretamente
        transform.position += Vector3.left * velocidade * Time.deltaTime;

        // Se o jogador não coletar a bolsa e ela sumir da tela, ela se destrói
        if (transform.position.x <= limiteEsquerda)
        {
            Destroy(gameObject);
        }
    }

    // Gerencia a colisão: quando o Player encosta na bolsa, ela some da tela
    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            Destroy(gameObject);
        }
    }
}