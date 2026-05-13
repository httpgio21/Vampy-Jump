using UnityEngine;

public class PlayerWalking : MonoBehaviour
{
    public Sprite[] runSprites;

    public Sprite jumpSprite;

    public float animationSpeed = 0.1f;

    private SpriteRenderer sr;

    private int currentFrame;

    private float timer;

    public bool isJumping;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }

    void Update()
    {
        if (isJumping)
        {
            sr.sprite = jumpSprite;
            return;
        }

        timer += Time.deltaTime;

        if (timer >= animationSpeed)
        {
            timer = 0;

            currentFrame++;

            if (currentFrame >= runSprites.Length)
            {
                currentFrame = 0;
            }

            sr.sprite = runSprites[currentFrame];
        }
    }
}