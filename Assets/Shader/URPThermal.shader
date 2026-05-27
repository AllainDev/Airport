Shader "Universal Render Pipeline/ThermalRunway"
{
    Properties {
        _BaseMap("Base Map", 2D) = "white" {}
        _NormalMap("Normal Map", 2D) = "bump" {}
        _ThermalNoise("Thermal Noise Texture", 2D) = "white" {}
        _HeatIntensity("Heat Intensity", Range(0, 5)) = 1.0
    }
    SubShader {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline" }
        Pass {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"

            struct Attributes { float4 positionOS : POSITION; float2 uv : TEXCOORD0; float3 normalOS : NORMAL; };
            struct Varyings { float4 positionCS : SV_POSITION; float2 uv : TEXCOORD0; float3 normalWS : TEXCOORD1; };

            TEXTURE2D(_BaseMap); SAMPLER(sampler_BaseMap);
            TEXTURE2D(_NormalMap); SAMPLER(sampler_NormalMap);
            TEXTURE2D(_ThermalNoise); SAMPLER(sampler_ThermalNoise);
            float _HeatIntensity;

            Varyings vert(Attributes input) {
                Varyings output;
                output.positionCS = TransformObjectToHClip(input.positionOS.xyz);
                output.uv = input.uv;
                output.normalWS = TransformObjectToWorldNormal(input.normalOS);
                return output;
            }

            half4 frag(Varyings input) : SV_Target {
                half4 base = SAMPLE_TEXTURE2D(_BaseMap, sampler_BaseMap, input.uv);
                float noise = SAMPLE_TEXTURE2D(_ThermalNoise, sampler_ThermalNoise, input.uv).r;
                Light mainLight = GetMainLight();
                float diff = saturate(dot(input.normalWS, mainLight.direction));
                float thermal = diff * noise * _HeatIntensity;
                return base + half4(thermal, thermal, thermal, 1.0);
            }
            ENDHLSL
        }
    }
}
    