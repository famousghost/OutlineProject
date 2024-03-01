namespace McOutlineFeature
{
    using UnityEngine;

    public sealed class OutlineObject : MonoBehaviour
    {
        #region Inspector Variables
        [Header("Outline Properties")]
        [SerializeField] private bool _UseDefaultSettings;
        [SerializeField] private float _OutlineSize;
        [SerializeField] private Color _OutlineColor;

        [Header("Debug")]
        [SerializeField] private bool _Enable = true;
        #endregion Inspector Variables

        #region Public Variables
        public MeshRenderer Renderer => _CurrentMeshRenderer;

        public float OutlineSize => _OutlineSize;
        public Color OutlineColor => _OutlineColor;

        public float AlphaCutoff => _AlphaCutoff;
        public float AlphaCutoffEnable => _AlphaCutoffEnable;

        public Texture AlphaTextureToAlphaCutoff => _AlphaTextureToAlphaCutoff;

        public Vector2 Tiling => _Tiling;

        public bool IsActive => _IsActive;

        #endregion Public Variables

        #region Public Methods
        public void DisableOutline()
        {
            _IsActive = false;
        }

        public void EnableOutline()
        {
            _IsActive = true;
        }
        #endregion Public Methods

        #region Unity Methods

        private void Start()
        {
            if (McOutlineManager.Instance != null)
            {
                McOutlineManager.Instance.Register(this);
            }
            if (_UseDefaultSettings)
            {
                if (McOutlineManager.Instance = null)
                {
                    Debug.Log("There is no instance of object McOutlineManager");
                    return;
                }
                _OutlineSize = McOutlineManager.Instance.Settings.DefaultOutlineSize;
                _OutlineColor = McOutlineManager.Instance.Settings.DefaultOutlineColor;
            }
            Deinitialize();
            Initialize();
        }


        private void OnValidate()
        {
            if (McOutlineManager.Instance == null)
            {
                return;
            }
            if (_UseDefaultSettings)
            {
                _OutlineSize = McOutlineManager.Instance.Settings.DefaultOutlineSize;
                _OutlineColor = McOutlineManager.Instance.Settings.DefaultOutlineColor;
            }
#if UNITY_EDITOR
            UpdateMaterialsProperties();
            if(_Enable)
            {
                EnableOutline();
            }
            else
            {
                DisableOutline();
            }    
#endif
        }

        private void OnEnable()
        {
            if (McOutlineManager.Instance != null)
            {
                McOutlineManager.Instance.Register(this);
            }
            Deinitialize();
            Initialize();
        }

        private void OnDisable()
        {
            if (McOutlineManager.Instance != null)
            {
                McOutlineManager.Instance.Unregister(this);
            }

            Deinitialize();
        }
#endregion Unity Methods

        #region Private Variables

        private static readonly int _AlphaCutoffEnableId = Shader.PropertyToID("_AlphaCutoffEnable");
        private static readonly int _AlphaCutoffId = Shader.PropertyToID("_AlphaCutoff");
        private static readonly int _BaseColorMapId = Shader.PropertyToID("_BaseColorMap");
        
        //Properties from materials
        private float _AlphaCutoff;
        private float _AlphaCutoffEnable;
        private Texture _AlphaTextureToAlphaCutoff;
        private Vector2 _Tiling;
        private bool _IsActive;

        private Shader _StencilBufferShader;
        private Shader _OutlineShader;

        private MeshRenderer _CurrentMeshRenderer;
        #endregion Private Variables

        #region Private Methods

                private void Initialize()
                {
                    _CurrentMeshRenderer = GetComponent<MeshRenderer>();
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
                    _AlphaCutoff = 0.0f;
                    _Tiling = new Vector2(1.0f, 1.0f);
                }

                private void CreateShaders()
                {
                    if(McOutlineManager.Instance == null)
                    {
                        return;
                    }
                    _OutlineShader = McOutlineManager.Instance.Settings.OutlineShader;
                    _StencilBufferShader = McOutlineManager.Instance.Settings.StencilBufferShader;
                }

                private void CreateMaterials()
                {
                    var materials = _CurrentMeshRenderer.materials;
                    for (int i = 0; i < materials.Length; ++i)
                    {

                        _AlphaCutoff = materials[i].GetFloat(_AlphaCutoffId);
                        _Tiling = materials[i].GetTextureScale(_BaseColorMapId);
                    }

                }

        private void UpdateMaterialsProperties()
        {
            if (_CurrentMeshRenderer == null)
            {
                return;
            }
            for (int i = 0; i < _CurrentMeshRenderer.materials.Length; ++i)
            {
                var currentMaterial = _CurrentMeshRenderer.materials[i];
                _AlphaCutoff = currentMaterial.GetFloat(_AlphaCutoffId);
                _Tiling = currentMaterial.GetTextureScale(_BaseColorMapId);
                _AlphaTextureToAlphaCutoff = currentMaterial.GetTexture(_BaseColorMapId);
                _AlphaCutoffEnable = currentMaterial.GetFloat(_AlphaCutoffEnableId);
            }
        }

        #endregion Private Methods
    }
}
