Shader "Custom/wall"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Dark("밝기", Float) = 0.3

    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

        CGPROGRAM
        #pragma surface surf Lambert alpha:blend
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        float _Dark;
        
        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
            o.Albedo = c.rgb*_Dark;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
