using System.Collections;
using UnityEngine;

public class MovimientoNPC : MonoBehaviour
{
    [Header("Configuración")]
    public Transform[] puntos;           // Puntos de movimiento (en local)
    public float velocidad = 3f;         // Velocidad de movimiento
    public float tiempoEspera = 2f;      // Tiempo de espera en cada punto

    private Transform destinoActual;
    private bool esperando = false;
    private Coroutine coroutine;

    void Start()
    {
        ElegirNuevoDestino();
    }

    void Update()
    {
        if (destinoActual == null || esperando) return;

        // --- Movimiento en espacio local ---
        transform.localPosition = Vector3.MoveTowards(
            transform.localPosition,
            destinoActual.localPosition,
            velocidad * Time.deltaTime
        );

        // Revisar si ya llegó (exacto)
        if (transform.localPosition == destinoActual.localPosition)
        {
            if (coroutine != null) StopCoroutine(coroutine);
            coroutine = StartCoroutine(EsperarYContinuar());
        }
    }

    IEnumerator EsperarYContinuar()
    {
        esperando = true;
        yield return new WaitForSeconds(tiempoEspera);
        ElegirNuevoDestino();
        esperando = false;
    }

    void ElegirNuevoDestino()
    {
        if (puntos.Length == 0) return;
        destinoActual = puntos[Random.Range(0, puntos.Length)];
    }

    private void OnEnable()
    {
        if (coroutine != null) StopCoroutine(coroutine);
        coroutine = StartCoroutine(EsperarYContinuar());
    }
}
