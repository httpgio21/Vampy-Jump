using TMPro;
using UnityEngine;

/// <summary>
/// Coloque este componente em qualquer GameObject de uma cena
/// que precise exibir a pontuação. Ele busca o TextMeshPro
/// automaticamente pelo nome "ScoreText" no Canvas da cena.
///
/// Se preferir apontar manualmente, arraste o TMP no Inspector.
/// </summary>
public class ScoreDisplay : MonoBehaviour
{
    [Header("Texto (opcional — detectado automaticamente)")]
    [Tooltip("Deixe vazio para detectar o objeto 'ScoreText' no Canvas da cena")]
    public TextMeshProUGUI textoScore;

    [Header("Formato")]
    [Tooltip("Use {0} onde o número deve aparecer. Ex: 'Score: {0}'")]
    public string formato = "Score: {0}";

    void Start()
    {
        // Auto-detecção: mesmo padrão usado pelo PlayerHealth para corações
        if (textoScore == null)
        {
            Canvas canvas = FindFirstObjectByType<Canvas>();
            if (canvas != null)
            {
                textoScore = EncontrarTMPPorNome(canvas.transform, "ScoreText");
            }
        }

        if (textoScore == null)
        {
            Debug.LogWarning("ScoreDisplay: nenhum 'ScoreText' encontrado na cena. " +
                             "Crie um TextMeshPro no Canvas com esse nome, ou arraste manualmente no Inspector.");
        }

        AtualizarDisplay();
    }

    void Update()
    {
        AtualizarDisplay();
    }

    void AtualizarDisplay()
    {
        if (textoScore == null || ScoreManager.Instance == null) return;
        textoScore.text = string.Format(formato, ScoreManager.Instance.PontuacaoAtual);
    }

    // Busca recursiva igual à do PlayerHealth
    TextMeshProUGUI EncontrarTMPPorNome(Transform pai, string nomeAlvo)
    {
        foreach (Transform filho in pai.GetComponentsInChildren<Transform>(true))
        {
            if (filho.gameObject.name.ToLower() == nomeAlvo.ToLower())
            {
                TextMeshProUGUI tmp = filho.GetComponent<TextMeshProUGUI>();
                if (tmp != null) return tmp;
            }
        }
        return null;
    }
}
