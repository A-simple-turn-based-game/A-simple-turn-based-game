Shader "Unlit/Character"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Bump ("Bump",2D) = "Bump"{}
		_BumpScale ("BumpScale" , Range(0,10)) = 3
		_Diffuse ("Diffuse", Color) = (1,1,1,1)
		_AmbientScale("AmbientScale",range(0,1)) = 0.1
		//_OutLine ("OutLine",Range(0,0.1)) = 0
		//_OutLineColor ("OutLineColor" , Color) = (0,0,0,0)
		//_Step ("Step" , Range(0,5)) = 1
		//_ToonEffect ("ToonEffect" , Range(0,1)) = 1
 
		//_Snow("Snow",range(0,1)) = 1
		//_SnowDir("SnowDir" , vector) = (0,1,0)
		//_SnowColor("SnowColor",COLOR) = (1,1,1,1)

	}
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 100

		//UsePass "Unlit/Comic/OutLine"

        Pass
        {
            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag

			//#pragma multi_compile __ SNOW_ON
			#pragma multi_compile_fog
            #include "UnityCG.cginc"
			#include "Lighting.cginc"
			 
            struct v2f
            {

                float4 vertex : SV_POSITION;
                float4 uv : TEXCOORD0; 
				float4 t2w0 : TEXCOORD1;
				float4 t2w1 : TEXCOORD2;
				float4 t2w2 : TEXCOORD3;
				UNITY_FOG_COORDS(4)
			};

            sampler2D _MainTex;
            float4 _MainTex_ST;
			sampler2D _Bump;
			float4 _Bump_ST;
			float4 _Diffuse;
			//float4 _SnowDir;
			//float4 _SnowColor;
			//float _Snow;
			float _BumpScale;
			float _AmbientScale;
			//float _Step;
			//float _ToonEffect;

            v2f vert (appdata_tan v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				float3 worldPos = mul(unity_ObjectToWorld,v.vertex);

                o.uv.xy = v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw;
				o.uv.zw = v.texcoord.xy * _Bump_ST.xy + _Bump_ST.zw;

				float3 worldNormal = normalize(UnityObjectToWorldDir(v.normal));
				float3 worldTangent = normalize(UnityObjectToWorldDir(v.tangent.xyz));
				float3 worldBinnormal = cross(worldNormal,worldTangent) * v.tangent.w;

				o.t2w0 = float4(worldTangent.x,worldBinnormal.x,worldNormal.x,worldPos.x);
				o.t2w1 = float4(worldTangent.y,worldBinnormal.y,worldNormal.y,worldPos.y);
				o.t2w2 = float4(worldTangent.z,worldBinnormal.z,worldNormal.z,worldPos.z);
                UNITY_TRANSFER_FOG(o,o.vertex);
				return o;
            } 

            fixed4 frag (v2f i) : SV_Target
            {
                float3 worldPos = float3(i.t2w0.w,i.t2w1.w,i.t2w2.w);
                
				float3 viewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				float3 lightDir = normalize(UnityWorldSpaceLightDir(worldPos));

				float3 bump = UnpackNormal(tex2D(_Bump,i.uv.zw) );
				bump.xy *= _BumpScale;
				bump.z = sqrt(1.0 - saturate(dot( bump.xy, bump.xy) ));
				
				bump = normalize( float3 (dot( i.t2w0.xyz,bump ),dot(i.t2w1.xyz,bump),dot(i.t2w2.xyz,bump) )  );

				fixed3 albedo = tex2D(_MainTex, i.uv.xy);
				UNITY_APPLY_FOG(i.fogCoord, albedo); 
				
				fixed3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * albedo;

				//fixed3 toon = dot(bump , lightDir) * 0.5 + 0.5;
				//fixed3 step = smoothstep(0,1,toon);
				//step =  floor( step * _Step )/_Step;
				//step = lerp(step,toon,_ToonEffect);
				//fixed3 diffuse = _LightColor0.rgb * albedo * _Diffuse * step;
				fixed3 diffuse = _LightColor0.rgb * albedo * _Diffuse;
				//#if SNOW_ON

				//if( dot(bump ,_SnowDir) > _Snow)
				//{
				//	diffuse += _SnowColor;
				//}
				
				//#endif
                return fixed4( diffuse  + ambient * _AmbientScale , 1);
            }
            ENDCG
        }
    }
	FallBack "Standard"
}
