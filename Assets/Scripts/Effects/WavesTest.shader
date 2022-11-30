Shader "Unlit/WavesTest"
{
    Properties
    {
        //_MainTex ("Texture", 2D) = "white" {}
        //_Value ("Value", Float) = 1.0
        _ColorA ("Color A", Color) = (1,1,1,1)
        _ColorB ("Color B", Color) = (1,1,1,1)

        _CurveStrength("CurveStr", Range(0,1))= 0
        _WaveAmp ("Wave Amp", Range(0,0.1)) = 0
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

            #define TAU 6.283185307179586

            float4 _ColorA;
            float4 _ColorB;

            float _WaveAmp;
            float _CurveStrength;

            // Per-vertex mesh data.
            struct MeshData
            {
                // Vertex position.
                float4 vertex : POSITION;

                float3 normals : NORMAL;
                //float4 tangent : TANGENT;
                //float4 color : COLOR;

                // UV0 coordinates diffuse / normal maps.
                float2 uv0 : TEXCOORD0;
                // UV1 coordinates lightmap coordinates.
                float2 uv1 : TEXCOORD1;
            };

            // This will blend / interpolate across each triangle
            struct Interpolators
            {
                // Clip space position.
                float4 vertex : SV_POSITION;
                float3 worldPos : TEXCOORD0;
                float2 uv : TEXCOORD1;
            };

            Interpolators vert (MeshData v)
            {
                Interpolators o;
                o.worldPos = mul( UNITY_MATRIX_M, float4(v.vertex.xyz, 1) );
                // Local space to clip space.

                float wave = cos(o.worldPos.x * _CurveStrength + _Time.y );

                v.vertex.y = wave * _WaveAmp;

                o.vertex = UnityObjectToClipPos(v.vertex);

                o.uv = v.uv0;

                return o;
            }

            fixed4 frag (Interpolators i) : SV_Target
            {
                // Lerp.

                //float t = saturate(InverseLerp(_ColorStart, _ColorEnd, i.uv.x));

                //float4 _outColor = lerp(_ColorA, _ColorB, t);

                //return _outColor;

                //float t = abs( frac(i.uv.y * 5) * 2 - 1 );

                float t = cos(i.worldPos.x * _CurveStrength + _Time.y) * 0.5 + 0.5;

                float4 _outColor = lerp(_ColorA, _ColorB, t);

                return _outColor;
            }
            ENDCG
        }
    }
}
