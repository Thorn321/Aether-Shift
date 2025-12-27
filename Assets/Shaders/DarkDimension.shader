Shader "Custom/DarkDimensionInvert"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _DarkStrength ("Dark Strength", Range(0,1)) = 0.6
    }

    SubShader
    {
        Tags { "RenderPipeline"="UniversalPipeline" }
        Pass
        {
            Name "DarkInvertPass"

            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS : POSITION;
                float2 uv : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS : SV_POSITION;
                float2 uv : TEXCOORD0;
            };

            sampler2D _MainTex;
            float _DarkStrength;

            Varyings vert (Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                OUT.uv = IN.uv;
                return OUT;
            }

            half4 frag (Varyings IN) : SV_Target
            {
                half4 col = tex2D(_MainTex, IN.uv);

                // Invert barvy
                col.rgb = 1.0 - col.rgb;

                // Ztmavení
                col.rgb *= (1.0 - _DarkStrength);

                return col;
            }
            ENDHLSL
        }
    }
}