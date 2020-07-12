Shader "Unlit/GhostSprite"
{
    Properties
    {
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Color ("Color", Color) = (1, 1, 1, 1)
        _FadeRange("Fade Out", Range(0, 1)) = 0
    }
    SubShader
    {
        Tags{"Queue" = "Transparent" "IgnoreProjector" = "False" "RenderType" = "Transparent"}
        Fog {Mode Off}
        Blend One OneMinusSrcAlpha
        ZWrite Off
        Cull off
        //Tags { "RenderType"="Opaque" }
        //LOD 100

        Pass
        {
            Tags{"LightMode" = "Always"}
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            // make fog work
            //#pragma multi_compile_fog

            #include "UnityCG.cginc"

            struct appdata
            {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f
            {
                float2 uv : TEXCOORD0;

                UNITY_FOG_COORDS(1)

                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            fixed4 _Color;
            fixed _FadeRange;

            v2f vert (appdata v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = v.uv.xy;
                o.color = v.color;

                //o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                //UNITY_TRANSFER_FOG(o,o.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
                // sample the texture
                fixed4 col = tex2D(_MainTex, i.uv);
                col = fixed4(max(_FadeRange, col.r), max(_FadeRange, col.g), max(_FadeRange, col.b), col.a);
                
                // apply fog
                //UNITY_APPLY_FOG(i.fogCoord, col);

                return col * ((i.color * _Color) * col.a);
            }
            ENDCG
        }
    }
}
