using UnityEngine;

public class RainSimpleController : MonoBehaviour
{
    public ParticleSystem rainSystem;

    [Header("Valores máximos")]
    public float maxRateOverTime = 1000f;
    public int maxParticles = 50000;

    [Range(0f, 1f)] public float intensity = 0f; // 0 = nada, 1 = lluvia total

    ParticleSystem.EmissionModule emission;
    ParticleSystem.MainModule main;

    void Start()
    {
        if (rainSystem == null) rainSystem = GetComponent<ParticleSystem>();
        emission = rainSystem.emission;
        main = rainSystem.main;
    }

    void Update()
    {
        // Escalamos directo según intensidad
        emission.rateOverTime = maxRateOverTime * intensity;
        main.maxParticles = Mathf.RoundToInt(maxParticles * intensity);
    }
}
