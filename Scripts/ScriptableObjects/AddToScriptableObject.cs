using UnityEngine;

namespace Olympus
{
    public class AddToScriptableObject : MonoBehaviour
    {
        // Reference to the ScriptableObject
        [SerializeField] CameraStateData sceneReferenceSO;

        void Start() => AddSelfToList();
        void OnEnable() => AddSelfToList();
        void AddSelfToList() => sceneReferenceSO.sequences[0].target = transform;
        void OnDisable() => sceneReferenceSO.sequences[0].target = transform;
    }
}