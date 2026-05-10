using UnityEngine;

public class MoveScenario : MonoBehaviour
{
    public float speed = 5f;
    public float limitX = -20f;
    public float resetX = 20f;

    void Update()
    {
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        if(transform.position.x < limitX)
        {
            transform.position = new Vector3(resetX, transform.position.y, transform.position.z);
        }
    }
}