Shader "Custom/WaveColor" {
	Properties 
	{
		_Tint ("Tint", Color) = (.5,.5,.5,1)
		_BGTex("Background Texture",2D) = "black"{}
		_Ramp      ("Ramp Texture",2D) = "white"{}
	}
	SubShader 
	{
	Lighting Off
	 Pass 
    	{
		LOD 200
		
		CGPROGRAM
		//Upgrade NOTE: excluded shader from DX11 and Xbox360; has structs without semantics (struct v2f members billowsUV)
		#pragma exclude_renderers d3d11 xbox360
		#pragma vertex vert
		#pragma fragment frag
		#include "UnityCG.cginc"
		 
			half4 _Tint;
			sampler2D _Ramp;
			sampler2D _BGTex;

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			//float4 _Ramp_ST;
			float4 _BGTex_ST;

			v2f vert(appdata_full v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_BGTex);
				return o;
			}
			
			half4 frag (v2f i) : COLOR
			{ 
				half4 ramp = tex2D(_Ramp,float2(i.color.r,.1));
				half4 finalColor = (tex2D(_BGTex,i.texcoord) + ramp) * _Tint;
				finalColor.a = 1;
				return finalColor;
			}		
			
		ENDCG
		}
	} 
	FallBack "Diffuse"
}
