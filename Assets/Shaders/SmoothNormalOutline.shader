// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

// Unlit shader. Simplest possible colored shader.
// - no lighting
// - no lightmap support
// - no texture

Shader "Hidden/SmoothNormalOutline" {
Properties {
    _Color ("Main Color", Color) = (1,1,1,1)
	_OutlineWidth("Outline Width", float) = 0.1
    _OutlineColor("Outline Color", Color) = (0,0,0)
}

SubShader {
    Tags { "RenderType"="Opaque" }
    LOD 100

    Pass {
		Cull Front
		ZTest Always
		ZWrite Off
		
        CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
            #pragma target 2.0
            #pragma multi_compile_fog

            #include "UnityCG.cginc"

             half _OutlineWidth;
			 fixed4 _OutlineColor;
			 half _Offset_Z;

			 struct appdata
			 {
				 float4 vertex : POSITION;
				 float3 normal:NORMAL;
				 float4 tangent:TANGENT;
				 float2 uv : TEXCOORD0;
				 float2 uvRef : TEXCOORD6;
				 float3 bakedNormal:TEXCOORD7;
			 };

			 struct v2f
			 {
				 float4 pos : SV_POSITION;
				 float3 normalDir : TEXCOORD1;
				 float3 debugCol : TEXCOORD2;
			 };
			 
			  float3 OctahedronToUnitVector(float2 Oct)
			 {
				 float3 N = float3(Oct, 1 - dot(1, abs(Oct)));
				 if (N.z < 0)
				 {
					 N.xy = (1 - abs(N.yx)) * (N.xy >= 0 ? float2(1, 1) : float2(-1, -1));
				 }
				 return normalize(N);
			 }

			 float3 TransformTBN(float2 bakedNormal, float3x3 tbn)
			 {
				 float3 normal =
					 OctahedronToUnitVector(bakedNormal);
					 //float3(bakedNormal, 0);
				 //normal.z = sqrt(1.0 - saturate(dot(normal.xy, normal.xy)));
				 return  (mul(normal, tbn));
			 }

             v2f vert(appdata v)
			 {
				 v2f o;
				 float3 normalOS = normalize(v.normal);
				 float3 tangentOS = v.tangent;
				 tangentOS = normalize(tangentOS);
				 float3 bitangentOS = normalize(cross(normalOS, tangentOS) * v.tangent.w);
				 float3x3 tbn = float3x3(tangentOS, bitangentOS, normalOS);

				 float3 _BakedNormalDir = (TransformTBN(v.bakedNormal, tbn));

				 float4 pos = UnityObjectToClipPos(v.vertex);
/* 				 float Set_OutlineWidth = pos.w * _OutlineWidth;
				 Set_OutlineWidth = min(Set_OutlineWidth, _OutlineWidth);
				 Set_OutlineWidth *= _OutlineWidth;
				 Set_OutlineWidth = min(Set_OutlineWidth, _OutlineWidth) * 0.2; */
				
				//Auto figures out if should use the model's normals or the baked smoothed uvs(7)
				//Looks if the uv7 are the same as uv6, since if uv7 is not set, they take the value of last set uv. if 6==7 then uv7 not set.
				 float distToBaseUV = distance(v.bakedNormal.xy, v.uvRef.xy);
				 float3 Set_NormalDir = lerp(v.normal, _BakedNormalDir, step(0.01,distToBaseUV));
				 o.debugCol =half3(v.bakedNormal.xy, 1);
		
				 //Animate outline width
				 half outlineWidth = _OutlineWidth* 0.2;
				 outlineWidth *= (cos(_Time.y * 10)-1) * 0.1 +1;
				 
				 o.pos = UnityObjectToClipPos(v.vertex + Set_NormalDir * outlineWidth);

				 return o;
			 }

			 fixed4 frag(v2f i) : SV_Target
			 {
				 //Animate color
				 half luminance = _OutlineColor.r* 0.3+ _OutlineColor.g * 0.59 +_OutlineColor.b + 0.11;
				 return _OutlineColor+ ((cos(_Time.y * 10)+1) * 0.05 * luminance);
			 }
        ENDCG
    }
}

}
