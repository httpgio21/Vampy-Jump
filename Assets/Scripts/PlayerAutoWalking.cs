using UnityEngine;

public class PlayerAutoWalk : MonoBehaviour
{
    public float walkSpeed = 2f;

    public bool autoWalk = false;

    private PlayerJump playerJump;

    void Start()
    {
        playerJump = GetComponent<PlayerJump>();
    }

    void Update()
    {
        if(autoWalk)
        {
            transform.Translate(Vector2.right * walkSpeed * Time.deltaTime);

            // bloqueia o pulo
            playerJump.canJump = false;
        }
    }
}