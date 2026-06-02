using UnityEngine;

public class ResetPlayerPosition : MonoBehaviour
{
    private Vector3 posicaoInicialOriginal;
    private Rigidbody2D rb;

    void Awake()
    {
        // 1. Salva a posição exata onde o Vampiro está na Cena 3 ANTES de qualquer transição o mover
        posicaoInicialOriginal = transform.position;
        rb = GetComponent<Rigidbody2D>();
    }

    void Start()
    {
        // 2. No primeiro frame da Cena 3, força o Vampiro a ir para a posição correta
        transform.position = posicaoInicialOriginal;

        // 3. Se ele usar física, zera todas as velocidades acumuladas da Fase 2
        if (rb != null)
        {
            rb.position = posicaoInicialOriginal;
            rb.linearVelocity = Vector2.zero;
            rb.angularVelocity = 0f;
        }
    }
}