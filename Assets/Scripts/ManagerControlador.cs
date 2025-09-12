using TMPro;
using UnityEngine;

public class ManagerControlador : MonoBehaviour
{
    public GameObject imagenPreview;
    public GameObject padre;
    public GameObject panelDespedida;
    public TextMeshProUGUI txtTituloInicial;
    public ParticleSystem particulas;

    [HideInInspector]
    public bool lluviaActiva;
    public static ManagerControlador singleton;

    private void Awake()
    {
        // Si ya existe una instancia y no es esta → destruir el duplicado
        if (singleton != null && singleton != this)
        {
            Destroy(gameObject);
            return;
        }

        // Asignamos la instancia
        singleton = this;
    }

    /// <summary>
    /// Metodo invocado desde Zappar Image Tracking Target al momento de ver la imagen de referencia
    /// </summary>
    public void Visible()
    {
        if (lluviaActiva && particulas != null) particulas.Play();

        imagenPreview.SetActive(true);
        padre.SetActive(true);
        txtTituloInicial.text = "";
    }

    /// <summary>
    /// Metodo invocado desde Zappar Image Tracking Target al momento de dejar ver la imagen de referencia
    /// </summary>
    public void Invisible()
    {
        if (lluviaActiva && particulas != null) particulas.Stop();

        imagenPreview.SetActive(false);
        padre.SetActive(false);
        txtTituloInicial.text = "Vuelve a enfocar la imagen";
    }

    public void SalirAplicacion()
    {
        txtTituloInicial.text = "";
        panelDespedida.SetActive(true);
        Application.Quit();
    }

}
