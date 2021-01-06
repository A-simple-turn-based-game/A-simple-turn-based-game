// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "beffio/ParticleAlphaBlendStroke"
{
Properties
{
	_MainTex ("MainTexture", 2D) = "white" {}
	_TintColor ("Tint Color", Color) = (0.5,0.5,0.5,0.5)
}

Category
{
	Tags { 
	"Queue"="Transparent" 
	"IgnoreProjector"="True" 
	"RenderType"="Transparent" 
	}
	
	Blend SrcAlpha OneMinusSrcAlpha
	AlphaTest Greater .01
	Cull Off 
	Lighting Off 
	ZWrite Off
	
	SubShader
	{
		Pass
		{
		
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			#pragma multi_compile_particles
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			fixed4 _TintColor;
			
			struct v2f
			{
				float4 vertex : POSITION;
				float3 normal : TEXCOORD3;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			struct appdata_a
			{
				float4 vertex : POSITION;
				float3 normal : NORMAL;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			float4 _MainTex_ST;
			
			v2f vert (appdata_a v)
			{
				v2f g;
				g.vertex = UnityObjectToClipPos(v.vertex);
				g.color = v.color;
				g.texcoord = TRANSFORM_TEX(v.texcoord,_MainTex);
				g.normal = v.normal;
				
				return g;
			}
			
			sampler2D _CameraDepthTexture;
			
			fixed4 frag (v2f i) : COLOR
			{
				return 2.0f * i.color * _TintColor * tex2D(_MainTex, i.texcoord).a;
			}
			ENDCG 
		}
	} 	
	
}
}
