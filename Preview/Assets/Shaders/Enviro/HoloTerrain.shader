Shader "MODev/Environment/HoloTerrain" 
{
	Properties 
	{
		[HDR]_Color("_Color", Color) = (1,1,1,1)
		[HDR]_EmissionColor("_EmissionColor", Color) = (1,1,1,1)
		[HDR]_EmissionColorOnDanger("_EmissionColorOnDanger", Color) = (1,1,1,1)
		_RandomMaxColorStep("_RandomMaxColorStep", Range(0, 1)) = 0.7

		_HeightToColorPow("_HeightToColorPow", Float) = 2
		_HeightFactor("_HeightFactor", Float) = 1
		[HideInInspector]_HeightMap("_HeightMap", 2D) = "white" {}
		[HideInInspector]_HeightMapParams("_HeightMapParams", Vector) = (0,0,1,1)

		[HDR]_EmissionColorOnShipClose("_EmissionColorOnShipClose", Color) = (1,1,1,1)
		_ShipDistanceTolerance("_ShipDistanceTolerance", Float) = 5
		_ShipCloseIntensity("_ShipCloseIntensity", Float) = 2
	}
	SubShader
	{
		Tags { "RenderType" = "Opaque" "RenderQueue" = "Geometry" }

		Pass
		{
			Tags { "LightMode" = "Deferred" }
			CGPROGRAM
			#pragma target 4.0
			#pragma vertex vert
			#pragma geometry geom
			#pragma fragment frag
			#pragma exclude_renderers xbox360 ps3 flash
			#pragma multi_compile_prepassfinal noshadowmask nodynlightmap nodirlightmap nolightmap

			#include "UnityCG.cginc"
			#include "UnityGBuffer.cginc"
			#include "UnityStandardUtils.cginc"

			float _HeightFactor;
			float4 _EmissionColor;//HDR
			float4 _EmissionColorOnDanger;//HDR
			float4 _Color;//HDR
			float _RandomMaxColorStep;
			float _HeightToColorPow;
			sampler2D _HeightMap;
			float4 _HeightMapParams;//x,y world map begin, z,w - world map width and depth
			float3 _EmissionColorOnShipClose;//HDR
			float _ShipDistanceTolerance;
			float _ShipCloseIntensity;
			float3 _Global_ShipPosition;//global property set by Ship's effect emmiter

			struct VertexInput
			{
				float4 position : POSITION;
				half4 color : COLOR;
			};

			struct VertToGeom
			{
				float4 worldPos : POSITION;
				half4 color : TEXCOORD1;
			};

			VertToGeom vert(VertexInput input)
			{
				VertToGeom output;
				output.worldPos = mul(unity_ObjectToWorld, input.position);
				output.color = input.color;
				return output;
			}

			struct VertToFrag
			{
				float4 position : POSITION;
				float3 worldNorm : TEXCOORD0;
				float3 worldPos : TEXCOORD1;
				float4 vertCol : TEXCOORD2;
				half3 ambient : TEXCOORD3;
				fixed heightMap : TEXCOORD4;
			};

			VertToFrag geomVert(float3 worldPos, fixed3 worldNorm, fixed heightMap, float4 vertCol)
			{
				VertToFrag output;
				output.position = UnityWorldToClipPos(float4(worldPos, 1));
				output.worldPos = worldPos;
				output.worldNorm = worldNorm;
				output.ambient = ShadeSHPerVertex(worldNorm, 0);
				output.vertCol = vertCol;
				output.heightMap = heightMap;

				return output;
			}


			inline float3 pointsToNormal(float3 p0, float3 p1, float3 p2)
			{
				return normalize(cross(p1 - p0, p2 - p0));
			}

			void addQuad(float3 wp0, float3 wp1, float3 wp2, float3 wp3, fixed heightMap, float4 vertCol, inout TriangleStream<VertToFrag> outStream)
			{
				float3 wNorm = pointsToNormal(wp0, wp1, wp2);
				outStream.Append( geomVert(wp0, wNorm, heightMap, vertCol) );
				outStream.Append( geomVert(wp1, wNorm, heightMap, vertCol) );
				outStream.Append( geomVert(wp2, wNorm, heightMap, vertCol) );
				outStream.Append( geomVert(wp3, wNorm, heightMap, vertCol) );
				outStream.RestartStrip();
			}

			[maxvertexcount(15)]
			void geom(triangle VertToGeom input[3], uint pid : SV_PrimitiveID, inout TriangleStream<VertToFrag> outStream)
			{
				float heightMap = tex2Dlod(_HeightMap, float4((input[0].worldPos.x - _HeightMapParams.x) / _HeightMapParams.z, 
															  (input[0].worldPos.z - _HeightMapParams.y) / _HeightMapParams.w, 0, 0)).r;
				float height = _HeightFactor * heightMap;

				float3 wpOffset = float3(0, height, 0);
				float3 wp0 = input[0].worldPos + wpOffset;
				float3 wp1 = input[1].worldPos + wpOffset;
				float3 wp2 = input[2].worldPos + wpOffset;
				
				float3 wp0Down = input[0].worldPos;
				float3 wp1Down = input[1].worldPos;
				float3 wp2Down = input[2].worldPos;

				float3 upWNorm = float3(0, 1, 0);
				outStream.Append(geomVert(wp0, upWNorm, heightMap, input[0].color));
				outStream.Append(geomVert(wp1, upWNorm, heightMap, input[0].color));
				outStream.Append(geomVert(wp2, upWNorm, heightMap, input[0].color));
				outStream.RestartStrip();

				addQuad(wp1Down, wp1, wp0Down, wp0, heightMap, input[0].color, outStream);
				addQuad(wp2Down, wp2, wp1Down, wp1, heightMap, input[0].color, outStream);
				addQuad(wp0Down, wp0, wp2Down, wp2, heightMap, input[0].color, outStream);
			}

			void frag(  VertToFrag input,
						out half4 outGBuffer0 : SV_Target0,//diffuse, occlusion
						out half4 outGBuffer1 : SV_Target1,//spec(rgb), roughtness
						out half4 outGBuffer2 : SV_Target2,//wNormal, a- unused
						out half4 outGBuffer3 : SV_Target3//emission
					 )
			{
				float colorIntensityFactor = pow(saturate(input.heightMap * 2), _HeightToColorPow) * lerp(1, _RandomMaxColorStep, input.vertCol.g);

				UnityStandardData data;
				data.diffuseColor = _Color/* * colorIntensityFactor*/;
				data.occlusion = 1;
				data.specularColor = 0;;
				data.smoothness = 0;
				data.normalWorld = input.worldNorm;
				UnityStandardDataToGbuffer(data, outGBuffer0, outGBuffer1, outGBuffer2);

				half isShipOverTerrain = pow(saturate(_Global_ShipPosition.y - input.worldPos.y), 4);
				half3 emissionColor = (_EmissionColor.rgb * isShipOverTerrain + _EmissionColorOnDanger * (1 - isShipOverTerrain)) * colorIntensityFactor;
				float distToShip = length(input.worldPos - _Global_ShipPosition);// +lerp(0, _RandomMaxColorStep, input.vertCol.g);//this is for pixelisation the radius but smoth looked better
				float emissionLerp = saturate(saturate(1 - distToShip / _ShipDistanceTolerance) * _ShipCloseIntensity);
				emissionColor = lerp(emissionColor, _EmissionColorOnShipClose, emissionLerp);
				outGBuffer3 = half4(input.ambient + emissionColor, 1);

			}

			ENDCG
		}
    }
	FallBack "Diffuse"
}
