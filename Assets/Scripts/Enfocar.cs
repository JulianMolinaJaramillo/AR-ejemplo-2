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
                ManagerControlador.singleton.ActivarImagenCuenca();

                if (ManagerControlador.singleton.desastreSecundarioActivo && !ManagerControlador.singleton.puntoEncuentroActivo)
                {
                    SimpleAudioManager.singleton.Gritos();
                }

                if (ManagerControlador.singleton.momentoReunionTerminadoCuenca)
                {
                    SimpleAudioManager.singleton.Hablando();
                }
            }
            else if (btnCasas)
            {
                ManagerControlador.singleton.DesastreCasas();
                ManagerControlador.singleton.ActivarImagenCasas();

                if (ManagerControlador.singleton.desastreSecundarioActivo)
                {
                    SimpleAudioManager.singleton.Gritos();
                    SimpleAudioManager.singleton.Alarma();
                }
            }

            aumentarEscala.AsignarObjetivo(objeto);
        }
        else
        {
            if (btnCuencaInterno)
            {
                ManagerControlador.singleton.ActivarImagenIguana();

                if (ManagerControlador.singleton.desastreSecundarioActivo)
                {
                    SimpleAudioManager.singleton.DesactivarGritos();
                }

                if (ManagerControlador.singleton.momentoReunionTerminado)
                {
                    ManagerControlador.singleton.momentoReunionTerminado = false;
                    ManagerControlador.singleton.botonReunion.SetActive(true);
                }

                if (ManagerControlador.singleton.momentoDosCasasTerminado && ManagerControlador.singleton.momentoDosCuencaTerminado)
                {
                    ManagerControlador.singleton.AntesDeNormalizarCuenca();
                }

                if (ManagerControlador.singleton.momentoReunionTerminadoCuenca)
                {
                    ManagerControlador.singleton.NormalizarCuenca();
                }

                if (ManagerControlador.singleton.momentoReunionTerminadoCuenca)
                {
                    SimpleAudioManager.singleton.DesactivarHablando();
                }
            }
            else if (btnCasasInterno)
            {
                ManagerControlador.singleton.ActivarImagenIguana();

                if (ManagerControlador.singleton.desastreSecundarioActivo)
                {
                    SimpleAudioManager.singleton.DesactivarGritos();
                    SimpleAudioManager.singleton.DesactivarAlarma();
                }

                if (ManagerControlador.singleton.momentoReunionTerminado)
                {
                    ManagerControlador.singleton.momentoReunionTerminado = false;
                    ManagerControlador.singleton.botonReunion.SetActive(true);
                }

                if (ManagerControlador.singleton.momentoDosCasasTerminado && ManagerControlador.singleton.momentoDosCuencaTerminado)
                {
                    ManagerControlador.singleton.AntesDeNormalizarCuenca();
                }

                if (ManagerControlador.singleton.momentoReunionTerminadoCuenca)
                {
                    ManagerControlador.singleton.NormalizarCuenca();
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
                ManagerControlador.singleton.ActivarImagenCuenca();

                if (ManagerControlador.singleton.desastreSecundarioActivo && !ManagerControlador.singleton.puntoEncuentroActivo)
                {
                    SimpleAudioManager.singleton.Gritos();
                }

                if (ManagerControlador.singleton.momentoReunionTerminadoCuenca)
                {
                    SimpleAudioManager.singleton.Hablando();
                }
            }
            else if (btnCasas)
            {
                ManagerControlador.singleton.DesastreCasas();
                ManagerControlador.singleton.ActivarImagenCasas();

                if (ManagerControlador.singleton.desastreSecundarioActivo)
                {
                    SimpleAudioManager.singleton.Gritos();
                    SimpleAudioManager.singleton.Alarma();
                }
            }

            aumentarEscala.AsignarObjetivo(objeto);
        }
        else
        {
            if (btnCuencaInterno)
            {
                ManagerControlador.singleton.ActivarImagenIguana();

                if (ManagerControlador.singleton.desastreSecundarioActivo)
                {
                    SimpleAudioManager.singleton.DesactivarGritos();
                }

                if (ManagerControlador.singleton.momentoReunionTerminado)
                {
                    ManagerControlador.singleton.momentoReunionTerminado = false;
                    ManagerControlador.singleton.botonReunion.SetActive(true);
                }

                if (ManagerControlador.singleton.momentoDosCasasTerminado && ManagerControlador.singleton.momentoDosCuencaTerminado)
                {
                    ManagerControlador.singleton.AntesDeNormalizarCuenca();
                }

                if (ManagerControlador.singleton.momentoReunionTerminadoCuenca)
                {
                    ManagerControlador.singleton.NormalizarCuenca();
                }

                if (ManagerControlador.singleton.momentoReunionTerminadoCuenca)
                {
                    SimpleAudioManager.singleton.DesactivarHablando();
                }
            }
            else if (btnCasasInterno)
            {
                ManagerControlador.singleton.ActivarImagenIguana();

                if (ManagerControlador.singleton.desastreSecundarioActivo)
                {
                    SimpleAudioManager.singleton.DesactivarGritos();
                    SimpleAudioManager.singleton.DesactivarAlarma();
                }

                if (ManagerControlador.singleton.momentoReunionTerminado)
                {
                    ManagerControlador.singleton.momentoReunionTerminado = false;
                    ManagerControlador.singleton.botonReunion.SetActive(true);
                }

                if (ManagerControlador.singleton.momentoDosCasasTerminado && ManagerControlador.singleton.momentoDosCuencaTerminado)
                {
                    ManagerControlador.singleton.AntesDeNormalizarCuenca();
                }

                if (ManagerControlador.singleton.momentoReunionTerminadoCuenca)
                {
                    ManagerControlador.singleton.NormalizarCuenca();
                }
            }

            aumentarEscala.Escalar();
        }
    }
}

