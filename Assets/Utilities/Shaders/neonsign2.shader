Shader "Custom/neonsign2"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Signal("_Signal (RGB)", 2D) = "white" {}
        _MaskTex("Masking", 2D) = "white" {}
        _timesp("깜빡임 재생 속도", Range(0,2)) = 0.5
        _lightpw("깜빡임 밝기 강하기", Float) = 1
        _lightdf("기존 밝기 강하기", Float) = 1
    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

        CGPROGRAM
        #pragma surface surf Lambert alpha:blend
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MaskTex;
        sampler2D _Signal;

        float _timesp;
        float _lightpw;
        float _lightdf;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MaskTex;
            float2 uv_Signal;
        };

        fixed4 _Color;

        void surf(Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 d = tex2D(_Signal, float2(0 + _Time.y * _timesp, 0.5));
            fixed4 m = tex2D(_MaskTex, IN.uv_MaskTex);
            o.Albedo = (c.rgb * _lightdf) + float3(d.rgb*m.r*(c.rgb*_lightpw));
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
