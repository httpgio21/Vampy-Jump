using UnityEngine;
using System.Collections;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 25f;

    private Rigidbody2D rb;
    private bool isGrounded;
    private PlayerWalking anim;
    private SpriteRenderer sr;

    private int jumpCount = 0;
    public int maxJumps = 2;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        anim = GetComponent<PlayerWalking>();
        sr = GetComponent<SpriteRenderer>();

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

            if (Mathf.Abs(contact.normal.x) > 0.5f)
            {
                StartCoroutine(DamageEffect());
            }
        }
    }

    IEnumerator DamageEffect()
    {
        for (int i = 0; i < 6; i++)
        {
            sr.enabled = false;
            yield return new WaitForSeconds(0.08f);

            sr.enabled = true;
            yield return new WaitForSeconds(0.08f);
        }
    }
}