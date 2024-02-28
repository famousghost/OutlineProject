Shader "Unlit/OutlineUnlitCustomLeafs"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _LeafTexture ("Leaf Texture", 2D) = "white" {}
        _OutlineColor ("Outline Color", Color) = (1, 1, 1, 1)
        _Tiling ("Tiling", Vector) = (1, 1, 1, 1)
        _OutlineScale("Outline Scale", Float) = 0.0
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "Queue" = "Transparent+1" }
        LOD 100

        Pass
        {
            Cull Off
             Stencil
             {
                 Ref 3
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
            float _OutlineScale;


            v2f vert (appdata v)
            {
                v2f o;

                v.vertex.xyz += v.normal * _OutlineScale; 

                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                return o;
            }

            float4 frag (v2f i) : SV_Target
            {
                // sample the texture
                float4 col = tex2D(_LeafTexture, i.uv * _Tiling);
                if(col.a <= 0.0f)
                {
                    discard;
                }
                return _OutlineColor;
            }
            ENDCG
        }
    }
}
