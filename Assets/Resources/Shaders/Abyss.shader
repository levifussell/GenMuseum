Shader "Custom/Abyss"
{
    Properties
    {
        _Color ("Color", Color) = (1,1,1,1)
        _MainTex ("Albedo (RGB)", 2D) = "white" {}
        _Glossiness ("Smoothness", Range(0,1)) = 0.5
        _Metallic ("Metallic", Range(0,1)) = 0.0

		[Header(Abyss Parameters)] 
        _AbyssPos("Abyss Position", Vector) = (0, 0, 0, 0)
        _AbyssRadius("Abyss Radius", Range(0.0, 10.0)) = 1.0
        _AbyssRamp("Abyss Ramp", Range(0.0, 5.0)) = 2.0
        _AbyssMask("Abyss Mask", Vector) = (0, 0, 0, 0)

    }
    SubShader
    {
        Tags { "RenderType"="Opaque" }
        LOD 200

        CGPROGRAM
        // Physically based Standard lighting model, and enable shadows on all light types
        #pragma surface surf Standard fullforwardshadows finalcolor:abyss

        // Use shader model 3.0 target, to get nicer looking lighting
        #pragma target 3.0

        sampler2D _MainTex;

        struct Input
        {
            float2 uv_MainTex;
            float4 vertex : SV_POSITION;
            float3 worldPos;
        };

        half _Glossiness;
        half _Metallic;
        fixed4 _Color;

        fixed4 _AbyssPos;
        half _AbyssRadius;
        half _AbyssRamp;
        fixed4 _AbyssMask;

        // Add instancing support for this shader. You need to check 'Enable Instancing' on materials that use the shader.
        // See https://docs.unity3d.com/Manual/GPUInstancing.html for more information about instancing.
        // #pragma instancing_options assumeuniformscaling
        UNITY_INSTANCING_BUFFER_START(Props)
            // put more per-instance properties here
        UNITY_INSTANCING_BUFFER_END(Props)

        void surf (Input IN, inout SurfaceOutputStandard o)
        {
            // Albedo comes from a texture tinted by color
            fixed4 c = tex2D (_MainTex, IN.uv_MainTex) * _Color;
            o.Albedo = c.rgb;
            // Metallic and smoothness come from slider variables
            o.Metallic = _Metallic;
            o.Smoothness = _Glossiness;
            o.Alpha = c.a;
        }

        void abyss(Input IN, SurfaceOutputStandard o, inout fixed4 color)
        {
            // cicular shadow.
            //float vDistToCentre = length(float2(IN.worldPos.x, IN.worldPos.z));
            //float shadow = smoothstep(_AbyssRadius - _AbyssRamp, _AbyssRadius + _AbyssRamp, vDistToCentre) * 2.0 - 1.0;

            // square shadow.
            //float2 vDiffToCentre = float2(IN.worldPos.x, IN.worldPos.z);
            float3 diffToCentre = IN.worldPos - _AbyssPos;
            float4 vDiffToCentre = float4(
                diffToCentre.x,
                diffToCentre.z,
                diffToCentre.x,
                diffToCentre.z);
            float4 vRadiiMin = float4(
                _AbyssRadius - _AbyssRamp,
                _AbyssRadius - _AbyssRamp,
                -_AbyssRadius + _AbyssRamp,
                -_AbyssRadius + _AbyssRamp);
            float4 vRadiiMax = float4(
                _AbyssRadius + _AbyssRamp,
                _AbyssRadius + _AbyssRamp,
                -_AbyssRadius - _AbyssRamp,
                -_AbyssRadius - _AbyssRamp);
            float4 shadowDiff = 1.0 - smoothstep(vRadiiMin, vRadiiMax, vDiffToCentre) * (1.0 - _AbyssMask);
            float shadow = 1.0 - shadowDiff.x * shadowDiff.y * shadowDiff.z * shadowDiff.w;

            float abyssLimit = (1.0f - step(0.01f, _AbyssRadius));

			color *= clamp(shadow + abyssLimit, 0.0f, 1.0f);
        }

        ENDCG
    }
    FallBack "Diffuse"
}
