Shader "Custom/WireFrame"
{
 Properties
 {
   _LineColor ("Line Color", Color) = (0.0,1.0,0.0,1.0)
   _BGColor ("Background Color", Color) = (1.0,1.0,1.0,1.0)
   _LineWidth ("Line Width", Range(0.0,0.5)) = 0.1
   _MainTexture("Main Texture",2D) = "white" {}
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
		 
			half4 _LineColor;
			half4 _BGColor;
			half _LineWidth;
			sampler2D _MainTexture;

			struct v2f {
				float4 vertex : POSITION;
				float2 texcoord : TEXCOORD0;
				float2 wireUVs;
			};
			
			float4 _MainTexture_ST;

			v2f vert(appdata_full v)
			{
				v2f o;
				o.vertex = mul(UNITY_MATRIX_MVP, v.vertex);
				o.texcoord = TRANSFORM_TEX(v.texcoord,_MainTexture);
				o.wireUVs = v.texcoord;
				return o;
			}
			
			half4 frag (v2f i) : COLOR
			{ 
				half4 finalColor = _BGColor * tex2D(_MainTexture,i.texcoord);
				//horizontal lines
//				if((i.texcoord < i.wireUVs + _LineWidth && i.texcoord > i.wireUVs - _LineWidth))
					finalColor += _LineColor;
					
				return _LineColor;
			}		
			
		ENDCG
		}
	} 
	FallBack "Diffuse"
}
