namespace McOutlineFeature
{

    using UnityEngine;
    using UnityEngine.Rendering.HighDefinition;
    using UnityEngine.Rendering;

    public sealed class OutlineCustomPass : CustomPass
    {
        [SerializeField, HideInInspector]
        private Shader _StencilBufferShader;

        [SerializeField, HideInInspector]
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

        protected override void Setup(ScriptableRenderContext renderContext, CommandBuffer cmd)
        {
            if (_StencilBufferShader == null)
            {
                _StencilBufferShader = McOutlineManager.Instance.Settings.StencilBufferShader;
            }
            if(_OutlineShader == null)
            {
                _OutlineShader = McOutlineManager.Instance.Settings.OutlineShader;
            }

            _OutlineMaterialPropertyBlock = new MaterialPropertyBlock();

            _OutlineStencilBufferMaterial = CoreUtils.CreateEngineMaterial(_StencilBufferShader);
            _OutlineMaterial = CoreUtils.CreateEngineMaterial(_OutlineShader);
        }

        protected override void Execute(CustomPassContext ctx)
        {
            _OutlineStencilBufferMaterial.SetInt("_StencilWriteMask", (int)UserStencilUsage.UserBit0);

            RenderObjectsMy(ctx.cmd, _OutlineStencilBufferMaterial);

            RenderObjectsMy(ctx.cmd, _OutlineMaterial);
        }

        private void RenderObjectsMy(CommandBuffer cmd, Material renderMaterial)
        {
            if(McOutlineManager.Instance == null)
            {
                return;
            }
            var outlineObjects = McOutlineManager.Instance.OutlineObjects;
            if (outlineObjects.Count != 0)
            {
                foreach (var outlineObject in outlineObjects)
                {
                    if(!outlineObject.OutlineActive)
                    {
                        continue;
                    }
                    var outlineRenderer = outlineObject.Renderer;


                    for (int i = 0; i < outlineRenderer.materials.Length; ++i)
                    {
                        _OutlineMaterialPropertyBlock.SetFloat(_OutlineSizeId, outlineObject.OutlineSize);
                        _OutlineMaterialPropertyBlock.SetFloat(_AlphaCutoffId, outlineObject.AlphaCutoff[i]);
                        _OutlineMaterialPropertyBlock.SetFloat(_AlphaCutoffEnableId, outlineObject.AlphaCutoffEnable[i]);
                        _OutlineMaterialPropertyBlock.SetColor(_OutlineColorId, outlineObject.OutlineColor);
                        if (outlineObject.AlphaTextureToAlphaCutoff[i] == null)
                        {
                            _OutlineMaterialPropertyBlock.SetTexture(_LeafTextureId, Texture2D.whiteTexture);
                        }
                        else
                        {
                            _OutlineMaterialPropertyBlock.SetTexture(_LeafTextureId, outlineObject.AlphaTextureToAlphaCutoff[i]);
                        }
                        _OutlineMaterialPropertyBlock.SetVector(_TilingId, outlineObject.Tiling[i]);

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
    }
}