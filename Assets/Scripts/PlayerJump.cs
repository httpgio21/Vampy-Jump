using UnityEngine;
using System.Collections;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 25f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private PlayerWalking anim;
    private SpriteRenderer sr;
    private PlayerHealth playerHealth;

    private int jumpCount = 0;
    public int maxJumps = 2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<PlayerWalking>();
        sr = GetComponent<SpriteRenderer>();
        playerHealth = GetComponent<PlayerHealth>();

        rb.gravityScale = 1.5f;
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && jumpCount < maxJumps)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, 0f);
            rb.AddForce(Vector2.up * jumpForce, ForceMode2D.Impulse);

            jumpCount++;

            anim.isJumping = true;
            isGrounded = false;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        if (collision.gameObject.CompareTag("Ground"))
        {
            isGrounded = true;
            anim.isJumping = false;
            jumpCount = 0; // reset do pulo duplo
        }

        if (collision.gameObject.CompareTag("Obstacle"))
        {
            ContactPoint2D contact = collision.GetContact(0);

            // Se bateu de LADO (Dano!)
            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                // Pega o colisor específico DESTA Lápide que acabou de bater
                Collider2D colisorObstaculo = collision.collider;

                // Começa o efeito de piscar e passa a Lápide que bateu para ela virar fantasma
                StartCoroutine(DamageEffect(colisorObstaculo));

                if (playerHealth != null)
                {
                    playerHealth.TomarDano();
                }
            }
            // Se o Vampirinho pisou POR CIMA da Lápide
            else if (contact.normal.y > 0.5f)
            {
                isGrounded = true;
                anim.isJumping = false;
                jumpCount = 0; // Permite pular de cima dela!
            }
        }
    }

    // O efeito de dano agora recebe o colisor da Lápide atingida
    IEnumerator DamageEffect(Collider2D obstaculo)
    {
        // Se a Lápide ainda existir, transforma APENAS ELA em fantasma (Trigger)
        if (obstaculo != null)
        {
            obstaculo.isTrigger = true;
        }

        // Vampirinho pisca por um tempo
        for (int i = 0; i < 6; i++)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(0.08f);

            sr.enabled = true;
            yield return new WaitForSeconds(0.08f);
        }

        // Depois que ele terminou de piscar, se a Lápide ainda estiver na tela, ela volta a ser sólida
        if (obstaculo != null)
        {
            obstaculo.isTrigger = false;
        }
    }
}