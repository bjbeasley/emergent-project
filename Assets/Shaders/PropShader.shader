Shader "Unlit/PropShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Perspective("Perspective", Range(0, 1)) = 1
		_FillMap("Fill map", 2D) = "white" {}
		_Cutoff("Cutoff", Range(0,1)) = 0.5
		_FillScale ("Fill scale", Range (0, 1)) = 1
    }
    SubShader
    {
		ZWrite On

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
                float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
                UNITY_FOG_COORDS(1)
                float4 vertex : SV_POSITION;
				float2 fillPos : TEXCOORD2;
				
            };

            sampler2D _MainTex;
			sampler2D _FillMap;
            float4 _MainTex_ST;
			float4 _FillMap_ST;
			float _Perspective;
			float _Cutoff;
			float _FillScale;

            v2f vert (appdata v)
            {
                v2f o;
				v.vertex.y += v.vertex.z * _Perspective;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.uv1 = TRANSFORM_TEX(v.uv1, _FillMap);
				o.fillPos = v.vertex.xy;
				o.fillPos *= _FillScale;
                UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

			struct fragOutput {
				fixed4 color : SV_Target;
			};

			fragOutput frag(v2f i)
			{
                // sample the texture
				fragOutput o;
                o.color = tex2D(_MainTex, i.uv);
				float fill = tex2D(_FillMap, i.fillPos ).r;
				clip(fill + 1 - _Cutoff * 2 - (o.color.r));
				clip(o.color.a - 0.5f);
                // apply fog
                UNITY_APPLY_FOG(i.fogCoord, col);
                return o;
            }
            ENDCG
        }
    }
}
