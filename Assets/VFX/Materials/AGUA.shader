Shader "Custom/AGUA_ProcNoise_FresDepth_MinSafe"
{
    Properties
    {
        _Color ("Color base", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.85
        _Metallic   ("Metallic",   Range(0,1)) = 0.0

        // Procedural Noise (FBM)
        _NoiseScale      ("Noise Scale", Float) = 1.0
        _NoiseOctaves    ("Noise Octaves (1-6)", Range(1,6)) = 4
        _NoiseLacunarity ("Lacunarity", Float) = 2.0
        _NoiseGain       ("Gain", Float) = 0.5
        _NoisePower      ("Noise Power", Range(0.5,4.0)) = 1.4
        _NoiseSpeed      ("Noise Speed (XY)", Vector) = (0.2, 0.1, 0, 0)
        _Seed            ("Seed", Float) = 0.0

        // Displacement & Normals
        _DisplaceAmp     ("Amplitud desplazamiento", Range(0,0.5)) = 0.05
        _NormalStrength  ("Fuerza normal", Range(0,4)) = 1.5
        _NormalDelta     ("Delta derivadas", Range(0.001, 0.2)) = 0.02

        // Fresnel
        _FresnelPower    ("Fresnel Power", Range(0.5,8.0)) = 3.0
        _FresnelStrength ("Fresnel Strength", Range(0,2)) = 0.8
        _EdgeColor       ("Fresnel/Edge Color", Color) = (0.75, 0.9, 1.0, 1.0)
        _BaseAlpha       ("Alpha base", Range(0,1)) = 0.5

        // Intersection (Depth)
        _IntersectionThickness ("Grosor intersección", Range(0.001, 1.0)) = 0.2
        _IntersectionStrength  ("Fuerza intersección", Range(0,3)) = 1.2
        _FoamColor             ("Color espuma", Color) = (1,1,1,1)
    }

    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }
        LOD 350

        Blend SrcAlpha OneMinusSrcAlpha
        ZWrite Off
        Cull Back

        CGPROGRAM
        #pragma surface surf Standard alpha:fade fullforwardshadows vertex:vert
        #pragma target 3.0
        #include "UnityCG.cginc"

        sampler2D _MainTex;
        fixed4 _Color;
        half _Glossiness;
        half _Metallic;

        // Depth texture declarada directo (sin macro)
        sampler2D _CameraDepthTexture;

        // Noise params
        float  _NoiseScale;
        float  _NoiseLacunarity;
        float  _NoiseGain;
        float  _NoisePower;
        float4 _NoiseSpeed; // xy
        float  _Seed;
        float  _NoiseOctaves;

        // Displace/Normals
        float  _DisplaceAmp;
        float  _NormalStrength;
        float  _NormalDelta;

        // Fresnel
        float  _FresnelPower;
        float  _FresnelStrength;
        fixed4 _EdgeColor;
        float  _BaseAlpha;

        // Intersection
        float  _IntersectionThickness;
        float  _IntersectionStrength;
        fixed4 _FoamColor;

        struct Input
        {
            float2 uv_MainTex;
            float3 worldPos;
            float3 viewDir;
            float4 screenPos;
        };

        // -------- Simple value noise + FBM --------
        float rand2d(float2 p, float seed)
        {
            return frac(sin(dot(p, float2(12.9898,78.233)) + seed) * 43758.5453123);
        }

        float noise2(float2 p, float seed)
        {
            float2 i = floor(p);
            float2 f = frac(p);
            float2 u = f*f*f*(f*(f*6 - 15) + 10);

            float a = rand2d(i + float2(0,0), seed);
            float b = rand2d(i + float2(1,0), seed);
            float c = rand2d(i + float2(0,1), seed);
            float d = rand2d(i + float2(1,1), seed);

            float ab = lerp(a, b, u.x);
            float cd = lerp(c, d, u.x);
            return lerp(ab, cd, u.y);
        }

        int ClampOctaves(float v)
        {
            int oct = (int)floor(v + 0.5);
            if (oct < 1) oct = 1;
            if (oct > 6) oct = 6;
            return oct;
        }

        float fbm(float2 p, int oct, float lac, float gain, float power, float seed)
        {
            float amp = 1.0;
            float sum = 0.0;
            float norm = 0.0;

            // 6 máx octavas
            [unroll] for (int i = 0; i < 6; i++)
            {
                if (i >= oct) break;
                sum  += noise2(p, seed + (float)i * 19.19) * amp;
                norm += amp;
                p   *= lac;
                amp *= gain;
            }

            float h = (norm > 0.0) ? (sum / norm) : 0.0;
            return pow(saturate(h), power);
        }

        // ---------- Helpers depth ----------
        float SampleSceneDepth01(float4 sp)
        {
            // Usa macros de UnityCG para coord correctas
            #if defined(UNITY_REVERSED_Z)
                float raw = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(sp));
            #else
                float raw = SAMPLE_DEPTH_TEXTURE_PROJ(_CameraDepthTexture, UNITY_PROJ_COORD(sp));
            #endif
            return raw; // 0..1
        }

        float SampleSceneEye(float4 sp)
        {
            float raw = SampleSceneDepth01(sp);
            return LinearEyeDepth(raw);
        }

        // ---------- Vertex: displacement ----------
        void vert (inout appdata_full v)
        {
            float3 wpos = mul(unity_ObjectToWorld, v.vertex).xyz;

            float2 nUV = wpos.xz * _NoiseScale + _Time.y * _NoiseSpeed.xy;
            int oct = ClampOctaves(_NoiseOctaves);

            float h = fbm(nUV, oct, _NoiseLacunarity, _NoiseGain, _NoisePower, _Seed);
            float disp = (h - 0.5) * 2.0 * _DisplaceAmp;

            float3 wnorm = UnityObjectToWorldNormal(v.normal);
            wpos += wnorm * disp;

            v.vertex = mul(unity_WorldToObject, float4(wpos,1));
        }

        // ---------- Surface ----------
        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 baseCol = tex2D(_MainTex, IN.uv_MainTex) * _Color;

            float2 nUV = IN.worldPos.xz * _NoiseScale + _Time.y * _NoiseSpeed.xy;
            int oct = ClampOctaves(_NoiseOctaves);

            float hC = fbm(nUV, oct, _NoiseLacunarity, _NoiseGain, _NoisePower, _Seed);

            // Normales desde derivadas en XZ mundo
            float d = _NormalDelta;
            float hU = fbm(nUV + float2(d, 0), oct, _NoiseLacunarity, _NoiseGain, _NoisePower, _Seed);
            float hV = fbm(nUV + float2(0, d), oct, _NoiseLacunarity, _NoiseGain, _NoisePower, _Seed);
            float dhdu = (hU - hC) / d;
            float dhdv = (hV - hC) / d;
            float3 nT = normalize(float3(-dhdu * _NormalStrength, -dhdv * _NormalStrength, 1.0));
            o.Normal = nT;

            // Fresnel (en este espacio, Z es “normal base”)
            float3 N = normalize(float3(0,0,1));
            float3 V = normalize(IN.viewDir);
            float fTerm = pow(1.0 - saturate(dot(N, V)), _FresnelPower);
            float fres  = saturate(fTerm * _FresnelStrength);

            // Intersección con otros objetos (depth)
            float sceneEye = SampleSceneEye(IN.screenPos);
            float thisEye  = IN.screenPos.w;   // profundidad del fragmento actual
            float diff = max(0.0, sceneEye - thisEye);
            float intersection = saturate(1.0 - diff / max(1e-5, _IntersectionThickness));
            intersection *= _IntersectionStrength;

            float3 rim = _EdgeColor.rgb * fres + _FoamColor.rgb * intersection;

            o.Albedo     = baseCol.rgb;
            o.Metallic   = _Metallic;
            o.Smoothness = _Glossiness;
            o.Emission   = rim;
            o.Alpha      = saturate(_BaseAlpha + (fres + intersection) * 0.5);
        }
        ENDCG
    }

    FallBack "Transparent"
}
