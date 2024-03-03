namespace McOutlineFeature
{
    using System;
    using System.Collections.Generic;
    using UnityEditor;
    using UnityEngine;
    using UnityEngine.UI;

    [Serializable]
    public struct OutlineMaterialProperties
    {
        #region Inspector Variables
        [Tooltip("AlphaCutoff\n if _AdvancedSettings is false then takes values from current material\nList size must match amount of materials inside MeshRenderer"), SerializeField, Range(0.0f, 1.0f)] 
        private float _AlphaCutoffProperties;
        [Tooltip("Activate/Deactivate alpha cutoff feature\n if _AdvancedSettings is false then takes values from current material\nList size must match amount of materials inside MeshRenderer"), SerializeField] 
        private bool _AlphaCuoffEnableProperties;
        [Tooltip("Setup tiling for texture\n if _AdvancedSettings is false then takes values from current materials\nList size must match amount of materials inside MeshRenderer"), SerializeField] 
        private Vector2 _TilingProperties;
        [Tooltip("Setup Alpha Texture\n if _AdvancedSettings is false then takes values from current materials\nList size must match amount of materials inside MeshRenderer"), SerializeField] 
        private Texture _AlphaTextureProperties;
        #endregion Inspector Variables

        #region Public Variables
        public float AlphaCutoffProperties => _AlphaCutoffProperties;
        public bool AlphaCuoffEnableProperties => _AlphaCuoffEnableProperties;
        public Vector2 TilingProperties => _TilingProperties;
        public Texture AlphaTextureProperties => _AlphaTextureProperties;
        #endregion
    }

    public struct OutputOutlineMaterialProperties
    {
        #region Public Variables
        public float AlphaCutoff;
        public float AlphaCutoffEnable;
        public Vector2 Tiling;
        public Texture AlphaTexture;

        public OutputOutlineMaterialProperties(float alphaCutoff, float alphaCutoffEnable, Vector2 tiling, Texture alphaTexture)
        {
            AlphaCutoff = alphaCutoff;
            AlphaCutoffEnable = alphaCutoffEnable;
            Tiling = tiling;
            AlphaTexture = alphaTexture;
        }
        #endregion
    }

    [ExecuteAlways]
    public sealed class MC_OutlineObject : MonoBehaviour
    {
        #region Inspector Variables
        [Header("Simple Outline Settings")]
        [Tooltip("Outline size value"), SerializeField] 
        private float _OutlineSize = 15.0f;

        [Tooltip("Outline color value"), SerializeField] 
        private Color _OutlineColor = Color.green;

        [Header("Advanced Outline Settings")]
        [Tooltip("Activate advanced settings\nthis checkbox activates that alpha cutoff properties can be set by developer"), SerializeField] 
        private bool _AdvancedSettings = false;

        [Tooltip("Advanced Settings\n if _AdvancedSettings is false then takes values from current material\nList size must match amount of materials inside MeshRenderer"), SerializeField] 
        List<OutlineMaterialProperties> _OutlinePropertiesList;

        [Header("Debug")]
        [Tooltip("Only debug purpose to show enable/disable feature for object\nWorks only in editor"), SerializeField] 
        private bool _Enable = true;

        #endregion Inspector Variables

        #region Public Variables
        public MeshRenderer Renderer => _CurrentMeshRenderer;

        public float OutlineSize => _OutlineSize;
        public Color OutlineColor => _OutlineColor;

        public List<OutputOutlineMaterialProperties> OutputOutlineMaterialsProperties => _OutputOutlineMaterialsProperties;

        public bool OutlineActive => _OutlineActive;

#if UNITY_EDITOR
        public static Action _InitializeAndRegisterObjectsDelegate;
        public static Action _DeinitializeAndUnregisterObjectsDelegate;
#endif

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
#if UNITY_EDITOR
            if (_Enable && !Application.isPlaying)
            {
                EnableOutline();
            }
            else
            {
                DisableOutline();
            }
#endif
            InitializeAndRegisterObject();
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            if(OutputOutlineMaterialsProperties == null)
            {
                return;
            }
            OutputOutlineMaterialsProperties.Clear();
            UpdateProperties();

            if(_Enable && !Application.isPlaying)
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
#if UNITY_EDITOR
            _InitializeAndRegisterObjectsDelegate += InitializeAndRegisterObject;
            _DeinitializeAndUnregisterObjectsDelegate += DeinitializeAndUnregisterObject;
            MC_OutlineManager.InitializeObjectsDelegate += InitializeAndRegisterObject;
            MC_OutlineManager.DeinitializeObjectsDelegate += DeinitializeAndUnregisterObject;
            InitializeAndRegisterObject();
#endif
        }

        private void OnDisable()
        {
#if UNITY_EDITOR
            _InitializeAndRegisterObjectsDelegate -= InitializeAndRegisterObject;
            _DeinitializeAndUnregisterObjectsDelegate -= DeinitializeAndUnregisterObject;
            MC_OutlineManager.InitializeObjectsDelegate -= InitializeAndRegisterObject;
            MC_OutlineManager.DeinitializeObjectsDelegate -= DeinitializeAndUnregisterObject;
#endif
            DeinitializeAndUnregisterObject();
        }

        #endregion Unity Methods

        #region Private Variables

        private static readonly int _AlphaCutoffEnableId = Shader.PropertyToID("_AlphaCutoffEnable");
        private static readonly int _AlphaCutoffId = Shader.PropertyToID("_AlphaCutoff");
        private static readonly int _BaseColorMapId = Shader.PropertyToID("_BaseColorMap");
        
        //Properties from materials
        private bool _OutlineActive;
        private List<OutputOutlineMaterialProperties> _OutputOutlineMaterialsProperties;

        private MeshRenderer _CurrentMeshRenderer;
        #endregion Private Variables

        #region Private Methods

        private void Initialize()
        {
            _CurrentMeshRenderer = GetComponent<MeshRenderer>();
            _OutputOutlineMaterialsProperties = new List<OutputOutlineMaterialProperties>();
            UpdateProperties();
        }

        private void Deinitialize()
        {
            if(_OutputOutlineMaterialsProperties == null)
            {
                return;
            }
            _OutputOutlineMaterialsProperties.Clear();
        }


        private void UpdateProperties()
        {
            if (!_AdvancedSettings)
            {
                UpdateMaterialFromObjectMaterials();
                return;
            }
            UpdateMaterialManually();
        }

        private void UpdateMaterialFromObjectMaterials()
        {
            if (_CurrentMeshRenderer == null)
            {
                return;
            }
            if(_OutputOutlineMaterialsProperties == null)
            {
                return;
            }
            for (int i = 0; i < _CurrentMeshRenderer.sharedMaterials.Length; ++i)
            {
                var currentMaterial = _CurrentMeshRenderer.sharedMaterials[i];
                _OutputOutlineMaterialsProperties.Add(new OutputOutlineMaterialProperties(currentMaterial.GetFloat(_AlphaCutoffId),
                                                                                          currentMaterial.GetFloat(_AlphaCutoffEnableId),
                                                                                          currentMaterial.GetTextureScale(_BaseColorMapId),
                                                                                          currentMaterial.GetTexture(_BaseColorMapId)));
            }
        }

        private void UpdateMaterialManually()
        {
            if (_CurrentMeshRenderer == null)
            {
                return;
            }
            if(_OutputOutlineMaterialsProperties == null)
            {
                return;
            }
            for (int i = 0; i < _CurrentMeshRenderer.sharedMaterials.Length; ++i)
            {
                if(_OutlinePropertiesList.Count != _CurrentMeshRenderer.sharedMaterials.Length)
                {
                    return;
                }
                var outlinePropertiesElement = _OutlinePropertiesList[i];

                _OutputOutlineMaterialsProperties.Add(new OutputOutlineMaterialProperties(outlinePropertiesElement.AlphaCutoffProperties,
                                                                                          outlinePropertiesElement.AlphaCuoffEnableProperties ? 1.0f : 0.0f,
                                                                                          outlinePropertiesElement.TilingProperties,
                                                                                          outlinePropertiesElement.AlphaTextureProperties));
            }
        }

        private void InitializeAndRegisterObject()
        {
            if (MC_OutlineManager.Instance == null)
            {
                return;
            }
            MC_OutlineManager.Instance.Register(this);
            Initialize();
        }

        private void DeinitializeAndUnregisterObject()
        {
            if (MC_OutlineManager.Instance == null)
            {
                return;
            }
            MC_OutlineManager.Instance.Unregister(this);
            Deinitialize();
        }
#if UNITY_EDITOR
        [UnityEditor.Callbacks.DidReloadScripts]
        private static void OnScriptsReloaded()
        {
            _DeinitializeAndUnregisterObjectsDelegate?.Invoke();
            _InitializeAndRegisterObjectsDelegate?.Invoke();
        }
#endif
        #endregion Private Methods
    }
}
