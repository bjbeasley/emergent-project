Shader "Unlit/Multiply3"
{
    Properties
    {
        _Tex0 ("Texture 0", 2D) = "white" {}
		_Tex1 ("Texture 1", 2D) = "white" {}
		_Tex2 ("Texture 2", 2D) = "white" {}
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

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
            };

            struct v2f
            {
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
				float2 uv2 : TEXCOORD2;
                float4 vertex : SV_POSITION;
            };

            sampler2D _Tex0;
			sampler2D _Tex1;
			sampler2D _Tex2;
            float4 _Tex0_ST;
			float4 _Tex1_ST;
			float4 _Tex2_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv0 = TRANSFORM_TEX(v.uv0, _Tex0);
				o.uv1 = TRANSFORM_TEX(v.uv1, _Tex1);
				o.uv2 = TRANSFORM_TEX(v.uv2, _Tex2);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col0 = tex2D(_Tex0, i.uv0);
				fixed4 col1 = tex2D(_Tex1, i.uv1);
				fixed4 col2 = tex2D(_Tex2, i.uv2);
                return col0 * col1 * col2;
            }
            ENDCG
        }
    }
}
