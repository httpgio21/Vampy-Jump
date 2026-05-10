using UnityEngine;

public class PlayerJump : MonoBehaviour
{
    public float jumpForce = 7f;

    public Sprite idleSprite;
    public Sprite jumpSprite;

    private Rigidbody2D rb;
    private SpriteRenderer sr;

    private bool isGrounded;

    void Start()
    {
        rb = GetComponent<Rigidbody2D>();
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Space) && isGrounded)
        {
            rb.linearVelocity = new Vector2(rb.linearVelocity.x, jumpForce);

            sr.sprite = jumpSprite;
        }
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        isGrounded = true;

        sr.sprite = idleSprite;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        isGrounded = false;
    }
}