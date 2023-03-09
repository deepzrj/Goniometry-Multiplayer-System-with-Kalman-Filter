Shader "Hidden/MSP_GUI"
{
	Properties 
	{
	    _MainTex ("Base (RGB) Trans (A)", 2D) = "white" {}
	}
	
	SubShader 
	{
	    Tags {"Queue"="Overlay" "RenderType"="Overlay" "DisableBatching"="True" "ForceNoShadowCasting"="True" "IgnoreProjector"="True" "PreviewType"="Plane"}
	    LOD 100

	    ZWrite Off
	    Blend SrcAlpha OneMinusSrcAlpha
	
	    Pass 
	    {
	        CGPROGRAM
	            #pragma vertex vert
	            #pragma fragment frag

	            struct appdata
	            {
	                float4 vertex : POSITION;
	                float2 texcoord : TEXCOORD0;
	            };
	
	            struct v2f
	            {
	                float4 vertex : SV_POSITION;
	                float2 texcoord : TEXCOORD0;
	            };
	
	            sampler2D _MainTex;
	            float4 _MainTex_ST;
	
	            v2f vert(appdata v)
	            {
	                v2f o;
	                o.vertex = mul(UNITY_MATRIX_VP, mul(unity_ObjectToWorld, v.vertex));
	                o.texcoord = (v.texcoord.xy * _MainTex_ST.xy + _MainTex_ST.zw);
					return o;
	            }
	           
	            fixed4 frag(v2f i) : SV_Target
	            {
					fixed4 col = tex2D(_MainTex, i.texcoord);
					return col;
	            }

	        ENDCG
	    }
	}
}
