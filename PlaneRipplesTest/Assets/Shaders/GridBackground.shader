Shader "Custom/GridBackground" {
	Properties 
	{
		_Tint ("Tint", Color) = (.5,.5,.5,1)
		_BGTex ("GlowWeb GreyPack",2D) = "black"{}
		_Ramp ("Ramp Texture",2D) = "white"{}
		_Offset("Ramp Offest",Range(0,1)) = 0
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
			half _Offset;

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};
			
			float4 _BGTex_ST;
			float4 _Ramp_ST;

			v2f vert(appdata_full v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_BGTex);
				o.texcoord1 = TRANSFORM_TEX(v.texcoord1,_Ramp);
				//o.texcoord1 = v.texcoord1.xy;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed gridH = tex2D(_BGTex,i.texcoord + fixed2(-.04,.04)*_Time.y).r;
				fixed gridV = tex2D(_BGTex,i.texcoord + fixed2(-.04,.04)*_Time.y).g;
				fixed hAccents = tex2D(_BGTex,i.texcoord + fixed2(.15,.04)*_Time.y).b;
				fixed vAccents = tex2D(_BGTex,i.texcoord + fixed2(-.04,-.23)*_Time.y).a; 
				hAccents = (1-step(gridH,.06)) * hAccents; 
				vAccents = (1-step(gridV,.05)) * vAccents;
				
				//return tex2D(_BGTex,i.texcoord);
				return fixed4(0,1,0,1) * (gridH + gridV + hAccents + vAccents);
				
			}		
			
		ENDCG
		}
	} 
	FallBack "Diffuse"
}