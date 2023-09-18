// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/DistortingGrabPass" 
{

	Properties
	{

		_Intensity("Intensity", Range(0, 25)) = 0
		_NoiseTexture("Noise Texture", 2D) = "white" {}
		_Scale("Scale", Range(0.000001,50)) = 10
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always

		GrabPass { "_GrabTexture" }

		Pass 
		{
			Tags { "Queue" = "Transparent" }

			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#include "UnityCG.cginc"

			struct v2f {
				float4 pos : SV_POSITION;
				float4 worldPos : TEXCOORD0;
			};

			sampler2D _GrabTexture;
			sampler2D _NoiseTexture;
			half _Intensity;
			half _Scale;
			half _Zoom;
			//UNITY_DECLARE_TEX2D(_NoiseTexture);

			v2f vert(appdata_base v) {
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}

			half4 frag(v2f i) : COLOR {
				//fixed4 offsets = UNITY_SAMPLE_TEX2D(_NoiseTexture,i.pos.xy);
				float4 offsets = tex2Dlod(_NoiseTexture, i.worldPos / _Scale);

				float4 worldPos1 = i.worldPos;
				worldPos1.x += (offsets.x - 0.5) * _Intensity;


				worldPos1.y += (offsets.y - 0.5) * _Intensity;
				float4 pos = UnityObjectToClipPos(mul(unity_WorldToObject, worldPos1));
				float4 grabPos = ComputeGrabScreenPos(pos);
				float4 color1 = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(grabPos));

				worldPos1 = i.worldPos;
				worldPos1.x -= (offsets.x - 0.5) * _Intensity;
				worldPos1.y -= (offsets.y - 0.5) * _Intensity;
				pos = UnityObjectToClipPos(mul(unity_WorldToObject, worldPos1));
				grabPos = ComputeGrabScreenPos(pos);
				float4 color2 = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(grabPos));

				worldPos1 = i.worldPos;
				worldPos1.x += (offsets.z - 0.5) * _Intensity;
				worldPos1.y -= (offsets.x - 0.5) * _Intensity;
				pos = UnityObjectToClipPos(mul(unity_WorldToObject, worldPos1));
				grabPos = ComputeGrabScreenPos(pos);
				float4 color3 = tex2Dproj(_GrabTexture, UNITY_PROJ_COORD(grabPos));

				return (color1 + color2 + color3) / 3;
			}
			ENDCG
		}
	}
	FallBack "Diffuse"
}
