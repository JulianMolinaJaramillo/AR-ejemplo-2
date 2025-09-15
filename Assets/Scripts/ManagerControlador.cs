using System.Collections;
using TMPro;
using UnityEngine;

public class ManagerControlador : MonoBehaviour
{
    [Header("Configuraciónes para Zappar Visible e invisible")]
    public GameObject imagenPreview;
    public GameObject padre;
    public GameObject panelImgTextoInicial;
    public TextMeshProUGUI txtTituloInicial;

    [Header("Configuraciónes evento lluvia")]
    public CrecimientoNubes crecimientoNubes;
    public ParticleSystem particulas;
    public PulsoEscala[] pulsoEscala;
    public Temblor[] temblorTerrenos;
    public Temblor[] temblorCasas;
    public MovimientoNPC[] npcs;

    [Header("Configuraciónes adicionales")]
    public GameObject panelDespedida;

    private bool lluviaActiva;
    private Coroutine coroutine;

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
        panelImgTextoInicial.SetActive(false);
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
        panelImgTextoInicial.SetActive(true);
        txtTituloInicial.text = "Vuelve a enfocar la imagen";
    }

    public void SalirAplicacion()
    {
        txtTituloInicial.text = "";
        panelDespedida.SetActive(true);
        Application.Quit();
    }

    [ContextMenu("Iniciar")]
    public void EmpezarEventoLluviaCreciente()
    {
        // Empezamos el evento
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(LluviaCreciente());
    }

    private IEnumerator LluviaCreciente()
    {
        // Generamos las nubes
        crecimientoNubes.ActivarCrecimiento();


        yield return new WaitForSeconds(1f);


        // Activamos sonido lluvia
        if (SimpleAudioManager.singleton != null)
        {
            SimpleAudioManager.singleton.DetenerAudioFondo();
            SimpleAudioManager.singleton.audioSourceFondo.clip = SimpleAudioManager.singleton.clips[2];
            SimpleAudioManager.singleton.RestaurarAudioFondo();
        }

        // Activamos la lluvia
        lluviaActiva = true;
        if (particulas != null) particulas.Play();

        yield return new WaitForSeconds(1f);


        for (int i = 0; i < pulsoEscala.Length; i++)
        {
            pulsoEscala[i].IniciarAlerta();
        }


        yield return new WaitForSeconds(1f);


        for (int i = 0; i < temblorTerrenos.Length; i++)
        {
            temblorTerrenos[i].IniciarVibracion(0.001f);
        }

        yield return new WaitForSeconds(1f);


        for (int i = 0; i < temblorCasas.Length; i++)
        {
            temblorCasas[i].IniciarVibracion(0.001f);
        }

        yield return new WaitForSeconds(1f);


        for (int i = 0; i < npcs.Length; i++)
        {
            npcs[i].CorrerPorSuVida();
        }

    }
}
