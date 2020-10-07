Shader "Custom/GlassRefShader"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _ReflMask ("ReflMask", 2D) = "white" {}
        _ReflTex ("반사 이미지", 2D) = "white" {}
        _RefleX ("반사x", Float) = 0
        _RefleY ("반사y", Float) = 0
        _Reflepw ("반사 이미지 밝기", Float) = 1.2
        _Reflepow ("반사 이미지 대비같은 거", Float) = 1.2
        _Spepw ("유리 정반사 밝기", Float) = 1.1
        _Imagesize ("이미지 크기", Vector) = (1,1,1,1)
        _Zoom ("확대율", Float) = 1
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

        float _Reflepw;
        float _Reflepow;
        float _Spepw;
        float _Zoom;

        float4 _Imagesize;

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 rm = tex2D(_ReflMask, IN.uv_ReflMask);
            fixed4 r = pow(tex2D(_ReflTex, IN.worldPos.xy/float2(_Imagesize.x/_Imagesize.z*_Zoom,_Imagesize.y/_Imagesize.w*_Zoom)+float2(0.1,0.52))*_Reflepw, _Reflepow);

            if (rm.r == 1)
            {
                o.Albedo = (rm.r * r.a * r.rgb) * _Spepw - (c.rgb * (1 - c.a));
            }
            else
            {
                o.Albedo = (rm.r * r.a * r.rgb) * 0.7 - (c.rgb * (1 - c.a));
            };

            o.Emission = (1-r.a*rm.r)*c.rgb;
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Transparent"
}
