Shader "iPhone/AlphaBlendOnScreenTop_Color" {
	Properties {
		R ("R", Range(0, 1)) = 1
		G ("G", Range(0, 1)) = 1
		B ("B", Range(0, 1)) = 1
		_Alpha ("Alpha", Range(0, 1)) = 0.5
		_MainTex ("Texture", 2D) = "white" {}
	}
	//DummyShaderTextExporter
	SubShader{
		Tags { "RenderType"="Opaque" }
		LOD 200
		CGPROGRAM
#pragma surface surf Standard
#pragma target 3.0

		sampler2D _MainTex;
		struct Input
		{
			float2 uv_MainTex;
		};

		void surf(Input IN, inout SurfaceOutputStandard o)
		{
			fixed4 c = tex2D(_MainTex, IN.uv_MainTex);
			o.Albedo = c.rgb;
			o.Alpha = c.a;
		}
		ENDCG
	}
}