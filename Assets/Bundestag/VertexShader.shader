Shader "Custom/Vertex color" {
SubShader {
    Pass {
        CGPROGRAM
        #pragma vertex vert
        #pragma fragment frag
        #include "UnityCG.cginc"

        // vertex input: position, color
        struct appdata {
            float4 vertex : POSITION;
            fixed4 color : COLOR;
            float3 normal : NORMAL;

            UNITY_VERTEX_INPUT_INSTANCE_ID

        };

        struct v2f {
            float4 pos : SV_POSITION;
            fixed4 color : COLOR;
            float3 normal : NORMAL;

            UNITY_VERTEX_OUTPUT_STEREO

        };
        
        v2f vert (appdata v) {
            v2f o;
            UNITY_SETUP_INSTANCE_ID(v);
            UNITY_INITIALIZE_OUTPUT(v2f, o);
            UNITY_INITIALIZE_VERTEX_OUTPUT_STEREO(o);
            o.pos = UnityObjectToClipPos(v.vertex );
            o.color = v.color;
            o.normal = v.normal * 0.5 + 0.5;
            return o;
        }
        
        fixed4 frag (v2f i) : SV_Target { return i.color; }
        ENDCG
    }
}
}
