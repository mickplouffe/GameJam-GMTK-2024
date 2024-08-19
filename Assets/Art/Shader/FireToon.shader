// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "EdShaders/FireToon"
{
	Properties
	{
		_Cutoff( "Mask Clip Value", Float ) = 0.5
		_FireTexture("Fire Texture", 2D) = "white" {}
		_OpacityCutoff("Opacity Cutoff", Range( 0 , 1)) = 0.5
		_InnerOuterStep("InnerOuterStep", Range( 0 , 1)) = 0.5
		_ColorInner("Color Inner", Color) = (0.9044118,0.8710238,0.7315095,0)
		_ColorOuterTop("Color Outer Top", Color) = (0.9779412,0.8154799,0.1366241,0)
		_ColorOuter("Color Outer", Color) = (0.9779412,0.8154799,0.1366241,0)
		_TopBottom("Top Bottom", Range( 0 , 1)) = 1
		_Brightness("Brightness", Float) = 1
		_ShadingStrength("Shading Strength", Range( 0 , 1)) = 0.5
		_NoisePrimary("Noise Primary", 2D) = "white" {}
		_Noise2Scale("Noise 2 Scale", Float) = 0
		_Noise1Scale("Noise 1 Scale", Float) = 0
		_Noise1Speed("Noise 1 Speed", Float) = 0
		_Noise2Speed("Noise 2 Speed", Float) = 0
		_CorePower("Core Power", Range( 0 , 8)) = 1
		_CoreStrength("Core Strength", Range( 0 , 5)) = 1
		_PushForward("Push Forward", Float) = 0
		_DepthFade("Depth Fade", Float) = 1
		_PremultiplyBlend("Premultiply Blend", Range( 0 , 1)) = 1
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Custom"  "Queue" = "Transparent+0" "IsEmissive" = "true"  }
		Cull Back
		Blend One OneMinusSrcAlpha
		
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#include "UnityCG.cginc"
		#pragma target 3.0
		#pragma surface surf Unlit keepalpha noshadow novertexlights nolightmap  nodynlightmap nodirlightmap vertex:vertexDataFunc 
		struct Input
		{
			float2 uv_texcoord;
			float4 screenPos;
		};

		uniform float _PushForward;
		uniform float4 _ColorOuterTop;
		uniform float _TopBottom;
		uniform float4 _ColorOuter;
		uniform float _InnerOuterStep;
		uniform sampler2D _NoisePrimary;
		uniform float _Noise1Speed;
		uniform float _Noise1Scale;
		uniform float4 _NoisePrimary_ST;
		uniform float _Noise2Speed;
		uniform float _Noise2Scale;
		uniform float _CoreStrength;
		uniform sampler2D _FireTexture;
		uniform float4 _FireTexture_ST;
		uniform float _CorePower;
		uniform sampler2D _CameraDepthTexture;
		uniform float _DepthFade;
		uniform float4 _ColorInner;
		uniform float _Brightness;
		uniform float _ShadingStrength;
		uniform float _PremultiplyBlend;
		uniform float _OpacityCutoff;
		uniform float _Cutoff = 0.5;

		void vertexDataFunc( inout appdata_full v, out Input o )
		{
			UNITY_INITIALIZE_OUTPUT( Input, o );
			//Calculate new billboard vertex position and normal;
			float3 upCamVec = normalize ( UNITY_MATRIX_V._m10_m11_m12 );
			float3 forwardCamVec = -normalize ( UNITY_MATRIX_V._m20_m21_m22 );
			float3 rightCamVec = normalize( UNITY_MATRIX_V._m00_m01_m02 );
			float4x4 rotationCamMatrix = float4x4( rightCamVec, 0, upCamVec, 0, forwardCamVec, 0, 0, 0, 0, 1 );
			v.normal = normalize( mul( float4( v.normal , 0 ), rotationCamMatrix ));
			//This unfortunately must be made to take non-uniform scaling into account;
			//Transform to world coords, apply rotation and transform back to local;
			v.vertex = mul( v.vertex , unity_ObjectToWorld );
			v.vertex = mul( v.vertex , rotationCamMatrix );
			v.vertex = mul( v.vertex , unity_WorldToObject );
			float4 transform173 = mul(unity_ObjectToWorld,float4( 0,0,0,1 ));
			float4 normalizeResult156 = normalize( ( float4( _WorldSpaceCameraPos , 0.0 ) - transform173 ) );
			float3 ase_vertex3Pos = v.vertex.xyz;
			v.vertex.xyz += ( ( _PushForward * normalizeResult156 ) + float4( ( 0.1 * ( 0 + ase_vertex3Pos ) ) , 0.0 ) ).xyz;
		}

		inline half4 LightingUnlit( SurfaceOutput s, half3 lightDir, half atten )
		{
			return half4 ( 0, 0, 0, s.Alpha );
		}

		void surf( Input i , inout SurfaceOutput o )
		{
			float smoothstepResult207 = smoothstep( 0.0 , _TopBottom , i.uv_texcoord.y);
			float temp_output_208_0 = saturate( smoothstepResult207 );
			float2 appendResult89 = (float2(0.1 , _Noise1Speed));
			float2 uv_NoisePrimary = i.uv_texcoord * _NoisePrimary_ST.xy + _NoisePrimary_ST.zw;
			float2 panner4 = ( 1.0 * _Time.y * appendResult89 + ( _Noise1Scale * uv_NoisePrimary ));
			float2 appendResult87 = (float2(-0.1 , _Noise2Speed));
			float2 panner69 = ( 1.0 * _Time.y * appendResult87 + ( uv_NoisePrimary * _Noise2Scale ));
			float2 uv_FireTexture = i.uv_texcoord * _FireTexture_ST.xy + _FireTexture_ST.zw;
			float temp_output_94_0 = saturate( ( _CoreStrength * pow( tex2D( _FireTexture, uv_FireTexture ).r , _CorePower ) ) );
			float4 ase_screenPos = float4( i.screenPos.xyz , i.screenPos.w + 0.00000000001 );
			float4 ase_screenPosNorm = ase_screenPos / ase_screenPos.w;
			ase_screenPosNorm.z = ( UNITY_NEAR_CLIP_VALUE >= 0 ) ? ase_screenPosNorm.z : ase_screenPosNorm.z * 0.5 + 0.5;
			float screenDepth108 = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture,UNITY_PROJ_COORD( ase_screenPos ))));
			float distanceDepth108 = abs( ( screenDepth108 - LinearEyeDepth( ase_screenPosNorm.z ) ) / ( _DepthFade ) );
			float FireSoft78 = ( ( ( ( tex2D( _NoisePrimary, panner4 ).r * ( ( 1.0 - i.uv_texcoord.y ) + tex2D( _NoisePrimary, panner69 ).r ) ) + temp_output_94_0 ) * temp_output_94_0 ) * saturate( distanceDepth108 ) );
			float temp_output_15_0 = step( _InnerOuterStep , FireSoft78 );
			o.Emission = ( ( ( ( ( _ColorOuterTop * temp_output_208_0 ) + ( _ColorOuter * ( 1.0 - temp_output_208_0 ) ) ) * ( 1.0 - temp_output_15_0 ) ) + ( _ColorInner * temp_output_15_0 ) ) * ( _Brightness * saturate( ( ( 1.0 - _ShadingStrength ) + FireSoft78 ) ) ) ).rgb;
			o.Alpha = saturate( ( _PremultiplyBlend + FireSoft78 ) );
			clip( step( _OpacityCutoff , FireSoft78 ) - _Cutoff );
		}

		ENDCG
	}
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=15600
488;645;1504;918;1762.947;358.9236;1.99995;True;False
Node;AmplifyShaderEditor.CommentaryNode;151;-3598.344,-209.5613;Float;False;2071.021;1015.128;;18;85;5;88;87;70;41;49;90;84;69;83;68;89;47;4;73;2;72;Animated Noise;1,1,1,1;0;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;5;-3483.287,124.5369;Float;False;0;49;2;3;2;SAMPLER2D;;False;0;FLOAT2;1.5,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;88;-3145.103,610.7612;Float;False;Property;_Noise2Speed;Noise 2 Speed;14;0;Create;True;0;0;False;0;0;-1.27;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;85;-3245.031,482.0056;Float;False;Property;_Noise2Scale;Noise 2 Scale;11;0;Create;True;0;0;False;0;0;0.76;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;84;-3296.631,22.72701;Float;False;Property;_Noise1Scale;Noise 1 Scale;12;0;Create;True;0;0;False;0;0;1.22;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;181;-1468.408,213.5996;Float;False;1178.76;789.5344;;9;98;1;192;95;191;94;96;97;99;Flame Tapered Shaping;1,1,1,1;0;0
Node;AmplifyShaderEditor.RangedFloatNode;90;-3332.888,-125.235;Float;False;Property;_Noise1Speed;Noise 1 Speed;13;0;Create;True;0;0;False;0;0;-1;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.DynamicAppendNode;87;-2939.145,576.4349;Float;False;FLOAT2;4;0;FLOAT;-0.1;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;70;-2874.046,388.596;Float;False;2;2;0;FLOAT2;0,0;False;1;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.TexturePropertyNode;49;-2940.978,97.54047;Float;True;Property;_NoisePrimary;Noise Primary;10;0;Create;True;0;0;False;0;None;c7398e7e53515af44b11b2cdca4d44d2;False;white;Auto;Texture2D;0;1;SAMPLER2D;0
Node;AmplifyShaderEditor.RangedFloatNode;99;-1418.408,475.4309;Float;False;Property;_CorePower;Core Power;15;0;Create;True;0;0;False;0;1;1.54;0;8;0;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;41;-2791.53,654.7308;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;83;-3084.394,-1.451848;Float;False;2;2;0;FLOAT;0;False;1;FLOAT2;0,0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PannerNode;69;-2688.931,425.3902;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;0.1,-1.27;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.DynamicAppendNode;89;-3126.93,-159.5613;Float;False;FLOAT2;4;0;FLOAT;0.1;False;1;FLOAT;0;False;2;FLOAT;0;False;3;FLOAT;0;False;1;FLOAT2;0
Node;AmplifyShaderEditor.SamplerNode;1;-1399.862,630.4403;Float;True;Property;_FireTexture;Fire Texture;1;0;Create;True;0;0;False;0;None;d852f80200c0a90418d02507f8df20bb;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;68;-2464.398,321.3872;Float;True;Property;_TextureSample0;Texture Sample 0;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;47;-2486.735,684.017;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;97;-1357.027,367.6212;Float;False;Property;_CoreStrength;Core Strength;16;0;Create;True;0;0;False;0;1;1.13;0;5;0;1;FLOAT;0
Node;AmplifyShaderEditor.PannerNode;4;-2803.821,-68.52036;Float;False;3;0;FLOAT2;0,0;False;2;FLOAT2;-0.1,-1;False;1;FLOAT;1;False;1;FLOAT2;0
Node;AmplifyShaderEditor.PowerNode;98;-1082.53,523.3502;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;152;-1359.462,1133.579;Float;False;619.8999;191.1072;;3;109;108;110;Depth Fade;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;96;-999.8955,369.5725;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;-2111.089,339.4333;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;2;-2438.902,-39.8803;Float;True;Property;_NoiseTexture;Noise Texture;2;0;Create;True;0;0;False;0;None;None;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;109;-1309.462,1209.687;Float;False;Property;_DepthFade;Depth Fade;18;0;Create;True;0;0;False;0;1;0.16;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;143;-1238.19,-1254.248;Float;False;1703.119;898.7413;;18;185;184;183;186;206;13;200;201;15;132;24;100;199;123;120;203;207;208;Fire Colour;1,1,1,1;0;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;72;-2000.493,154.1694;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;94;-813.5461,395.737;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;203;-1189.844,-649.3407;Float;False;Property;_TopBottom;Top Bottom;7;0;Create;True;0;0;False;0;1;0.535;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;191;-741.0963,279.5883;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.DepthFade;108;-1132.748,1206.184;Float;False;True;1;0;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.TextureCoordinatesNode;120;-1083.623,-823.9695;Float;False;0;-1;2;3;2;SAMPLER2D;;False;0;FLOAT2;1,1;False;1;FLOAT2;0,0;False;5;FLOAT2;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;95;-636.7469,380.3757;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;110;-914.561,1183.579;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SmoothstepOpNode;207;-842.4242,-738.6846;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;192;-423.5411,381.1195;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;147;-596.3651,-57.39346;Float;False;847.5149;210.4687;;4;105;104;102;103;Shading;1,1,1,1;0;0
Node;AmplifyShaderEditor.SaturateNode;208;-644.9465,-712.9776;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;199;-936.9932,-1200.756;Float;False;Property;_ColorOuterTop;Color Outer Top;5;0;Create;True;0;0;False;0;0.9779412,0.8154799,0.1366241,0;0.2924528,0.06156901,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.RangedFloatNode;24;-381.4496,-539.6417;Float;False;Property;_InnerOuterStep;InnerOuterStep;3;0;Create;True;0;0;False;0;0.5;0.661;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;150;-202.3192,792.0416;Float;False;794.3567;460.7173;;5;156;154;165;173;117;Push Forward;1,1,1,1;0;0
Node;AmplifyShaderEditor.GetLocalVarNode;100;-349.9102,-446.0075;Float;False;78;FireSoft;0;1;FLOAT;0
Node;AmplifyShaderEditor.RegisterLocalVarNode;78;-215.2192,408.4617;Float;False;FireSoft;-1;True;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.OneMinusNode;132;-654.3342,-852.0253;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;102;-546.3651,-7.393465;Float;False;Property;_ShadingStrength;Shading Strength;9;0;Create;True;0;0;False;0;0.5;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;123;-950.4648,-997.7971;Float;False;Property;_ColorOuter;Color Outer;6;0;Create;True;0;0;False;0;0.9779412,0.8154799,0.1366241,0;0.8773585,0.543499,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.WorldSpaceCameraPos;165;-161.7349,936.4604;Float;False;0;4;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3
Node;AmplifyShaderEditor.CommentaryNode;182;-106.8259,1371.149;Float;False;664.8013;320.0908;;5;176;174;175;180;179;Billboard (camera facing);1,1,1,1;0;0
Node;AmplifyShaderEditor.ObjectToWorldTransfNode;173;-158.0469,1080.181;Float;False;1;0;FLOAT4;0,0,0,1;False;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;105;-262.373,22.3563;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;200;-380.5216,-1142.554;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;15;-82.27489,-532.1779;Float;False;2;0;FLOAT;0.5;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;201;-386.6911,-898.4379;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleSubtractOpNode;154;135.5719,984.1703;Float;False;2;0;FLOAT3;0,0,0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.PosVertexDataNode;176;-66.32135,1519.179;Float;False;0;0;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;103;-69.62875,20.07519;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.BillboardNode;174;-70.70294,1429.771;Float;False;Spherical;False;0;1;FLOAT3;0
Node;AmplifyShaderEditor.ColorNode;13;-385.8916,-741.341;Float;False;Property;_ColorInner;Color Inner;4;0;Create;True;0;0;False;0;0.9044118,0.8710238,0.7315095,0;1,0.8455111,0.3160377,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;206;-33.61771,-944.4053;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.OneMinusNode;186;-85.6693,-819.217;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;117;-152.3192,842.0416;Float;False;Property;_PushForward;Push Forward;17;0;Create;True;0;0;False;0;0;0.23;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;175;207.6569,1434.248;Float;False;2;2;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.RangedFloatNode;180;225.1924,1539.455;Float;False;Constant;_Float0;Float 0;20;0;Create;True;0;0;False;0;0.1;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.NormalizeNode;156;293.4073,970.0475;Float;False;1;0;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.RangedFloatNode;18;267.4661,-252.3786;Float;False;Property;_Brightness;Brightness;8;0;Create;True;0;0;False;0;1;2.65;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;184;104.6017,-726.9062;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;183;115.9049,-913.41;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SaturateNode;104;76.14963,21.0927;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;209;13.00834,231.0616;Float;False;Property;_PremultiplyBlend;Premultiply Blend;19;0;Create;True;0;0;False;0;1;1;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;185;274.1502,-830.52;Float;False;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;80;14.55021,558.0551;Float;False;Property;_OpacityCutoff;Opacity Cutoff;2;0;Create;True;0;0;False;0;0.5;0.183;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;146;434.5428,-189.3274;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;119;473.5952,858.9095;Float;False;2;2;0;FLOAT;0;False;1;FLOAT4;0,0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleAddOpNode;210;358.9995,283.0603;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;179;414.4167,1421.149;Float;False;2;2;0;FLOAT;0;False;1;FLOAT3;0,0,0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.SimpleAddOpNode;178;741.7178,1063.822;Float;False;2;2;0;FLOAT4;0,0,0,0;False;1;FLOAT3;0,0,0;False;1;FLOAT4;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;144;738.7364,-318.7038;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.StepOpNode;79;329.3688,458.0639;Float;False;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;211;474.9968,169.0631;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1065.32,-306.5498;Float;False;True;2;Float;ASEMaterialInspector;0;0;Unlit;EdShaders/FireToon;False;False;False;False;False;True;True;True;True;False;False;False;False;False;False;False;False;False;False;False;Back;0;False;-1;0;False;-1;False;0;False;-1;0;False;-1;False;0;Custom;0.5;True;False;0;True;Custom;;Transparent;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;0;False;-1;False;0;False;-1;255;False;-1;255;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;-1;False;2;15;10;25;False;0.5;False;3;1;False;-1;10;False;-1;0;0;False;-1;0;False;-1;0;False;-1;0;False;-1;0;False;0;0,0,0,0;VertexOffset;True;False;Spherical;True;Relative;0;;0;-1;-1;-1;0;False;0;0;False;-1;-1;0;False;-1;0;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;87;1;88;0
WireConnection;70;0;5;0
WireConnection;70;1;85;0
WireConnection;83;0;84;0
WireConnection;83;1;5;0
WireConnection;69;0;70;0
WireConnection;69;2;87;0
WireConnection;89;1;90;0
WireConnection;68;0;49;0
WireConnection;68;1;69;0
WireConnection;47;0;41;2
WireConnection;4;0;83;0
WireConnection;4;2;89;0
WireConnection;98;0;1;1
WireConnection;98;1;99;0
WireConnection;96;0;97;0
WireConnection;96;1;98;0
WireConnection;73;0;47;0
WireConnection;73;1;68;1
WireConnection;2;0;49;0
WireConnection;2;1;4;0
WireConnection;72;0;2;1
WireConnection;72;1;73;0
WireConnection;94;0;96;0
WireConnection;191;0;72;0
WireConnection;191;1;94;0
WireConnection;108;0;109;0
WireConnection;95;0;191;0
WireConnection;95;1;94;0
WireConnection;110;0;108;0
WireConnection;207;0;120;2
WireConnection;207;2;203;0
WireConnection;192;0;95;0
WireConnection;192;1;110;0
WireConnection;208;0;207;0
WireConnection;78;0;192;0
WireConnection;132;0;208;0
WireConnection;105;0;102;0
WireConnection;200;0;199;0
WireConnection;200;1;208;0
WireConnection;15;0;24;0
WireConnection;15;1;100;0
WireConnection;201;0;123;0
WireConnection;201;1;132;0
WireConnection;154;0;165;0
WireConnection;154;1;173;0
WireConnection;103;0;105;0
WireConnection;103;1;78;0
WireConnection;206;0;200;0
WireConnection;206;1;201;0
WireConnection;186;0;15;0
WireConnection;175;0;174;0
WireConnection;175;1;176;0
WireConnection;156;0;154;0
WireConnection;184;0;13;0
WireConnection;184;1;15;0
WireConnection;183;0;206;0
WireConnection;183;1;186;0
WireConnection;104;0;103;0
WireConnection;185;0;183;0
WireConnection;185;1;184;0
WireConnection;146;0;18;0
WireConnection;146;1;104;0
WireConnection;119;0;117;0
WireConnection;119;1;156;0
WireConnection;210;0;209;0
WireConnection;210;1;78;0
WireConnection;179;0;180;0
WireConnection;179;1;175;0
WireConnection;178;0;119;0
WireConnection;178;1;179;0
WireConnection;144;0;185;0
WireConnection;144;1;146;0
WireConnection;79;0;80;0
WireConnection;79;1;78;0
WireConnection;211;0;210;0
WireConnection;0;2;144;0
WireConnection;0;9;211;0
WireConnection;0;10;79;0
WireConnection;0;11;178;0
ASEEND*/
//CHKSM=6CCDD29C01AFBD0865C07D283CE251C7C8D9DE55