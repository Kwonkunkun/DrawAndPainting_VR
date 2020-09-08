// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Simple alpha blending overlay shader.
// Small for mobile usage.
Shader "Beffio/LightShaftOverlayEffectShader"
{
	Properties
	{
		_MainTex ("Texture", 2D) = "white" {}
		_OverlayTex ("Texture", 2D) = "white" {}
		_Color("Color", Color) = (0,0,0,1)
	}
	
	CGINCLUDE
		// Proprocessor directives creates separate shader per key
		#pragma multi_compile ALPHABLEND ADDITIVE
		#include "UnityCG.cginc"
		
		//************
		// PROPERTIES
		//************
		sampler2D _MainTex;
		float4 _MainTex_TexelSize;
		sampler2D _OverlayTex;
		uniform float _highlight;
		
		//*********
		// STRUCTS
		//*********
		struct basicVData 
		{
			float4 pos : SV_POSITION;
			float2 uv : TEXCOORD0;
		};
		
		//*********************
		// BASIC VERTEX SHADER
		//*********************
		basicVData vert(appdata_img v ) 
		{
			// Pass through basic data
			basicVData o = (basicVData)0;
			o.pos = UnityObjectToClipPos(v.vertex);
			o.uv = v.texcoord.xy;

			return o;
		} 
		
		//*************************************
		// ALPHA BLEND OVERLAY FRAGMENT SHADER
		//*************************************
		half4 fragOverlay(basicVData i) : SV_Target 
		{
   			// Get source image color and texture colorize color
			half4 background = tex2D (_MainTex, i.uv);


			// On non-GL when AA is used, the main texture and scene depth texture
			// will come out in different vertical orientations.
			// So flip sampling of the texture when that is the case (main texture
			// texel size will have negative Y).

#if UNITY_UV_STARTS_AT_TOP
			if (_MainTex_TexelSize.y < 0)
				i.uv.y = 1 - i.uv.y;
#endif
			half4 overlay = tex2D (_OverlayTex, i.uv);

			// Calculate background brightness
			half backBrightness = max(background.r, max(background.g, background.b));
			// Add brightness with highlight to light shaft area (power2 for subtler effect)
			background += backBrightness * _highlight * overlay.a * overlay.a * 2;

			// Return color based on blending mode
			#if ALPHABLEND
				// Blend with the alpha channel
				half4 output = half4(lerp(background.rgb, overlay.rgb, overlay.a), background.a);
			#else
				// Additive
				half4 output = half4(background.rgb + overlay.rgb * overlay.a, background.a);
			#endif

			return saturate(output);
		}		
	ENDCG
	
	
	SubShader
	{
		// No culling or depth
		Cull Off 
		ZWrite Off 
		ZTest Always
		
		// 0 - Alpha blended overlay
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragOverlay
			ENDCG
		}
	}
}
