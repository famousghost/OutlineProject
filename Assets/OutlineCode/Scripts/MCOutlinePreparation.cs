namespace McOutlineFeature
{
    using System.Collections.Generic;
    using UnityEngine;

    public sealed class MCOutlineManager : MonoBehaviour
    {
        #region Public Variables
        public static List<MCOutline> OutlineObjects = new List<MCOutline>();
        #endregion Public Variables

        #region Unity Methods
        void Start()
        {
            if(OutlineObjects.Count == 0)
            {
                return;
            }
            foreach(var outlineObject in OutlineObjects)
            {

            }
        }
        #endregion Unity Methods
    }
}
