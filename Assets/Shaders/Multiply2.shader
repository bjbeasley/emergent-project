Shader "Unlit/Multiply3"
{
    Properties
    {
        _Tex0 ("Texture 0", 2D) = "white" {}
		_Tex1 ("Texture 1", 2D) = "white" {}
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
            };

            struct v2f
            {
				float2 uv0 : TEXCOORD0;
				float2 uv1 : TEXCOORD1;
                float4 vertex : SV_POSITION;
            };

            sampler2D _Tex0;
			sampler2D _Tex1;
            float4 _Tex0_ST;
			float4 _Tex1_ST;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv0 = TRANSFORM_TEX(v.uv0, _Tex0);
				o.uv1 = TRANSFORM_TEX(v.uv1, _Tex1);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col0 = tex2D(_Tex0, i.uv0);
				fixed4 col1 = tex2D(_Tex1, i.uv1);
                return col0 * col1;
            }
            ENDCG
        }
    }
}
