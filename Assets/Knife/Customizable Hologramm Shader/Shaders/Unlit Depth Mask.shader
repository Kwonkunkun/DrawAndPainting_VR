Shader /*ase_name*/ "Hidden/Templates/Knife/Unlit Depth Mask" /*end*/
{
	Properties
	{
		/*ase_props*/
	}
	
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue" = "Transparent" }
		
		/*ase_pass*/
		Pass
		{
			/*ase_hide_pass:SyncP*/
			Name "Depth Mask"
			Blend SrcAlpha OneMinusSrcAlpha
			ZWrite On
			ColorMask 0
			ZTest LEqual
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			/*ase_pragma*/
			/*ase_globals*/

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				/*ase_vdata:p=p;n=n*/
			};
			
			struct v2f
			{
				float4 vertex : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				/*ase_interp(1,):sp=sp.xyzw*/
			};

			
			v2f vert ( appdata v /*ase_vert_input*/)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f,o);
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				
				/*ase_vert_code:v=appdata;o=v2f*/
				
				v.vertex.xyz = /*ase_vert_out:Local Vertex;Float3;_Vertex*/ float3(0,0,0) /*end*/;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			float4 frag (v2f i /*ase_frag_input*/) : SV_Target
			{
				/*ase_frag_code:i=v2f*/
				return 0;
			}
			ENDCG
		}

		LOD 100

		CGINCLUDE
		#pragma target 3.0 
		ENDCG
		
		/*ase_pass*/
		Pass
		{
			/*ase_main_pass*/
			Name "Unlit"

			/*ase_all_modules*/
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"
			/*ase_pragma*/
			/*ase_globals*/

			struct appdata
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				/*ase_vdata:p=p;n=n*/
			};
			
			struct v2f
			{
				float4 pos : SV_POSITION;
				UNITY_VERTEX_INPUT_INSTANCE_ID
				UNITY_VERTEX_OUTPUT_STEREO
				/*ase_interp(1,):sp=sp.xyzw*/
			};
			
			v2f vert ( appdata v /*ase_vert_input*/)
			{
				v2f o;
				UNITY_INITIALIZE_OUTPUT(v2f,o);
				UNITY_SETUP_INSTANCE_ID(v);
				UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
				UNITY_TRANSFER_INSTANCE_ID(v, o);
				
				/*ase_vert_code:v=appdata;o=v2f*/
				
				v.vertex.xyz = /*ase_vert_out:Local Vertex;Float3;_Vertex*/ float3(0,0,0) /*end*/;
				o.pos = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			float4 frag (v2f i /*ase_frag_input*/) : SV_Target
			{
				float4 outColor;

				/*ase_frag_code:i=v2f*/
				
				outColor = /*ase_frag_out:Color;Float4;_Color*/float4(1,1,1,1)/*end*/;
				return outColor;
			}
			ENDCG
		}
		/*ase_pass_end*/
	}
	CustomEditor "ASEMaterialInspector"
}
