Shader "Paint in 3D/P3D Alpha"
{
	Properties
	{
		[NoScaleOffset]_MainTex("Albedo (RGB) Alpha (A)", 2D) = "white" {}
		[NoScaleOffset][Normal]_BumpMap("Normal (RGBA)", 2D) = "bump" {}
		[NoScaleOffset]_MetallicGlossMap("Metallic (R) Occlusion (G) Smoothness (B)", 2D) = "white" {}

		_Color("Color", Color) = (1,1,1,1)
		_BumpScale("Normal Map Strength", Range(0,5)) = 1
		_Metallic("Metallic", Range(0,1)) = 0
		_GlossMapScale("Smoothness", Range(0,1)) = 1
		_Tiling("Tiling", Float) = 1.0

		[NoScaleOffset]_AlbedoTex("Secondary Albedo (RGB~A) Premultiplied", 2D) = "black" {}
		[NoScaleOffset]_OpacityTex("Secondary Opacity (R~A) Premultiplied", 2D) = "black" {}
		[NoScaleOffset]_NormalTex("Secondary Normal (RG~A) Premultiplied", 2D) = "black" {}
		[NoScaleOffset]_MetallicTex("Secondary Metallic (R~A) Premultiplied", 2D) = "black" {}
		[NoScaleOffset]_OcclusionTex("Secondary Occlusion (R~A) Premultiplied", 2D) = "black" {}
		[NoScaleOffset]_SmoothnessTex("Secondary Smoothness (R~A) Premultiplied", 2D) = "black" {}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 400
		CGPROGRAM
			#pragma surface surf Standard fullforwardshadows vertex:vert alpha:blend
			#pragma target 3.0
			#include "UnityCG.cginc"
			#include "UnityPBSLighting.cginc"

			sampler2D _MainTex;
			sampler2D _BumpMap;
			sampler2D _MetallicGlossMap;

			sampler2D _AlbedoTex;
			sampler2D _OpacityTex;
			sampler2D _NormalTex;
			sampler2D _MetallicTex;
			sampler2D _OcclusionTex;
			sampler2D _SmoothnessTex;

			float4 _Color;
			float  _BumpScale;
			float  _Metallic;
			float  _GlossMapScale;
			float  _Tiling;

			struct Input
			{
				float2 first;
				float2 second;
			};

			void vert(inout appdata_full v, out Input o)
			{
				UNITY_INITIALIZE_OUTPUT(Input, o);
				
				o.first  = v.texcoord.xy * _Tiling;
				o.second = v.texcoord1.xy;
			}

			void surf(Input i, inout SurfaceOutputStandard o)
			{
				float4 texMain = tex2D(_MainTex, i.first);
				float4 gloss   = tex2D(_MetallicGlossMap, i.first);
				float4 bump    = tex2D(_BumpMap, i.first);

				o.Albedo     = texMain.rgb * _Color.rgb;
				o.Normal     = UnpackScaleNormal(bump, _BumpScale);
				o.Metallic   = gloss.r * _Metallic;
				o.Occlusion  = gloss.g;
				o.Smoothness = gloss.b * _GlossMapScale;
				o.Alpha      = texMain.a * _Color.a;

				// Override albedo with secondary?
				float4 albedo = tex2D(_AlbedoTex, i.second);
				o.Albedo = (1.0f - albedo.a) * o.Albedo + albedo.rgb;

				// Override opacity with secondary?
				float4 opacity = tex2D(_OpacityTex, i.second);
				o.Alpha = (1.0f - opacity.a) * o.Alpha + opacity.r;

				// Override normal with secondary?
				float4 normal = tex2D(_NormalTex, i.second);
				o.Normal = (1.0f - normal.a) * o.Normal + normal.r;

				// Override metallic with secondary?
				float4 metallic = tex2D(_MetallicTex, i.second);
				o.Metallic = (1.0f - metallic.a) * o.Metallic + metallic.r;

				// Override occlusion with secondary?
				float4 occlusion = tex2D(_OcclusionTex, i.second);
				o.Occlusion = (1.0f - occlusion.a) * o.Occlusion + occlusion.r;

				// Override smoothness with secondary?
				float4 smoothness = tex2D(_SmoothnessTex, i.second);
				o.Smoothness = (1.0f - smoothness.a) * o.Smoothness + smoothness.r;
			}
		ENDCG
	}
	FallBack "Standard"
}