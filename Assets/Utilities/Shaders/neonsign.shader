Shader "Custom/neonsign"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Signal ("_Signal (RGB)", 2D) = "white" {}
		_timesp ("깜빡임 재생 속도", Range(0,2)) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent" }

        CGPROGRAM
        #pragma surface surf Lambert alpha:blend
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _Signal;

		float _timesp;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_Signal;
        };

        fixed4 _Color;

        void surf (Input IN, inout SurfaceOutput o)
        {
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex);
			fixed4 d = tex2D(_Signal, float2(0+_Time.y*_timesp, 0.5));
			o.Albedo = lerp(c.rgb*0.3, c.rgb*3, d.r);
            o.Alpha = c.a;
        }
        ENDCG
    }
    FallBack "Transparent"
}
