using UnityEngine;

public class Enfocar : MonoBehaviour
{
    public GameObject objeto; 
    public AumentarEscala aumentarEscala;

    void OnMouseDown()
    {
        if (objeto != null)
        {
            aumentarEscala.AsignarObjetivo(objeto);
        }
        else
        {
            aumentarEscala.Escalar();
        }
        
    }
}
