Shader "Hidden/Paint in 3D/Fill"
{
	Properties
	{
		_ReplaceTexture("Replace Texture", 2D) = "white" {}
		_Texture("Texture", 2D) = "white" {}
	}
	SubShader
	{
		Pass
		{
			Blend Off
			Cull Off
			ZWrite Off

			CGPROGRAM
				#pragma vertex Vert
				#pragma fragment Frag
				#pragma multi_compile __ P3D_A // 0-1
				#pragma multi_compile __ P3D_B // 0-2
				#pragma multi_compile __ P3D_C // 0-4
				#pragma multi_compile __ P3D_D // 0-8
				#define BLEND_MODE_INDEX (P3D_A * 1 + P3D_B * 2 + P3D_C * 4 + P3D_D * 8)

				float4    _Channels;
				sampler2D _Texture;
				float4    _Color;
				float     _Opacity;
				float4    _Minimum;

				#include "BlendModes.cginc"

				struct a2v
				{
					float4 vertex    : POSITION;
					float2 texcoord0 : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex   : SV_POSITION;
					float2 texcoord : TEXCOORD0;
				};

				struct f2g
				{
					float4 color : SV_TARGET;
				};

				void Vert(a2v i, out v2f o)
				{
					o.vertex   = float4(i.texcoord0.xy * 2.0f - 1.0f, 0.5f, 1.0f);
					o.texcoord = i.texcoord0;
#if UNITY_UV_STARTS_AT_TOP
					o.vertex.y = -o.vertex.y;
#endif
				}

				void Frag(v2f i, out f2g o)
				{
					float4 color = tex2D(_Texture, i.texcoord) * _Color;

					//o.color = Blend(color, _Opacity, _Buffer, i.uv);
					o.color = BlendMinimum(color, _Opacity, i.texcoord, _Minimum, _Channels);
				}
			ENDCG
		} // Pass
	} // SubShader
} // Shader