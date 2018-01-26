

Shader "Custom/Billboard" 
{
	Properties 
	{
		[HDR] _Color ("color", Color) = (1,1,1,1)
		_Size ("Size", float) = 0.5
	}

	SubShader 
	{
		Pass
		{
			Tags { "RenderType"="Transparent" }
			// Blend One One // Additive
		    // Ztest Always
			LOD 200
		
			CGPROGRAM
				#pragma target 5.0
				#pragma vertex VS_Main
				#pragma fragment FS_Main
				#pragma geometry GS_Main
				#include "UnityCG.cginc" 

				struct pointData
				{
					float4	pos		: POSITION;
					float3	normal	: NORMAL;
					float2  tex0	: TEXCOORD0;
					float4  color : COLOR;

				};

				struct Particle
				{
					float3 position;
					float3 color;
				};

				float _Size;
				sampler2D _SpriteTex;
				float4 _Color;
				float4x4 modelToWorld;
				StructuredBuffer<Particle> particleBuffer;

				// Vertex Shader ------------------------------------------------
   				pointData VS_Main(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID)
				{
					pointData output = (pointData)0;
					float4 particlePos = float4(particleBuffer[instance_id].position, 1.0f);
					output.pos =  mul(modelToWorld, particlePos);
 					output.color = float4(particleBuffer[instance_id].color, 1.0);
					return output;
				}


				float compute_depth(float4 clippos)
				{
					#if defined(SHADER_TARGET_GLSL) || defined(SHADER_API_GLES) || defined(SHADER_API_GLES3)
						return ((clippos.z / clippos.w) + 1.0) * 0.5;
					#else
						return clippos.z / clippos.w;
					#endif
				}

				// Geometry Shader -----------------------------------------------------
				[maxvertexcount(4)]
				void GS_Main(point pointData p[1], inout TriangleStream<pointData> triStream)
				{
					float4 v[4];

					float halfS = 0.5f * _Size * 0.001  * length(p[0].pos - _WorldSpaceCameraPos);
					float3 delta = float3(halfS, -halfS, 0);

					p[0].pos = UnityObjectToClipPos(p[0]. pos);

					v[3] = p[0].pos + float4(halfS,-halfS,0,0);
					v[1] = p[0].pos + float4(-halfS,-halfS,0,0);
					v[0] = p[0].pos + float4(-halfS,halfS,0,0);
					v[2] = p[0].pos + float4(halfS,halfS,0,0);

					pointData pIn;
					pIn.color  = p[0].color;
					pIn.normal = p[0].normal;

					pIn.pos = v[0];
					pIn.tex0 = float2(1.0f, 0.0f);
					triStream.Append(pIn);

					pIn.pos = v[1];
					pIn.tex0 = float2(1.0f, 1.0f);
					triStream.Append(pIn);

					pIn.pos = v[2];
					pIn.tex0 = float2(0.0f, 0.0f);
					triStream.Append(pIn);

					pIn.pos = v[3];
					pIn.tex0 = float2(0.0f, 1.0f);
					triStream.Append(pIn);
				}

			fixed4 FS_Main (pointData i) : SV_Target
			{

				clip (0.5 - length(i.tex0.xy - 0.5));

				return i.color;

			}

			ENDCG
		}
	} 
}
