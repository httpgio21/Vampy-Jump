using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NextLevelTrigger : MonoBehaviour
{
    public CanvasGroup fadePanel;
    public float fadeDuration = 0.5f;
    public string nextSceneName;

    private bool isTransitioning = false;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player") && !isTransitioning)
        {
            isTransitioning = true;

            // trava o player
            Rigidbody2D rb = collision.GetComponent<Rigidbody2D>();

            if (rb != null)
            {
                rb.linearVelocity = Vector2.zero;
                rb.simulated = false;
            }

            StartCoroutine(FadeAndLoad());
        }
    }

    IEnumerator FadeAndLoad()
    {
        float time = 0f;

        fadePanel.gameObject.SetActive(true);

        while (time < fadeDuration)
        {
            time += Time.deltaTime;

            fadePanel.alpha = time / fadeDuration;

            yield return null;
        }

        fadePanel.alpha = 1f;

        SceneManager.LoadScene(nextSceneName);
    }
}