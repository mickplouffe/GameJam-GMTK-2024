// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Unlit/OutlineShader"
{
    Properties
    {
        [MainColor] _OutlineColor("Outline Color", Color) = (0,0,0,1)
        _OutlineWidth("Outline width", Range(0.0, 1.0)) = 0.05
    }
    SubShader
    {
        Tags { "RenderType" = "Outline" "Queue" = "Geometry"  }
        
        Pass
        {
            Name "Stencil Pass"
            
            ColorMask 0
            Cull Off
            ZWrite Off
            Stencil
            {
                Ref 2
                Comp Always
                Pass Replace
            }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
            };

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            // The fragment shader definition.
            half4 frag() : SV_Target
            {
                // Defining the color variable and returning it.
                return half4(1,0,0,0);
            }
            ENDHLSL
        }
        
        Pass
        {
            Name "Outline Pass"
            Tags { "LightMode" = "UniversalForward" }
            Cull Off
            ZWrite On
            Stencil
            {
                Ref 2
                Comp NotEqual
                Pass Keep
                Fail Keep
            }
            
            HLSLPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"

            struct Attributes
            {
                float4 positionOS   : POSITION;
                half3 normal        : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
            };


            CBUFFER_START(UnityPerMaterial)
                half4 _OutlineColor;
                float _OutlineWidth;
            CBUFFER_END
            

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz + IN.normal * _OutlineWidth);
                return OUT;
            }

            // The fragment shader definition.
            half4 frag() : SV_Target
            {
                // Defining the color variable and returning it.
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
    Fallback "Diffuse"
}
