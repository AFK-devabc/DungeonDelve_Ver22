using UnityEngine;

namespace BrainFailProductions.PolyFew
{
    public class PolyFew : MonoBehaviour
    {
#if UNITY_EDITOR

        [SerializeField]
        [HideInInspector]
        public DataContainer dataContainer;

        public PolyFew()
        {
            dataContainer ??= new DataContainer();
        }

        private void Start() { }

#endif
    }
}
