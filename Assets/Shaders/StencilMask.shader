Shader "Custom/StencilMask"
{
Properties{
}

SubShader{

 ColorMask 0
Tags { 
 "RenderType" = "Opaque" 
 }
 
 Pass{
	ZWrite Off
	Stencil
             {
                 Ref 1
                 Comp Always
                 Pass Replace
             }   

		}
	}
}