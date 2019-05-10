Shader "CatExplorer/HandReactiveUnlit" {
  Properties {
    [NoScaleOffset]
    _ProximityGradient("Proximity Gradient", 2D) = "white" {}
    _ProximityMapping("Map: DistMin, DistMax, GradMin, GradMax", Vector) = (0, 0.04, 1, 0)
  }
  SubShader {
    Tags { }
    LOD 100

    Pass {
      CGPROGRAM
      #pragma vertex vert
      #pragma fragment frag
      #pragma multi_compile_instancing
      #include "UnityCG.cginc"
      #include "Assets/AppModules/TodoUMward/Shader Hand Data/Resources/HandData.cginc"

      struct appdata {
        float4 vertex : POSITION;
        UNITY_VERTEX_INPUT_INSTANCE_ID
      };

      struct v2f {
        float4 vertex : SV_POSITION;
        float4 worldPos : TEXCOORD0;
      };

      sampler2D _ProximityGradient;
      float4 _ProximityMapping;
      
      v2f vert (appdata v) {
        UNITY_SETUP_INSTANCE_ID(v);

        v2f o;
        o.vertex = UnityObjectToClipPos(v.vertex);
        o.worldPos = mul(unity_ObjectToWorld, v.vertex);
        return o;
      }
      
      fixed4 frag (v2f i) : SV_Target {
        return evalProximityColor(i.worldPos, _ProximityGradient, _ProximityMapping);
      }
      ENDCG
    }
  }
  Fallback Off
}
