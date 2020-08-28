// Made with Amplify Shader Editor
// Available at the Unity Asset Store - http://u3d.as/y3X 
Shader "Animmal/Magma"
{
	Properties
	{
		_EmisionContrast1("Emision Contrast 1", Range( 0 , 2)) = 0
		_EmisionContrast2("Emision Contrast 2", Range( 0 , 2)) = 0
		_EmisionBase("Emision Base", Range( 0 , 2)) = 0
		_EimisionContrast3("Eimision Contrast 3", Range( 0 , 2)) = 0
		_Specular("Specular ", 2D) = "white" {}
		_Normal("Normal", 2D) = "bump" {}
		_Emision1("Emision 1 ", 2D) = "white" {}
		_AlbedoEmision("Albedo Emision", 2D) = "white" {}
		_Emision3("Emision 3", 2D) = "white" {}
		_Emision2("Emision 2", 2D) = "white" {}
		_Albedo("Albedo", 2D) = "white" {}
		[HideInInspector] _texcoord( "", 2D ) = "white" {}
		[HideInInspector] __dirty( "", Int ) = 1
	}

	SubShader
	{
		Tags{ "RenderType" = "Opaque"  "Queue" = "Geometry+0" "IsEmissive" = "true"  }
		Cull Back
		CGPROGRAM
		#include "UnityShaderVariables.cginc"
		#pragma target 3.0
		#pragma surface surf StandardSpecular keepalpha addshadow fullforwardshadows 
		struct Input
		{
			fixed2 uv_texcoord;
		};

		uniform sampler2D _Normal;
		uniform float4 _Normal_ST;
		uniform sampler2D _Albedo;
		uniform float4 _Albedo_ST;
		uniform sampler2D _AlbedoEmision;
		uniform float4 _AlbedoEmision_ST;
		uniform fixed _EmisionBase;
		uniform sampler2D _Emision1;
		uniform float4 _Emision1_ST;
		uniform fixed _EimisionContrast3;
		uniform sampler2D _Emision2;
		uniform float4 _Emision2_ST;
		uniform fixed _EmisionContrast1;
		uniform sampler2D _Emision3;
		uniform float4 _Emision3_ST;
		uniform fixed _EmisionContrast2;
		uniform sampler2D _Specular;
		uniform float4 _Specular_ST;

		void surf( Input i , inout SurfaceOutputStandardSpecular o )
		{
			float2 uv_Normal = i.uv_texcoord * _Normal_ST.xy + _Normal_ST.zw;
			o.Normal = UnpackNormal( tex2D( _Normal, uv_Normal ) );
			float2 uv_Albedo = i.uv_texcoord * _Albedo_ST.xy + _Albedo_ST.zw;
			float2 uv_AlbedoEmision = i.uv_texcoord * _AlbedoEmision_ST.xy + _AlbedoEmision_ST.zw;
			fixed4 tex2DNode74 = tex2D( _AlbedoEmision, uv_AlbedoEmision );
			o.Albedo = ( tex2D( _Albedo, uv_Albedo ) + ( tex2DNode74 * ( _EmisionBase * ( ( _SinTime.z * 3 ) + 0 ) ) ) ).rgb;
			float2 uv_Emision1 = i.uv_texcoord * _Emision1_ST.xy + _Emision1_ST.zw;
			float2 uv_Emision2 = i.uv_texcoord * _Emision2_ST.xy + _Emision2_ST.zw;
			float2 uv_Emision3 = i.uv_texcoord * _Emision3_ST.xy + _Emision3_ST.zw;
			o.Emission = ( ( tex2D( _Emision1, uv_Emision1 ) * ( _EimisionContrast3 * ( ( _CosTime.w * 5 ) + 0 ) ) ) + ( tex2D( _Emision2, uv_Emision2 ) * ( _EmisionContrast1 * ( ( _SinTime.w * 2 ) + 0 ) ) ) + ( tex2D( _Emision3, uv_Emision3 ) * ( _EmisionContrast2 * ( ( _CosTime.w * 7 ) + 0 ) ) ) + tex2DNode74 ).rgb;
			float2 uv_Specular = i.uv_texcoord * _Specular_ST.xy + _Specular_ST.zw;
			o.Smoothness = tex2D( _Specular, uv_Specular ).r;
			o.Alpha = 1;
		}

		ENDCG
	}
	Fallback "Diffuse"
	CustomEditor "ASEMaterialInspector"
}
/*ASEBEGIN
Version=14501
293;259;1471;639;-546.1498;217.5514;1.403835;True;True
Node;AmplifyShaderEditor.SinTimeNode;70;294.5857,-221.0512;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CosTime;138;-917.0685,-90.59318;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SinTimeNode;45;-784.6197,644.6348;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.CosTime;145;-1042.845,1245.25;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.ScaleNode;71;450.5464,-215.9485;Float;True;3;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;46;-593.6588,667.7374;Float;True;2;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;33;-500.4558,61.71972;Float;True;5;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.ScaleNode;56;-500.6649,1134.899;Float;True;7;1;0;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;58;-338.8442,1141.263;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;73;612.3667,-209.5847;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;4;-464.9475,-73.99216;Float;False;Property;_EimisionContrast3;Eimision Contrast 3;3;0;Create;True;0;0;0.15;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;57;-461.1566,1002.187;Float;False;Property;_EmisionContrast2;Emision Contrast 2;1;0;Create;True;0;0;0.1990622;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;72;490.0546,-348.6605;Float;False;Property;_EmisionBase;Emision Base;2;0;Create;True;0;0;0.1591238;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;5;-343.6353,66.08354;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleAddOpNode;48;-432.1383,658.8012;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.RangedFloatNode;47;-553.1505,549.0255;Float;False;Property;_EmisionContrast1;Emision Contrast 1;0;0;Create;True;0;0;0.3374095;0;2;0;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;75;788.1443,-342.8167;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;60;-163.0668,1008.031;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;8;-167.8577,-67.14825;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;49;-255.0607,526.8694;Float;False;2;2;0;FLOAT;0;False;1;FLOAT;0;False;1;FLOAT;0
Node;AmplifyShaderEditor.SamplerNode;74;553.9358,-603.4869;Float;True;Property;_AlbedoEmision;Albedo Emision;7;0;Create;True;0;None;26a61bca12be5304ab74e4ef32da0a1a;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;36;-380.3701,-279.4199;Float;True;Property;_Emision1;Emision 1 ;6;0;Create;True;0;None;b10573b6882edca4cb3e1e383a663c9d;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;50;-467.5731,314.5976;Float;True;Property;_Emision2;Emision 2;9;0;Create;True;0;None;b0c1617eee685624e8cbb25c3ea6119b;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;59;-375.579,795.759;Float;True;Property;_Emision3;Emision 3;8;0;Create;True;0;None;82a0a2c08892cb24cb528cfe1794b9ac;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;9;41.521,138.5178;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;61;31.2081,400.903;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;76;995.4104,-388.5857;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;37;1209.889,-733.1044;Float;True;Property;_Albedo;Albedo;10;0;Create;True;0;None;964ebf4902dbdbc40944ae4450c06019;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleMultiplyOpNode;51;34.29464,272.5956;Float;False;2;2;0;COLOR;0,0,0,0;False;1;FLOAT;0;False;1;COLOR;0
Node;AmplifyShaderEditor.SinTimeNode;55;-774.6259,1131.796;Float;False;0;5;FLOAT4;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;69;1399.479,-205.0648;Float;True;2;2;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.SamplerNode;40;1179.787,367.4144;Float;True;Property;_Specular;Specular ;4;0;Create;True;0;None;0f8108d51cc8c5c4a89934cdc31c9411;True;0;False;white;Auto;False;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;COLOR;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SamplerNode;34;942.6261,-150.4564;Float;True;Property;_Normal;Normal;5;0;Create;True;0;None;c79ab918fd1601c42a87d2afbd3fec70;True;0;True;bump;Auto;True;Object;-1;Auto;Texture2D;6;0;SAMPLER2D;;False;1;FLOAT2;0,0;False;2;FLOAT;0;False;3;FLOAT2;0,0;False;4;FLOAT2;0,0;False;5;FLOAT;1;False;5;FLOAT3;0;FLOAT;1;FLOAT;2;FLOAT;3;FLOAT;4
Node;AmplifyShaderEditor.SimpleAddOpNode;133;448.0981,256.784;Float;False;4;4;0;COLOR;0,0,0,0;False;1;COLOR;0,0,0,0;False;2;COLOR;0,0,0,0;False;3;COLOR;0,0,0,0;False;1;COLOR;0
Node;AmplifyShaderEditor.StandardSurfaceOutputNode;0;1663.238,131.0954;Fixed;False;True;2;Fixed;ASEMaterialInspector;0;0;StandardSpecular;Animmal/Magma;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;False;Back;0;0;False;0;0;False;0;Opaque;0.5;True;True;0;False;Opaque;;Geometry;All;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;True;False;0;255;255;0;0;0;0;0;0;0;0;False;2;15;10;25;False;0.5;True;0;Zero;Zero;0;Zero;Zero;OFF;OFF;0;False;0;0,0,0,0;VertexOffset;True;False;Cylindrical;False;Relative;0;;-1;-1;-1;-1;0;0;0;False;0;0;16;0;FLOAT3;0,0,0;False;1;FLOAT3;0,0,0;False;2;FLOAT3;0,0,0;False;3;FLOAT3;0,0,0;False;4;FLOAT;0;False;5;FLOAT;0;False;6;FLOAT3;0,0,0;False;7;FLOAT3;0,0,0;False;8;FLOAT;0;False;9;FLOAT;0;False;10;FLOAT;0;False;13;FLOAT3;0,0,0;False;11;FLOAT3;0,0,0;False;12;FLOAT3;0,0,0;False;14;FLOAT4;0,0,0,0;False;15;FLOAT3;0,0,0;False;0
WireConnection;71;0;70;3
WireConnection;46;0;45;4
WireConnection;33;0;138;4
WireConnection;56;0;145;4
WireConnection;58;0;56;0
WireConnection;73;0;71;0
WireConnection;5;0;33;0
WireConnection;48;0;46;0
WireConnection;75;0;72;0
WireConnection;75;1;73;0
WireConnection;60;0;57;0
WireConnection;60;1;58;0
WireConnection;8;0;4;0
WireConnection;8;1;5;0
WireConnection;49;0;47;0
WireConnection;49;1;48;0
WireConnection;9;0;36;0
WireConnection;9;1;8;0
WireConnection;61;0;59;0
WireConnection;61;1;60;0
WireConnection;76;0;74;0
WireConnection;76;1;75;0
WireConnection;51;0;50;0
WireConnection;51;1;49;0
WireConnection;69;0;37;0
WireConnection;69;1;76;0
WireConnection;133;0;9;0
WireConnection;133;1;51;0
WireConnection;133;2;61;0
WireConnection;133;3;74;0
WireConnection;0;0;69;0
WireConnection;0;1;34;0
WireConnection;0;2;133;0
WireConnection;0;4;40;0
ASEEND*/
//CHKSM=9867F7EBB52B576CDBEF30E3B877BFC85A802899