Shader "Custom/Toon" {
Properties {
	_MainTex ("Texture", 2D) = "white" {}
	_Glossiness("Glossiness", Range(0,1)) = 0
	_GlossinessTex ("Glossiness Texture", 2D) = "black" {}
	_Tint("tint", Color) = (1,1,1,1)
	_ToonRamp("Toon Ramp", Range(0,0.2)) = 0.03
	_WriteStencil ("Stencil ID", Float) = 0
	_WriteMask ("Stencil WriteMask", Float) = 255
}

SubShader {
    Tags { 
	"RenderType"="Opaque"
	"Queue"="Geometry"
	}
    LOD 100

	Stencil
	{
		Ref [_WriteStencil]
		Comp Always
		Pass Replace
		WriteMask [_WriteMask]
	}
	ZWrite On

      Pass
        {            
            HLSLPROGRAM            
            #pragma vertex vert            
            #pragma fragment frag

			#pragma multi_compile _SHADOWS_SOFT
			#pragma multi_compile _MAIN_LIGHT_SHADOWS_CASCADE
			#pragma multi_compile _MAIN_LIGHT_SHADOWS
			#pragma multi_compile _ADDITIONAL_LIGHTS

 
            #include "./ToonLighting.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
			
            struct Attributes
            {
                float4 positionOS   : POSITION;
                // Declaring the variable containing the normal vector for each
                // vertex.
                half3 normal        : NORMAL;
				float2 uv           : TEXCOORD0;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
				float2 uv           : TEXCOORD0;
				half3 normal        : TEXCOORD1;
				half3 WorldPos        : TEXCOORD2;
            };           

			half4 _Tint;
			half _Glossiness;
			half _ToonRamp;

			TEXTURE2D(_MainTex);
			SAMPLER(sampler_MainTex);
			 float4 _MainTex_ST;

			TEXTURE2D(_GlossinessTex);
			SAMPLER(sampler_GlossinessTex);

            Varyings vert(Attributes IN)
            {                
                Varyings OUT;  
                OUT.normal = TransformObjectToWorldNormal(IN.normal);    				
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz);     
                OUT.WorldPos = TransformObjectToWorld(IN.positionOS.xyz);     
				OUT.uv = TRANSFORM_TEX(IN.uv, _MainTex);				
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {                      
				half4 color = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, IN.uv);

				half3 toonOutput;
				float3 viewDir = normalize(_WorldSpaceCameraPos.xyz - IN.WorldPos);

				//Glossiness
				half glossiness = SAMPLE_TEXTURE2D(_GlossinessTex, sampler_GlossinessTex, IN.uv).r + _Glossiness;
				
				ToonShading_float(IN.normal,IN.positionHCS,IN.WorldPos,viewDir,glossiness,_ToonRamp,toonOutput);
				color.rgb *= toonOutput;
				
                return color*_Tint;
            }
            ENDHLSL
        }

		// Pass to render object as a shadow caster
		Pass
			{
				Tags{ "LightMode" = "ShadowCaster" }
				CGPROGRAM
				#pragma vertex VSMain
				#pragma fragment PSMain

				float4 VSMain (float4 vertex:POSITION) : SV_POSITION
				{
					return UnityObjectToClipPos(vertex);
				}

				float4 PSMain (float4 vertex:SV_POSITION) : SV_TARGET
				{
					return 0;
				}
			   
				ENDCG
			}
}

}
