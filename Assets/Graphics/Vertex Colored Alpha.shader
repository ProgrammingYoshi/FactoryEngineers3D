// Upgrade NOTE: replaced 'mul(UNITY_MATRIX_MVP,*)' with 'UnityObjectToClipPos(*)'

// Upgrade NOTE: replaced '_Object2World' with 'unity_ObjectToWorld'

Shader "Vertex Color Alpha" {
	Properties{
		_Color("Color", Color) = (1.0,1.0,1.0,1.0)
		_Texture("Texture", 2D) = "white"
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

					fixed4 _Color;
				uniform sampler2D _Texture;
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
					o.texcoord = v.texcoord;
					o.normal = v.normal;
					return o;
				}

				fixed4 frag(v2f i) : SV_Target
				{
					//discard;
					//i.color.xyz *= 1 / (i.distance / 10);
					//return _Color * tex2D(_Texture, clamp(i.texcoord - floor(i.texcoord * 16) / 16, 4.0 / 1024.0, 60.0 / 1024.0) + floor(i.texcoord * 16) / 16); //Prevents texture issues
					fixed4 color = _Color * tex2D(_Texture, i.texcoord);
					return fixed4(color.xyz * max(dot(i.normal, _WorldSpaceLightPos0.xyz), _MinimumBrightness), color.w);
				}
				ENDCG
			}
		}
	}
}
