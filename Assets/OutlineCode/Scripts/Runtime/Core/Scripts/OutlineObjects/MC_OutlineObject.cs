namespace McOutlineFeature
{
    using System.Collections.Generic;
    using Unity.VisualScripting;
    using UnityEngine;

    public sealed class MC_OutlineObject : MonoBehaviour
    {
        #region Inspector Variables
        [Header("Simple Outline Settings")]
        [Tooltip("Outline size value"), SerializeField] private float _OutlineSize;
        [Tooltip("Outline color value"), SerializeField] private Color _OutlineColor;

        [Header("Advanced Outline Settings")]
        [Tooltip("Activate advanced settings\nthis checkbox activates that alpha cutoff properties can be set by developer"), SerializeField] private bool _AdvancedSettings = false;

        [Tooltip("AlphaCutoff\n if _AdvancedSettings is false then takes values from current material\nList size must match amount of materials inside MeshRenderer"), SerializeField, Range(0.0f, 1.0f)] private List<float> _AlphaCutoffProperties;
        [Tooltip("Activate/Deactivate alpha cutoff feature\n if _AdvancedSettings is false then takes values from current material\nList size must match amount of materials inside MeshRenderer"), SerializeField] private List<bool> _AlphaCuoffEnableProperties;
        [Tooltip("Setup tiling for texture\n if _AdvancedSettings is false then takes values from current materials\nList size must match amount of materials inside MeshRenderer"), SerializeField] private List<Vector2> _TilingProperties;
        [Tooltip("Setup Alpha Texture\n if _AdvancedSettings is false then takes values from current materials\nList size must match amount of materials inside MeshRenderer"), SerializeField] private List<Texture> _AlphaTextureToAlphaCutoffProperties;

        [Header("Debug")]
        [Tooltip("Only debug purpose to show enable/disable feature for object"), SerializeField] private bool _Enable = true;
        #endregion Inspector Variables

        #region Public Variables
        public MeshRenderer Renderer => _CurrentMeshRenderer;

        public float OutlineSize => _OutlineSize;
        public Color OutlineColor => _OutlineColor;

        public List<float> AlphaCutoff => _AlphaCutoff;
        public List<float> AlphaCutoffEnable => _AlphaCutoffEnable;

        public List<Texture> AlphaTextureToAlphaCutoff => _AlphaTextureToAlphaCutoff;

        public List<Vector2> Tiling => _Tiling;

        public bool OutlineActive => _OutlineActive;

        #endregion Public Variables

        #region Public Methods
        public void DisableOutline()
        {
            _OutlineActive = false;
        }

        public void EnableOutline()
        {
            _OutlineActive = true;
        }
        #endregion Public Methods

        #region Unity Methods

        private void Start()
        {
            if (MC_OutlineManager.Instance == null)
            {
                Debug.LogError("There is no McOutlineManager please add it then proceed work with outline feature");
                return;
            }
            EnableOutline();
#if UNITY_EDITOR
            if (_Enable)
            {
                EnableOutline();
            }
            else
            {
                DisableOutline();
            }
#endif
            _CurrentMeshRenderer = GetComponent<MeshRenderer>();
            MC_OutlineManager.Instance.Register(this);
            Initialize();
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if (MC_OutlineManager.Instance == null)
            {
                return;
            }

            Deinitialize();
            UpdateProperties();
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
            if (MC_OutlineManager.Instance == null)
            {
                return;
            }
            MC_OutlineManager.Instance.Register(this);
            Initialize();
        }

        private void OnDisable()
        {
            if (MC_OutlineManager.Instance == null)
            {
                return;
            }
            MC_OutlineManager.Instance.Unregister(this);
            Deinitialize();
        }
        #endregion Unity Methods

        #region Private Variables

        private static readonly int _AlphaCutoffEnableId = Shader.PropertyToID("_AlphaCutoffEnable");
        private static readonly int _AlphaCutoffId = Shader.PropertyToID("_AlphaCutoff");
        private static readonly int _BaseColorMapId = Shader.PropertyToID("_BaseColorMap");
        
        //Properties from materials
        private List<float> _AlphaCutoff;
        private List<float> _AlphaCutoffEnable;
        [SerializeField] private List<Texture> _AlphaTextureToAlphaCutoff;
        private List<Vector2> _Tiling;
        private bool _OutlineActive;

        private MeshRenderer _CurrentMeshRenderer;
        #endregion Private Variables

        #region Private Methods

        private void Initialize()
        {
            _AlphaCutoff = new List<float>();
            _AlphaCutoffEnable = new List<float>();
            _AlphaTextureToAlphaCutoff = new List<Texture>();
            _Tiling = new List<Vector2>();
            UpdateProperties();
        }

        private void Deinitialize()
        {
            _AlphaCutoff.Clear();
            _Tiling.Clear();
            _AlphaTextureToAlphaCutoff.Clear();
            _AlphaCutoffEnable.Clear();
        }


        private void UpdateProperties()
        {
            if (!_AdvancedSettings)
            {
                UpdateMaterialSimpleWay();
                return;
            }
            UpdateMaterialAdvancedWay();
        }

        private void UpdateMaterialSimpleWay()
        {
            if (_CurrentMeshRenderer == null)
            {
                return;
            }
            for (int i = 0; i < _CurrentMeshRenderer.materials.Length; ++i)
            {
                var currentMaterial = _CurrentMeshRenderer.materials[i];
                _AlphaCutoff.Add(currentMaterial.GetFloat(_AlphaCutoffId));
                _Tiling.Add(currentMaterial.GetTextureScale(_BaseColorMapId));
                _AlphaTextureToAlphaCutoff.Add(currentMaterial.GetTexture(_BaseColorMapId));
                _AlphaCutoffEnable.Add(currentMaterial.GetFloat(_AlphaCutoffEnableId));
            }
        }

        private void UpdateMaterialAdvancedWay()
        {
            if (_CurrentMeshRenderer == null)
            {
                return;
            }

            for (int i = 0; i < _CurrentMeshRenderer.materials.Length; ++i)
            {
                _AlphaCutoff.Add(_AlphaCutoffProperties[i]);
                _Tiling.Add(_TilingProperties[i]);
                _AlphaTextureToAlphaCutoff.Add(_AlphaTextureToAlphaCutoffProperties[i]);
                _AlphaCutoffEnable.Add(_AlphaCuoffEnableProperties[i] ? 1.0f : 0.0f);
            }
        }

        #endregion Private Methods
    }
}
