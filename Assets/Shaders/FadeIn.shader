Shader "Unlit/FadeIn"
{
    Properties
    {
		_Color ("Color", Color) = (0,0,0,0)
		_FillMap("Fill Map", 2D) = "white" {}
		_Cutoff("Cutoff", Range(0,1)) = 1
		_Scale("Scale", Range(0,1)) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
            };

            struct v2f
            {
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float2 fillPos : TEXCOORD0;
            };

			sampler2D _FillMap;
			float4 _FillMap_ST;
			float _Cutoff;
			float4 _Color;
			float _Scale;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.fillPos = v.vertex.xy * _Scale;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
				float fill = tex2D(_FillMap, i.fillPos);

				clip(fill - _Cutoff);

                return _Color;
            }
            ENDCG
        }
    }
}
