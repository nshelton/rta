

Shader "Custom/Billboard" 
{
	Properties 
	{
		[HDR] _Color ("color", Color) = (1,1,1,1)
		_MainTex ("noise", 2D) = "white" {}
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
				#include "SimplexNoiseGrad3D.cginc"

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
				sampler2D _MainTex;
				float4 _Color;
				float4x4 modelToWorld;
				StructuredBuffer<Particle> particleBuffer;

				float nrand(float2 uv, float salt)
				{
					uv += float2(salt, 1);
					return frac(sin(dot(uv, float2(12.9898, 78.233))) * 43758.5453);
				}

				// Vertex Shader ------------------------------------------------
   				pointData VS_Main(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID)
				{
					pointData output = (pointData)0;
					float4 particlePos = float4(particleBuffer[instance_id].position, 1.0f);
					output.pos =  mul(modelToWorld, particlePos);
 					output.color = float4(particleBuffer[instance_id].color, 1.0);
#if DISTORION_FIELD
					float scale = sin(_Time.x / 3.0  ) * 2.0 + 4.0;
					float3 noisePos = scale * output.pos.xyz + float3(0, 0, _Time.x);
					float noiseAmp = 0.05 * sin(_Time.x); // *pow(sin(_Time.y + output.pos.y * 2.0) , 4.0);

					float noise = tex2Dlod(_MainTex, float4(0.05 * output.pos.xy + _Time.x + 10.0 * sin(_Time.x * 0.1), 0,0)).r;
					output.pos.z -= noise;
					//output.pos.xyz +=   noiseAmp;
				//	output.pos.z += 0.1 * frac(sin(output.pos.x * 10.0 + _Time.y)) > 0.9 ? 0.1: snoise_grad(output.pos.y+ sin(_Time.y)).z* 0.1;
#endif 
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

					// float2 aspect = float2(_ScreenParams.x/_ScreenParams.y, 1.0);

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

				return i.color + 0.1;

			}

			ENDCG
		}
	} 
}
