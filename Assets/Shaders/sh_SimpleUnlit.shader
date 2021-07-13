Shader "Unlit/sh_SimpleUnlit"
{
    Properties
    {
        _ColorA ("Color A", Color) = (0,0,0,0)
        _ColorB ("Color B", Color) = (1,1,1,1)
        _ColorStart ("Color Start", Range(0, 1)) = 0
        _ColorEnd ("Color End", Range(0, 1)) = 1
        _Multi ("Multi", Float) = 1
        _Scale ("Scale", Float) = 1
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

            float4 _ColorA;
            float4 _ColorB;
            float _ColorStart;
            float _ColorEnd;
            float _Multi;
            float _Scale;

            float InvLerp (float a, float b, float v)
            {
                return (v-a)/b-a;
            }
            
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
                o.uv = v.uv * _Scale;
                o.normal = UnityObjectToWorldNormal(v.normal) * _Multi;
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                float t = InvLerp(_ColorStart, _ColorEnd, i.uv.x);
                return frac(t);
               float4 outValue = saturate(lerp(_ColorA, _ColorB, t));
               return outValue;
               // return lerp(_ColorA, _ColorB, i.uv.x);
               // InvLerp()
            }
            ENDCG
        }
    }
}
