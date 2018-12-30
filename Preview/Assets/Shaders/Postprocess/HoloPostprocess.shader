Shader "MODev/Postprocess/Holo"
{
	Properties
	{
		[HideInInspector]_MainTex("_MainTex", 2D) = "white" {}
		_Color("_Color (alpha is colorisation power)", Color) = (1, 1, 1, 0.1)
		_ScanLineTiling("_ScanLineTiling", Float) = 0.1
		_ScanLineSpeed("_ScanLineSpeed", Float) = 1
		_GlowTiling("_GlowTiling", Float) = 0.1
		_GlowSpeed("_GlowSpeed", Float) = 1
	}
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Fog { Mode Off } Blend Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma target 3.0//this is to let me know when shader will game too many operations - nothing from 4.0 or 5.0 is used
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#include "UnityCG.cginc"

			sampler2D _MainTex;
			float4 _Color;
			float _ScanLineTiling;
			float _ScanLineSpeed;
			float _GlowTiling;
			float _GlowSpeed;

			float4x4 _ClipToWorld;//set by script

			sampler2D_float _CameraDepthTexture;
			float4 _CameraDepthTexture_ST;

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldDir : TEXCOORD1;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				o.worldDir = mul(_ClipToWorld, float4(o.pos.xy, 0.0, 1.0)) - _WorldSpaceCameraPos;
				return o;
			}			

			float4 frag(v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);

				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, i.uv.xy);
				depth = LinearEyeDepth(depth);
				float3 worldPos = i.worldDir * depth + _WorldSpaceCameraPos;
				float scanLine = step(frac(worldPos.z * _ScanLineTiling + _Time.y * _ScanLineSpeed), 0.1);
				float glow = frac(worldPos.z * _GlowTiling + _Time.y * _GlowSpeed);
				col.rgb = lerp(col.rgb, col.rgb * _Color, saturate(scanLine + glow));
				return col;
				
			}
			ENDCG
		}
	}

		
	SubShader
	{
		Cull Off ZWrite Off ZTest Always
		Fog { Mode Off } Blend Off

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			#pragma fragmentoption ARB_precision_hint_fastest
			
			#include "UnityCG.cginc"

			struct appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdata v)
			{
				v2f o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}
			
			sampler2D _MainTex;

			float4 frag (v2f i) : SV_Target
			{
				float4 col = tex2D(_MainTex, i.uv);
				return col;
			}
			ENDCG
		}
	}
}
