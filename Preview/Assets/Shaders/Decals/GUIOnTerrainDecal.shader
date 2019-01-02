Shader "MODev/Decals/GUIOnTerrainDecal"
{
	Properties
	{
		[HDR]_Color("_Color", Color) = (1,1,1,1)
		[HDR]_EmissionColor("_EmissionColor", Color) = (0,0,0,0)
		_MainTex("_MainTex", 2D) = "black" {}
		_NormalsFromUI("_NormalsFromUI", Float) = 0
		_BumpingNormalsByUI("_BumpingNormalsByUI", Float) = 1
		_VisibilityByNormalDetolerance("_VisibilityByNormalDetolerance (the bigger the less normals will accept)", Float) = 0.1
		_NormalsToUI("_NormalsToUI", Float) = 0.001
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		Fog { Mode Off }
		ZWrite Off

		Pass
		{
			CGPROGRAM
			#pragma target 3.0
			#pragma vertex vert
			#pragma fragment frag
			#pragma exclude_renderers nomrt
			#include "UnityCG.cginc"
							
			float4 _Color;//HDR
			float4 _EmissionColor;//HDR
			sampler2D _MainTex;
			float _NormalsFromUI;
			float _BumpingNormalsByUI;
			float _VisibilityByNormalDetolerance;
			float _NormalsToUI;

			sampler2D _GBufferNormals;
			sampler2D_float _CameraDepthTexture;
			
			struct VertToFrag
			{
				float4 pos : SV_POSITION;
				half2 uv : TEXCOORD0;
				float4 screenUV : TEXCOORD1;
				float3 ray : TEXCOORD2;
				half3 orientation : TEXCOORD3;
				half3 orientationX : TEXCOORD4;
				half3 orientationZ : TEXCOORD5;
			};

			VertToFrag vert(float3 v : POSITION)
			{
				VertToFrag o;
				o.pos = UnityObjectToClipPos(float4(v,1));
				o.uv = v.xz + 0.5;
				o.screenUV = ComputeScreenPos(o.pos);
				o.ray = UnityObjectToViewPos(float4(v,1)).xyz * float3(-1,-1,1);
				o.orientation = mul((float3x3)unity_ObjectToWorld, float3(0,1,0));
				o.orientationX = mul((float3x3)unity_ObjectToWorld, float3(1,0,0));
				o.orientationZ = mul((float3x3)unity_ObjectToWorld, float3(0,0,1));
				return o;
			}

			void frag(VertToFrag input, out half4 outDiffuse : COLOR0, out half4 outNormal : COLOR1, out half4 outEmission : COLOR2)
			{
				input.ray = input.ray * (_ProjectionParams.z / input.ray.z);
				float2 uv = input.screenUV.xy / input.screenUV.w;
				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, uv);
				depth = Linear01Depth(depth);
				float4 vpos = float4(input.ray * depth,1);
				float3 wpos = mul(unity_CameraToWorld, vpos).xyz;
				float3 opos = mul(unity_WorldToObject, float4(wpos, 1)).xyz;

				clip(float3(0.5,0.5,0.5) - abs(opos.xyz));
				
				input.uv = opos.xz + 0.5;

				half3 normal = tex2D(_GBufferNormals, uv).rgb;
				half3 wNorm = normal.rgb * 2.0 - 1.0;
				clip(dot(wNorm, input.orientation) - _VisibilityByNormalDetolerance);

				float4 col = tex2D(_MainTex, uv + wNorm.xz * _NormalsToUI);
				clip(col.a - 0.01);

				outDiffuse = col * _Color;
				outEmission = half4((_EmissionColor.rgb * col.rgb) * col.a, 1);

				half3 normalFromUI = normalize(lerp(wNorm, col.rgb, _NormalsFromUI));
				half3 newNormal = lerp(normalFromUI, normalFromUI * _BumpingNormalsByUI, col.a);
				outNormal = half4((newNormal + 1.0) * 0.5, 1);

			}
			ENDCG
		}
	}
}
