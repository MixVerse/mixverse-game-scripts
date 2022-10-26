// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "PixelGame/particle/_a1 additive multiply"
{
	Properties
	{
		_TintColor("色调", Color) = (0.5,0.5,0.5,0.5)
		_MainTex("粒子贴图", 2D) = "white" {}
		_InvFade("软粒子因子", Range(0.01,3.0)) = 1.0
		_AdjustA("透明调整", Range ( 0.01, 1 )) = 1
		_Enhance("增强倍数", Range(0.0, 4.0)) = 1
	}

	Category
	{
		Tags { "Queue"="Transparent+100" "IgnoreProjector"="True" "RenderType"="Transparent" }
		Blend SrcAlpha One
		AlphaTest Greater .01
		//ColorMask RGB
		Cull Off Lighting Off ZWrite Off
		
		// ---- Fragment program cards
		SubShader
		{
			Pass
			{
				CGPROGRAM
				#pragma vertex vert
				#pragma fragment frag
				#pragma multi_compile_particles
				#pragma multi_compile_fog

				#include "UnityCG.cginc"

				sampler2D _MainTex;
				fixed4 _TintColor;
				
				struct appdata_t
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
				};

				struct v2f
				{
					float4 vertex : POSITION;
					fixed4 color : COLOR;
					float2 texcoord : TEXCOORD0;
					#ifdef SOFTPARTICLES_ON
					float4 projPos : TEXCOORD1;
					#endif
				};
				
				float4 _MainTex_ST;

				v2f vert (appdata_t v)
				{
					v2f o;
					o.vertex = UnityObjectToClipPos(v.vertex);
					#ifdef SOFTPARTICLES_ON
					o.projPos = ComputeScreenPos(o.vertex);
					COMPUTE_EYEDEPTH(o.projPos.z);
					#endif
					o.color = v.color;
					o.texcoord = TRANSFORM_TEX(v.texcoord, _MainTex);
					return o;
				}

				sampler2D_float _CameraDepthTexture;
				float _InvFade;
				fixed _AdjustA;
				fixed _Enhance;
				
				fixed4 frag (v2f i) : COLOR
				{
					#ifdef SOFTPARTICLES_ON
					float sceneZ = LinearEyeDepth(UNITY_SAMPLE_DEPTH(tex2Dproj(_CameraDepthTexture, UNITY_PROJ_COORD(i.projPos))));
					float partZ = i.projPos.z;
					float fade = saturate (_InvFade * 2.0f * (sceneZ-partZ));
					i.color.a *= (fade * _AdjustA);
					#else
					i.color.a *= _AdjustA;
					#endif
					fixed4 texColor = tex2D(_MainTex, i.texcoord);
					fixed4 diff = i.color * _TintColor;
					fixed alpha = texColor.a * i.color.a * _TintColor.a;
					fixed4 col = (diff * texColor) * _Enhance;
					col.a = alpha;
					return col;
				}
				ENDCG 
			}
		}
	}
}