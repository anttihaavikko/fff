Shader "Hidden/Custom/ColorSplit"
{
    HLSLINCLUDE

        #include "Packages/com.unity.postprocessing/PostProcessing/Shaders/StdLib.hlsl"

        TEXTURE2D_SAMPLER2D(_MainTex, sampler_MainTex);
        float _Amount;

        float2 _RedOffset;
        float2 _GreenOffset;
        float2 _BlueOffset;

        float4 Frag(VaryingsDefault i) : SV_Target
        {
            float2 coords = i.texcoord;

            float2 redOffset = float2(coords.x - _Amount, coords.y);
            float2 greenOffset = float2(coords.x, coords.y);
            float2 blueOffset = float2(coords.x + _Amount, coords.y);
           
            //Red Channel
            float4 red = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, coords + _RedOffset * _Amount);
            //Green Channel
            float4 green = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, coords + _GreenOffset * _Amount);
            //Blue Channel
            float4 blue = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, coords + _BlueOffset * _Amount);
           
            float4 finalColor = float4(red.r, green.g, blue.b, 1.0f);
            return finalColor;
        }

    ENDHLSL

    SubShader
    {
        Cull Off ZWrite Off ZTest Always

        Pass
        {
            HLSLPROGRAM

                #pragma vertex VertDefault
                #pragma fragment Frag

            ENDHLSL
        }
    }
}