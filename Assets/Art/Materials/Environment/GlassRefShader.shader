Shader "Custom/GlassRefShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ReflMask ("ReflMask", 2D) = "white" {}
        _ReflTex ("반사 이미지", 2D) = "white" {}
        _RefleX ("반사x", Float) = 0
        _RefleY ("반사y", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        CGPROGRAM
        #pragma surface surf Lambert alpha:blend
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _ReflTex;
        sampler2D _ReflMask;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_ReflTex;
            float2 uv_ReflMask;
            float3 worldPos;
        };

        float _RefleX;
        float _RefleY;

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 rm = tex2D(_ReflMask, IN.uv_ReflMask);
            fixed4 r = tex2D(_ReflTex, IN.worldPos.xy/float2(73.846153,28.8846153));

            o.Albedo = (rm.r * r.a * r.rgb)*0.7-(c.rgb*(1-c.a));
            o.Emission = (1-r.a*rm.r)*c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Transparent"
}
