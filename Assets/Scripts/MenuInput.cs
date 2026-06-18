using UnityEngine;
using UnityEngine.EventSystems;

public class MenuInput : MonoBehaviour
{
    public GameObject primeiroBotao;

    void Start()
    {
        EventSystem.current.SetSelectedGameObject(primeiroBotao);
    }
}
