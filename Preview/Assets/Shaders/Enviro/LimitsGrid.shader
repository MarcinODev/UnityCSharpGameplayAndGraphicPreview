Shader "MODev/Environment/LimitsGrid"
{
	Properties
	{
		[HDR]_Color("_Color", Color) = (1,1,1,1)
		_MainTex("_MainTex", 2D) = "white" {}
		_ShipDistanceToleranceXZ("_ShipDistanceToleranceXZ", Float) = 5
		_ShipDistanceToleranceY("_ShipDistanceToleranceY", Float) = 5
		_ShipCloseIntensity("_ShipCloseIntensity", Float) = 2
	}
	SubShader
	{
		Tags { "RenderType" = "Transparent" "Queue" = "Transparent+1000" }
		ZWrite Off
		Cull Off
		Blend SrcAlpha OneMinusSrcAlpha

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct Appdata
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct VertToFrag
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
			};

			sampler2D _MainTex;
			float4 _MainTex_ST;
			float4 _Color;//HDR
			float _ShipDistanceToleranceXZ;
			float _ShipDistanceToleranceY;
			float _ShipCloseIntensity;
			float3 _Global_ShipPosition;//global property set by Ship's effect emmiter
			
			VertToFrag vert(Appdata v)
			{
				VertToFrag o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = TRANSFORM_TEX(v.uv, _MainTex);
				o.worldPos = mul(unity_ObjectToWorld, v.vertex);
				return o;
			}
			
			float4 frag(VertToFrag input) : SV_Target
			{
				float distToShipXZ = length(input.worldPos.xz - _Global_ShipPosition.xz);
				float distVisibilityByXZ = saturate(1 - distToShipXZ / _ShipDistanceToleranceXZ);
				float distToShipY = abs(input.worldPos.y - _Global_ShipPosition.y);
				float distVisibilityByY = saturate(1 - distToShipY / _ShipDistanceToleranceY);
				
				float visibility = saturate( pow(distVisibilityByXZ * distVisibilityByY, _ShipCloseIntensity) );				
				clip(visibility - 0.0001);
				
				float4 col = tex2D(_MainTex, input.uv) * _Color;
				col.a *= visibility;
				return col;
			}
			ENDCG
		}
	}
}
