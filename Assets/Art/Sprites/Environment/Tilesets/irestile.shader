Shader "Custom/irestile"
{
    Properties
    {
        _MainTex ("AlbedoOn", 2D) = "white" {}
        _MainTex2 ("AlbedoOff", 2D) = "white" {}
        _onoff ("onoff", Float) = 0
    }
    SubShader
    {
        Tags { "RenderType"="Transparent" }

        CGPROGRAM
        #pragma surface surf Standard alpha:blend
        #pragma target 3.0

        sampler2D _MainTex;
        sampler2D _MainTex2;
        float _onoff;

        struct Input
        {
            float2 uv_MainTex;
            float2 uv_MainTex2;
        };

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            fixed4 irestileon = tex2D(_MainTex, IN.uv_MainTex);
            fixed4 irestileoff = tex2D(_MainTex2, IN.uv_MainTex2);
            o.Emission = lerp(irestileoff.rgb, irestileon.rgb,_onoff);
            o.Alpha = irestileon.a;
        }
        ENDCG
    }
    FallBack "Diffuse"
}
