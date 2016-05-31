Shader "Unlit/Instancing" {
	Properties {
		_MainTex ("Texture", 2D) = "white" {}
	}
	SubShader {
		Tags { "RenderType"="Opaque" }
		LOD 100

		Pass {
			CGPROGRAM
			#pragma target 5.0
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct v2f {
				float2 uv : TEXCOORD0;
				float4 vertex : SV_POSITION;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;

			StructuredBuffer<int> _TriangleBuf;
			StructuredBuffer<float3> _VertexBuf;
			StructuredBuffer<float2> _UvBuf;
			StructuredBuffer<float4x4> _WorldMatBuf;
			
			v2f vert (uint triId : SV_VertexID, uint instId : SV_InstanceID) {
				int vid = _TriangleBuf[triId];
				float3 vertex = _VertexBuf[vid];
				float2 uv = _UvBuf[vid];
				float4x4 world = _WorldMatBuf[instId];

				v2f o;
				o.vertex = mul(UNITY_MATRIX_VP, mul(world, float4(vertex, 1.0)));
				o.uv = TRANSFORM_TEX(uv, _MainTex);
				return o;
			}
			
			fixed4 frag (v2f i) : SV_Target {
				fixed4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
