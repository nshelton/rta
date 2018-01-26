// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "XParticle/1 - FreeParticle"
{
	Properties
	{
		 [HDR] _Color ("Color", Color) = (0, 0, 0.5, 0.3)
	}

	SubShader 
	{
		Pass 
		{
			CGPROGRAM
			#pragma target 5.0
			
			#pragma vertex vert
			#pragma fragment frag
			
			#include "UnityCG.cginc"
			
			// Particle's data
			struct Particle
			{
				float3 position;
				float3 color;
			};
			
			// Pixel shader input
			struct PS_INPUT
			{
				float4 position : SV_POSITION;
				float4 color : COLOR;
			};
			
			struct fragOut
			{
				half4 color : SV_Target;
				float depth : SV_Depth;
			};

			// Particle's data, shared with the compute shader
			StructuredBuffer<Particle> particleBuffer;
			
			// Vertex shader
			PS_INPUT vert(uint vertex_id : SV_VertexID, uint instance_id : SV_InstanceID)
			{
				PS_INPUT o = (PS_INPUT)0;

				// Color
 				o.color = float4(particleBuffer[instance_id].color, 1.0);

				// Position
				o.position = UnityObjectToClipPos(float4(particleBuffer[instance_id].position, 1.0f));

				return o;
			}


			float4 _Color;
			// Pixel shader
			fragOut frag(PS_INPUT i)
			{
				fragOut o;

				// o.depth = i.position.z;
				o.color = i.color * _Color;

				return o;
			}
			
			ENDCG
		}
	}

	Fallback Off
}
