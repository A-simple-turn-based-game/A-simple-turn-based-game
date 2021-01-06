Shader "Unlit/Comic"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
		_Diffuse ("COLOR",COLOR) = (1,1,1,1)
		_OutLine ("OutLine",range(0,0.01)) = 0
		_OutLineColor ("OutLineColor",COLOR) = (0,0,0,0) //描边颜色
		_Step ("Step",range(1,10)) = 1 //颜色分层
		_ToonEffect ("ToonEffect" , Range(0,1)) = 0.5 //分层强度
		_RimColor ("RimColor",Color) = (0,0,0,0)
		_RimPower ("RimPower",Range(0.0001,3)) = 1
		_RayColor ("RayColor",Color) = (0,0,0,0)
		_RayPower ("RayPower",Range(0.0001,3)) = 1
    }
    SubShader
    {
        Tags { "Queue" = "Geometry+1000" "RenderType"="Opaque" }
        LOD 100
 

		pass{

			Name "XRay"
			ZTest Greater
			ZWrite Off
			Blend SrcColor OneMinusSrcColor
			CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
  
            #include "UnityCG.cginc"
			#include "Lighting.cginc"
			 
			 float4 _RayColor;
			 float _RayPower;

            struct v2f
            {
				float4 vertex : SV_POSITION;
				fixed3 color : TEXCOORD0;
            };
			 
            v2f vert (appdata_base v)
            {
                v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				float3 worldNormal = normalize(UnityObjectToWorldNormal(v.normal));
				float3 worldPos = mul(unity_ObjectToWorld,v.vertex);
				float3 worldviewDir = normalize(UnityWorldSpaceViewDir(worldPos));
				float power = 1 - dot(worldNormal , worldviewDir);
				o.color = _RayColor * pow(power,_RayPower);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            { 
                return fixed4(i.color , 1.0);
            }
            ENDCG
		}

		pass{
			
			Name "OutLine"
			Cull Front
			 CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
  
            #include "UnityCG.cginc"
			#include "Lighting.cginc"

			float _OutLine;
			float4 _OutLineColor;

            struct v2f
            {
				float4 vertex : SV_POSITION;
 
            };
			 
            v2f vert (appdata_base v)
            {
                v2f o;
				v.vertex.xyz += v.normal * _OutLine;
				o.vertex = UnityObjectToClipPos(v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            { 
                return _OutLineColor;
            }
            ENDCG

		}
	 
        Pass
        {

            CGPROGRAM
            #pragma vertex vert
            #pragma fragment frag
  
            #include "UnityCG.cginc"
			#include "Lighting.cginc"
            struct v2f
            {
				float4 vertex : SV_POSITION;
                float2 uv : TEXCOORD0;
				fixed3 worldNormal : TEXCOORD1;
                float3 worldPos : TEXCOORD2;
            };

            sampler2D _MainTex;
            float4 _MainTex_ST;
			float4 _Diffuse;
			float _Step;
			float _ToonEffect;
			float4 _RimColor;
			float _RimPower;

            v2f vert (appdata_base v)
            {
                v2f o;
                o.vertex = UnityObjectToClipPos(v.vertex);
				o.worldNormal = UnityObjectToWorldNormal(v.normal);
                o.uv = TRANSFORM_TEX(v.texcoord, _MainTex);
                o.worldPos = mul(unity_ObjectToWorld,v.vertex);
                return o;
            }

            fixed4 frag (v2f i) : SV_Target
            {
 
                fixed4 col = tex2D(_MainTex, i.uv);

				float3 LightDir = normalize( UnityWorldSpaceLightDir(i.worldPos));
				float3 ViewDir = normalize(UnityWorldSpaceViewDir(i.worldPos));
				float3 ambient = UNITY_LIGHTMODEL_AMBIENT.xyz * col;
				 
				float3 lambert = dot(i.worldNormal , LightDir) * 0.5 + 0.5;
				
				lambert = smoothstep(0,1,lambert);
				//颜色离散化,在1到0之间划分区域
				float3 toon = floor(lambert * _Step) / _Step; 
				//通过ToonEffect控制 最终呈现出来的效果
				lambert = lerp(toon,lambert,_ToonEffect);

				float3 diffuse = _LightColor0.rgb * col * lambert ;
				
				float Rim = 1 - dot(i.worldNormal,ViewDir);
				float3 rcolor = _RimColor *  pow(Rim,1/_RimPower); 
                return float4( ambient + diffuse + rcolor , 1.0);
            }
            ENDCG
        }
    }
}
