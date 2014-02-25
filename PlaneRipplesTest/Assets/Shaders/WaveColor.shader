Shader "Custom/WaveColor" {
	Properties 
	{
		_MainColor ("Main Color", Color) = (.5,.5,.5,1)
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
		 
			half4 _MainColor;
			half4 _Ramp;

			struct v2f {
				float4 vertex : POSITION;
				fixed4 color : COLOR;
				float2 texcoord : TEXCOORD0;
			};
			
			//float4 _Ramp_ST;

			v2f vert(appdata_full v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.color = v.color;
				o.texcoord = TRANSFORM_TEX(v.texcoord,_Ramp);
				return o;
			}
			
			half4 frag (v2f i) : COLOR
			{ 
				half4 finalColor = tex2D(_Ramp,i.texcoord);
				finalColor.a = 1;
				return finalColor * _MainColor;
			}		
			
		ENDCG
		}
	} 
	FallBack "Diffuse"
}
