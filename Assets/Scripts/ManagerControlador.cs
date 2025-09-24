using System.Collections;
using TMPro;
using UnityEngine;

public class ManagerControlador : MonoBehaviour
{
    [Header("Configuraciónes para Zappar Visible e invisible")]
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
    public MovimientoNPC[] npcsCuencaFinales;
    public Rigidbody[] npcAhogados;
    public DerrumbeCasas[] temblorCasas;
    public GameObject[] casasDesarmadas;
    public NivelAgua rioCuenca; // arrastras el objeto con el material al inspector
    public Roca[] rocas;

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
    public GameObject botonLluviaCreciente;
    public GameObject botonReunion;
    public ManagerVehiculos[] vehiculos;
    public GameObject panelDespedida;
    public PulsoEscala[] pulsosInternos;
    public TextoEscalonado canvasInformativo;
    public TextoEscalonado canvasInformativoCuencaInterno;
    public TextoEscalonado canvasInformativoCasasInterno;
    public GameObject imagenIguana;
    public GameObject imagenCuenca;
    public GameObject imagenCasas;

    private bool lluviaActiva;
    private Coroutine coroutine;
    private Coroutine coroutine2;
    private Coroutine coroutine3;
    private Coroutine coroutine4;
    private Coroutine coroutine5;
    //
    private bool iniciarPDF;
    private bool desastreCuencaActivo;
    private bool desastreCasasActivo;
    [HideInInspector]
    public bool desastreInicialActivo;
    [HideInInspector]
    public bool desastreSecundarioActivo;
    [HideInInspector]
    public bool puntoEncuentroActivo;

    public static ManagerControlador singleton;
    private bool imagenIguanaActiva;
    private bool imagenCuencaActiva;
    private bool imagenCasasActiva;

    private bool momentoUnoCuencaActivo;
    private bool momentoUnoCasasActivo;

    private bool momentoUnoCuencaTerminado;
    private bool momentoUnoCasasTerminado;

    [HideInInspector]
    public bool momentoDosCuencaTerminado;
    [HideInInspector]
    public bool momentoDosCasasTerminado;

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

        if (momentoUnoCasasTerminado && momentoUnoCuencaTerminado)
        {
            botonLluviaCreciente.SetActive(true);
            momentoUnoCasasTerminado = false;
            momentoUnoCuencaTerminado = false;
        }

        if (momentoDosCasasTerminado && momentoDosCuencaTerminado)
        {
            botonReunion.SetActive(true);
            momentoDosCasasTerminado = false;
            momentoDosCuencaTerminado = false;
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

        if (imagenIguanaActiva)
        {
            imagenIguana.SetActive(true);
        }
        else if (imagenCuencaActiva)
        {
            imagenCuenca.SetActive(true);
        }
        else if (imagenCasasActiva)
        {
            imagenCasas.SetActive(true);
        }
        
        
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

        if (imagenIguanaActiva)
        {
            imagenIguana.SetActive(false);
        }
        else if (imagenCuencaActiva)
        {
            imagenCuenca.SetActive(false);
        }
        else if (imagenCasasActiva)
        {
            imagenCasas.SetActive(false);
        }

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

    public void ActivarImagenIguana()
    {
        imagenIguanaActiva = true;
        imagenCuencaActiva = false;
        imagenCasasActiva = false;

        imagenIguana.SetActive(true);
        imagenCasas.SetActive(false);
        imagenCuenca.SetActive(false);
    }

    public void ActivarImagenCuenca()
    {
        imagenIguanaActiva = false;
        imagenCuencaActiva = true;
        imagenCasasActiva = false;

        imagenIguana.SetActive(false);
        imagenCasas.SetActive(false);
        imagenCuenca.SetActive(true);
    }

    public void ActivarImagenCasas()
    {
        imagenIguanaActiva = false;
        imagenCuencaActiva = false;
        imagenCasasActiva = true;

        imagenIguana.SetActive(false);
        imagenCasas.SetActive(true);
        imagenCuenca.SetActive(false);
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
        for (int i = 0; i < pulsosInternos.Length; i++)
        {
            pulsosInternos[i].gameObject.SetActive(false);
        }

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

        if (rio.gameObject.activeInHierarchy)
        {
            rio.SubirDisplace();
        }
        else
        {
            rio.rioActivo = true;
        }
        
        yield return new WaitForSeconds(1f);
        
        DesactivarPanelCasas();
        DesactivarPanelCuenca();
        iniciarPDF = true;

        pulsoEscalaCuenca.IniciarAlerta();
        ActivarPanelCuenca();
        txtPanelCuenca.textoAlmacenado = "El cielo de la cuenca comienza a cubrirse de nubes y se inicia una llovizna; el agua de la quebrada empieza a crecer.";
        txtPanelCuenca.MostrarTexto("El cielo de la cuenca comienza a cubrirse de nubes y se inicia una llovizna; el agua de la quebrada empieza a crecer.");

        canvasInformativoCuencaInterno.textoAlmacenado = "El agua golpea las laderas con pendientes fuertes, lo que acelera los procesos de escorrentía superficial.";
        canvasInformativoCuencaInterno.MostrarTexto("El agua golpea las laderas con pendientes fuertes, lo que acelera los procesos de escorrentía superficial.");

        yield return new WaitForSeconds(0.5f);

        pulsoEscalaCasas.IniciarAlerta();
        ActivarPanelCasas();  
        txtPanelCasas.textoAlmacenado = "A la zona residencial empiezan a llegar las nubes cargadas de lluvia; crece peligrosamente el nivel del agua.";
        txtPanelCasas.MostrarTexto("A la zona residencial empiezan a llegar las nubes cargadas de lluvia; crece peligrosamente el nivel del agua.");

        canvasInformativoCasasInterno.textoAlmacenado = "los habitantes se percatan de que el nivel del caudal se incrementa y el sensor del nivel de agua se activa.";
        canvasInformativoCasasInterno.MostrarTexto("los habitantes se percatan de que el nivel del caudal se incrementa y el sensor del nivel de agua se activa.");


        if (cuenca.activeInHierarchy)
        {
            momentoUnoCuencaActivo = true;
            DesastreCuenca();
        }
        else
        {
            momentoUnoCuencaActivo = true;
        }

        if (casas.activeInHierarchy)
        {
            momentoUnoCasasActivo = true;
            DesastreCasas();
        }
        else
        {
            momentoUnoCasasActivo = true;
        }
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
        for (int i = 0; i < pulsosInternos.Length; i++)
        {
            pulsosInternos[i].gameObject.SetActive(false);
        }

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

        canvasInformativoCuencaInterno.textoAlmacenado = "El suelo se desprende en bloques, arrastrado por la corriente, generando un socavamiento progresivo, y se ve como la columna va quedando expuesta.";
        canvasInformativoCuencaInterno.MostrarTexto("El suelo se desprende en bloques, arrastrado por la corriente, generando un socavamiento progresivo, y se ve como la columna va quedando expuesta.");

        if (rio.gameObject.activeInHierarchy)
        {
            rio.tope = 0.031f;
            rio.SubirDisplace();
        }
        else
        {
            rio.rioActivo = true;
        }
        
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

        yield return new WaitForSeconds(0.5f);

        ActivarPanelCasas();
        txtPanelCasas.textoAlmacenado = "La capacidad de infiltración del suelo se ve superada; las zonas urbanas se inundan y suena la alarma para una pronta evacuación.";
        txtPanelCasas.MostrarTexto("La capacidad de infiltración del suelo se ve superada; las zonas urbanas se inundan y suena la alarma para una pronta evacuación.");

        canvasInformativoCasasInterno.textoAlmacenado = "El nivel del agua aumenta, se activa una alarma comunitaria de emergencia instalada como parte del sistema de monitoreo y alertas tempranas - SATC.";
        canvasInformativoCasasInterno.MostrarTexto("El nivel del agua aumenta, se activa una alarma comunitaria de emergencia instalada como parte del sistema de monitoreo y alertas tempranas - SATC.");

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

    [ContextMenu("Iniciar 3")]
    public void EmpezarEventoPuntoEncuentro()
    {
        // Empezamos el evento
        if (coroutine5 != null) StopCoroutine(coroutine5);
        coroutine5 = StartCoroutine(PuntoEncuentro());
    }

    private IEnumerator PuntoEncuentro()
    {
        canvasInformativo.textoAlmacenado = "4. Reunión punto de encuentro.";
        canvasInformativo.MostrarTexto("4. Reunión punto de encuentro.");
        

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < npcsCuenca.Length; i++)
        {
            npcsCuenca[i].gameObject.SetActive(false);
        }

        for (int i = 0; i < npcsCuencaFinales.Length; i++)
        {
            npcsCuencaFinales[i].gameObject.SetActive(true);
        }

        DesactivarPanelCasas();
        DesactivarPanelCuenca();

        ActivarPanelCuenca();
        txtPanelCuenca.textoAlmacenado = "La comunidad se reune en el punto de encuentro.";
        txtPanelCuenca.MostrarTexto("La comunidad se reune en el punto de encuentro.");
        puntoEncuentroActivo = true;

        //
        yield return new WaitForSeconds(5f);

        var main = particulas.main;  // Módulo Main
        main.maxParticles = 500;

        var emission = particulas.emission;  // Módulo Emission
        emission.rateOverTime = 100;

        crecimientoNubes.RestablecerCrecimiento();

        yield return new WaitForSeconds(5f);

        particulas.Stop();
        lluviaActiva = false;

    }

    public void DesastreCuenca()
    {
        if (coroutine3 != null) StopCoroutine(coroutine3);
        coroutine3 = StartCoroutine(IniciadorCuenca());
    }

    private IEnumerator IniciadorCuenca()
    {
        yield return new WaitForSeconds(1f);

        if (momentoUnoCuencaActivo)
        {
            momentoUnoCuencaActivo = false;
            // Empezamos el evento
            if (coroutine3 != null) StopCoroutine(coroutine3);
            coroutine3 = StartCoroutine(MomentoUnoCuenca());
        }

        if (desastreCuencaActivo)
        {
            desastreCuencaActivo = false;
            // Empezamos el evento
            if (coroutine3 != null) StopCoroutine(coroutine3);
            coroutine3 = StartCoroutine(DesastreCuencaCorrutina());
        }

        if (puntoEncuentroActivo)
        {
            puntoEncuentroActivo = false;
        }
    }

    private IEnumerator MomentoUnoCuenca()
    {
        rioCuenca.SubirDisplace();

        yield return new WaitForSeconds(5f);

        pulsosInternos[0].gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        pulsosInternos[0].IniciarAlerta();
        momentoUnoCuencaTerminado = true;
    }

    private IEnumerator DesastreCuencaCorrutina()
    {
        SimpleAudioManager.singleton.PlaySound(3);

        yield return new WaitForSeconds(1f);

        for (int i = 0; i < rocas.Length; i++)
        {
            rocas[i].IniciarRecorrido();
        }

        yield return new WaitForSeconds(0.2f);

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


        pulsosInternos[0].gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        pulsosInternos[0].IniciarAlerta();
        momentoDosCuencaTerminado = true;
    }

    public void DesastreCasas()
    {   
        if (coroutine3 != null) StopCoroutine(coroutine3);
        coroutine3 = StartCoroutine(IniciadorCasas());
    }

    private IEnumerator IniciadorCasas()
    {
        yield return new WaitForSeconds(1f);

        if (momentoUnoCasasActivo)
        {
            momentoUnoCasasActivo = false;
            // Empezamos el evento
            if (coroutine3 != null) StopCoroutine(coroutine3);
            coroutine3 = StartCoroutine(MomentoUnoCasas());
        }

        if (desastreCasasActivo)
        {
            desastreCasasActivo = false;
            // Empezamos el evento
            if (coroutine4 != null) StopCoroutine(coroutine4);
            coroutine4 = StartCoroutine(DesastreCasasCorrutina());
        }
    }

    private IEnumerator MomentoUnoCasas()
    {
        movimientoSuavizadoCasas.IniciarDesplazamiento();
        camaraAlerta.IniciarAlerta();
        rioCasas.SubirGain();
        yield return new WaitForSeconds(5f);

        pulsosInternos[1].gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        pulsosInternos[1].IniciarAlerta();
        momentoUnoCasasTerminado = true;
    }

    private IEnumerator DesastreCasasCorrutina()
    {
        movimientoSuavizadoCasas.CambiarObjetivoSecundario();

        for (int i = 0; i < temblorTerrenoCasas.Length; i++)
        {
            temblorTerrenoCasas[i].Vibrar();
        }

        for (int i = 0; i < npcsCasas.Length; i++)
        {
            npcsCasas[i].CorrerPorSuVida();
        }

        yield return new WaitForSeconds(7f);

        pulsosInternos[1].gameObject.SetActive(true);

        yield return new WaitForSeconds(0.5f);

        pulsosInternos[1].IniciarAlerta();
        momentoDosCasasTerminado = true;
    }
}
