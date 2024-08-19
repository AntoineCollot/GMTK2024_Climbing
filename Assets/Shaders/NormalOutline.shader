Shader "Custom/NormalOutline" {
Properties {
	_OutlineWidth("Outline Width", Range(0,0.2)) = 0.1
	_OutlineColor("Outline Color", Color) = (1,1,1,1)
	_Stencil ("Stencil ID", Float) = 0
}

SubShader {
    Tags { 
	"RenderType"="Opaque"
	"Queue"="Geometry+1"
	}
    LOD 100

	Stencil
	{
		Ref [_Stencil]
		Comp NotEqual
		Pass Replace
		ReadMask 2
		WriteMask 1
	}
	ZWrite Off

      Pass
        {            
            HLSLPROGRAM            
            #pragma vertex vert            
            #pragma fragment frag

            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"            


            struct Attributes
            {
                float4 positionOS   : POSITION;
                // Declaring the variable containing the normal vector for each
                // vertex.
                half3 normal        : NORMAL;
            };

            struct Varyings
            {
                float4 positionHCS  : SV_POSITION;
            };           

			half4 _OutlineColor;
			half _OutlineWidth;

            Varyings vert(Attributes IN)
            {                
                Varyings OUT;  
                //OUT.normal = TransformObjectToWorldNormal(IN.normal);    				
                OUT.positionHCS = TransformObjectToHClip(IN.positionOS.xyz + IN.normal * _OutlineWidth);       
                return OUT;
            }

            half4 frag(Varyings IN) : SV_Target
            {                             
                return _OutlineColor;
            }
            ENDHLSL
        }
}

}
