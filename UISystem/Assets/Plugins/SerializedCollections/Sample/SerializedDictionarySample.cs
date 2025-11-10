using UnityEngine;

namespace AYellowpaper.SerializedCollections
{
    public class SerializedDictionarySample : MonoBehaviour
    {
        public enum ElementType
        {
            Fire,
            Air,
            Earth,
            Water
        }

        [SerializedDictionary("Element Type", "Description")]
        public SerializedDictionary<ElementType, string> ElementDescriptions;
    }
}
