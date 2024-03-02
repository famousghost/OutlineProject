namespace McOutlineFeature
{

    using UnityEngine;
    using UnityEngine.Rendering.HighDefinition;
    using UnityEngine.Rendering;

    [ExecuteAlways]
    public sealed class MC_OutlineCustomPass : CustomPass
    {

        #region Unity Methods
        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            if(MC_OutlineManager.Instance == null)
            {
                return;
            }
            _StencilBufferShader = MC_OutlineManager.Instance.Settings.StencilBufferShader;
            _OutlineShader = MC_OutlineManager.Instance.Settings.OutlineShader;

            _OutlineMaterialPropertyBlock = new MaterialPropertyBlock();

            _OutlineStencilBufferMaterial = CoreUtils.CreateEngineMaterial(_StencilBufferShader);
            _OutlineMaterial = CoreUtils.CreateEngineMaterial(_OutlineShader);
        }

        protected override void Execute(CustomPassContext ctx)
        {
            RenderObjects(ctx.cmd, _OutlineStencilBufferMaterial);
            RenderObjects(ctx.cmd, _OutlineMaterial);
        }

        private void RenderObjects(CommandBuffer cmd, Material renderMaterial)
        {

            if (MC_OutlineManager.Instance == null)
            {
                return;
            }
            var outlineObjects = MC_OutlineManager.Instance.OutlineObjects;
            if (outlineObjects.Count != 0)
            {
                foreach (var outlineObject in outlineObjects)
                {
                    if(!outlineObject.OutlineActive)
                    {
                        continue;
                    }
                    var outlineRenderer = outlineObject.Renderer;

                    if(outlineRenderer == null)
                    {
                        continue;
                    }

                    for (int i = 0; i < outlineRenderer.sharedMaterials.Length; ++i)
                    {
                        if(outlineObject.OutputOutlineMaterialsProperties.Count != outlineRenderer.sharedMaterials.Length)
                        {
                            break;
                        }
                        var outlinePropertiesElement = outlineObject.OutputOutlineMaterialsProperties[i];
                        _OutlineMaterialPropertyBlock.SetFloat(_OutlineSizeId, outlineObject.OutlineSize);
                        _OutlineMaterialPropertyBlock.SetFloat(_AlphaCutoffId, outlinePropertiesElement.AlphaCutoff);
                        _OutlineMaterialPropertyBlock.SetFloat(_AlphaCutoffEnableId, outlinePropertiesElement.AlphaCutoffEnable);
                        _OutlineMaterialPropertyBlock.SetColor(_OutlineColorId, outlineObject.OutlineColor);
                        if (outlinePropertiesElement.AlphaTexture == null)
                        {
                            _OutlineMaterialPropertyBlock.SetTexture(_LeafTextureId, Texture2D.whiteTexture);
                        }
                        else
                        {
                            _OutlineMaterialPropertyBlock.SetTexture(_LeafTextureId, outlinePropertiesElement.AlphaTexture);
                        }
                        _OutlineMaterialPropertyBlock.SetVector(_TilingId, outlinePropertiesElement.Tiling);

                        outlineRenderer.SetPropertyBlock(_OutlineMaterialPropertyBlock, i);
                        cmd.DrawRenderer(outlineRenderer, renderMaterial, i);
                    }
                }
            }
        }

        protected override void Cleanup()
        {
            // Cleanup code
        }
        #endregion Unity Methods

        #region Private Variables

        private Shader _StencilBufferShader;
        private Shader _OutlineShader;

        private Material _OutlineStencilBufferMaterial;
        private Material _OutlineMaterial;

        private MaterialPropertyBlock _OutlineMaterialPropertyBlock;

        //Shaders properties
        private static readonly int _AlphaCutoffEnableId = Shader.PropertyToID("_AlphaCutoffEnable");
        private static readonly int _LeafTextureId = Shader.PropertyToID("_LeafTexture");
        private static readonly int _AlphaCutoffId = Shader.PropertyToID("_AlphaCutoff");
        private static readonly int _TilingId = Shader.PropertyToID("_Tiling");
        private static readonly int _OutlineSizeId = Shader.PropertyToID("_OutlineSize");
        private static readonly int _OutlineColorId = Shader.PropertyToID("_OutlineColor");

        #endregion Private Variables
    }
}