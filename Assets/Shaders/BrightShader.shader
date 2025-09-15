Shader "Custom/GlowShader_NoSortConflict" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Brightness ("Brightness", Range(1, 3)) = 1.5 // ��������
    }
    SubShader {
        Tags { 
            "Queue"="Transparent" // �ؼ�������ͨSpriteһ�µ���Ⱦ����
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "IgnoreProjector"="True"
        }

        LOD 100
        ZWrite Off // ͸������ر����д�루���⵲ס�����͸�����壩
        Cull Off // 2D����رձ����޳�
        Blend SrcAlpha OneMinusSrcAlpha // ��׼͸�����ģʽ��ȷ���ڵ���ȷ��

        Pass {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #include "UnityCG.cginc"

            struct appdata {
                float4 vertex : POSITION;
                float2 uv : TEXCOORD0;
                float4 color : COLOR;
            };

            struct v2f {
                float2 uv : TEXCOORD0;
                float4 vertex : SV_POSITION;
                float4 color : COLOR;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
            fixed4 _Color;
            fixed _Brightness;

            v2f vert (appdata v) {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
                o.uv = TRANSFORM_TEX(v.uv, _MainTex);
                o.color = v.color * _Color;
                // �ؼ���ǿ��Z��Ϊ0�����⶯�����Zֵ���������
                o.vertex.z = 0; 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                
                // ����͸�����򷢹⣬����͸����Ե�����ڵ�
                if (col.a > 0.9) {
                    col.rgb *= _Brightness;
                    col.rgb = saturate(col.rgb);
                }
                
                return col;
            }
            ENDCG
        }
    }
}