Shader "Custom/AGUA_Caustics_Displace"
{
    Properties
    {
        _Color ("Color base", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}

        _Glossiness ("Smoothness", Range(0,1)) = 0.85
        _Metallic ("Metallic", Range(0,1)) = 0.0

        // Caustics / Noise
        _CausticsTex ("Caustics/Noise (R/G/B)", 2D) = "gray" {}
        _CausticsColor ("Color causticas", Color) = (0.7,0.9,1,1)
        _CausticsTiling ("Tiling (world space)", Float) = 1.0
        _CausticsIntensity ("Intensidad", Range(0,3)) = 1.2
        _CausticsContrast ("Contraste", Range(0.5,2.5)) = 1.4

        // Panners (dos capas para interferencia)
        _Speed1 ("Velocidad 1 (XY)", Vector) = (0.2, 0.1, 0, 0)
        _Speed2 ("Velocidad 2 (XY)", Vector) = (-0.15, 0.22, 0, 0)

        // Distorsión de UV del albedo (refracción ligera)
        _DistortStrength ("Fuerza distorsión UV", Range(0,1)) = 0.1
        _DistortScale ("Escala distorsión", Float) = 1.0

        // Displacement & Normals from noise
        _DisplaceAmp ("Amplitud desplazamiento", Range(0,0.5)) = 0.05
        _NormalStrength ("Fuerza normal (relieve)", Range(0,4)) = 1.5
        _NormalDelta ("Delta derivadas (escala muestras)", Range(0.001, 0.2)) = 0.02
    }

    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue"="Geometry" }
        LOD 300

        CGPROGRAM
        #pragma surface surf Standard fullforwardshadows vertex:vert
        #pragma target 3.0

        #include "UnityCG.cginc"

        sampler2D _MainTex;
        sampler2D _CausticsTex;

        fixed4 _Color;
        half _Glossiness;
        half _Metallic;

        fixed4 _CausticsColor;
        float _CausticsTiling;
        half  _CausticsIntensity;
        half  _CausticsContrast;

        float4 _Speed1;
        float4 _Speed2;

        half  _DistortStrength;
        float _DistortScale;

        float _DisplaceAmp;
        float _NormalStrength;
        float _NormalDelta;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos; // para mapear caústicas en mundo (XZ)
        };

        // ---- Funciones auxiliares ----
        inline half Contrast(half v, half k)
        {
            v = saturate(v);
            return pow(v, k);
        }

        inline half HeightFromNoise(float2 uv)
        {
            // Doble muestra para interferencia (igual que en surf/vert)
            half n1 = tex2D(_CausticsTex, uv).r;
            half n2 = tex2D(_CausticsTex, uv).r; // mismo canal; si tu tex tiene variación RGB, puedes cambiar este a .g
            half h  = Contrast(n1 * n2, _CausticsContrast);
            return h;
        }

        // Vertex: desplaza a lo largo de la normal del mesh con el mismo patrón
        void vert (inout appdata_full v)
        {
            float3 wpos = mul(unity_ObjectToWorld, v.vertex).xyz;
            float  t    = _Time.y;
            float2 uvW  = wpos.xz * _CausticsTiling;

            float2 uv1 = uvW + t * _Speed1.xy;
            float2 uv2 = uvW + t * _Speed2.xy;

            // Muestras en vertex shader (usar tex2Dlod)
            half n1 = tex2Dlod(_CausticsTex, float4(uv1, 0, 0)).r;
            half n2 = tex2Dlod(_CausticsTex, float4(uv2, 0, 0)).r;
            half h  = Contrast(n1 * n2, _CausticsContrast);

            float disp = h * _DisplaceAmp;

            float3 wnorm = UnityObjectToWorldNormal(v.normal);
            wpos += wnorm * disp;

            v.vertex = mul(unity_WorldToObject, float4(wpos,1));
        }

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            float t = _Time.y;

            // UV en mundo (XZ) para caústicas
            float2 uvWorld = IN.worldPos.xz * _CausticsTiling;
            float2 uv1 = uvWorld + t * _Speed1.xy;
            float2 uv2 = uvWorld + t * _Speed2.xy;

            // Lee noise
            half n1 = tex2D(_CausticsTex, uv1).r;
            half n2 = tex2D(_CausticsTex, uv2).r;

            // Distorsión de albedo (refracción suave)
            float2 distortDir = (float2(n1, n2) - 0.5) * _DistortScale;
            float2 uvMain = IN.uv_MainTex + distortDir * _DistortStrength;

            fixed4 baseCol = tex2D(_MainTex, uvMain) * _Color;

            // Altura/caústicas para emisión
            half ca = Contrast(n1 * n2, _CausticsContrast);
            ca = saturate(ca * _CausticsIntensity);

            // ----- Normales desde height (derivadas en plano XZ mundo) -----
            // Calculamos gradiente de la altura con pequeñas muestras alrededor
            float  d = _NormalDelta;            // tamaño del paso en UV mundo
            float2 du = float2(d, 0);
            float2 dv = float2(0, d);

            // Alturas
            half hC = Contrast(tex2D(_CausticsTex, uvWorld + t * _Speed1.xy).r *
                               tex2D(_CausticsTex, uvWorld + t * _Speed2.xy).r, _CausticsContrast);

            half hU = Contrast(tex2D(_CausticsTex, (uvWorld+du) + t * _Speed1.xy).r *
                               tex2D(_CausticsTex, (uvWorld+du) + t * _Speed2.xy).r, _CausticsContrast);

            half hV = Contrast(tex2D(_CausticsTex, (uvWorld+dv) + t * _Speed1.xy).r *
                               tex2D(_CausticsTex, (uvWorld+dv) + t * _Speed2.xy).r, _CausticsContrast);

            float dhdu = (hU - hC) / d;
            float dhdv = (hV - hC) / d;

            // Construimos una "normal de mapa" en espacio tangente sintético (u: +X mundo, v: +Z mundo)
            // Nota: esto no usa las tangentes del mesh; es suficiente para simular relieve del patrón.
            float3 nT = normalize(float3(-dhdu * _NormalStrength, -dhdv * _NormalStrength, 1.0));
            o.Normal = nT; // Surface Shader espera normal en "tangent space"

            // PBR
            o.Albedo     = baseCol.rgb * (1.0 + ca * 0.15);
            o.Metallic   = _Metallic;
            o.Smoothness = _Glossiness;
            o.Emission   = _CausticsColor.rgb * ca;
            o.Alpha      = baseCol.a;
        }
        ENDCG
    }

    FallBack "Diffuse"
}
