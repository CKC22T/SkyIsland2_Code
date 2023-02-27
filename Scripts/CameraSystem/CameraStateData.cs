using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Olympus
{
    [CreateAssetMenu(fileName = "CameraStateData", menuName = "ScriptableObjects/CameraStateData", order = 2)]
    public class CameraStateData : ScriptableObject
    {
        public enum SequenceType
        {
            Volume,
            Input,
            Time,
        }

        [System.Serializable]
        public class Sequence
        {
            public SequenceType sequenceType;
            public float fieldOfView;
            public Vector3 offset;
            public Vector3 eulerRotation;
            public Transform target;

            [Range(0.0f, 1.0f)]
            public float response;

            [Sirenix.OdinInspector.ShowIf("@sequenceType == SequenceType.Input")] public KeyCode keyCode;

            [Sirenix.OdinInspector.ShowIf("@sequenceType == SequenceType.Time")] public bool useAdvanced = false;
            [Sirenix.OdinInspector.ShowIf("@sequenceType == SequenceType.Time")] public DeltaTimer sequenceTimer;

            [Sirenix.OdinInspector.ShowIf("@useAdvanced == true && sequenceType == SequenceType.Time")] public AnimationCurve xOffsetCurve;
            [Sirenix.OdinInspector.ShowIf("@useAdvanced == true && sequenceType == SequenceType.Time")] public AnimationCurve yOffsetCurve;
            [Sirenix.OdinInspector.ShowIf("@useAdvanced == true && sequenceType == SequenceType.Time")] public AnimationCurve zOffsetCurve;

            [Sirenix.OdinInspector.ShowIf("@useAdvanced == true && sequenceType == SequenceType.Time")] public AnimationCurve rotationCurve;

            #region Internals
            public Vector3 InitialPosition { get; set; }
            public Vector3 EndPosition { get; set; }
            public Vector3 InitialRotaion { get; set; }
            public Vector3 EndRotation { get; set; }
            public bool IsDirty { get; set; }
            #endregion
        }

        public Sequence[] sequences;
    }
}