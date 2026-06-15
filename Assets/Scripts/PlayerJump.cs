using UnityEngine;
using System.Collections;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 25f;
    private Rigidbody2D rb;
    private bool isGrounded;
    private PlayerWalking anim;
    private SpriteRenderer sr;
    public bool canJump = true;

    private int jumpCount = 0;
    public int maxJumps = 2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<PlayerWalking>();
        sr = GetComponent<SpriteRenderer>();

        // CONFIGURAÇÃO NOVA: Não usamos mais o GetComponent aqui, 
        // já que o PlayerHealth agora roda de forma global e independente!

        rb.gravityScale = 1.5f;
    }

    void Update()
    {
        // PC (teclado) + Mobile (toque)
        bool jumpInput =
            Input.GetKeyDown(KeyCode.Space) ||
            Input.GetMouseButtonDown(0) ||   // clique no PC
            Input.GetKeyDown(KeyCode.JoystickButton0) || // toque no botão A no controle de Xbox
            Input.touchCount > 0 && Input.GetTouch(0).phase == TouchPhase.Began; // toque no mobile

        if (canJump && jumpInput && jumpCount < maxJumps)
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
                Collider2D colisorObstaculo = collision.collider;
                StartCoroutine(DamageEffect(colisorObstaculo));

                // AJUSTE AQUI: Chama o PlayerHealth através do Singleton global (Instance)
                if (PlayerHealth.Instance != null)
                {
                    PlayerHealth.Instance.TomarDano();
                }
                else
                {
                    Debug.LogWarning("PlayerHealth.Instance não foi encontrado na cena!");
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

    IEnumerator DamageEffect(Collider2D obstaculo)
    {
        if (obstaculo != null)
        {
            obstaculo.isTrigger = true;
        }

        for (int i = 0; i < 6; i++)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(0.08f);

            sr.enabled = true;
            yield return new WaitForSeconds(0.08f);
        }

        if (obstaculo != null)
        {
            obstaculo.isTrigger = false;
        }
    }
}