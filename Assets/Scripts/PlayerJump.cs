using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 7f;

    private Rigidbody2D rb;

    private bool isGrounded;

    private PlayerWalking anim;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();

        anim = GetComponent<PlayerWalking>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            anim.isJumping = true;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;

        anim.isJumping = false;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}