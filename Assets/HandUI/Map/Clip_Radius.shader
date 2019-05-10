
//    MIT License
//    
//    Copyright (c) 2017 Dustin Whirle
//    
//    My Youtube stuff: https://www.youtube.com/playlist?list=PL-sp8pM7xzbVls1NovXqwgfBQiwhTA_Ya
//    
//    Permission is hereby granted, free of charge, to any person obtaining a copy
//    of this software and associated documentation files (the "Software"), to deal
//    in the Software without restriction, including without limitation the rights
//    to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
//    copies of the Software, and to permit persons to whom the Software is
//    furnished to do so, subject to the following conditions:
//    
//    The above copyright notice and this permission notice shall be included in all
//    copies or substantial portions of the Software.
//    
//    THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
//    IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
//    FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
//    AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
//    LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
//    OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
//    SOFTWARE.

// clips within a radius

Shader "Clip/Radius" {
    Properties {
    
      _Radius ("Radius (World Space)", float) = 0.25
      
      _LineFreq("Line Frequency", Range(0,100)) = 0.1
      _LineOffset("LineOffset", float) = 0
      _LineColor("LineColor", Color) =(1,1,1,1)
      
      _Origin ("Origin (World Space)", Vector) = (0,0,0,0)
      _ConvertEmission ("Convert Emission", Range(0,1)) = 0.5
      _ConvertDistance ("Conversion Distance", float) = 0.1
      _Conversion ("Conversion (RGB)", 2D) = "white" {}
    
      _Tint("Color", Color) =(1,1,1,1)
      _MainTex ("Main Texture", 2D) = "white" {}
      _BumpMap ("Bumpmap", 2D) = "bump" {}
      _Glossiness ("Smoothness", Range(0,1)) = 0.5
	    _Metallic ("Metallic", Range(0,1)) = 0.0
	  
    }
    SubShader {
      // Pass{

      // }

      // Pass{
 Tags { "RenderType" = "Opaque" }
      Cull Off
      CGPROGRAM
    //   #pragma vertex vertex_shader

      // Physically based Standard lighting model, and enable shadows on all light types
	  #pragma surface surf Standard fullforwardshadows

	  // Use shader model 3.0 target, to get nicer looking lighting
	  #pragma target 5.0

	//   uniform RWStructuredBuffer<float> buffer : register(u1);

    //   uniform RWStructuredBuffer<float3> data : register(u1);
      // RWStructuredBuffer<float3> Board : register(s[0]);
    //   RWStructuredBuffer<float3> BoardOut : register(s[1]);

      struct SHADERDATA
      {
        float4 vertex : SV_POSITION;
      };

      struct Input {
          float2 uv_MainTex;
          float2 uv_Conversion;
          float2 uv_BumpMap;
          float3 worldPos;
      };

    struct APPDATA
    {
        float4 vertex : POSITION;
        float2 uv : TEXCOORD0;
        uint id : SV_VertexID;      
    };

      half _Glossiness;
	  half _Metallic;
	  half _ConvertDistance;
	  half _ConvertEmission;
      half _Radius;
      
      float _LineFreq;
      float _LineOffset;
      float4 _LineColor;
      float4 _LineThickness;

      float4 _Tint;

      float3 _Origin;
      
      sampler2D _MainTex;
      sampler2D _BumpMap;
      sampler2D _Conversion;

    //   void vert (inout appdata_full v) {
    //       v.vertex.xyz += v.normal * 0.4f;
    //   }

        // SHADERDATA vertex_shader (APPDATA IN)
        // {
        //     SHADERDATA vs;
            
        //     half dist = distance(IN.vertex.xyz, _Origin);
            
        //     if(dist < _Radius){
        //         if(IN.vertex.y < data[0].y){
        //             data[0] = IN.vertex.xyz;
        //         }
        //     }

        //     // IN.vertex.y = 0.0-tex2Dlod(_MainTex,float4(IN.uv,0,0)).r*(sin(_Time.g)*0.5+0.5);
            
        //     // data[IN.id] = IN.vertex.xyz;
        //     vs.vertex = UnityObjectToClipPos(IN.vertex);
        //     vs.uv = IN.uv;
        //     return vs;
        // }

      void surf (Input IN, inout SurfaceOutputStandard o) {
        
          float3 xzPos = IN.worldPos;
          xzPos.y = _Origin.y;

          half dist = distance( IN.worldPos, _Origin);
          half distXZ = distance( xzPos, _Origin);

          
         //clip (frac((IN.worldPos.y+IN.worldPos.z*0.1) * 5) - 0.5);
          clip (_Radius - distXZ);
          
          // min = 0 // value = dist // max = _ConvertDistance
          float convert_mask = (distXZ - _Radius)/ _ConvertDistance;
		      convert_mask = clamp(convert_mask, 0, 1);
          
          fixed4 albedo = tex2D (_MainTex, IN.uv_MainTex) ;
          albedo *= convert_mask;
          
          fixed4 convert = tex2D (_Conversion, IN.uv_Conversion);
          convert *= 1.0 - convert_mask;
          //IN.worldPos.y > _LineFreq * floor(IN.worldPos.y/_LineFreq) && 
          
          float dbot = _LineFreq * floor(IN.worldPos.y/_LineFreq);
          float dtop = _LineFreq * floor(IN.worldPos.y/_LineFreq) + 0.001;

          if(IN.worldPos.y + _LineOffset > dbot && IN.worldPos.y + _LineOffset < dtop)
              o.Emission = _LineColor;
          else
              o.Emission = convert.rgb * _ConvertEmission;

          o.Albedo = (albedo.rgb + convert.rgb)* _Tint;
          o.Normal = UnpackNormal (tex2D (_BumpMap, IN.uv_BumpMap));
          o.Metallic = _Metallic;
		      o.Smoothness = _Glossiness;
		      o.Alpha = albedo.a + convert.a;
          
      }
      ENDCG
      // }
     
    } 
    Fallback "Diffuse"
  }