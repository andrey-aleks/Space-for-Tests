Shader "Unlit/sh_SimpleUnlit"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _Multi ("Multi", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

            #include "UnityCG.cginc"

            float4 _Color;
            float _Multi;
            
            struct MeshData
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD1;
                float3 normal : NORMAL;
                
            };

            struct v2f
            {
                float2 uv : TEXCOORD1;
                float4 vertex : SV_POSITION;
                float3 normal : TEXCOORD0;
            };

            v2f vert (MeshData v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.normal = UnityObjectToWorldNormal(v.normal);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                return (float4(i.normal, 1) * _Multi);
            }
            ENDCG
        }
    }
}
