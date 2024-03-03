namespace McOutlineFeatureTest
{
    using McOutlineFeature;
    using UnityEngine;

    public sealed class MC_SelectObject : MonoBehaviour
    {

        #region Unity Methods

        private void OnMouseEnter()
        {
            var outlineComponent = GetComponent<MC_OutlineObject>();
            if (outlineComponent == null)
            {
                return;
            }
            outlineComponent.EnableOutline();
        }

        private void OnMouseExit()
        {
            var outlineComponent = GetComponent<MC_OutlineObject>();
            if (outlineComponent == null)
            {
                return;
            }
            outlineComponent.DisableOutline();
        }

        #endregion Unity Methods
    }
}
