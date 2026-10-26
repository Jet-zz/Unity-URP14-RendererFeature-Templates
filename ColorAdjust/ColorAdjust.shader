Shader "ColorAdjust"
{
    Properties
    {
        _MainTex("MainTex" , 2D) = "white"{}
        // _Intensity("Intensity", Range(0, 1)) = 1
        // _Brightness("Brightness", Range(-1, 1)) = 0
        // _Contrast("Contrast", Range(0, 10)) = 1
        // _Gamma("Gamma", Range(0.01, 10)) = 1
        // _Hue("Hue", Range(0, 1)) = 0
        // _Saturation("Saturation", Range(0, 2)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline"="UniversalPipeline"}

        Pass
        {
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"

            CBUFFER_START(UnityPerMaterial)
                float _Intensity;
                float _Brightness;
                float _Contrast;
                float _Gamma;
                float _Hue;
                float _Saturation;
            CBUFFER_END

            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv         : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv         : TEXCOORD0;
            };


            inline float3 ColorAdjust(float3 pixel)
            {
                // 对比度和亮度
                pixel = max(0.0, (pixel - 0.5) * _Contrast + 0.5 + _Brightness);

                // 色相
                float3 hsv = RgbToHsv(pixel);
                hsv.x = frac(hsv.x + _Hue);
                pixel = HsvToRgb(hsv);

                // Gamma
                pixel = pow(abs(pixel), _Gamma);

                // 饱和度
                float luma = Luminance(pixel);
                pixel = luma.xxx + _Saturation * (pixel - luma.xxx);

                return max(0.0, pixel);
            }

            Varyings vert(Attributes v)
            {
                Varyings o;
                o.positionCS = TransformObjectToHClip(v.positionOS.xyz);
                o.uv = v.uv;
                return o;
            }

            half4 frag(Varyings i) : SV_Target
            {
                half4 baseMap = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex , i.uv);
                half3 adjustedColor = ColorAdjust(baseMap.rgb);
                half4 finalColor = half4(adjustedColor, baseMap.a);

                return lerp(baseMap, finalColor, _Intensity);
            }
            ENDHLSL
        }
    }
}
