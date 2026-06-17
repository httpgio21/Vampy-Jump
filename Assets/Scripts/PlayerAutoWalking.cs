using UnityEngine;

public class PlayerAutoWalk : MonoBehaviour
{
    public float walkSpeed = 2f;

    public bool autoWalk = false;

    private PlayerJump playerJump;

    void Start()
    {
        playerJump = GetComponent<PlayerJump>();

        // CORREÇÃO (Opção A): Garante que o autoWalk sempre começa desligado
        // quando o objeto entra em uma nova cena, evitando que o estado
        // "autoWalk = true" da cena anterior persista via DontDestroyOnLoad.
        autoWalk = false;
        walkSpeed = 2f;
    }

    void Update()
    {
        if (autoWalk)
        {
            transform.Translate(Vector2.right * walkSpeed * Time.deltaTime);

            // Bloqueia o pulo enquanto está em autoWalk
            if (playerJump != null)
                playerJump.canJump = false;
        }
        else
        {
            // CORREÇÃO (Opção A): Restaura o pulo quando autoWalk está desligado,
            // impedindo que o canJump fique travado em false entre cenas.
            if (playerJump != null)
                playerJump.canJump = true;
        }
    }
}
