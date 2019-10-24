// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

Shader "Custom/Bloom"
{
	Properties
	{
		[HDR]_MainColor("MainColor", Color) = (1,1,1,1)
		_Facter("Facter", float) = 1

	}

	SubShader
	{
		Pass
		{
			Tags{"RenderType" = "Oquape"}
			LOD 200
			CGPROGRAM

			#pragma vertex vert
			#pragma fragment frag

			fixed4 _MainColor;
			float _Facter;

			struct appdata
			{
				float2 uv : TEXCOORD0;
				float4 vertex : POSITION;
			};

			struct v2f
			{
				float2 uv : TEXCOORDO;
				float4 vertex : SV_POSITION;
			};

			v2f vert(appdata v)
			{
				v2f o;
				o.vertex = UnityObjectToClipPos(v.vertex);
				o.uv = v.uv;
				return o;
			}

			fixed4 frag(v2f u) : SV_Target
			{
				fixed FinalColor = _MainColor * _Facter;
				return FinalColor;
			}
			ENDCG
		}	
	}	
}