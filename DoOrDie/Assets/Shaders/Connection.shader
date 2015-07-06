Shader "Custom/Connection"
{
	Properties
	{
		_MainTex ("Base (RGB)", 2D) = "white" {}
		_OtherTex ("Base (RGB)", 2D) = "white" {}
	}
	SubShader
	{
		Tags { "RenderType"="Transparent" "Queue"="Transparent" }
		LOD 200
		Blend SrcAlpha One
		
		CGPROGRAM
		#pragma surface surf Lambert

		sampler2D _MainTex;
		sampler2D _OtherTex;

		struct Input
		{
			float2 uv_MainTex;
			float2 uv_OtherTex;
		};

		void surf (Input IN, inout SurfaceOutput o)
		{
			half4 c = tex2D (_MainTex, IN.uv_MainTex);
			half4 d = tex2D (_MainTex, IN.uv_OtherTex);
			
			o.Emission = c.rgb * d.rgb;
			o.Alpha = c.a * d.a;
		}
		ENDCG
	} 
	FallBack "Diffuse"
}
