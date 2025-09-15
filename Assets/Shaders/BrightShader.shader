Shader "Custom/GlowShader_NoSortConflict" {
    Properties {
        _MainTex ("Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        _Brightness ("Brightness", Range(1, 3)) = 1.5 // 发光亮度
    }
    SubShader {
        Tags { 
            "Queue"="Transparent" // 关键：与普通Sprite一致的渲染队列
            "RenderType"="Transparent" 
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
            "IgnoreProjector"="True"
        }

        LOD 100
        ZWrite Off // 透明物体关闭深度写入（避免挡住后面的透明物体）
        Cull Off // 2D物体关闭背面剔除
        Blend SrcAlpha OneMinusSrcAlpha // 标准透明混合模式（确保遮挡正确）

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
                // 关键：强制Z轴为0（避免动画误改Z值导致排序错）
                o.vertex.z = 0; 
                return o;
            }

            fixed4 frag (v2f i) : SV_Target {
                fixed4 col = tex2D(_MainTex, i.uv) * i.color;
                
                // 仅不透明区域发光，避免透明边缘干扰遮挡
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