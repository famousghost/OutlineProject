namespace McOutlineFeatureTest
{
    using McOutlineFeature;
    using UnityEngine;

    public sealed class MC_SelectObject : MonoBehaviour
    {

        #region Unity Methods

        private void OnMouseDown()
        {
            var outlineComponent = GetComponent<MC_OutlineObject>();
            if(outlineComponent == null)
            {
                return;
            }
            outlineComponent.EnableOutline();
        }

        #endregion Unity Methods
    }
}
