using UnityEngine;

public class Enfocar : MonoBehaviour
{
    public GameObject objeto; 
    public AumentarEscala aumentarEscala;
    public bool btnCuenca;
    public bool btnCasas;
    public bool btnCuencaInterno;
    public bool btnCasasInterno;

    void OnMouseDown()
    {
        SimpleAudioManager.singleton.PlaySound(6);
        if (objeto != null)
        {
            if (btnCuenca)
            {
                ManagerControlador.singleton.DesastreCuenca();

                if (ManagerControlador.singleton.desastreInicialActivo)
                {
                    Debug.Log("g0");
                    ManagerControlador.singleton.pulsosInternos[0].IniciarAlerta();
                    ManagerControlador.singleton.AumentarRioCuenca();
                }

                if (ManagerControlador.singleton.desastreSecundarioActivo)
                {
                    SimpleAudioManager.singleton.Gritos();
                    ManagerControlador.singleton.AumentarMasRioCuenca();
                }
            }
            else if (btnCasas)
            {
                ManagerControlador.singleton.DesastreCasas();

                if (ManagerControlador.singleton.desastreInicialActivo)
                {
                    Debug.Log("g0");
                    ManagerControlador.singleton.pulsosInternos[1].IniciarAlerta();
                    ManagerControlador.singleton.camaraAlerta.IniciarAlerta();
                    ManagerControlador.singleton.AumentarRioCasas();
                }

                if (ManagerControlador.singleton.desastreSecundarioActivo)
                {
                    SimpleAudioManager.singleton.Gritos();
                    SimpleAudioManager.singleton.Alarma();
                    ManagerControlador.singleton.AumentarMasRioCasas();
                }
            }

            aumentarEscala.AsignarObjetivo(objeto);
        }
        else
        {
            if (btnCuencaInterno)
            {
                if (ManagerControlador.singleton.desastreSecundarioActivo)
                {
                    SimpleAudioManager.singleton.DesactivarGritos();
                }
            }
            else if (btnCasasInterno)
            {
                if (ManagerControlador.singleton.desastreSecundarioActivo)
                {
                    SimpleAudioManager.singleton.DesactivarGritos();
                    SimpleAudioManager.singleton.DesactivarAlarma();
                }
            }

            aumentarEscala.Escalar();
        }  
    }

    [ContextMenu("avticar")]
    public void Ejecutar()
    {
        SimpleAudioManager.singleton.PlaySound(6);
        if (objeto != null)
        {
            if (btnCuenca)
            {
                ManagerControlador.singleton.DesastreCuenca();

                if (ManagerControlador.singleton.desastreInicialActivo)
                {
                    Debug.Log("g0");
                    ManagerControlador.singleton.pulsosInternos[0].IniciarAlerta();
                    ManagerControlador.singleton.AumentarRioCuenca();
                }

                if (ManagerControlador.singleton.desastreSecundarioActivo && !ManagerControlador.singleton.puntoEncuentroActivo)
                {
                    SimpleAudioManager.singleton.Gritos();
                    ManagerControlador.singleton.AumentarMasRioCuenca();
                }
            }
            else if (btnCasas)
            {
                ManagerControlador.singleton.DesastreCasas();

                if (ManagerControlador.singleton.desastreInicialActivo)
                {
                    Debug.Log("g0");
                    ManagerControlador.singleton.pulsosInternos[1].IniciarAlerta();
                    ManagerControlador.singleton.camaraAlerta.IniciarAlerta();
                    ManagerControlador.singleton.AumentarRioCasas();
                }

                if (ManagerControlador.singleton.desastreSecundarioActivo)
                {
                    SimpleAudioManager.singleton.Gritos();
                    SimpleAudioManager.singleton.Alarma();
                    ManagerControlador.singleton.AumentarMasRioCasas();
                }
            }

            aumentarEscala.AsignarObjetivo(objeto);
        }
        else
        {
            if (btnCuencaInterno)
            {
                if (ManagerControlador.singleton.desastreSecundarioActivo)
                {
                    SimpleAudioManager.singleton.DesactivarGritos();
                }
            }
            else if (btnCasasInterno)
            {
                if (ManagerControlador.singleton.desastreSecundarioActivo)
                {
                    SimpleAudioManager.singleton.DesactivarGritos();
                    SimpleAudioManager.singleton.DesactivarAlarma();
                }
            }

            aumentarEscala.Escalar();
        }
    }
}

