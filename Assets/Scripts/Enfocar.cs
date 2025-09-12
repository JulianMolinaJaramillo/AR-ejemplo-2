using UnityEngine;

public class Enfocar : MonoBehaviour
{
    public GameObject objeto; 
    public GameObject canvasWorldSpace; 
    public AumentarEscala aumentarEscala;

    void OnMouseDown()
    {
        if (objeto != null)
        {
            aumentarEscala.AsignarObjetivo(objeto);
            canvasWorldSpace.SetActive(true);
        }
        else
        {
            canvasWorldSpace.SetActive(false);
            aumentarEscala.Escalar();
        }  
    }
}
