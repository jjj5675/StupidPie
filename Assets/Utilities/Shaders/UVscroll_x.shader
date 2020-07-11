Shader "Custom/UVscroll_x"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
		_CloudSpeed("UV.u 스크롤 속도", float) = 0.5
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" "Queue"="Transparent"}

        CGPROGRAM
        #pragma surface surf Lambert noshadow alpha:blend

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
        };
		
		float _CloudSpeed;

        void surf (Input IN, inout SurfaceOutput o)
        {
            float4 c = tex2D (_MainTex, IN.uv_MainTex + float2(_Time.y*_CloudSpeed,0));
            o.Emission = c.rgb;
            o.Alpha = c.a;
        }


        ENDCG
    }
    FallBack "Diffuse"
}
