using UnityEngine;

public class MoveScenario : MonoBehaviour
{
    public float speed = 5f;
    public float limitX = -20f;
    public float resetX = 20f;

    public bool infiniteLoop = true;

    public bool stopMoving = false;

    void Update()
    {
        // PARA TUDO
        if(stopMoving)
            return;

        // movimento
        transform.Translate(Vector2.left * speed * Time.deltaTime);

        // loop infinito
        if(transform.position.x < limitX && infiniteLoop)
        {
            transform.position = new Vector3(
                resetX,
                transform.position.y,
                transform.position.z
            );
        }
    }
}