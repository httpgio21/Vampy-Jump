using UnityEngine;

public class FadeEnding : MonoBehaviour
{
    public CanvasGroup fadePanel;

    public Transform player;

    // posição X onde começa o fade
    public float playerMiddleX = 0f;

    public float fadeSpeed = 1f;

    private bool startFade = false;

    void Update()
    {
        // quando player chegar no meio
        if (player.position.x >= playerMiddleX)
        {
            startFade = true;
        }

        // fade
        if (startFade)
        {
            fadePanel.alpha += fadeSpeed * Time.deltaTime;

            fadePanel.alpha = Mathf.Clamp01(fadePanel.alpha);
        }
    }
}