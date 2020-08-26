// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Animmal/Sun"
{
	Properties
	{
		_ColorIn("Color In", Color) = (1,0.9310344,0,0)
		_ColorOut("Color Out", Color) = (1,0.9310344,0,0)
		_BaseSmoothness("Base Smoothness", Range( 0 , 1)) = 0
		_CoatAmount("Coat Amount", Float) = 0
		_Color0("Color 0", Color) = (1,0.3103448,0,0)
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGINCLUDE
		#include "UnityPBSLighting.cginc"
		#include "Lighting.cginc"
		#pragma target 3.0
		struct Input
		{
			float3 worldPos;
			float3 worldNormal;
			float4 vertexColor : COLOR;
		};

		struct SurfaceOutputCustomLightingCustom
		{
			fixed3 Albedo;
			fixed3 Normal;
			half3 Emission;
			half Metallic;
			half Smoothness;
			half Occlusion;
			fixed Alpha;
			Input SurfInput;
			UnityGIInput GIData;
		};

		uniform float _CoatAmount;
		uniform float4 _ColorIn;
		uniform float4 _ColorOut;
		uniform float _BaseSmoothness;
		uniform float4 _Color0;

		inline half4 LightingStandardCustomLighting( inout SurfaceOutputCustomLightingCustom s, half3 viewDir, UnityGI gi )
		{
			UnityGIInput data = s.GIData;
			Input i = s.SurfInput;
			half4 c = 0;
			SurfaceOutputStandard s1 = (SurfaceOutputStandard ) 0;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNDotV201 = dot( normalize( ase_worldNormal ), ase_worldViewDir );
			float fresnelNode201 = ( 0.01 + 1 * pow( 1.0 - fresnelNDotV201, 1 ) );
			float4 lerpResult278 = lerp( _ColorIn , _ColorOut , fresnelNode201);
			s1.Albedo = lerpResult278.rgb;
			s1.Normal = i.worldNormal;
			s1.Emission = float3( 0,0,0 );
			s1.Metallic = 0;
			s1.Smoothness = _BaseSmoothness;
			float fresnelNDotV279 = dot( normalize( ase_worldNormal ), ase_worldViewDir );
			float fresnelNode279 = ( 0.05 + 1 * pow( 1.0 - fresnelNDotV279, 5 ) );
			float lerpResult306 = lerp( i.vertexColor.r , 5.0 , 0);
			s1.Occlusion = ( saturate( ( 1.0 - fresnelNode279 ) ) * lerpResult306 );

			data.light = gi.light;

			UnityGI gi1 = gi;
			#ifdef UNITY_PASS_FORWARDBASE
			Unity_GlossyEnvironmentData g1 = UnityGlossyEnvironmentSetup( s1.Smoothness, data.worldViewDir, s1.Normal, float3(0,0,0));
			gi1 = UnityGlobalIllumination( data, s1.Occlusion, s1.Normal, g1 );
			#endif

			float3 surfResult1 = LightingStandard ( s1, viewDir, gi1 ).rgb;
			surfResult1 += s1.Emission;

			SurfaceOutputStandardSpecular s166 = (SurfaceOutputStandardSpecular ) 0;
			s166.Albedo = float3( 0,0,0 );
			s166.Normal = i.worldNormal;
			s166.Emission = _Color0.rgb;
			float3 temp_cast_2 = (1.2).xxx;
			s166.Specular = temp_cast_2;
			s166.Smoothness = 0;
			s166.Occlusion = 1;

			data.light = gi.light;

			UnityGI gi166 = gi;
			#ifdef UNITY_PASS_FORWARDBASE
			Unity_GlossyEnvironmentData g166 = UnityGlossyEnvironmentSetup( s166.Smoothness, data.worldViewDir, s166.Normal, float3(0,0,0));
			gi166 = UnityGlobalIllumination( data, s166.Occlusion, s166.Normal, g166 );
			#endif

			float3 surfResult166 = LightingStandardSpecular ( s166, viewDir, gi166 ).rgb;
			surfResult166 += s166.Emission;

			float3 lerpResult208 = lerp( surfResult1 , surfResult166 , ( ( fresnelNode279 * _CoatAmount ) * lerpResult306 ));
			c.rgb = lerpResult208;
			c.a = 1;
			return c;
		}

		inline void LightingStandardCustomLighting_GI( inout SurfaceOutputCustomLightingCustom s, UnityGIInput data, inout UnityGI gi )
		{
			s.GIData = data;
		}

		void surf( Input i , inout SurfaceOutputCustomLightingCustom o )
		{
			o.SurfInput = i;
			float3 ase_worldPos = i.worldPos;
			float3 ase_worldViewDir = normalize( UnityWorldSpaceViewDir( ase_worldPos ) );
			float3 ase_worldNormal = i.worldNormal;
			float fresnelNDotV279 = dot( normalize( ase_worldNormal ), ase_worldViewDir );
			float fresnelNode279 = ( 0.05 + 1 * pow( 1.0 - fresnelNDotV279, 5 ) );
			float lerpResult306 = lerp( i.vertexColor.r , 5.0 , 0);
			float3 lerpResult208 = lerp( float3(0,0,0) , float3(0,0,0) , ( ( fresnelNode279 * _CoatAmount ) * lerpResult306 ));
			o.Emission = lerpResult208;
		}

		ENDCG
		CGPROGRAM
		#pragma surface surf StandardCustomLighting keepalpha fullforwardshadows 

		ENDCG
		Pass
		{
			Name "ShadowCaster"
			Tags{ "LightMode" = "ShadowCaster" }
			ZWrite On
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0
			#pragma multi_compile_shadowcaster
			#pragma multi_compile UNITY_PASS_SHADOWCASTER
			#pragma skip_variants FOG_LINEAR FOG_EXP FOG_EXP2
			#include "HLSLSupport.cginc"
			#if ( SHADER_API_D3D11 || SHADER_API_GLCORE || SHADER_API_GLES3 || SHADER_API_METAL || SHADER_API_VULKAN )
				#define CAN_SKIP_VPOS
			#endif
			#include "UnityCG.cginc"
			#include "Lighting.cginc"
			#include "UnityPBSLighting.cginc"
			struct v2f
			{
				V2F_SHADOW_CASTER;
				float3 worldPos : TEXCOORD1;
				float3 worldNormal : TEXCOORD2;
				fixed4 color : COLOR0;
				UNITY_VERTEX_INPUT_INSTANCE_ID
			};
			v2f vert( appdata_full v )
			{
				v2f o;
				UNITY_SETUP_INSTANCE_ID( v );
				UNITY_INITIALIZE_OUTPUT( v2f, o );
				UNITY_TRANSFER_INSTANCE_ID( v, o );
				float3 worldPos = mul( unity_ObjectToWorld, v.vertex ).xyz;
				fixed3 worldNormal = UnityObjectToWorldNormal( v.normal );
				o.worldNormal = worldNormal;
				o.worldPos = worldPos;
				TRANSFER_SHADOW_CASTER_NORMALOFFSET( o )
				o.color = v.color;
				return o;
			}
			fixed4 frag( v2f IN
			#if !defined( CAN_SKIP_VPOS )
			, UNITY_VPOS_TYPE vpos : VPOS
			#endif
			) : SV_Target
			{
				UNITY_SETUP_INSTANCE_ID( IN );
				Input surfIN;
				UNITY_INITIALIZE_OUTPUT( Input, surfIN );
				float3 worldPos = IN.worldPos;
				fixed3 worldViewDir = normalize( UnityWorldSpaceViewDir( worldPos ) );
				surfIN.worldPos = worldPos;
				surfIN.worldNormal = IN.worldNormal;
				surfIN.vertexColor = IN.color;
				SurfaceOutputCustomLightingCustom o;
				UNITY_INITIALIZE_OUTPUT( SurfaceOutputCustomLightingCustom, o )
				surf( surfIN, o );
				#if defined( CAN_SKIP_VPOS )
				float2 vpos = IN.pos;
				#endif
				SHADOW_CASTER_FRAGMENT( IN )
			}
			ENDCG
		}
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14501
293;259;1471;639;156.0211;-309.5863;2.302687;True;True
Node;AmplifyShaderEditor.FresnelNode;279;433.3885,1433.73;Float;True;World;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.05;False;2;FLOAT;1;False;3;FLOAT;5;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;307;746.2328,1783.859;Float;False;Constant;_Float0;Float 0;17;0;Create;True;0;5;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.VertexColorNode;301;741.2299,1615.661;Float;False;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.OneMinusNode;296;741.5596,1432.913;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;384.358,1687.471;Float;False;Property;_CoatAmount;Coat Amount;3;0;Create;True;0;0;7.76;1;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.CommentaryNode;292;525.7,489.5775;Float;False;348.1;312.8;Comment;1;201;Dual Toning Factor;1,1,1,1;0;0
Node;AmplifyShaderEditor.LerpOp;306;1006.333,1778.962;Float;False;3;0;FLOAT;0;False;1;FLOAT;0;False;2;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;271;584.1458,219.167;Float;False;Property;_ColorOut;Color Out;1;0;Create;True;0;1,0.9310344,0,0;1,0.8557006,0.2426471,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ColorNode;4;586.7281,40.94933;Float;False;Property;_ColorIn;Color In;0;0;Create;True;0;1,0.9310344,0,0;1,0.9103637,0.4926471,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.FresnelNode;201;556.7,545.277;Float;True;World;4;0;FLOAT3;0,0,0;False;1;FLOAT;0.01;False;2;FLOAT;1;False;3;FLOAT;1;False;1;FLOAT;0
Node;AmplifyShaderEditor.SaturateNode;298;979.6241,1437.678;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;237;754.8387,1519.356;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;299;1231.222,1446.256;Float;False;2;2;0;FLOAT;1;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;308;1243.331,1586.266;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;211;1709.519,1302.074;Float;False;Constant;_Spec;Spec;18;0;Create;True;0;1.2;0;0;0;0;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;278;1326.438,297.2802;Float;False;3;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.RangedFloatNode;233;1161.823,545.5279;Float;False;Property;_BaseSmoothness;Base Smoothness;2;0;Create;True;0;0;0.5412941;0;1;0;1;FLOAT;0
Node;AmplifyShaderEditor.ColorNode;320;1654.466,1096.44;Float;False;Property;_Color0;Color 0;4;0;Create;True;0;1,0.3103448,0,0;1,0.3103448,0,0;0;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CustomStandardSurface;166;1965.31,1169.65;Float;False;Specular;Tangent;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,1;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.CustomStandardSurface;1;1584.245,315.3055;Float;False;Metallic;Tangent;6;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,1;False;2;FLOAT3;0,0,0;False;3;FLOAT;0;False;4;FLOAT;0;False;5;FLOAT;1;False;1;FLOAT3;0
Node;AmplifyShaderEditor.WireNode;319;1992.36,1548.037;Float;False;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.LerpOp;208;2395.602,1132.678;Float;False;3;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT;0;False;1;FLOAT3;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;2722.613,1084.461;Float;False;True;2;Float;ASEMaterialInspector;0;0;CustomLighting;Animmal/Sun;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;0;4;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;Add;Add;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;15;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;296;0;279;0
WireConnection;306;0;301;1
WireConnection;306;1;307;0
WireConnection;298;0;296;0
WireConnection;237;0;279;0
WireConnection;237;1;47;0
WireConnection;299;0;298;0
WireConnection;299;1;306;0
WireConnection;308;0;237;0
WireConnection;308;1;306;0
WireConnection;278;0;4;0
WireConnection;278;1;271;0
WireConnection;278;2;201;0
WireConnection;166;2;320;0
WireConnection;166;3;211;0
WireConnection;1;0;278;0
WireConnection;1;4;233;0
WireConnection;1;5;299;0
WireConnection;319;0;308;0
WireConnection;208;0;1;0
WireConnection;208;1;166;0
WireConnection;208;2;319;0
WireConnection;0;2;208;0
WireConnection;0;13;208;0
ASEEND*/
//CHKSM=0DCB1394C93390DD1927CE85CA3A001D6FA926FE