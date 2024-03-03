Shader "Renderers/CustomOutlineHDRP"
{
    Properties
    {
        _Color("Color", Color) = (1,1,1,1)
        _ColorMap("ColorMap", 2D) = "white" {}

        // Transparency
        _AlphaCutoff("Alpha Cutoff", Range(0.0, 1.0)) = 0.5
        [HideInInspector]_BlendMode("_BlendMode", Range(0.0, 1.0)) = 0.5

        _LeafTexture("Leaf Texture", 2D) = "white" {}
        _OutlineColor("Outline Color", Color) = (1, 1, 1, 1)
        _Tiling("Tiling", Vector) = (1, 1, 1, 1)
        _OutlineSize("Outline Scale", Float) = 0.0
        _AlphaCutoffEnable("_AlphaCutoffEnable", Float) = 0.0
    }

    HLSLINCLUDE

    #pragma target 5.0
    #pragma only_renderers d3d11 playstation xboxone xboxseries vulkan metal switch

    // #pragma enable_d3d11_debug_symbols

    //enable GPU instancing support
    #pragma multi_compile_instancing
    #pragma multi_compile _ DOTS_INSTANCING_ON

    ENDHLSL

    SubShader
    {
        Tags{ "RenderPipeline" = "HDRenderPipeline" }
        Pass
        {
            Name "FirstPass"
            Tags { "LightMode" = "FirstPass" }

            Blend Off
            ZWrite Off
            ZTest Always

            Cull Off

             Stencil
            {
                Ref [_OutlineBitMask]
                Comp NotEqual
            }

            HLSLPROGRAM

            // Toggle the alpha test
            #define _ALPHATEST_ON

            // Toggle transparency
            // #define _SURFACE_TYPE_TRANSPARENT

            // Toggle fog on transparent
            #define _ENABLE_FOG_ON_TRANSPARENT
            
            // List all the attributes needed in your shader (will be passed to the vertex shader)
            // you can see the complete list of these attributes in VaryingMesh.hlsl
            #define ATTRIBUTES_NEED_TEXCOORD0
            #define ATTRIBUTES_NEED_NORMAL
            #define ATTRIBUTES_NEED_TANGENT

            // List all the varyings needed in your fragment shader
            #define VARYINGS_NEED_TEXCOORD0
            #define VARYINGS_NEED_TANGENT_TO_WORLD

            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Common.hlsl"
            
            TEXTURE2D(_ColorMap);
            TEXTURE2D(_LeafTexture);

            // Declare properties in the UnityPerMaterial cbuffer to make the shader compatible with SRP Batcher.
CBUFFER_START(UnityPerMaterial)
            float4 _ColorMap_ST;
            float4 _LeafTexture_ST;
            float4 _Color;
            SamplerState sampler_linear_repeat;

            float _AlphaCutoff;
            float _BlendMode;



            float4 _OutlineColor;
            float2 _Tiling;
            float _OutlineSize;
            float _AlphaCutoffEnable;
CBUFFER_END

            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/RenderPass/CustomPass/CustomPassRenderersV2.hlsl"

            // If you need to modify the vertex datas, you can uncomment this code
            // Note: all the transformations here are done in object space
            #define HAVE_MESH_MODIFICATION
            AttributesMesh ApplyMeshModification(AttributesMesh input, float3 timeParameters)
            {
                float3 viewNormal = TransformWorldToViewNormal(TransformObjectToWorldNormal(input.normalOS));
                float3 viewPos = TransformWorldToView(TransformObjectToWorld(input.positionOS));
                viewPos += viewNormal * _OutlineSize/1000.0f;

                input.positionOS = TransformWorldToObject(TransformViewToWorld(viewPos));

                //It also works, but calculating this values in view space nad getting back to object space gives better result
                //input.positionOS += input.normalOS * _OutlineSize / 1000.0f;
                return input;
            }

            // Put the code to render the objects in your custom pass in this function
            void GetSurfaceAndBuiltinData(FragInputs fragInputs, float3 viewDirection, inout PositionInputs posInput, out SurfaceData surfaceData, out BuiltinData builtinData)
            {
                float2 leafTextureUv = TRANSFORM_TEX(fragInputs.texCoord0.xy, _LeafTexture);
                const float opacity = SAMPLE_TEXTURE2D(_LeafTexture, sampler_linear_repeat, leafTextureUv * _Tiling).a;

#ifdef _ALPHATEST_ON
                DoAlphaTest(opacity, _AlphaCutoff);
#endif

                // Write back the data to the output structures
                ZERO_BUILTIN_INITIALIZE(builtinData); // No call to InitBuiltinData as we don't have any lighting
                ZERO_INITIALIZE(SurfaceData, surfaceData);
                builtinData.opacity = 1.0f;
                builtinData.emissiveColor = float3(0, 0, 0);
                surfaceData.color = _OutlineColor.rgb;
            }

            #include "Packages/com.unity.render-pipelines.high-definition/Runtime/RenderPipeline/ShaderPass/ShaderPassForwardUnlit.hlsl"

            #pragma vertex Vert
            #pragma fragment Frag

            ENDHLSL
        }
    }
}
