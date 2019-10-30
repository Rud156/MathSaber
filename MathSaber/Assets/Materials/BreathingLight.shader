Shader "Custom/BreathingLight"
{
    Properties
	{	
		//_MainTex("MainTex", 2D) = "white" {}
		//_BasicColor("BasicColor", Color) = (1,1,1,1)
		[HDR]_BloomColorTop("BloomColorTop", COlor) = (1,1,1,1)
		[HDR]_BloomColorMiddle("BloomColorMiddle", Color) = (1,1,1,1)
		[HDR]_BloomColorButton("BloomColorButton", COlor) = (1,1,1,1)
		_Speed("BreathingSpeed", float) = 1
		[PowerSlider(1)] _LerpPartTop("Top", Range(0,1)) = 0
		[PowerSlider(1)] _LerpPartButton("Button", Range(0,1)) = 0

	}

	SubShader
	{
		Pass
		{
			Tags { "RenderType" = "Opaque"}
			LOD 200
		
			CGPROGRAM

			#include "UnityCG.cginc"
			#pragma vertex vert
			#pragma fragment frag

			//sampler2D _MainTex;
			fixed4 _BloomColorTop;
			fixed4 _BloomColorMiddle;
			fixed4 _BloomColorButton;
			float _Speed;
			//fixed4 _BasicColor;
			float _LerpPartTop;
			float _LerpPartButton;
			

			struct appdate_base
			{
				float4 vertex : POSITION;
				float2 uv : TEXCOORD0;
			};

			struct v2f
			{
				float4 vertex : SV_POSITION;
				float2 uv : TEXCOORD0;
			};

			v2f vert (appdate_base v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f u) : SV_Target
			{	
				fixed4 LerpColor;
				float Lerp = 0;
				if(u.uv.y >= _LerpPartTop)
				{
					Lerp = (1 - u.uv.y) / (1 - _LerpPartTop);
					LerpColor = lerp(_BloomColorTop, _BloomColorMiddle, Lerp);
				}

				else if(u.uv.y <= _LerpPartButton)
				{
					Lerp = u.uv.y / _LerpPartButton;
					LerpColor = lerp(_BloomColorButton, _BloomColorMiddle, Lerp);
				}
				else
				{
					LerpColor = _BloomColorMiddle;
				}

				//fixed4 TexColor = tex2D(_MainTex, u.uv);
				//fixed4 Num = sin(_Time.y * _Speed) + 1.05;
				//fixed4 FinalColor = LerpColor * Num;
				fixed4 FinalColor = LerpColor;
				return FinalColor;
			}			
			ENDCG
		}
	}
	//FallBack "Diffuse"
}
