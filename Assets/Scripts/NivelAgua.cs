using System.Collections;
using UnityEngine;

public class NivelAgua : MonoBehaviour
{
    [Header("Referencia")]
    public Renderer objetoRenderer; // El objeto que usa el shader

    [Header("Configuración")]
    public float velocidad = 0.2f;  // Qué tan rápido sube
    public float tope = 0.5f;       // Valor máximo permitido

    private Material mat;

    void Start()
    {
        mat = objetoRenderer.material;
    }

    // Método público para iniciar el aumento escalado
    public void SubirDisplace()
    {
        StopAllCoroutines(); // por si ya estaba corriendo
        StartCoroutine(AumentarDisplace());
    }

    private IEnumerator AumentarDisplace()
    {
        float valorActual = mat.GetFloat("_DisplaceAmp");

        while (valorActual < tope)
        {
            valorActual += Time.deltaTime * velocidad;
            mat.SetFloat("_DisplaceAmp", Mathf.Min(valorActual, tope));
            yield return null;
        }
    }
}
