namespace McOutlineFeature
{
    using UnityEngine;
    using System.Collections.Generic;

    public static class MaterialExtensions
    {
        #region Public Methods
        public static void AddResource(this ICollection<Material> materials, Material material)
        {
            if (material)
                materials.Add(material);
            else
                Debug.LogError("Cannot find material");
        }
        #endregion
    }
}