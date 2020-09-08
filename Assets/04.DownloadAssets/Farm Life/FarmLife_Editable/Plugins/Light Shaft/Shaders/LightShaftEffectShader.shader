// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Light Shaft shader
// Can render light shaft,
// render light shaft only (for rendering a light shaft texture)
Shader "Beffio/LightShaftEffectShader"
{
	Properties
	{
		_MainTex("Texture", 2D) = "white" {}
	}

	CGINCLUDE
	#pragma target 3.0
	// Proprocessor directives creates separate shader per key combination
	#pragma multi_compile LINEAR RADIAL
	#pragma multi_compile ALPHABLEND ADDITIVE
	#include "UnityCG.cginc"

	//************
	// PROPERTIES
	//************
	// PI
	static half PI = 3.14159265;

	// Main texture
	sampler2D _MainTex;
	float4 _MainTex_TexelSize;

	// Gradient colors of the light shaft (rgb are colors, a is the position)
	half4 _Color[8];
	// Gradient alphas of the light shaft (x is alpha, y is position)
	half2 _Alpha[8];

	// Light shaft Parameters:
	// X: Distance
	// Y: Rotation
	// Z: Size
	// W: Sapect ratio
	uniform half4 _params;

	uniform float _highlight;

	//*********
	// STRUCTS
	//*********
	struct VData_basic
	{
		half4 pos : SV_POSITION;
		half2 uv : TEXCOORD0;
		half4 shaftData: TEXCOORD1; // xy: shaftUV, yz: shaftCenter
	};

	//**************************
	// LINEAR GRADIENT DISTANCE
	//**************************
	// Unified function for calculating the radial light shaft mask
	half CalculateLinearDistance(half4 shaftData)
	{
		// POSITION
		// Define gradient directions
		half2 gradientDirection = -normalize(shaftData.zw);

		// Calculate signed distance
		half dist = dot(gradientDirection, (shaftData.xy - shaftData.zw));

		return dist;
	}

	//**************************
	// RADIAL GRADIENT DISTANCE
	//**************************
	// Unified function for calculating the radial light shaft mask
	half CalculateRadialDistance(half4 shaftData)
	{
		// POSITION
		// Calculate radial distance
		half dist = distance(shaftData.xy, shaftData.zw);

		return dist;
	}

	//***********************************
	// GENERAL LIGHT SHAFT MASK FUNCTION
	//***********************************
	// Unified function for calculating the linear light shaft mask
	half4 CalculateLightShaftColor(half4 shaftData)
	{

		// Calculate the light shaft mask
#if LINEAR
		// Calculate linear distance
		half dist = CalculateLinearDistance(shaftData);
#else
		// Calculate radial distance
		half dist = CalculateRadialDistance(shaftData);
#endif

		// Apply Size
		dist /= _params.z;

		// COLOR
		half4 color = (half4)1;
		if (dist > _Color[7].a)
			color.rgb = _Color[7].rgb;
		else if (dist < .0001)
			color.rgb = _Color[0].rgb;
		else
		{
			// Get color and alpha index from gradient
			uint indexColor = 0;
			// Set index each time if it smaller than the color position
			for (uint c = 0; c < 8; ++c)
			{
				// Set index each time if it smaller than the alpha position
				if (dist > _Color[c].a)
					indexColor = c;
			}

			// Get start & end values
			half4 startColor, endColor;
			startColor = _Color[indexColor];
			endColor = _Color[(indexColor + 1 < 8 ? indexColor + 1 : indexColor)];
//			startColor = _Color[0];
//			endColor = _Color[2];

			// Calculate Color
			// Calculate t between indexed value and next
			half subGradientDistance = endColor.a - startColor.a;
			// Calculate sub distance
			half subDist = dist - startColor.a;
			subDist = saturate(subDist);
			half t = subDist / subGradientDistance;
			// Set correct color
			color.rgb = lerp(startColor.rgb, endColor.rgb, t);
		}

		// ALPHA
		if (dist > _Alpha[7].y)
			color.a = _Alpha[7].x;
		else if (dist < .0001)
			color.a = _Alpha[0].x;
		else
		{
			// Get color and alpha index from gradient
			uint indexAlpha = 0;
			for (uint c = 0; c < 8; ++c)
			{
				// Set index each time if it smaller than the alpha position
				if (dist > _Alpha[c].y)
					indexAlpha = c;
			}

			// Get start & end values
			half2 startAlpha, endAlpha;
			startAlpha = _Alpha[indexAlpha];
			endAlpha = _Alpha[indexAlpha + 1 < 8 ? indexAlpha + 1 : indexAlpha];

			// Calculate Alpha
			// Calculate t between indexed value and next
			half subGradientDistance = endAlpha.y - startAlpha.y;
			// Calculate sub distance
			half subDist = dist - startAlpha.y;
			subDist = saturate(subDist);
			half t = subDist / subGradientDistance;
			// Set correct alpha
			color.a = lerp(startAlpha.x, endAlpha.x, t);
		}

		// Output saturated color
		return saturate(color);
	}

	//*********************
	// BASIC VERTEX SHADER
	//*********************
	VData_basic vert(appdata_img v)
	{
		// Pass through basic data
		VData_basic o = (VData_basic)0;
		o.pos = UnityObjectToClipPos(v.vertex);
		o.uv.xy = v.texcoord.xy;

		// Calculate shaft UV
		o.shaftData.xy = v.texcoord.xy;

		// On non-GL when AA is used, the main texture and scene depth texture
		// will come out in different vertical orientations.
		// So flip sampling of the texture when that is the case (main texture
		// texel size will have negative Y).

#if UNITY_UV_STARTS_AT_TOP
		if (_MainTex_TexelSize.y < 0)
			o.shaftData.xy.y = 1 - o.shaftData.xy.y;
#endif

		// Convert UVs to screen space coordinates
		o.shaftData.xy *= 2;
		o.shaftData.xy -= 1;
		// Normalize UVs to aspect ratio
		o.shaftData.y *= _params.w;


		// Calculate shaft data
		// From degrees to radians
		half angle = _params.y * PI / 180;
		// Convert from polar to cartesian coordinates
		o.shaftData.zw = half2(_params.x * cos(angle), _params.x * sin(angle));

		return o;
	}

	//***********************************
	// BASIC LIGHT SHAFT FRAGMENT SHADER
	//***********************************
	half4 fragLightShaft(VData_basic i) : SV_Target
	{

		// Get Color from original texture
		half4 background = tex2D(_MainTex, i.uv.xy);

		// Calculate the light shaft mask
		half4 overlay = CalculateLightShaftColor(i.shaftData);

		// Calculate background brightness
		half backBrightness = max(background.r, max(background.g, background.b));
		// Add brightness with highlight to light shaft area (2*power2 for subtler effect)
		background += backBrightness * _highlight * pow(overlay.a, 2) * 2;

		// Return color based on blending mode
		#if ALPHABLEND
			// Blend with the alpha channel
			half4 output = half4(lerp(background.rgb, overlay.rgb, overlay.a), background.a);
		#else
			// Additive
			half4 output = half4(background.rgb + overlay.rgb * overlay.a, background.a);
		#endif

		// return _Color[0];

		return saturate(output);
	}

	//**********************************
	// LIGHT SHAFT ONLY FRAGMENT SHADER
	//**********************************
	half4 fragLightShaftOnly(VData_basic i) : SV_Target
	{
		// Calculate the light shaft mask
		half4 overlay = CalculateLightShaftColor(i.shaftData);

		// Light shaft with the alpha channel
		return saturate(overlay);
	}

	ENDCG


	SubShader
	{
		// No culling or depth
		Cull Off
		ZWrite Off
		ZTest Always

		// 0 - Light shaft
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragLightShaft
			ENDCG
		}
		// 1 - Light shaft only (no main texture is used)
		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment fragLightShaftOnly
			ENDCG
		}

	}
}
