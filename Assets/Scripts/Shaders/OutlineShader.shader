Shader "Custom/OutlineShader"
{
// The properties block of the Unity shader. In this example this block is empty
    // because the output color is predefined in the fragment shader code.
    Properties
    { 
        _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        _OutlineThickness("Outline Thickness", Float) = 0.1
    }

    // The SubShader block containing the Shader code.
    SubShader
    {
        Tags { 
            "RenderType" = "Opaque"
            "RenderingPipeline" = "UniversalPipeline"
        }
        
        Pass
        {
            // We don't need color, throw it away
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

            #pragma vertex vert;
            #pragma fragment frag;

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            

            struct Attributes
            {
                float4 positionOS   : POSITION;                 
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
            };

            // To make the Unity shader SRP Batcher compatible, declare all
            // properties related to a Material in a a single CBUFFER block with 
            // the name UnityPerMaterial.
            CBUFFER_START(UnityPerMaterial)
                // The following line declares the _BaseColor variable, so that you
                // can use it in the fragment shader.
                half4 _BaseColor;            
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);
                return OUT;
            }

            half4 frag() : SV_Target
            {
                // Returning the _BaseColor value.                
                return half4(1, 1, 0, 1);
            }
            ENDHLSL
        }

        Pass
        {
            Cull Off
            ZWrite On
            Stencil
            {
                Ref 2
                Comp NotEqual
                Pass Keep
                Fail Keep
            }
            ZTest Always
            
            HLSLPROGRAM
            #pragma vertex vert;
            #pragma fragment frag;

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            

            struct Attributes
            {
                float4 positionOS   : POSITION;
                // Declaring the variable containing the normal vector for each vertex.
                half3 normal        : NORMAL;                
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
            };

            // To make the Unity shader SRP Batcher compatible, declare all
            // properties related to a Material in a a single CBUFFER block with 
            // the name UnityPerMaterial.
            CBUFFER_START(UnityPerMaterial)
                // The following line declares the _BaseColor variable, so that you
                // can use it in the fragment shader.
                half4 _OutlineColor;
                float _OutlineThickness;            
            CBUFFER_END

            Varyings vert(Attributes IN)
            {
                Varyings OUT;
                float3 newVertexPos = IN.positionOS.xyz + IN.normal * _OutlineThickness;
                OUT.positionHCS = TransformObjectToHClip(newVertexPos);
                return OUT;
            }

            half4 frag() : SV_Target
            {
                // Returning the _BaseColor value.                
                return _OutlineColor;
            }
            ENDHLSL
        }
    }
}
