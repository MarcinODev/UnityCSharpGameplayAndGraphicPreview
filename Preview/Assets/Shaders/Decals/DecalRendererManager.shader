Shader "MODev/Decals/InvisibleManager"
{
	Properties
	{
	}
	SubShader
	{
		Tags { "RenderType"="Opaque" }

		Pass
		{
			CGPROGRAM
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"

			struct Appdata
			{
				float4 vertex : POSITION;
			};

			struct VertToFrag
			{
				float4 vertex : SV_POSITION;
			};
			
			VertToFrag vert(Appdata v)
			{
				VertToFrag o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				return o;
			}
			
			fixed4 frag (VertToFrag input) : SV_Target
			{
				clip(-1);
				return fixed4(0,0,0,0);
			}
			ENDCG
		}
	}
}
