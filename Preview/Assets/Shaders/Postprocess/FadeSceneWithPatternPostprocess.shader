Shader "MODev/Postprocess/FadeSceneWithPattern"
{
	Properties
	{
		[HideInInspector]_MainTex("_MainTex", 2D) = "white" {}
		_PatternTex("_PatternTex (needs to contain dissolve gradient-grayscale)", 2D) = "white" {}
		_Progress("_Progress", Range(0,1)) = 0
		_MinDepth("_MinDepth (when dissolve starts)", Range(0,1)) = 0
		_MaxDepth("_MaxDepth (when dissolve ends)", Range(0,1)) = 1
		[HDR]_DissolveColor("_Color (of dissolve border)", Color) = (1, 1, 1, 1)
		_DissolveBorder("_DissolveBorder", Range(-1,1)) = 0
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
			sampler2D _PatternTex;
			float4 _PatternTex_ST;
			float4 _Color;
			fixed _Progress;
			float _MinDepth;
			float _MaxDepth;
			float4 _DissolveColor;//HDR
			fixed _DissolveBorder;

			sampler2D_float _CameraDepthTexture;
			float4 _CameraDepthTexture_ST;

			struct Appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct VertToFrag
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			VertToFrag vert(Appdata v)
			{
				VertToFrag o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}			

			float4 frag(VertToFrag input) : SV_Target
			{
				float4 mainCol = tex2D(_MainTex, input.uv);

				float depth = SAMPLE_DEPTH_TEXTURE(_CameraDepthTexture, input.uv.xy);
				depth = (Linear01Depth(depth) - _MinDepth) / (_MaxDepth - _MinDepth);

				float2 patternUV = float2(input.uv.x, depth) * _PatternTex_ST.xy + _PatternTex_ST.zw;
				fixed4 pattern = tex2D(_PatternTex, patternUV);

				fixed dissolve = (1 - pattern.r)/10 + (patternUV.y - frac(patternUV.y))/100 - _Progress;

				fixed invisibility = step(dissolve, -0.0001);
				float4 col = lerp(mainCol, float4(0,0,0,1), invisibility);
				float3 dissolveCol = _DissolveColor * lerp(1, length(mainCol.rgb), _DissolveColor.a);
				col.rgb = lerp(col.rgb, dissolveCol, step(_DissolveBorder, dissolve) * invisibility);

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
			
			sampler2D _MainTex;

			struct Appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct VertToFrag
			{
				float4 pos : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			VertToFrag vert (Appdata v)
			{
				VertToFrag o;
				o.pos = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			float4 frag (VertToFrag input) : SV_Target
			{
				float4 col = tex2D(_MainTex, input.uv);
				return col;
			}
			ENDCG
		}
	}
}
