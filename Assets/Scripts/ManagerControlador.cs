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
    public NivelAgua rio; // arrastras el objeto con el material al inspector
    
    public CrecimientoNubes crecimientoNubes;
    public ParticleSystem particulas;
    public ParticleSystem particulasNubes;
    public ParticleSystem particulasPrueba;

    [Header("Configuraciónes Cuenca")]
    public GameObject cuenca;
    public GameObject canvasCuenca;
    public PulsoEscala pulsoEscalaCuenca;
    public GameObject panelInformativoCuenca;
    public TextoEscalonado txtPanelCuenca;
    public Temblor temblorTerrenoCuenca;
    public MovimientoNPC[] npcsCuenca;
    public Rigidbody[] npcAhogados;
    public DerrumbeCasas[] temblorCasas;
    public GameObject[] casasDesarmadas;
    public NivelAgua rioCuenca; // arrastras el objeto con el material al inspector
    public MovimientoSuavizado movimientoSuavizadoCuenca;

    [Header("Configuraciónes Casas")]
    public GameObject casas;
    public GameObject canvasCasas;
    public PulsoEscala pulsoEscalaCasas;
    public GameObject panelInformativoCasas;
    public TextoEscalonado txtPanelCasas;
    public Temblor[] temblorTerrenoCasas;
    public MovimientoNPC[] npcsCasas;
    public CamaraAlerta camaraAlerta;
    public NivelAgua rioCasas; // arrastras el objeto con el material al inspector
    public MovimientoSuavizado movimientoSuavizadoCasas;

    [Header("Configuraciónes adicionales")]
    public GameObject imgLLuvia;
    public ManagerVehiculos[] vehiculos;
    public GameObject panelDespedida;
    public PulsoEscala[] pulsosInternos;
    public TextoEscalonado canvasInformativo;

    private bool lluviaActiva;
    private Coroutine coroutine;
    private Coroutine coroutine2;
    private Coroutine coroutine3;
    private Coroutine coroutine4;
    //
    private bool iniciarPDF;
    private bool desastreCuencaActivo;
    private bool desastreCasasActivo;
    [HideInInspector]
    public bool desastreInicialActivo;
    [HideInInspector]
    public bool desastreSecundarioActivo;

    public static ManagerControlador singleton;
    private bool activacion;
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

        //// Accedemos al material del objeto
        //Material mat = rio.material;

        //mat.SetFloat("_DisplaceAmp", 0.2f);

        canvasInformativo.textoAlmacenado = "1. Cuenca en estado normal.";
        canvasInformativo.MostrarTexto("1. Cuenca en estado normal.");
    }

    private void Start()
    {
        casas.SetActive(true);
        cuenca.SetActive(true);
    }

    private void Update()
    {
        if (!activacion)
        {
            casas.SetActive(false);
            cuenca.SetActive(false);
            activacion = true;
        }    
    }

    /// <summary>
    /// Metodo invocado desde Zappar Image Tracking Target al momento de ver la imagen de referencia
    /// </summary>
    public void Visible()
    {
        if (lluviaActiva)
        {
            particulas.gameObject.SetActive(true);
            particulasNubes.gameObject.SetActive(true);
        }

        imagenPreview.SetActive(true);
        imgLLuvia.SetActive(true);
        padre.SetActive(true);
        panelImgTextoInicial.SetActive(false);
        txtTituloInicial.text = "";

        if (!iniciarPDF)
        {
            ActivarPanelCuenca();
            txtPanelCuenca.textoAlmacenado = "Aquí podemos apreciar el estado normal de la cuenca y la vida cotidiana de las personas.";
            txtPanelCuenca.MostrarTexto("Aquí podemos apreciar el estado normal de la cuenca y la vida cotidiana de las personas.");

            ActivarPanelCasas();
            txtPanelCasas.textoAlmacenado = "Zona de viviendas en su estado normal, sin ser afectadas por inundaciones ni desastres.";
            txtPanelCasas.MostrarTexto("Zona de viviendas en su estado normal, sin ser afectadas por inundaciones ni desastres.");
        }
    }

    /// <summary>
    /// Metodo invocado desde Zappar Image Tracking Target al momento de dejar ver la imagen de referencia
    /// </summary>
    public void Invisible()
    {
        if (lluviaActiva)
        {
            particulas.gameObject.SetActive(false);
            particulasNubes.gameObject.SetActive(false);
        }

        imagenPreview.SetActive(false);
        imgLLuvia.SetActive(false);
        padre.SetActive(false);
        panelImgTextoInicial.SetActive(true);
        txtTituloInicial.text = "Vuelve a enfocar la imagen.";
    }

    public void SalirAplicacion()
    {
        txtTituloInicial.text = "";
        panelDespedida.SetActive(true);
        Application.Quit();
    }


    public void ActivarPanelCasas()
    {
        panelInformativoCasas.SetActive(true);
    }


    public void ActivarPanelCuenca()
    {
        panelInformativoCuenca.SetActive(true);
    }

    public void DesactivarPanelCasas()
    {
        panelInformativoCasas.SetActive(false);
    }


    public void DesactivarPanelCuenca()
    {
        panelInformativoCuenca.SetActive(false);
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
        canvasInformativo.textoAlmacenado = "2. Precipitación de lluvia.";
        canvasInformativo.MostrarTexto("2. Precipitación de lluvia.");

        desastreInicialActivo = true;
        // Generamos las nubes
        crecimientoNubes.ActivarCrecimiento();

        lluviaActiva = true;

        yield return new WaitForSeconds(1f);

        // Activamos sonido lluvia
        if (SimpleAudioManager.singleton != null)
        {
            SimpleAudioManager.singleton.DetenerAudioFondo();
            SimpleAudioManager.singleton.audioSourceFondo.clip = SimpleAudioManager.singleton.clips[2];
            SimpleAudioManager.singleton.RestaurarAudioFondo();
        }

        if (particulasNubes != null) particulasNubes.gameObject.SetActive(true);
        // Activamos la lluvia
        if (particulas != null) particulas.gameObject.SetActive(true);

        yield return new WaitForSeconds(1f);
        
        DesactivarPanelCasas();
        DesactivarPanelCuenca();
        iniciarPDF = true;

        pulsoEscalaCuenca.IniciarAlerta();
        ActivarPanelCuenca();
        txtPanelCuenca.textoAlmacenado = "El cielo de la cuenca comienza a cubrirse de nubes y se inicia una llovizna; el agua de la quebrada empieza a crecer.";
        txtPanelCuenca.MostrarTexto("El cielo de la cuenca comienza a cubrirse de nubes y se inicia una llovizna; el agua de la quebrada empieza a crecer.");
        pulsosInternos[0].IniciarAlerta();
        movimientoSuavizadoCuenca.IniciarDesplazamiento();
        rio.SubirDisplace();
        
        yield return new WaitForSeconds(12f);

        pulsoEscalaCasas.IniciarAlerta();
        ActivarPanelCasas();  
        txtPanelCasas.textoAlmacenado = "A la zona residencial empiezan a llegar las nubes cargadas de lluvia; crece peligrosamente el nivel del agua.";
        txtPanelCasas.MostrarTexto("A la zona residencial empiezan a llegar las nubes cargadas de lluvia; crece peligrosamente el nivel del agua.");
        pulsosInternos[1].IniciarAlerta();
        movimientoSuavizadoCasas.IniciarDesplazamiento();
        camaraAlerta.IniciarAlerta();
    }

    [ContextMenu("Iniciar 2")]
    public void AumentarEventoLluviaCreciente()
    {
        // Empezamos el evento
        if (coroutine2 != null) StopCoroutine(coroutine2);
        coroutine2 = StartCoroutine(ContinuaLluviaCreciente());
    }

    private IEnumerator ContinuaLluviaCreciente()
    {

        canvasInformativo.textoAlmacenado = "3. Concentración de caudales";
        canvasInformativo.MostrarTexto("3. Concentración de caudales");

        desastreSecundarioActivo = true;
        // Aumentar Particulas lluvia

        var main = particulas.main;  // Módulo Main
        main.maxParticles = 1000;

        var emission = particulas.emission;  // Módulo Emission
        emission.rateOverTime = 1000;


        yield return new WaitForSeconds(1f);

        DesactivarPanelCasas();
        DesactivarPanelCuenca();

        ActivarPanelCuenca();
        txtPanelCuenca.textoAlmacenado = "Cunde el caos entre las personas, se presentan desprendimientos de tierra, las viviendas colapsan y la quebrada se desborda.";
        txtPanelCuenca.MostrarTexto("Cunde el caos entre las personas, se presentan desprendimientos de tierra, las viviendas colapsan y la quebrada se desborda.");
        movimientoSuavizadoCuenca.CambiarObjetivoSecundario();

        rio.tope = 0.073f;
        rio.SubirDisplace();

        if (cuenca.activeInHierarchy)
        {
            desastreCuencaActivo = true;
            SimpleAudioManager.singleton.Gritos();
            DesastreCuenca();
        }
        else
        {
            desastreCuencaActivo = true;
        }

        yield return new WaitForSeconds(5f);

        ActivarPanelCasas();
        txtPanelCasas.textoAlmacenado = "La capacidad de infiltración del suelo se ve superada; las zonas urbanas se inundan y suena la alarma para una pronta evacuación.";
        txtPanelCasas.MostrarTexto("La capacidad de infiltración del suelo se ve superada; las zonas urbanas se inundan y suena la alarma para una pronta evacuación.");
        movimientoSuavizadoCasas.CambiarObjetivoSecundario();

        if (casas.activeInHierarchy)
        {
            desastreCasasActivo = true;
            SimpleAudioManager.singleton.Gritos();
            SimpleAudioManager.singleton.Alarma();
            DesastreCasas();
        }
        else
        {
            desastreCasasActivo = true;
        }

    }

    public void DesastreCuenca()
    {
        if (desastreCuencaActivo)
        {
            desastreCuencaActivo = false;
            // Empezamos el evento
            if (coroutine3 != null) StopCoroutine(coroutine3);
            coroutine3 = StartCoroutine(DesastreCuencaCorrutina());
        }         
    }

    private IEnumerator DesastreCuencaCorrutina()
    {
        SimpleAudioManager.singleton.PlaySound(3);
        // Aumentar Particulas lluvia

        for (int i = 0; i < temblorCasas.Length; i++)
        {
            temblorCasas[i].IniciarDerrumbe();
        }

        for (int i = 0; i < npcsCuenca.Length; i++)
        {
            npcsCuenca[i].CorrerPorSuVida();
        }

        for (int i = 0; i < vehiculos.Length; i++)
        {
            vehiculos[i].DetenerSpawn();
        }

        yield return new WaitForSeconds(7f);

        SimpleAudioManager.singleton.PlaySound(4);
        

        for (int i = 0; i < casasDesarmadas.Length; i++)
        {
            casasDesarmadas[i].SetActive(true);
        }

        for (int i = 0; i < npcsCuenca.Length - 1; i++)
        {
            npcsCuenca[i].enabled = false;
        }

        for (int i = 0; i < npcAhogados.Length; i++)
        {
            npcAhogados[i].isKinematic = false;
        }
    }

    public void DesastreCasas()
    {
        if (desastreCasasActivo)
        {
            desastreCasasActivo = false;
            // Empezamos el evento
            if (coroutine4 != null) StopCoroutine(coroutine4);
            coroutine4 = StartCoroutine(DesastreCasasCorrutina());
        }        
    }

    private IEnumerator DesastreCasasCorrutina()
    {
        for (int i = 0; i < temblorTerrenoCasas.Length; i++)
        {
            temblorTerrenoCasas[i].Vibrar();
        }

        for (int i = 0; i < npcsCasas.Length; i++)
        {
            npcsCasas[i].CorrerPorSuVida();
        }

        yield return new WaitForSeconds(10f);

        //SimpleAudioManager.singleton.DesactivarAlarma();
    }

    public void AumentarRioCuenca()
    {   
        //rioCuenca.SubirDisplace();
    }

    public void AumentarRioCasas()
    {
        camaraAlerta.IniciarAlerta();
        //rioCasas.SubirDisplace();
    }

    public void AumentarMasRioCuenca()
    {
        //rioCuenca.tope = 0.073f;
        //rioCuenca.SubirDisplace();
    }

    public void AumentarMasRioCasas()
    {
       // rioCasas.tope = 0.073f;
        //rioCasas.SubirDisplace();
    }
}
