using UnityEngine;

namespace Helpers
{
    public class HierarchyIcon : MonoBehaviour
    {
#if UNITY_EDITOR
        public Texture2D icon;

        [TextArea]
        public string tooltip;
        [Range(0, 5)]
        [Tooltip("Right to left position")]
        public int position = 0;
#endif
    }
}