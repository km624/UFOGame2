Shader "Custom/BounceShader"
{
     Properties
    {
        _MainTex ("Texture", 2D) = "white" {}  // �⺻ �ؽ�ó
        _WobbleStrength ("Wobble Strength", Float) = 0.1  // ���� ����
        _Speed ("Wobble Speed", Float) = 2.0  // �ⷷ�̴� �ӵ�
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        CGPROGRAM
        #pragma surface surf Lambert vertex:vert

        struct Input 
        {
            float2 uv_MainTex;
        };

        sampler2D _MainTex;
        float _WobbleStrength;
        float _Speed;

        void vert (inout appdata_full v)
        {
             float wobbleX = sin(_Time.y * _Speed + v.vertex.y * 2) * _WobbleStrength;
             float wobbleZ = sin(_Time.y * _Speed + v.vertex.x * 2) * _WobbleStrength;
            v.vertex.x += wobbleX;  // �¿� ����
            v.vertex.z += wobbleZ;  // �յ� ����
        }

        void surf (Input IN, inout SurfaceOutput o)
        {
            o.Albedo = tex2D (_MainTex, IN.uv_MainTex).rgb;
        }
        ENDCG
    }
}
