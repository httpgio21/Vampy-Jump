using UnityEngine;

public class PlayerAutoWalk : MonoBehaviour
{
    public float walkSpeed = 2f;

    public bool autoWalk = false;

    void Update()
    {
        if(autoWalk)
        {
            transform.Translate(Vector2.right * walkSpeed * Time.deltaTime);
        }
    }
}