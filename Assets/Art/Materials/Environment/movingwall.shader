Shader "Custom/movigwall"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Dark("밝기", Float) = 0.3
        _Lpw("빛의 세기", Float) = 5

    }
    SubShader
    {
        Tags { "RenderType" = "Transparent" "Queue" = "Transparent" }

        CGPROGRAM
        #pragma surface surf hallway alpha:blend
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };

        float _Dark;
        float _Lpw;


        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex+float2(0,cos(_Time.y)/15));
            c.rgb = c.rgb * _Dark;
            o.Albedo = c.rgb;
            o.Alpha = c.a;
            o.Normal = float3(0,0.5,2000);
        }

        float4 Lightinghallway(SurfaceOutput s, float3 LightDir, float atten)
        {
            float ndotl = saturate(dot(s.Normal, LightDir))*_Lpw;
            
            float4 final;
            final.rgb = s.Albedo * ndotl * _LightColor0.rgb * atten;
            final.a = s.Alpha;

            return final;
        }

        ENDCG
    }
    FallBack "Diffuse"
}
