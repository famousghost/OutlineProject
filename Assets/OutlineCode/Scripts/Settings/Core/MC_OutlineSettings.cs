namespace McOutlineFeature
{
    using UnityEngine;


    [CreateAssetMenu(menuName = "McOutline/OutlineCoreSettings")]
    public class Mc_OutlineSettings : ScriptableObject
    {
        #region Public Variables
        public float DefaultOutlineSize => _DefaultOutlineSize;
        public Color DefaultOutlineColor => _DefaultOutlineColor;
        public Shader StencilBufferShader => _StencilBufferShader;
        public Shader OutlineShader => _OutlineShader;
        #endregion Public Variables

        #region Inspector Variables
        [Header("Default Outline settings")]
        [SerializeField] private float _DefaultOutlineSize;
        [SerializeField] private Color _DefaultOutlineColor;

        [Header("Outline necessary shaders (HDRP)")]
        [SerializeField] private Shader _StencilBufferShader;
        [SerializeField] private Shader _OutlineShader;

        #endregion Inspector Variables
    }
}