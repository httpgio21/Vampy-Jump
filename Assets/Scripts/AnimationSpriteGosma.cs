using UnityEngine;

public class AnimationSpriteGosma : MonoBehaviour
{
    public Sprite[] sprites;
    public float tempoTroca = 0.15f;

    private SpriteRenderer sr;
    private int indiceAtual;
    private float timer;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();

        if (sprites.Length > 0)
            sr.sprite = sprites[0];
    }

    void Update()
    {
        if (sprites.Length == 0)
            return;

        timer += Time.deltaTime;

        if (timer >= tempoTroca)
        {
            timer = 0f;

            indiceAtual++;

            if (indiceAtual >= sprites.Length)
                indiceAtual = 0;

            sr.sprite = sprites[indiceAtual];
        }
    }
}