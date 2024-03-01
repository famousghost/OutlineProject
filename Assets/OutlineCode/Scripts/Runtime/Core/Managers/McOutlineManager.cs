namespace McOutlineFeature
{
    using System.Collections;
    using System.Collections.Generic;
    using UnityEngine;

    public sealed class McOutlineManager : MonoBehaviour
    {
        #region Inspector Variables
        [SerializeField] private Mc_OutlineSettings _Settings;
        #endregion Inspector Variables

        #region Public Methods
        public void Register(OutlineObject outline)
        {
            if (!OutlineObjects.Contains(outline))
            {
                OutlineObjects.Add(outline);
            }
        }

        public void Unregister(OutlineObject outline)
        {
            if(OutlineObjects.Contains(outline))
            {
                OutlineObjects.Remove(outline);
            }
        }
        #endregion Public Methods

        #region Public Variables
        public List<OutlineObject> OutlineObjects = new List<OutlineObject>();
        public Mc_OutlineSettings Settings
        {
            get => _Settings;
            private set { _Settings = value; }
        }

        public static McOutlineManager Instance;
        #endregion Public Variables

        #region Unity Methods
        private void Awake()
        {
            Instance = this;
            Debug.Log("Manager Awake");
        }
        #endregion Unity Methods
    }
}
