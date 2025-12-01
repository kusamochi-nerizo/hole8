Shader "Custom/URP_VoronoiCrackGlass_Grayscale_CenterBiased"
{
    Properties
    {
        _CellScale("Cell Scale", Float) = 1.5
        _CrackWidth("Crack Width", Float) = 0.18
        _DistortStrength("Distort Strength", Float) = 0.02
        _GlassAlpha("Glass Alpha", Range(0,1)) = 0.3
        _CrackAlpha("Crack Alpha", Range(0,1)) = 0.0
        _BiasStrength("Bias Strength", Float) = 1.5
        _Center("Crack Center", Vector) = (0.5,0.5,0,0)
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        Pass
        {
            Blend SrcAlpha OneMinusSrcAlpha
            ZWrite Off

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            TEXTURE2D(_CameraOpaqueTexture);
            SAMPLER(sampler_CameraOpaqueTexture);

            float _CellScale;
            float _CrackWidth;
            float _DistortStrength;
            float _GlassAlpha;
            float _CrackAlpha;
            float _BiasStrength;
            float4 _Center;

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionCS : SV_POSITION;
                float2 uv : TEXCOORD0;
                float4 screenPos : TEXCOORD1;
            };

            float2 random2(float2 p)
            {
                return frac(sin(float2(dot(p,float2(127.1,311.7)),dot(p,float2(269.5,183.3))))*43758.5453);
            }

            // 中心寄りに点を配置する
            float2 biasedRandom2(float2 cell, float2 center, float biasStrength)
            {
                float2 rnd = random2(cell);
                float2 cellCenter = (cell + 0.5) / _CellScale;
                float dist = length(cellCenter - center);
                float bias = 1.0 - saturate(dist * biasStrength);
                rnd = lerp(rnd, float2(0.5, 0.5), bias);
                return rnd;
            }

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                OUT.screenPos = OUT.positionCS;
                return OUT;
            }

            float4 frag(Varyings IN) : SV_Target
            {
                float2 uv = IN.uv * _CellScale;
                float2 cell = floor(uv);
                float2 fuv = frac(uv);

                float minDist = 1.0;
                float secondMinDist = 1.0;
                float2 minVec = 0;

                float2 center = _Center.xy; // ひび割れの中心（デフォルトは画面中央）

                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 neighbor = float2(x, y);
                        // 中心寄り点配置
                        float2 rndPt = biasedRandom2(cell + neighbor, center, _BiasStrength);
                        float2 diff = neighbor + rndPt - fuv;
                        float dist = length(diff);

                        if (dist < minDist)
                        {
                            secondMinDist = minDist;
                            minDist = dist;
                            minVec = diff;
                        }
                        else if (dist < secondMinDist)
                        {
                            secondMinDist = dist;
                        }
                    }
                }

                float crack = secondMinDist - minDist;
                float edge = smoothstep(_CrackWidth, 0.0, crack);

                float2 screenUV = IN.screenPos.xy / IN.screenPos.w;
                #if UNITY_UV_STARTS_AT_TOP
                screenUV.y = 1.0 - screenUV.y;
                #endif

                float2 refractUV = screenUV + minVec * _DistortStrength * (1-edge);

                float4 sceneCol = SAMPLE_TEXTURE2D(_CameraOpaqueTexture, sampler_CameraOpaqueTexture, refractUV);

                // グレースケール化
                float gray = dot(sceneCol.rgb, float3(0.299, 0.587, 0.114));
                sceneCol.rgb = gray.xxx;

                float alpha = lerp(_CrackAlpha, _GlassAlpha, edge);

                return float4(sceneCol.rgb, alpha);
            }
            ENDHLSL
        }
    }
}
    