// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'



Shader "Auto Texcoord" {
	Properties{
		_Color("Color", Color) = (1.0,1.0,1.0,1.0)
		_Texture("Texture", 3D) = "white"
		_MinimumBrightness("Minimum Brightness", Float) = 0.1
	}

		Category
	{
		Tags{ "Queue" = "Transparent" "IgnoreProjector" = "True" "RenderType" = "Transparent" "LightMode" = "ForwardBase" }
		Blend SrcAlpha OneMinusSrcAlpha
		ColorMask RGB
		Lighting Off

		SubShader
	{
		Pass
	{

		CGPROGRAM
#pragma vertex vert
#pragma fragment frag
#pragma shader_feature _ _ALPHATEST_ON _ALPHABLEND_ON _ALPHAPREMULTIPLY_ON
#include "UnityCG.cginc"

		fixed4 _Color;
	uniform sampler3D _Texture;
	fixed _MinimumBrightness;

	struct appdata_t {
		float4 vertex : POSITION;
		fixed3 normal : NORMAL;
		fixed4 color : COLOR;
		fixed2 texcoord : TEXCOORD0;
	};

	struct v2f {
		float4 processedVertex : SV_POSITION;
		fixed3 normal : NORMAL; //TODO: Find out the fuck that after the : is
		float distance : TEXCOORD7;
		float4 vertex : TEXCOORD6;
		fixed2 texcoord : TEXCOORD0;
		fixed4 color : COLOR;
	};

	v2f vert(appdata_t v)
	{
		v2f o;
		o.processedVertex = UnityObjectToClipPos(v.vertex);
		float3 pos = (_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex)) * (_WorldSpaceCameraPos - mul(unity_ObjectToWorld, v.vertex));
		o.distance = pow(pos.x + pos.y + pos.z, 0.5);
		o.color = v.color;
		o.texcoord = fixed4(v.vertex.xy, 0, 0);
		o.normal = v.normal;
		o.vertex = mul(unity_ObjectToWorld, v.vertex);
		return o;
	}

	fixed4 frag(v2f i) : SV_Target
	{
		//discard;
		//i.color.xyz *= 1 / (i.distance / 10);
		//return _Color * tex2D(_Texture, clamp(i.texcoord - floor(i.texcoord * 16) / 16, 4.0 / 1024.0, 60.0 / 1024.0) + floor(i.texcoord * 16) / 16); //Prevents texture issues
		fixed4 color = _Color * tex3D(_Texture, i.vertex *4 + _SinTime);//i.texcoord);
	return fixed4(color.xyzw);// *max(dot(i.normal, _WorldSpaceLightPos0.xyz), _MinimumBrightness), color.w);
	}
	/*float4 vert(appdata_base v) : POSITION
	{
		return UnityObjectToClipPos(v.vertex);
	}

	fixed4 frag(float4 sp:WPOS) : COLOR
	{
		return tex2D(_Texture, fixed4(sp.xy / _ScreenParams.xy * 4,0.0,1.0));
	}*/
	ENDCG
	}
	}
	}
}
