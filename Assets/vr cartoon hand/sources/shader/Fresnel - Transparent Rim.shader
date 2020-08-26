// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "FresnelPack/Transparent Rim Unlit" 
{
	Properties 
	{
		_RimColor("Rim Color", Color) = (1.0, 1.0, 1.0, 1.0)
		_Scale("Fresnel Scale", Range(0.0, 1.0)) = 1.0
	}
	SubShader 
	{
		Tags {"Queue"="Transparent" "IgnoreProjector"="True" "RenderType"="Transparent"}

		Pass
		{ 
   			ZWrite On
   			Blend SrcAlpha OneMinusSrcAlpha 

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag

			uniform float4 _Tint;
			uniform float4 _RimColor;
			uniform float _Scale;

			struct vIN
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
			};

			struct vOUT
			{
				float4 pos : SV_POSITION;
				float3 posWorld : TEXCOORD0;
				float3 normWorld : TEXCOORD1;
				
			};

			vOUT vert(vIN v)
			{
				vOUT o;
				o.pos = UnityObjectToClipPos(v.vertex);

				o.posWorld = mul(unity_ObjectToWorld, v.vertex);
				o.normWorld = normalize(mul( (float3x3)unity_ObjectToWorld, v.normal));

				return o;
			}

			float4 frag(vOUT i) : COLOR
			{
				float3 I = normalize(i.posWorld - _WorldSpaceCameraPos.xyz);

				float refFactor = max(0.0, min(1.0,_Scale * pow(1.0 + dot(I, i.normWorld), 1.4)));
				return  lerp(float4(1.0, 1.0, 1.0, 0.0),_RimColor,refFactor);
			}

			ENDCG
		}
	}

	FallBack "Diffuse"
}
