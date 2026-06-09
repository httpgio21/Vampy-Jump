using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class SceneTransition : MonoBehaviour
{
    public Image fadeImage;
    public float fadeDuration = 1f;

    public void CarregarCena(string nomeCena)
    {
        StartCoroutine(FadeAndLoad(nomeCena));
    }

    IEnumerator FadeAndLoad(string nomeCena)
    {
        Color cor = fadeImage.color;

        float tempo = 0;

        while (tempo < fadeDuration)
        {
            tempo += Time.deltaTime;

            cor.a = Mathf.Lerp(0, 1, tempo / fadeDuration);

            fadeImage.color = cor;

            yield return null;
        }

        SceneManager.LoadScene(nomeCena);
    }
}