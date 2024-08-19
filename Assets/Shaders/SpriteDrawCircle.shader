// Unity built-in shader source. Copyright (c) 2016 Unity Technologies. MIT license (see license.txt)

Shader "Custom/Sprite/DrawCircle"
{
    Properties
    {
        [PerRendererData] _MainTex ("Sprite Texture", 2D) = "white" {}
        _Color ("Tint", Color) = (1,1,1,1)
        [MaterialToggle] PixelSnap ("Pixel snap", Float) = 0
        [HideInInspector] _RendererColor ("RendererColor", Color) = (1,1,1,1)
        [HideInInspector] _Flip ("Flip", Vector) = (1,1,1,1)
        [PerRendererData] _AlphaTex ("External Alpha", 2D) = "white" {}
        [PerRendererData] _EnableExternalAlpha ("Enable External Alpha", Float) = 0
		
		//Circle
		_CircleRadius ("Radius",Range(0,1)) = 1
		_CircleWidth("Circle Width",Range(0,1)) = 1
		_CircleFade("Circle Fade",Range(0,1)) = 0.03
    }

    SubShader
    {
        Tags
        {
            "Queue"="Transparent"
            "IgnoreProjector"="True"
            "RenderType"="Transparent"
            "PreviewType"="Plane"
            "CanUseSpriteAtlas"="True"
        }

        Cull Off
        Lighting Off
        ZWrite Off
        Blend One OneMinusSrcAlpha

        Pass
        {
        CGPROGRAM
			#pragma vertex SpriteVert
            #pragma fragment SpriteFragNoise
            #pragma target 2.0
            #pragma multi_compile_instancing
            #pragma multi_compile_local _ PIXELSNAP_ON
            #pragma multi_compile _ ETC1_EXTERNAL_ALPHA
            #include "UnitySprites.cginc"
			
			//Circle
			half _CircleRadius;
			half _CircleWidth;
			half _CircleFade;
			
			fixed4 SpriteFragNoise(v2f IN) : SV_Target
			{
				fixed4 c = SampleSpriteTexture (IN.texcoord) * IN.color;
				
				//circle
				half effectiveRadius = _CircleRadius;
				half dist = length(IN.texcoord-float2(0.5,0.5));
				c.a *= smoothstep(dist, dist+_CircleFade+0.00001,effectiveRadius*.5);
				half threshold = effectiveRadius*.5 * (1-_CircleWidth);
				c.a *= smoothstep(threshold-_CircleFade-0.00001,threshold, dist * (1-_CircleFade*2));
				
				half greyscale = c.r + c.g+c.b;
				c.rgb *= c.a;
				return c;
			}
			
        ENDCG
        }
    }
}
