namespace McOutlineFeature
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;


    [ExecuteAlways]
    public sealed class MC_OutlineManager : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private Mc_OutlineSettings _Settings;
        #endregion Inspector Variables

        #region Public Methods
        public void Register(MC_OutlineObject outline)
        {
            if (!OutlineObjects.Contains(outline))
            {
                OutlineObjects.Add(outline);
            }
        }

        public void Unregister(MC_OutlineObject outline)
        {
            if(OutlineObjects.Contains(outline))
            {
                OutlineObjects.Remove(outline);
            }
        }
        #endregion Public Methods

        #region Public Variables
        public List<MC_OutlineObject> OutlineObjects = new List<MC_OutlineObject>();
        public Mc_OutlineSettings Settings
        {
            get => _Settings;
            private set { _Settings = value; }
        }

        public static MC_OutlineManager Instance;
        #endregion Public Variables

        #region Unity Methods
        private void Awake()
        {
            Instance = this;
        }

        private void OnValidate()
        {
#if UNITY_EDITOR
            Instance = this;
#endif
        }
        #endregion Unity Methods
    }
}
