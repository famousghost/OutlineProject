namespace McOutlineFeature
{
    using System.Collections.Generic;
    using Unity.VisualScripting;
    using UnityEngine;

    public sealed class MCOutline : MonoBehaviour
    {
        #region Inspector Variables
        [Header("Outline Properties")]
        [SerializeField] private float _OutlineSize;
        [SerializeField] private Color _OutlineColor;
        #endregion Inspector Variables

        #region Public Methods
        public void DisableOutline()
        {
            _OutlineMesh.SetActive(false);
            _StencilMesh.SetActive(false);
        }

        public void EnableOutline()
        {
            _OutlineMesh.SetActive(true);
            _StencilMesh.SetActive(true);
        }
        #endregion Public Methods


        #region Unity Methods

        private void Start()
        {
            Initialize();
            InstantiateChildObjects();
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            UpdateMaterialsProperties();
#endif
        }

        private void Update()
        {

        }

        private void OnDestroy()
        {
            Deinitialize();
        }
        #endregion Unity Methods

        #region Private Variables

        private List<Material> _CurrentObjectMaterials;

        private static readonly int _AlphaCutoffEnableId = Shader.PropertyToID("_AlphaCutoffEnable");
        private static readonly int _LeafTextureId = Shader.PropertyToID("_LeafTexture");
        private static readonly int _AlphaCutoffId = Shader.PropertyToID("_AlphaCutoff");
        private static readonly int _BaseColorMapId = Shader.PropertyToID("_BaseColorMap");
        private static readonly int _TilingId = Shader.PropertyToID("_Tiling");
        private static readonly int _OutlineSizeId = Shader.PropertyToID("_OutlineSize");
        private static readonly int _OutlineColorId = Shader.PropertyToID("_OutlineColor");


        private float _AlphaCutoff;
        private Vector2 _Tiling;

        private GameObject _StencilMesh;
        private GameObject _OutlineMesh;

        private Shader _StencilBufferShader;
        private Shader _OutlineShader;

        [SerializeField] private List<Material> _StencilBufferMaterials;
        [SerializeField] private List<Material> _OutlineMaterials;
        #endregion Private Variables

        #region Private Methods

        private void Initialize()
        {
            CreateShaders();
            if (_StencilBufferShader == null)
            {
                Debug.LogError("There is no Stencil buffer shader in project");
                return;
            }
            if (_OutlineShader == null)
            {
                Debug.LogError("There is no Outline shader in project");
                return;
            }

            CreateMaterials();
            UpdateMaterialsProperties();
        }

        private void Deinitialize()
        {
            _CurrentObjectMaterials.Clear();
            _AlphaCutoff = 0.0f;
            _Tiling = new Vector2(1.0f, 1.0f);
        }

        private void CreateShaders()
        {
            _OutlineShader = Shader.Find("McOutline/CustomOutline");
            _StencilBufferShader = Shader.Find("McOutline/CustomStencillBuffer");
        }

        private void CreateMaterials()
        {
            _CurrentObjectMaterials = new List<Material>();
            var materials = GetComponent<MeshRenderer>().materials;
            _StencilBufferMaterials = new List<Material>();
            _OutlineMaterials = new List<Material>();
            for (int i = 0; i < materials.Length; ++i)
            {

                _CurrentObjectMaterials.Add(materials[i]);
                _AlphaCutoff = materials[i].GetFloat(_AlphaCutoffId);
                _Tiling = materials[i].GetTextureScale(_BaseColorMapId);
                _StencilBufferMaterials.Add(new Material(_StencilBufferShader));
                _OutlineMaterials.Add(new Material(_OutlineShader));
            }

        }

        private void UpdateMaterialsProperties()
        {
            if(_CurrentObjectMaterials == null || _CurrentObjectMaterials.Count == 0)
            {
                return;
            }
            for (int i = 0; i < _CurrentObjectMaterials.Count; ++i)
            {
                _AlphaCutoff = _CurrentObjectMaterials[i].GetFloat(_AlphaCutoffId);
                _Tiling = _CurrentObjectMaterials[i].GetTextureScale(_BaseColorMapId);
                var texture = _CurrentObjectMaterials[i].GetTexture(_BaseColorMapId);
                var alphaCutoffEnable = _CurrentObjectMaterials[i].GetFloat(_AlphaCutoffEnableId);
                if (_StencilBufferMaterials[i] != null)
                {
                    _StencilBufferMaterials[i].SetFloat(_AlphaCutoffId, _AlphaCutoff);
                    _StencilBufferMaterials[i].SetVector(_TilingId, _Tiling);
                    _StencilBufferMaterials[i].SetTexture(_LeafTextureId, texture);
                    _StencilBufferMaterials[i].SetFloat(_AlphaCutoffEnableId, alphaCutoffEnable);
                }

                if (_OutlineMaterials[i] != null)
                {
                    _OutlineMaterials[i].SetFloat(_AlphaCutoffId, _AlphaCutoff);
                    _OutlineMaterials[i].SetVector(_TilingId, _Tiling);
                    _OutlineMaterials[i].SetFloat(_OutlineSizeId, _OutlineSize);
                    _OutlineMaterials[i].SetColor(_OutlineColorId, _OutlineColor);
                    _OutlineMaterials[i].SetTexture(_LeafTextureId, texture);
                    _OutlineMaterials[i].SetFloat(_AlphaCutoffEnableId, alphaCutoffEnable);
                }
            }
        }

        private void InstantiateChildObjects()
        {
            _StencilMesh = new GameObject("Stencil Object", typeof(MeshFilter), typeof(MeshRenderer));
            _StencilMesh.GetComponent<MeshFilter>().mesh = this.GetComponent<MeshFilter>().mesh;
            
            _StencilMesh.GetComponent<MeshRenderer>().materials = GetComponent<MeshRenderer>().materials;
            _StencilMesh.GetComponent<MeshRenderer>().materials = _StencilBufferMaterials.ToArray();

            _StencilMesh.transform.parent = transform;
            _StencilMesh.transform.localScale = Vector3.one;
            _StencilMesh.transform.localPosition = Vector3.zero;
            _StencilMesh.transform.localRotation = Quaternion.identity;

            _OutlineMesh = new GameObject("Outline Object", typeof(MeshFilter), typeof(MeshRenderer));
            _OutlineMesh.GetComponent<MeshFilter>().mesh = this.GetComponent<MeshFilter>().mesh;

            _OutlineMesh.GetComponent<MeshRenderer>().materials = GetComponent<MeshRenderer>().materials;
            _OutlineMesh.GetComponent<MeshRenderer>().materials = _OutlineMaterials.ToArray();


            _OutlineMesh.transform.parent = transform;
            _OutlineMesh.transform.localScale = Vector3.one;
            _OutlineMesh.transform.localPosition = Vector3.zero;
            _OutlineMesh.transform.localRotation = Quaternion.identity;
        }

        private void ClearObjects()
        {
            Destroy(_StencilMesh);
            Destroy(_OutlineMesh);
        }

        #endregion Private Methods
    }
}
