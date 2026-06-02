using UnityEngine;
using System.Collections;

public class DamageObstacle : MonoBehaviour
{
    private bool podeDarDano = true;
    private SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (!podeDarDano)
            return;

        // Verifica se quem encostou foi o jogador
        if (other.CompareTag("Player"))
        {
            // CORREÇÃO: Em vez de procurar o componente no corpo do player,
            // chama diretamente a Instância Global que veio da Cena 1!
            if (PlayerHealth.Instance != null)
            {
                PlayerHealth.Instance.TomarDano();
            }
            else
            {
                Debug.LogWarning("PlayerHealth.Instance não foi encontrado! Lembre-se de jogar iniciando da Cena 1 para o gerenciador existir.");
            }

            // O obstáculo pisca para indicar o acerto
            StartCoroutine(Piscar());
        }
    }

    IEnumerator Piscar()
    {
        podeDarDano = false;

        for (int i = 0; i < 6; i++)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(0.1f);

            sr.enabled = true;
            yield return new WaitForSeconds(0.1f);
        }

        podeDarDano = true;
    }
}