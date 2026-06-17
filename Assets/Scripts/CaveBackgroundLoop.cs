using UnityEngine;

/// <summary>
/// Loop visual de dois backgrounds para scroll infinito SEM PULOS.
///
/// SETUP na Unity:
///   1. Crie um GameObject vazio "BackgroundSystem".
///   2. Coloque este script nele.
///   3. Arraste os dois GameObjects de background para bg1 e bg2.
///      Posicione bg1 em x=0, bg2 em x=+larguraSprite (ex: x=20).
///   4. Configure larguraSprite com a largura exata do sprite em unidades de mundo.
///
/// O script alterna os dois sprites: quando bg1 sai pela esquerda,
/// ele é reposicionado à direita de bg2, e vice-versa — sem teleporte visível.
/// </summary>
public class CaveBackgroundLoop : MonoBehaviour
{
    [Header("Os dois GameObjects de background")]
    public Transform bg1;
    public Transform bg2;

    [Header("Configuração")]
    [Tooltip("Largura exata do sprite em unidades de mundo (World Units)")]
    public float larguraSprite = 20f;

    public float velocidade = 2f;

    void Update()
    {
        if (bg1 == null || bg2 == null) return;

        // Move os dois backgrounds juntos
        Vector3 delta = Vector3.left * velocidade * Time.deltaTime;
        bg1.position += delta;
        bg2.position += delta;

        // Quando um sai pela esquerda, vai para depois do outro
        // Isso elimina o pulo porque a posição é calculada relativa ao outro sprite
        if (bg1.position.x <= -larguraSprite)
            bg1.position = new Vector3(bg2.position.x + larguraSprite, bg1.position.y, bg1.position.z);

        if (bg2.position.x <= -larguraSprite)
            bg2.position = new Vector3(bg1.position.x + larguraSprite, bg2.position.y, bg2.position.z);
    }
}
