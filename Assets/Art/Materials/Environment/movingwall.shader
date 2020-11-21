Shader "Custom/movigwall"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Dark("밝기", Float) = 0.3
        _Lpw("빛의 세기", Float) = 5
        _UpSpd("상하 속도", Float) = 1
        _UpDis("상하 거리억제", Float) = 15
        _UpTim("상하 타이밍", Range(0,1)) = 0
        _LRSpd("좌우 속도", Float) = 0
        _LRDis("좌우 거리억제", Float) = 15
        _LRTim("좌우 타이밍", Range(0,1)) = 0

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

        float _UpSpd;
        float _UpDis;
        float _UpTim;
        float _LRSpd;
        float _LRDis;
        float _LRTim;

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex+float2(cos((_Time.y + _LRTim) * _LRSpd) / _LRDis,cos((_Time.y+_UpTim)* _UpSpd)/_UpDis));
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
