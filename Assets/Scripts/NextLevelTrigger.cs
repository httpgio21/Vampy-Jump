using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections;

public class NextLevelTrigger : MonoBehaviour
{
    public CanvasGroup fadePanel;
    public float fadeDuration = 1f;
    public string nextSceneName;

    private void OnTriggerEnter2D(Collider2D collision)
    {
        if (collision.CompareTag("Player"))
        {
            StartCoroutine(FadeAndLoad());
        }
    }

    IEnumerator FadeAndLoad()
    {
        float time = 0f;

        fadePanel.gameObject.SetActive(true); // garante que está ativo

        while (time < fadeDuration)
        {
            time += Time.deltaTime;
            fadePanel.alpha = time / fadeDuration;
            yield return null;
        }

        fadePanel.alpha = 1f;

        // SceneManager.LoadScene(nextSceneName);
    }
}