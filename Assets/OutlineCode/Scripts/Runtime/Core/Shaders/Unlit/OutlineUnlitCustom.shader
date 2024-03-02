// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "McOutline/CustomOutline"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LeafTexture ("Leaf Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _Tiling ("Tiling", Vector) = (1, 1, 1, 1)
        _OutlineSize("Outline Scale", Float) = 0.0
        _AlphaCutoff("AlphaCutoff", Float) = 0.0
        _AlphaCutoffEnable("_AlphaCutoffEnable", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Transparent+2" }
        LOD 100

        Pass
        {
            ZWrite Off
            ZTest Always
            Cull Off
            Stencil
            {
                Ref 15
                Comp NotEqual
            }    


            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float3 normal : NORMAL;
                float3 tangent : TANGENT;
                float2 uv : TEXCOORD0;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;

            sampler2D _LeafTexture;

            float4 _OutlineColor;
            float2 _Tiling;
            float _OutlineSize;
            float _AlphaCutoff;
            float _AlphaCutoffEnable;


            v2f vert (appdata v)
            {
                v2f o;

                const float3 viewPosition = UnityObjectToViewPos(v.vertex);
                const float3 viewNormal = normalize(mul((float3x3)UNITY_MATRIX_IT_MV, v.normal));

                o.vertex = UnityViewToClipPos(viewPosition + viewNormal * _OutlineSize / 1000.0f);
                //v.vertex.xyz += v.normal * _OutlineSize / 1000.0f;
                //o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                const float alpha = tex2D(_LeafTexture, i.uv * _Tiling).a;
                if(_AlphaCutoffEnable > 0.0f && alpha <= _AlphaCutoff)
                {
                    discard;
                }
                return _OutlineColor;
            }
            ENDCG
        }
    }
}
