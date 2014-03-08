Shader "Custom/Lava" {
	Properties {
		_FastLavaColor ("Fast Lava Color", Color) = (40,1.5,.1,1)
		_FastLavaExp("Fast Lava Exposure", float) = 9
		_FastLavaTex("Grey Pack (Red)", 2D) = "white" {}
		_BrightLavaTex("Bright Lava Texture",2D) = "white" {}
		_FireTex("Fire Texture",2D) = "white" {}
		_DarkLavaTex("Dark Lava Texture",2D) = "white" {}
		
		_Offset("Wave Offset",float) = 0
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
		 
		half4     _FastLavaColor;
		half  	  _FastLavaExp;
		sampler2D _FastLavaTex;
		sampler2D _BrightLavaTex;
		sampler2D _FireTex;
		sampler2D _DarkLavaTex;
		half      _Offset;

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
				float2 texcoord1 : TEXCOORD1;
			};
			
			float4 _FastLavaTex_ST;
//			float4 _BrightLavaTex_ST;
//			float4 _FireTex_ST;
//			float4 _DarkLavaTex_ST;

			v2f vert(appdata_full v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_FastLavaTex);
				o.texcoord1 = v.texcoord1.xy;
				o.vertex.y += min(.08,o.texcoord1.x) - .04;
				return o;
			}
			
			fixed4 frag (v2f i) : COLOR
			{
				fixed4 fastLavaColor = _FastLavaColor *
					tex2D(_FastLavaTex,i.texcoord*float2(2,4) + float2(-.15,.4)*_Time.y).r;
				fixed4 fastLavaBase = tex2D(_BrightLavaTex,i.texcoord*float2(1,2) + float2(-.08,.35)*_Time.y);
				
				fixed4 slowLavaColor = tex2D(_FireTex,i.texcoord*float2(1,2) + float2(-.05,.2)*_Time.y);
				fixed4 slowLavaBase = tex2D(_DarkLavaTex,i.texcoord*float2(1,2.5) + float2(-.02,.13)*_Time.y);
				
				fixed4 lavaLerp = lerp(fastLavaColor+fastLavaBase,slowLavaColor, pow(1-slowLavaColor.g,_FastLavaExp));
				//fixed4 lavaLerp = lerp(fastLavaColor+fastLavaBase,slowLavaColor, 0);
				
				//fixed4 finalColor = lavaLerp * slowLavaBase;
				fixed4 finalColor = lerp(slowLavaBase,lavaLerp,saturate(i.texcoord1.x*_Offset)); 
				finalColor.a = 1;
				return finalColor;
			}		
			
		ENDCG
		}
	} 
	FallBack "Diffuse"
}