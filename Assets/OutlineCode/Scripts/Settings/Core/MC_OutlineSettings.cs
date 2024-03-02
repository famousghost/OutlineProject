namespace McOutlineFeature
{
    using UnityEngine;


    [CreateAssetMenu(menuName = "McOutline/OutlineCoreSettings")]
    public class Mc_OutlineSettings : ScriptableObject
    {
        #region Public Variables
        public Shader StencilBufferShader => _StencilBufferShader;
        public Shader OutlineShader => _OutlineShader;
        #endregion Public Variables

        #region Inspector Variables
        [Header("Outline necessary shaders (HDRP)")]
        [SerializeField] private Shader _StencilBufferShader;
        [SerializeField] private Shader _OutlineShader;

        #endregion Inspector Variables
    }
}