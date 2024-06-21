Shader "Custom/WaterShader"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
        _WaveSpeed("Wave Speed", Float) = 1.0
        _WaveFrequency("Wave Frequency", Float) = 1.0
        _WaveHeight("Wave Height", Float) = 0.1
    }
    SubShader
    {
        Tags { "Queue"="Transparent" "RenderType"="Transparent" }
        LOD 200

        CGPROGRAM
        #pragma surface surf Lambert

        sampler2D _MainTex;
float _WaveSpeed;
float _WaveFrequency;
float _WaveHeight;

struct Input
{
    float2 uv_MainTex;
};

void surf(Input IN, inout SurfaceOutput o)
{
    // 시간에 따라 UV 좌표를 변형하여 흔들리는 효과 생성
    float wave = sin(_WaveFrequency * (IN.uv_MainTex.x + _Time.y * _WaveSpeed)) * _WaveHeight;
    float2 distortedUV = IN.uv_MainTex + wave;

    fixed4 c = tex2D(_MainTex, distortedUV);
    o.Albedo = c.rgb;
    o.Alpha = c.a;
}
ENDCG
    }
    FallBack "Diffuse"
}