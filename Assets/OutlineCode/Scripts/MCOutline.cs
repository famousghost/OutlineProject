namespace McOutlineFeature
{
    using System.Collections.Generic;
    using System.Linq;
    using UnityEngine;

    public sealed class MCOutline : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private float _OutlineSize;
        [SerializeField] private Color _OutlineColor;
        #endregion Inspector Variables

        #region Public Methods

        public void InstantiateChildObjects()
        {
            _StencilMesh = Instantiate(this.gameObject, this.transform);
        }

        #endregion Public Methods


        #region Unity Methods
        void Start()
        {

            _OutlineShader = Shader.Find("McOutline/CustomOutline");
            _StencilBufferShader = Shader.Find("McOutline/CustomStencillBuffer");

            _CurrentObjectMaterials = new List<Material>();
            var materials = GetComponent<MeshRenderer>().materials;
            for (int i =0; i < materials.Length; ++i)
            {
                _CurrentObjectMaterials.Add(materials[i]);
                _AlphaCutoff = materials[i].GetFloat(_AlphaCutoffId);
                _Tiling = materials[i].GetTextureScale(_BaseColorMapId);
            }
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if(_CurrentObjectMaterials == null || _CurrentObjectMaterials.Count == 0)
            {
                return;
            }
            for (int i = 0; i < _CurrentObjectMaterials.Count; ++i)
            {
                _AlphaCutoff = _CurrentObjectMaterials[i].GetFloat(_AlphaCutoffId);
                _Tiling = _CurrentObjectMaterials[i].GetTextureScale(_BaseColorMapId);
            }
#endif
        }

        void Update()
        {

        }
        #endregion Unity Methods

        #region Private Variables

        private List<Material> _CurrentObjectMaterials;
        private static readonly int _AlphaCutoffId = Shader.PropertyToID("_AlphaCutoff");
        private static readonly int _BaseColorMapId = Shader.PropertyToID("_BaseColorMap");


        private float _AlphaCutoff;
        private Vector2 _Tiling;

        private GameObject _StencilMesh;
        private GameObject _OutlineMesh;

        private Shader _StencilBufferShader;
        private Shader _OutlineShader;

        private Material _StencilBufferMaterial;
        private Material _OutlineMaterial;
        #endregion Private Variables

        #region Private Methods

        #endregion Private Methods
    }
}
